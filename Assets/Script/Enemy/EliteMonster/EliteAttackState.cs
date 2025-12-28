using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackState : StateMachineBehaviour
{
    Enemy enemy;
    [SerializeField] private int attackCount = 3;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("attack");
        enemy = animator.GetComponent<Enemy>();

        int randomIndex = 2;

        animator.SetInteger("AttackIndex", randomIndex);
        Debug.Log($"Attack Index Selected: {randomIndex}");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.atkDelay = enemy.atkCoolTime;
    }
}
