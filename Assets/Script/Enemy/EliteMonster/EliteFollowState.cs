using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EliteFollowState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    Entity entity;
    NavMeshAgent agent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("follow");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        entity = enemy.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /**if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 4)
        {
            animator.SetBool("isBack", true);
            animator.SetBool("isFollow", false);
        }
        else**/
        if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 3f)
        {
            /**
            float finalSpeed = entity.Speed;
            Debug.Log($"현재 속도는 {finalSpeed}");
            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, enemy.player.position, Time.deltaTime * finalSpeed);
            **/

            agent.isStopped = false;
            agent.speed = entity.Speed;
            agent.SetDestination(enemy.player.position);
        }
        else
        {
            // animator.SetBool("isBack", false);

            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetBool("isFollow", false);
        }
        enemy.DirectionEnemy(enemy.player.position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null) agent.isStopped = true;
    }
}
