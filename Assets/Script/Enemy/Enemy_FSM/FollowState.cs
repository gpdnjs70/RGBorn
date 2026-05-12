using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;
    // private float soundTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();

        // soundTimer = 0f;

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = entity.Speed;
            agent.isStopped = false; // 이동 재개
            // 일반 몬스터는 멈추는 거리를 공격 사거리에 맞춤
            agent.stoppingDistance = 1.4f;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PauseManager.IsPaused) return;

        /**
        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0f && enemy.followSound != null)
        {
            SoundManager.instance.SFXPlay(enemy.gameObject.name + "_Follow", enemy.followSound);

            // 재생 간격 설정 (예: 0.5초마다 하나씩). 
            // enemy.stepInterval 변수가 없다면 그냥 0.5f 같은 수치를 넣으세요.
            soundTimer = (enemy.stepInterval > 0) ? enemy.stepInterval : 0.5f;
        }
        **/

        if (enemy.player == null) return;

        float distance = Vector2.Distance(enemy.player.position, enemyTransform.position);

        // 1. 추격 포기 (거리가 너무 멀어짐)
        if (distance > 10f)
        {
            animator.SetBool("isFollow", false);
            return;
        }

        // 2. 공격 조건 체크
        if (distance <= 1.5f)
        {
            if (enemy.atkDelay <= 0)
            {
                agent.isStopped = true; // 공격 시 정지
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

            // NavMeshAgent가 가고자 하는 방향(steeringTarget)으로 스프라이트 반전
            enemy.DirectionEnemy(agent.steeringTarget);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}
