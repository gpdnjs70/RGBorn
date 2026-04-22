using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class slime_attr_patrolState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Enemy enemy;
    private Transform enemyTransform;
    private float wanderRange = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("patrol");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.transform;

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = enemy.GetComponent<Entity>().Speed * 0.5f;
            agent.isStopped = false;

            // NavMesh 영역 내 유효한 랜덤 좌표 가져오기
            Vector3 targetPos = GetRandomNavMeshPos(enemyTransform.position, wanderRange);
            agent.SetDestination(targetPos);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 플레이어 발견 시 추적
        if (dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        // 방향 업데이트
        if (!agent.isStopped)
        {
            enemy.DirectionEnemy(agent.steeringTarget);
        }

        // 목적지 도착 체크
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            animator.SetBool("isWander", false);
        }
    }

    private Vector3 GetRandomNavMeshPos(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitCircle * dist;
        randDir += origin;
        NavMeshHit navHit;
        // 2D 영역 내에서 유효한 바닥 좌표를 찾음
        if (NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas))
        {
            return new Vector3(navHit.position.x, navHit.position.y, 0);
        }
        return origin;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}
