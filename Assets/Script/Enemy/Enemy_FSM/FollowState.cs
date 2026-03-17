using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 currentPos = enemyTransform.position;
        float distance = Vector2.Distance(enemy.player.position, currentPos);

        if (distance > 10f)
        {
            animator.SetBool("isFollow", false);
            return;
        }

        if (distance <= 1.4f)
        {
            if (enemy.atkDelay <= 0)
            {
                enemy.atkDelay = enemy.atkCoolTime;
                enemy.DirectionEnemy(enemy.player.position);
                animator.SetTrigger("Attack");
            }
            animator.SetBool("isFollow", false);
            return;
        }

        float finalSpeed = entity.Speed;

        // 기본 플레이어 추적 방향
        Vector2 moveDir = ((Vector2)enemy.player.position - currentPos).normalized;

        // 분리 벡터 계산
        Vector2 separation = Vector2.zero;
        float detectionRadius = 1.2f; // 몬스터끼리 유지하고 싶은 최소 거리
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(currentPos, detectionRadius);

        foreach (var neighbor in neighbors)
        {
            // 자신을 제외한 다른 몬스터 태그 확인
            if (neighbor.gameObject != animator.gameObject && neighbor.CompareTag("Enemy"))
            {
                Vector2 diff = currentPos - (Vector2)neighbor.transform.position;
                // 거리가 가까울수록 더 강하게 밀어냄
                separation += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
            }
        }

        // 최종 방향 결정 (플레이어 방향 + 분리 힘 * 가중치)
        Vector2 finalDir = (moveDir + separation * 0.4f).normalized;

        enemyTransform.position += (Vector3)finalDir * finalSpeed * Time.deltaTime;

        enemy.DirectionEnemy(enemy.player.position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
