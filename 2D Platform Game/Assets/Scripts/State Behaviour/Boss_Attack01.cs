using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Attack01 : StateMachineBehaviour {

    private GameObject player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator.transform.localEulerAngles = player.transform.position.x > animator.transform.position.x ? new Vector2(0, 0) : new Vector2(0, 180);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
