using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Long_patrolState : StateMachineBehaviour
{
    private Vector2 targetPos;
    private Enemy enemy;
    private Transform enemyTransform;
    private float wanderRange = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("patrol");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.transform;

        float moveDistance = Random.Range(1.0f, wanderRange);

        if (Random.value > 0.5f) moveDistance *= -1;

        targetPos = new Vector2(enemyTransform.position.x + moveDistance, enemyTransform.position.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 플레이어 발견 시 산책 중단하고 추적
        if (dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        // 목표 지점까지 이동
        enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, targetPos, Time.deltaTime * (enemy.GetComponent<Entity>().Speed * 0.5f));
        enemy.DirectionEnemy(targetPos);

        // 도착하면 다시 Idle로
        if (Vector2.Distance(enemyTransform.position, targetPos) < 0.1f)
        {
            animator.SetBool("isWander", false);
            return;
        }
    }
}

