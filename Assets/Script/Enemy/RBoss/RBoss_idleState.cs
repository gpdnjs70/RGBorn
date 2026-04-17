using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBoss_idleState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("idle");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DirectionEnemy(enemy.player.position);

        if (enemy.atkDelay <= 0)
        {
            enemy.atkDelay = enemy.atkCoolTime;
            enemy.StartPattern();
        }

        // animator.SetBool("isReady", true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}