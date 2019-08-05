using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 1f;
    public float damage = 1f;

    public ParticleSystem hitEffect;
    private Vector3 direction;

    private Rigidbody2D rigidbody2D;
    private GameObject player;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        Vector3 playerPos = player.transform.position;
        CapsuleCollider2D capsuleCollider2D = player.GetComponent<CapsuleCollider2D>();
        Vector3 target = new Vector3(playerPos.x, playerPos.y + capsuleCollider2D.size.y, playerPos.z);
        direction = (target - transform.position).normalized;;
    }

    private void Update()
    {
        rigidbody2D.velocity = direction * speed;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.transform.GetComponent<Player>();
            if (player.playerState == Player.PlayerState.Hurt)
                return;
            player.TakeDamage(damage);
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), hitEffect.main.startLifetime.constant);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Ground")
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), hitEffect.main.startLifetime.constant);
            Destroy(gameObject);
        }
    }
}