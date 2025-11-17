using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class attackState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        if(enemy != null && enemy.targetHP != null)
        {
            int damage = Random.Range(1, 100);
            enemy.targetHP.TakeDamage(damage);
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
