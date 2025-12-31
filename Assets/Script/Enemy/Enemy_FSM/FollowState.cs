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
        /**if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 4)
        {
            animator.SetBool("isBack", true);
            animator.SetBool("isFollow", false);
        }
        else**/ if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 1f)
        {
            float finalSpeed = entity.Speed;
            Debug.Log($"현재 속도는 {finalSpeed}");
            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, enemy.player.position, Time.deltaTime * finalSpeed);
        }
        else
        {
            // animator.SetBool("isBack", false);
            animator.SetBool("isFollow", false);

            // 쿨타임이 다 끝났으면 공격 애니메이션 트리거
            if (enemy.atkDelay <= 0f)
            {
                animator.SetTrigger("Attack");
            }
        }
        enemy.DirectionEnemy(enemy.player.position.x, enemyTransform.position.x);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
