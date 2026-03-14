using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Golem_followState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    private Vector2 targetPosition; // 이동할 최종 목적지
    private float maxMoveDistance = 10f; // 최대 이동 거리 설정

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();

        enemy.atkDelay = enemy.atkCoolTime;

        Vector2 direction = ((Vector2)enemy.player.position - (Vector2)enemyTransform.position).normalized;
        targetPosition = (Vector2)enemyTransform.position + (direction * maxMoveDistance);
        
        enemy.DirectionEnemy(targetPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 고정된 목적지로 이동
        float finalSpeed = entity.Speed;
        enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, targetPosition, Time.deltaTime * finalSpeed);

        // 도착 체크
        float distToTarget = Vector2.Distance(enemyTransform.position, targetPosition);
        float distToPlayer = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 돌진 도중 플레이어를 만났는지 확인
        if (distToPlayer <= 1.4f)
        {
            Debug.Log("돌진 중 플레이어 접촉! 공격 시작");
            enemy.DirectionEnemy(enemy.player.position);

            animator.SetBool("isAttack", true);  // 공격 상태 전환
            animator.SetBool("isFollow", false); // 이동 중단

            enemy.atkDelay = enemy.atkCoolTime;
            return;
        }

        // 플레이어를 못 만났는데 목적지(Max 거리)에 도착
        if (distToTarget < 0.1f)
        {
            Debug.Log("플레이어를 못 만나고 최대 거리 도달");
            animator.SetBool("isFollow", false); // Idle 복귀

            enemy.atkDelay = enemy.atkCoolTime;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
