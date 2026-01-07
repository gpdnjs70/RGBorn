using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteReadyState : StateMachineBehaviour
{
    Transform enemyTransform;
    EliteMonster enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ready");
        enemy = animator.GetComponent<EliteMonster>();
        enemyTransform = animator.GetComponent<Transform>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector2.Distance(enemy.player.position, enemyTransform.position) <= 3f && enemy.atkDelay <= 0)
        {
            DetermineNextAttack(animator);
        }

        if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 3f)
        {
            animator.SetBool("isFollow", true);
        }

        enemy.DirectionEnemy(enemy.player.position.x, enemyTransform.position.x);
    }

    private void DetermineNextAttack(Animator animator)
    {
        List<int> possibleAttacks = new List<int>();

        // 0번: 기본 공격 (항상 가능)
        possibleAttacks.Add(0);

        // 1번: 스킬1 (쿨타임 체크)
        if (enemy.skill1Delay <= 0) possibleAttacks.Add(1);

        // 랜덤 선택
        int selectedIndex = possibleAttacks[Random.Range(0, possibleAttacks.Count)];

        // 개별 스킬 쿨타임 적용
        if (selectedIndex == 1)
        {
            enemy.skill1Delay = enemy.skill1CoolTime;
        }

        // 전체 공격 쿨타임(공통) 초기화
        enemy.atkDelay = enemy.atkCoolTime;

        // 애니메이터 실행
        animator.SetInteger("AttackIndex", selectedIndex);
        animator.SetTrigger("Attack");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
