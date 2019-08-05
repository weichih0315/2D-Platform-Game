using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    public float damage;
    public float speed;
    public int money;
    public int score;
    
    public override void Die()
    {
        GameManager.instance.AddMoney(money);
        GameManager.instance.AddScore(score);
        base.Die();
    }
}
