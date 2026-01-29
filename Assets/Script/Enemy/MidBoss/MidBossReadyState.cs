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
        else
        {
            if (enemy.phase == 1)
            {
                // selectedIndex = Random.Range(1, 4);
                if(selectedIndex == 0 || selectedIndex == 3) { selectedIndex = 1; }
                else if (selectedIndex == 1) { selectedIndex = 2; }
                else if (selectedIndex == 2) { selectedIndex = 3; }
            }
            else if (enemy.phase == 2)
            {
                if (selectedIndex == 0 || selectedIndex == 3 || selectedIndex == 6) { selectedIndex = 4; }
                else if (selectedIndex == 4) { selectedIndex = 5; }
                else if (selectedIndex == 5) { selectedIndex = 6; }
            }

            enemy.skill1Delay = enemy.skill1CoolTime;
        }

        // selectedIndex = 4;

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