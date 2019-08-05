using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    private readonly KeyCode JUMP_BUTTON = KeyCode.Z;
    private readonly KeyCode ATTACK_BUTTON = KeyCode.X;
    private readonly KeyCode DASH_BUTTON = KeyCode.C;

    public enum PlayerState {Idle, Run, Jump, Dash, Climb, WallSlide, Crouch , Attack , Hurt, Fall};
    public PlayerState playerState = PlayerState.Idle;

    [Header("Player")]
    public float moveSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 20f;
    public float leastJumpTime = 0.1f;
    public Vector2 wallJumpForce;
    public float wallJumpTime = 0.2f;
    public float leaseJumpLadderTime = 0.3f;

    [Header("Attack")]
    public float damage = 1f;
    public float attackColdDown = 0.5f;
    public FireBall fireBallPrefab;
    public Transform firePoint;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashTime = 0.2f;
    public float dashColdDown = 1f;

    [Header("Wall Slide")]
    public float wallSlideGravity = 10f;
    public float wallCheckLegth = 0.1f;

    [Header("Climb")]
    public LayerMask whatIsLadder;
    public float climbForce = 20f;

    [Header("Hurt")]
    public float hurtTime = 0.5f;
    public float invincibleTime = 2f;

    [Header("Check Mask")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public LayerMask whatIsEffecterPlatform;
    public float groundCheckLegth = 0.1f;
    
    [Header("Animation")]
    public Animator animator;
    public GameObject deadEffect;
    public SpriteRenderer spriteRenderer;

    private Vector2 moveVector;
    private bool isJump,isCanSecondJump, isWallJump, isWallSlide, isGround, isDash,isCanDash, isCrouch, isClimb,isCanClimb,isFall,isAttack,isCanAttack, isHurt;
    public bool isInvincible { get; private set; }
    private bool dashDirection;
    private float originalGravity;
    private Ladder crrentLadder;

    private Rigidbody2D rigidbody2D;
    private CapsuleCollider2D capsuleCollider2D;
    private AnimatorStateInfo animatorStateInfo;

    public static event System.Action OnPlayerHurt;

    #region Instance
    public static Player instance;

    private void Awake()
    {
        instance = this;
        rigidbody2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    #endregion

    protected override void Start()
    {
        base.Start();

        originalGravity = rigidbody2D.gravityScale;
        isCanSecondJump = true;
        isCanDash = true;
        isCanAttack = true;
        isCanClimb = true;

        GameUI.instance.UpdateLife();
    }

    private void Update()
    {
        moveVector = Vector3.zero;

        //Input Horizontal
        moveVector.x = FlowchartManager.instance.isTalking? 0f : Input.GetAxis("Horizontal");
        animator.SetFloat("Horizontal", moveVector.x);

        //Input Vertical
        moveVector.y = FlowchartManager.instance.isTalking ? 0f : Input.GetAxis("Vertical");
        animator.SetFloat("Vertical", moveVector.y);
        //Player Direction
        if (moveVector.x != 0)
            transform.localEulerAngles = (moveVector.x > 0) ? new Vector3(0, 0F, 0) : new Vector3(0, 180F, 0);

        ButtomInput();
        
        PlayerStateController();
                
        AnimatorController();
    }

    private void ButtomInput()
    {
        if (isDash || isHurt || isCrouch || FlowchartManager.instance.isTalking)
            return;

        if (Input.GetKeyDown(ATTACK_BUTTON) && isCanAttack)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(JUMP_BUTTON))
        {
            if (isGround)       //Jump
            {
                isCanSecondJump = true;
                StartCoroutine(Jump());
            }
            else if (isWallSlide)       //WallJump
            {
                StartCoroutine(WallJump());
            }
            else if (isClimb)
            {
                StartCoroutine(JumpLadder());
            }
            else
            {
                if (isCanSecondJump)                    //DoubleJump
                {
                    isCanSecondJump = false;
                    StartCoroutine(Jump());
                }
            }
        }

        if (Input.GetKeyDown(DASH_BUTTON) && isCanDash)
        {
            if (isClimb)
                isClimb = false;
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Die();
        }
    }

    IEnumerator Jump()
    {
        playerState = PlayerState.Jump;
        animator.SetTrigger("Jump");
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);        //消除重力加速度
        rigidbody2D.AddForce(Vector2.up * jumpForce);

        //避免一跳躍  馬上判定為地面  動畫BUG
        isJump = true;
        float time = 0;
        while (time < leastJumpTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        isJump = false;
    }

    IEnumerator WallJump()
    {
        isWallJump = true;
        isCanSecondJump = true;
        playerState = PlayerState.Jump;
        animator.SetTrigger("Jump");
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);            //消除原有重力加速度
        rigidbody2D.AddForce(new Vector2((transform.localEulerAngles.y == 0) ? -wallJumpForce.x : wallJumpForce.x, wallJumpForce.y));

        float tempWallJumpTime = 0;

        while (tempWallJumpTime < wallJumpTime)
        {
            tempWallJumpTime += Time.deltaTime;
            yield return null;
        }

        isWallJump = false;
    }

    IEnumerator JumpLadder()
    {
        isCanClimb = false;
        float tempJumpLadderTime = 0;

        playerState = PlayerState.Jump;
        isClimb = false;
        isCanSecondJump = true;
        rigidbody2D.AddForce(new Vector2(Mathf.Clamp(moveVector.x * 100, -1, 1) * jumpForce, jumpForce / 2));

        while (tempJumpLadderTime < leaseJumpLadderTime)
        {
            tempJumpLadderTime += Time.deltaTime;
            yield return null;
        }

        isCanClimb = true;
    }

    IEnumerator Attack()
    {
        playerState = PlayerState.Attack;
        animator.SetTrigger("Attack");
        isCanAttack = false;
        FireBall fireBall = Instantiate(fireBallPrefab, firePoint.position, Quaternion.identity);
        fireBall.SetDamage(damage);
        float tempAttackColdTime = 0;

        while (tempAttackColdTime < attackColdDown)
        {
            tempAttackColdTime += Time.deltaTime;
            yield return 0;
        }

        isCanAttack = true;
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("Dash");
        isDash = true;
        isCanDash = false;
        dashDirection = (transform.localEulerAngles.y == 0) ? true : false;
        rigidbody2D.gravityScale = 0;                   //保持無重力
        float tempDashTime = 0;

        while (tempDashTime < dashTime)                 //Dashing    
        {
            tempDashTime += Time.deltaTime;
            yield return null;
        }

        rigidbody2D.gravityScale = originalGravity;     //回復重力
        isDash = false;
        playerState = PlayerState.Idle;

        float tempDashColdDown = tempDashTime;          //isCanDash = CD,touch ground wall ladder
        bool tempIsCanDash = false;

        while (tempDashColdDown < dashColdDown)
        {
            tempDashColdDown += Time.deltaTime;
            tempIsCanDash = isGround || isWallSlide || isClimb;
            yield return null;
        }

        while (!tempIsCanDash)
        {
            tempIsCanDash = isGround || isWallSlide || isClimb;
            yield return null;
        }
        isCanDash = true;
    }

    private void PlayerStateController()
    {
        isGround = IsGround();
        isCrouch = IsCrouch();
        isClimb = IsClimb();
        isWallSlide = IsWallSlide() && !isClimb;    //避免梯子旁有牆
        isFall = (rigidbody2D.velocity.y < 0 && !isGround && !isClimb && !isWallSlide) ? true : false;

        if (isWallJump || isJump)
            return;
        if (isHurt)
            playerState = PlayerState.Hurt;
        else if (isClimb)
            playerState = PlayerState.Climb;
        else if (isDash)
            playerState = PlayerState.Dash;
        else if (isWallSlide)
            playerState = PlayerState.WallSlide;
        else if (isCrouch)
            playerState = PlayerState.Crouch;
        else if (isFall)
            playerState = PlayerState.Fall;
        else if (isGround)
            playerState = (moveVector.x == 0) ? PlayerState.Idle : PlayerState.Run;
    }

    private void AnimatorController()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (playerState == PlayerState.Idle && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Idle"))
            animator.SetTrigger("Idle");
        else if (playerState == PlayerState.Run && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Run"))
            animator.SetTrigger("Run");
        else if (playerState == PlayerState.Climb && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Climb"))
            animator.SetTrigger("Climb");
        else if (playerState == PlayerState.WallSlide && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_WallSlide"))
            animator.SetTrigger("WallSlide");
        else if (playerState == PlayerState.Fall && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Fall"))
            animator.SetTrigger("Fall");
        else if (playerState == PlayerState.Crouch && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Crouch"))
            animator.SetTrigger("Crouch");
        else if (playerState == PlayerState.Hurt && animatorStateInfo.fullPathHash != Animator.StringToHash("Base.Player_Hurt"))
            animator.SetTrigger("Hurt");
    }

    //各狀態移動方式
    private void FixedUpdate()
    {
        if (isWallJump || isHurt)
            return;
        if (playerState == PlayerState.Dash)
        {
            rigidbody2D.velocity = (dashDirection) ? Vector2.right * dashForce : Vector2.left * dashForce;
        }
        else if (playerState == PlayerState.Climb)
        {
            transform.position = new Vector2(crrentLadder.transform.position.x, transform.position.y);
            rigidbody2D.velocity = new Vector2(0, moveVector.y * climbForce);
        }
        else if (playerState == PlayerState.WallSlide)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -wallSlideGravity);
        }
        else if(playerState == PlayerState.Crouch)       //蹲下
        {
            rigidbody2D.velocity = new Vector2(moveVector.x * moveSpeed / 2, rigidbody2D.velocity.y);
        }
        else                    //跳躍 行走
        {
            rigidbody2D.gravityScale = originalGravity;     //回復重力
            rigidbody2D.velocity = (IsWall() && !isGround && moveVector.x != 0) ? new Vector2(0, rigidbody2D.velocity.y) : new Vector2(moveVector.x * moveSpeed, rigidbody2D.velocity.y); //1:避免往牆跳躍卡牆上 2:一般移動
        }
    }

    //overrider TakeDamage
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (OnPlayerHurt != null)
            OnPlayerHurt();
        StartCoroutine(Hurt());
        StartCoroutine(Invincible());
    }

    IEnumerator Hurt()
    {
        isHurt = true;
        animator.SetTrigger("Hurt");
        float tempTime = 0;

        while (tempTime < hurtTime)
        {
            tempTime += Time.deltaTime;
            yield return null;
        }
        rigidbody2D.velocity = Vector2.zero;
        isHurt = false;
    }

    IEnumerator Invincible()
    {
        isInvincible = true;
        float tempTime = 0;
        float pingPongTime = 0.1f;
        Color originalColor = spriteRenderer.color;
        while (tempTime < invincibleTime - hurtTime)
        {
            float colorA = ((1 / pingPongTime) * Mathf.PingPong(tempTime, pingPongTime) > 0.5f) ? 1f : 0f;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, colorA);
            tempTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    //override Die
    public override void Die()
    {
        Destroy(Instantiate(deadEffect, capsuleCollider2D.bounds.center, Quaternion.identity), 1f);
        base.Die();
        GameManager.instance.GameOver();
    }

    private bool IsCrouch()
    {
        if (moveVector.y < 0 && isGround)
        {
            capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
            return true;
        }
        else
        {
            if (isCrouch)
            {
                Vector2 checkTopPoint = new Vector2(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y + (capsuleCollider2D.size.y + groundCheckLegth) / 2);
                Collider2D[] isCrouchTopCheck = Physics2D.OverlapBoxAll(checkTopPoint, new Vector2(capsuleCollider2D.size.x - 0.1f, groundCheckLegth), 0, whatIsGround);
                if (isCrouchTopCheck.Length > 0)
                    return true;
            }
            capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
            return false;
        }
    }

    private bool IsInEffectorPlayform()
    {
        Collider2D[] effectoePlatformCheck = Physics2D.OverlapBoxAll(capsuleCollider2D.bounds.center, new Vector2(wallCheckLegth, capsuleCollider2D.size.y), 0, whatIsEffecterPlatform);
        if (effectoePlatformCheck.Length > 0)
            return true;
        return false;
    }

    private bool IsWallSlide()
    {
       if(IsWall() && !isGround && moveVector.x != 0 && rigidbody2D.velocity.y < 0)
        {
            return true;
        }
        return false;
    }

    //暫時不變動  需在改善成多設線判斷  多複雜動作時
    private bool IsWall()
    {
        Vector2 tempDirection = (transform.localEulerAngles.y == 0) ? Vector2.right : Vector2.left;
        Vector2 checkWallPoint = (transform.localEulerAngles.y == 0) ?
            new Vector2(capsuleCollider2D.bounds.center.x + (capsuleCollider2D.size.x + wallCheckLegth) / 2, capsuleCollider2D.bounds.center.y) :
            new Vector2(capsuleCollider2D.bounds.center.x - (capsuleCollider2D.size.x + wallCheckLegth) / 2, capsuleCollider2D.bounds.center.y);
        Collider2D[] targetsInRadius = Physics2D.OverlapBoxAll(checkWallPoint, new Vector2(wallCheckLegth, capsuleCollider2D.size.y - 0.1f), 0, whatIsWall);
        if (targetsInRadius.Length > 0)
            return true;

        return false;
    }

    private bool IsGround()
    {
        Vector2 checkGroundPoint = new Vector2(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y - (capsuleCollider2D.size.y + groundCheckLegth) / 2);
        Collider2D[] targetsInRadius = Physics2D.OverlapBoxAll(checkGroundPoint, new Vector2(capsuleCollider2D.size.x - 0.1f, groundCheckLegth), 0, whatIsGround);

        if (targetsInRadius.Length > 0)
            return true;
            
        return false;
    }

    private bool IsClimb()
    {
        bool tempIsClimb = isCanClimb && ClimbCheck();

        if (tempIsClimb)
        {
            rigidbody2D.gravityScale = 0;
            animator.speed = Mathf.Clamp01(Mathf.Abs(moveVector.y) * 5);
            if (moveVector.y > 0)
                crrentLadder.SetEffectorPlatform(true);
            else if (moveVector.y < 0)
                crrentLadder.SetEffectorPlatform(false);
        }
        else if (crrentLadder != null)
        {
            rigidbody2D.gravityScale = originalGravity;
            animator.speed = 1;
            crrentLadder.SetEffectorPlatform(true);
            crrentLadder = null;
        }

        return tempIsClimb;
    }
    
    private bool ClimbCheck()
    {
        Vector2 ladderCheckPointTop = new Vector2(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y + capsuleCollider2D.size.y);
        Vector2 ladderCheckPointBottom = new Vector2(capsuleCollider2D.bounds.center.x, capsuleCollider2D.bounds.center.y - capsuleCollider2D.size.y);
        Collider2D[] ladderCheckTop = Physics2D.OverlapBoxAll(ladderCheckPointTop, new Vector2(capsuleCollider2D.size.x, 0.1f), 0, whatIsLadder);
        Collider2D[] ladderCheckBottom = Physics2D.OverlapBoxAll(ladderCheckPointBottom, new Vector2(capsuleCollider2D.size.x, 0.1f), 0, whatIsLadder);
        
        if (ladderCheckTop.Length > 0 && ladderCheckBottom.Length > 0)     //梯子中
        {
            crrentLadder = ladderCheckTop[0].GetComponent<Ladder>();
            if (isClimb)                //已經在爬
                return true;
            else
                return (moveVector.y != 0) ? true : false;
        }
        else if (ladderCheckTop.Length > 0)                                 //梯子在上
        {
            crrentLadder = ladderCheckTop[0].GetComponent<Ladder>();
            if (isClimb)                //已經在爬
                return (isGround) ? false : true;
            else
            {
                if (moveVector.y > 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + groundCheckLegth * 2);    //避免往上爬 馬上判斷為地面直接墜落
                    return true;
                }
                else
                    return false;
            }
        }
        else if (ladderCheckBottom.Length > 0)                              //梯子在下
        {
            crrentLadder = ladderCheckBottom[0].GetComponent<Ladder>();
            if (isClimb)                //已經在爬
            {
                if (isGround)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + groundCheckLegth * 2);    //避免往上爬離開梯子EffectorCollider太慢  人物穿過collider直接墜落
                    return false;
                }
                else
                    return true;
            }
            else
            {
                if (moveVector.y < 0)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - groundCheckLegth * 2);    //避免往下爬 馬上判斷為地面直接墜落
                    return true;
                }
                else
                    return false;
            }
        }
        else
            return false;
    }
}