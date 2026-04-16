using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Golem_followState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    private Vector2 targetPosition; // 이동할 최종 목적지
    private float maxMoveDistance = 10f; // 최대 이동 거리 설정

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();

        enemy.atkDelay = enemy.atkCoolTime;

        Vector2 direction = ((Vector2)enemy.player.position - (Vector2)enemyTransform.position).normalized;
        targetPosition = (Vector2)enemyTransform.position + (direction * maxMoveDistance);
        
        enemy.DirectionEnemy(targetPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float finalSpeed = entity.Speed;

        Vector2 currentPos = enemyTransform.position;
        Vector2 moveDir = (targetPosition - currentPos).normalized;

        Vector2 separation = Vector2.zero;
        float detectionRadius = 1.2f;

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(currentPos, detectionRadius);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != animator.gameObject && neighbor.CompareTag("Enemy"))
            {
                Vector2 diff = currentPos - (Vector2)neighbor.transform.position;
                separation += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
            }
        }

        Vector2 finalDir = (moveDir + separation * 0.3f).normalized;

        enemyTransform.position += (Vector3)finalDir * finalSpeed * Time.deltaTime;

        float distToTarget = Vector2.Distance(enemyTransform.position, targetPosition);
        float distToPlayer = Vector2.Distance(enemyTransform.position, enemy.player.position);

        if (distToPlayer <= 1.1f)
        {
            enemy.DirectionEnemy(enemy.player.position);
            enemy.OnAttackEvent();

            animator.SetTrigger("isFall");
            animator.SetBool("isFollow", false);
            return;
        }

        if (enemy.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Wall")))
        {
            // Debug.Log("벽에 충돌함!");
            animator.SetTrigger("isFall");
            animator.SetBool("isFollow", false);
            return;
        }

        if (distToTarget < 0.1f)
        {
            animator.SetBool("isFollow", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
