using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Long_followState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = entity.Speed;
            agent.isStopped = false;
            // 핵심: 원거리 몬스터는 정지 거리를 사거리(6f)에 가깝게 설정합니다.
            // 5.5f 정도로 설정하면 사거리에 진입하자마자 자연스럽게 멈춥니다.
            agent.stoppingDistance = 5.5f;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PauseManager.IsPaused) return;

        if (enemy.player == null) return;

        float distance = Vector2.Distance(enemy.player.position, enemyTransform.position);

        // 1. 추격 포기 (최대 인식 거리 10f 초과 시)
        if (distance > 10f)
        {
            animator.SetBool("isFollow", false);
            return;
        }

        // 2. 사거리 진입 체크 및 공격 실행
        if (distance <= 6f)
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

        // 3. NavMesh를 통한 추격 (사거리 밖인 경우)
        if (agent != null && !agent.isStopped)
        {
            agent.SetDestination(enemy.player.position);

            // 이동 방향에 맞춰 스프라이트 반전
            enemy.DirectionEnemy(agent.steeringTarget);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}

