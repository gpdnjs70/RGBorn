using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("back");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Vector2.Distance(enemy.home, enemyTransform.position) < 0.1f || Vector2.Distance(enemyTransform.position, enemy.player.position) < 4)
        {
            animator.SetBool("isBack", false);
        }
        else
        {
            enemy.DirectionEnemy(enemy.home.x, enemyTransform.position.x);

            float finalSpeed = entity.MoveSpeed;
            Debug.Log($"현재 속도는 {finalSpeed}");
            enemyTransform.position = Vector2.MoveTowards(enemy.transform.position, enemy.home, Time.deltaTime * finalSpeed);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
