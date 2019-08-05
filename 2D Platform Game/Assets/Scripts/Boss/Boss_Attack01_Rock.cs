using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Attack01_Rock : MonoBehaviour {

    public float speed;
    public float damage;
    public ParticleSystem effect;

    private float timer;

    private void Start()
    {
        Destroy(gameObject, 5);
        timer = 0;
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if (timer <= 0)
        {
            timer = 0.01f;
            Destroy(Instantiate(effect, transform.position, Quaternion.identity).gameObject, effect.main.startLifetime.constant);
        }
        else
            timer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.transform.GetComponent<Player>();
            if (player.playerState == Player.PlayerState.Hurt)
                return;
            player.TakeDamage(damage);
        }
    }
}