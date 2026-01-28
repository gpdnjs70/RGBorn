using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossReadyState : StateMachineBehaviour
{
    Transform enemyTransform;
    MidBoss enemy;
    int selectedIndex = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ready");
        enemy = animator.GetComponent<MidBoss>();
        enemyTransform = animator.GetComponent<Transform>();
        animator.ResetTrigger("Attack");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector2.Distance(enemy.player.position, enemyTransform.position) <= 10f && enemy.atkDelay <= 0)
        {
            DetermineNextAttack(animator);
        }

        enemy.DirectionEnemy(enemy.player.position.x, enemyTransform.position.x);
    }

    private void DetermineNextAttack(Animator animator)
    {
        if (enemy.phase == 1 && enemy.passiveSkillDelay <= 0 && enemy.passiveSkillCount < enemy.maxPassiveCount)
        {
            selectedIndex = 0;

            enemy.passiveSkillDelay = enemy.passiveSkillCoolTime;
            enemy.passiveSkillCount++; // 사용 횟수 증가
        }
        else if (enemy.skill1Delay <= 0)
        {
            if (enemy.phase == 1)
            {
                selectedIndex = Random.Range(1, 4);
            }
            else if (enemy.phase == 2)
            {
                selectedIndex = Random.Range(4, 7);
            }

            enemy.skill1Delay = enemy.skill1CoolTime;
        }

        selectedIndex = 6;

        Debug.Log($"attackState 호출, selectedIndex: {selectedIndex}");

        enemy.atkDelay = enemy.atkCoolTime;
        animator.SetInteger("AttackIndex", selectedIndex);
        animator.SetTrigger("Attack");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isReady", false);
    }
}