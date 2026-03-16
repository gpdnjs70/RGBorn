using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Long_followState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector2.Distance(enemy.player.position, enemyTransform.position);

        // 추적 중지 조건
        if (distance > 15f) // 너무 멀어졌을때
        {
            animator.SetBool("isFollow", false);
            return;
        }

        if (distance <= 6f) // 플레이어와 가까워졌을때
        {
            if (enemy.atkDelay <= 0)
            {
                enemy.atkDelay = enemy.atkCoolTime; // 쿨타임 선적용
                enemy.DirectionEnemy(enemy.player.position);
                animator.SetTrigger("Attack"); // Ready로 바로 보냄
            }

            animator.SetBool("isFollow", false);
            return; // 이후 이동 로직 스킵
        }

        // 이동 로직
        float finalSpeed = entity.Speed;
        enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, enemy.player.position, Time.deltaTime * finalSpeed);

        // 방향 및 공격박스 업데이트
        enemy.DirectionEnemy(enemy.player.position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

