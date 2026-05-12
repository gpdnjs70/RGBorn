using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem_idleState : StateMachineBehaviour
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

        // 1. 에이전트 정지: Idle 상태가 되면 이전 상태의 이동 명령을 취소합니다.
        if (agent != null)
        {
            agent.isStopped = true; // 이동 멈춤
            agent.velocity = Vector3.zero; // 물리적 관성 제거
        }

        timer = Random.Range(2f, 5f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PauseManager.IsPaused) return;

        if (enemy.player == null) return;

        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 추적 조건
        if (dist <= 9f && enemy.atkDelay <= 0)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

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
