using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class patrolState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Enemy enemy;
    private Transform enemyTransform;
    private float wanderRange = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("patrol");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.transform;

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // 1. 에이전트 속도 설정 (산책이므로 평소보다 느리게)
            agent.speed = enemy.GetComponent<Entity>().Speed * 0.5f;
            agent.isStopped = false;

            // 2. NavMesh 영역 내에서 유효한 랜덤 목적지 설정
            Vector3 targetPos = GetRandomNavMeshPos(enemyTransform.position, wanderRange);
            agent.SetDestination(targetPos);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.player == null) return;

        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 플레이어 발견 시 산책 중단하고 추적
        if (dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        // 4. 방향 애니메이션 업데이트 (NavMesh가 계산한 다음 길 지점 바라보기)
        if (!agent.isStopped)
        {
            enemy.DirectionEnemy(agent.steeringTarget);
        }

        // 5. 목적지에 거의 도착했거나, 경로가 막혔을 때 Idle로 복귀
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            animator.SetBool("isWander", false);
        }
    }

    // NavMesh 바닥 안에서 랜덤 좌표를 찾는 핵심 함수
    private Vector3 GetRandomNavMeshPos(Vector3 origin, float dist)
    {
        // 2D 랜덤 방향 벡터 생성
        Vector3 randDir = Random.insideUnitCircle * dist;
        randDir += origin;

        NavMeshHit navHit;
        // 설정한 반경 내에서 가장 가까운 유효한 NavMesh 지점을 찾습니다.
        if (NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas))
        {
            return new Vector3(navHit.position.x, navHit.position.y, 0); // Z값 고정
        }

        return origin; // 실패 시 현재 위치 반환
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}
