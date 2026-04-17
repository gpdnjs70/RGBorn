using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_idleState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("idle");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        timer = Random.Range(2f, 5f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
