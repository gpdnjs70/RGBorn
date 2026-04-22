using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class slime_attr_idleState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    Transform enemyTransform;
    Enemy enemy;
    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("idle");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        agent = animator.GetComponent<NavMeshAgent>();

        // 에이전트 정지 및 물리적 관성 제거
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        timer = Random.Range(2f, 5f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.player == null) return;

        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 1. 공격 조건 (사거리 2f)
        if (dist <= 2f && enemy.atkDelay <= 0)
        {
            enemy.atkDelay = enemy.atkCoolTime;
            enemy.DirectionEnemy(enemy.player.position);
            animator.SetTrigger("Attack");
            return;
        }

        // 2. 추적 조건 (인식 범위 9f)
        if (dist > 2f && dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        // 3. 정찰(Patrol) 전환
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
