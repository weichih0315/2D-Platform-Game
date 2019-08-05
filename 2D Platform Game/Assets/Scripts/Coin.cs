using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public int money;
    public int score;

    public AudioClip coinCollectedAudio;
    public GameObject feedbackEffect;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.instance.AddMoney(money);
            GameManager.instance.AddScore(score);
            animator.SetTrigger("Collected");
            AudioManager.instance.PlaySound2D(coinCollectedAudio);
            Destroy(Instantiate(feedbackEffect, transform.position, Quaternion.identity), 0.3f);
            Destroy(gameObject);
        }
    }
}
