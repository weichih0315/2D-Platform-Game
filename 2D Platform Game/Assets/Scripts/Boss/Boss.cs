using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy {

    public Vector2 damageKnockbackForce;

    public Animator animator;
    public LayerMask whatIsEnemy;
    public GameObject UICanvas;
    public Slider healthBar;

    [Header("Walk")]
    public Transform[] walkPoint;

    [Header("Attack01")]
    public Boss_Attack01_Rock attack01_Rock;
    public float attack01Damage;
    public Transform attack01Point;

    [Header("Attack02")]
    public Projectile attack02_Rock;
    public float attack02Damage;
    public Transform attack02Point;

    [ContextMenu("StartBattle")]
    public void StartBattle()
    {
        animator.SetTrigger("Idle");
        UICanvas.SetActive(true);
    }

    public void Attack01()
    {
        Boss_Attack01_Rock rock = Instantiate(attack01_Rock, attack01Point.position, attack01Point.transform.rotation);
        rock.damage = attack01Damage;
    }

    public void Attack02()
    {
        Projectile rock = Instantiate(attack02_Rock, attack02Point.position, Quaternion.identity);
        rock.damage = attack02Damage;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.value = health / startingHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector2 dirToTarget = (collision.transform.position - transform.position).normalized;

            Player player = collision.gameObject.GetComponent<Player>();
            if (player.isInvincible)
                return;
            player.TakeDamage(damage);

            Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(new Vector2(dirToTarget.x > 0 ? 1 : -1, 1) * damageKnockbackForce);
        }
    }
}
