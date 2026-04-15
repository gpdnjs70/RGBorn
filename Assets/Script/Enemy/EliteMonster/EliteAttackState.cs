using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("doAttack", false);

        Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();
        animator.SetBool("isReady", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("doAttack", false);
        animator.SetInteger("AttackIndex", -1);

        Debug.Log("공격 종료");

        var attackEvent = animator.GetComponent<MidBossAttackEvent>();
        if (attackEvent != null)
        {
            attackEvent.ResetAllAttackFlags();
        }

        // enemy.atkDelay = enemy.atkCoolTime;
    }
}
