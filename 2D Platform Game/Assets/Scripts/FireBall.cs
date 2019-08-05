using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float damage = 1f;

    public LayerMask collisionMask;
    public LayerMask whatIsEnemy;
    public Vector2 velocity;
    public GameObject explodeEffect;

    private Rigidbody2D rigidbody2D;
    private bool right;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        right = player.transform.localEulerAngles.y == 0 ? true : false;

        rigidbody2D.velocity = right ? new Vector2(velocity.x, rigidbody2D.velocity.y) : new Vector2(-velocity.x, rigidbody2D.velocity.y);
    }

    private void Update()
    {
        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius, whatIsEnemy);

        if (targetsInRadius.Length > 0)
        {
            Enemy enemy = targetsInRadius[0].gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            Destroy(Instantiate(explodeEffect, transform.position, Quaternion.identity), 0.1f);
            Destroy(gameObject);
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.x != 0)
        {
            Destroy(Instantiate(explodeEffect, transform.position, Quaternion.identity), 0.1f);
            Destroy(gameObject);
        }
        else
        {
            rigidbody2D.velocity = right ? new Vector2(velocity.x, -velocity.y) : new Vector2(-velocity.x, -velocity.y);
        }
    }
}
