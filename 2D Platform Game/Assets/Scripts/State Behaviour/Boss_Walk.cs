using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Walk : StateMachineBehaviour {
    
    public float speed = 1f;

    private float timer;
    private Boss boss;
    private bool right;
    private Vector2 target;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject.GetComponent<Boss>();
        target = new Vector2(right ? boss.walkPoint[1].position.x : boss.walkPoint[0].position.x, animator.transform.position.y);
        animator.transform.localEulerAngles = right ? new Vector2(0, 0) : new Vector2(0, 180);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);
        Vector2 animatorPos = animator.transform.position;

        if (Mathf.Abs(target.x - animatorPos.x)  < 0.3f)
        {
            right = !right;
            animator.SetTrigger("Idle");
        }
    }
}
