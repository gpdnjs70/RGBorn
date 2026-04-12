using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();
        animator.SetBool("isReady", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("공격 종료");
        // enemy.atkDelay = enemy.atkCoolTime;
    }
}
