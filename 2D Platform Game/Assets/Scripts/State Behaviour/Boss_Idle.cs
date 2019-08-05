using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour {

    public float minTime;
    public float maxTime;

    private float timer;
    private GameObject player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = Random.Range(minTime, maxTime);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.localEulerAngles = player.transform.position.x > animator.transform.position.x? new Vector2(0, 0) : new Vector2(0,180);

        if (timer <= 0)
        {
            Vector2 playerPos = player.transform.position;
            Vector2 animatorPos = animator.transform.position;

            float rand = Random.Range(0, 2);
            if (rand < 1)
                animator.SetTrigger("Walk");
            else
            {
                if (playerPos.y - animatorPos.y < 0.5)
                {
                    float rand2 = Random.Range(0, 2);
                    if (rand2 < 1)
                        animator.SetTrigger("Attack01");
                    else
                    {
                        animator.SetTrigger("Attack02");
                    }
                }
                else
                    animator.SetTrigger("Attack02");
            }
        }
        else
            timer -= Time.deltaTime;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
