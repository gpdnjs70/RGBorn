using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Long_idleState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    Transform enemyTransform;
    Enemy enemy;
    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("idle");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        agent = animator.GetComponent<NavMeshAgent>();

        // 이동 중지 및 속도 초기화
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        timer = Random.Range(2f, 5f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PauseManager.IsPaused) return;

        if (enemy.player == null) return;

        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 1. 공격 사거리(6f) 안이고 쿨타임 완료 시 공격
        if (dist <= 6f && enemy.atkDelay <= 0)
        {
            enemy.atkDelay = enemy.atkCoolTime;
            enemy.DirectionEnemy(enemy.player.position);
            animator.SetTrigger("Attack");
            return;
        }

        // 2. 공격 사거리 밖이지만 인식 범위(9f) 안이면 추격
        if (dist > 6f && dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        // 3. 무작위 산책 전환
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            animator.SetBool("isFollow", false);
            animator.SetBool("isWander", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

