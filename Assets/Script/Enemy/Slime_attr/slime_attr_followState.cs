using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class slime_attr_followState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = entity.Speed;
            agent.stoppingDistance = 1.9f; // 사거리 2f보다 약간 짧게 설정
            agent.isStopped = false;
        }

        // soundTimer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.player == null) return;

        float distance = Vector2.Distance(enemy.player.position, enemyTransform.position);

        // 1. 추격 포기 (최대 인식 거리 10f 초과 시)
        if (distance > 10f)
        {
            animator.SetBool("isFollow", false);
            return;
        }

        // 2. 사거리 진입 체크 및 공격 실행 (사거리 2f)
        if (distance <= 2f)
        {
            if (enemy.atkDelay <= 0)
            {
                agent.isStopped = true; // 공격 전 이동 정지
                enemy.atkDelay = enemy.atkCoolTime;
                enemy.DirectionEnemy(enemy.player.position);
                animator.SetTrigger("Attack");
                animator.SetBool("isFollow", false);
            }
            return;
        }

        // 3. NavMesh 기반 목적지 설정 (플레이어 추적)
        if (agent != null && !agent.isStopped)
        {
            agent.SetDestination(enemy.player.position);

            // 에이전트가 가고자 하는 경로의 다음 지점을 바라봄
            enemy.DirectionEnemy(agent.steeringTarget);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}
