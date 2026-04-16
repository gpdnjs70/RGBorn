using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fluffy_attr_followState : StateMachineBehaviour
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

        if (distance <= 5f)
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

        // 플레이어를 향한 기본 방향
        Vector2 moveDir = ((Vector2)enemy.player.position - currentPos).normalized;

        // 분리 계산
        Vector2 separation = Vector2.zero;
        float detectionRadius = 1.5f; // 원거리 몬스터는 조금 더 넉넉하게 간격 유지
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(currentPos, detectionRadius);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != animator.gameObject && neighbor.CompareTag("Enemy"))
            {
                Vector2 diff = currentPos - (Vector2)neighbor.transform.position;
                // 너무 가까우면 강하게 밀어냄
                separation += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
            }
        }

        // 최종 이동 벡터
        Vector2 finalDir = (moveDir + separation * 0.5f).normalized;

        // 이동 적용
        enemyTransform.position += (Vector3)finalDir * finalSpeed * Time.deltaTime;

        // 플레이어 바라보기
        enemy.DirectionEnemy(enemy.player.position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

