using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_attackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();
        animator.SetBool("isAttack", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.atkDelay = enemy.atkCoolTime;
    }
}

