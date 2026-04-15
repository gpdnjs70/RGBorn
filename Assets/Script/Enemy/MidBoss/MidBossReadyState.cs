using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossReadyState : StateMachineBehaviour
{
    Transform enemyTransform;
    MidBoss enemy;
    int selectedIndex = 0;
    private int patternStep = 0;
    private bool isPatternSelected = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isPatternSelected = false;
        Debug.Log("ready");
        enemy = animator.GetComponent<MidBoss>();
        enemyTransform = animator.GetComponent<Transform>();
        //animator.ResetTrigger("Attack");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isPatternSelected && enemy.atkDelay <= 0)
        {
            isPatternSelected = true;
            Debug.Log("조건 만족! 공격 시도");
            DetermineNextAttack(animator);
        }

        enemy.DirectionEnemy(enemy.player.position);
    }

    private void DetermineNextAttack(Animator animator)
    {
        animator.SetBool("doAttack", false);

        switch (patternStep)
        {
            case 0: // 장판 (플레이어 쪽) -> 파이어볼
                selectedIndex = 1;
                patternStep = 1;
                break;
            case 1: // 순간이동 -> 지뢰 (보스 주변) -> 파이어볼
                selectedIndex = 4;
                patternStep = 0; // 다시 처음으로
                break;
        }

        Debug.Log($"attackState 호출, selectedIndex: {selectedIndex}");

        enemy.atkDelay = enemy.atkCoolTime;
        animator.SetInteger("AttackIndex", selectedIndex);
        animator.SetBool("doAttack", true);

        animator.SetBool("isReady", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}