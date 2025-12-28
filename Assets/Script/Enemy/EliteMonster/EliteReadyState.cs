using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteReadyState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ready");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector2.Distance(enemy.player.position, enemyTransform.position) <= 3f && enemy.atkDelay <= 0)
        {
            animator.SetTrigger("Attack");
        }

        if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 3f)
        {
            animator.SetBool("isFollow", true);
        }

        enemy.DirectionEnemy(enemy.player.position.x, enemyTransform.position.x);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
