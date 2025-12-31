using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class attackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();

        if(enemy != null && enemy.targetHP != null)
        {
            int damage = 2;

            if (UnityEngine.Random.value < 0.1f)
            {
                damage = 5;
            }

            enemy.DealDamageToTarget(damage, 3f);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.atkDelay = enemy.atkCoolTime;
    }
}
