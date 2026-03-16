using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Long_attackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();

        if (enemy != null && enemy.player != null)
        {
            Transform spawnPoint = (enemy.attackPoints != null && enemy.attackPoints.Length > 0)
                                   ? enemy.attackPoints[0] // 인덱스는 로직에 맞춰 수정
                                   : enemy.transform;

            enemy.SetLockedDirection(enemy.player.position);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.atkDelay = enemy.atkCoolTime;
        animator.ResetTrigger("Attack");
    }
}

