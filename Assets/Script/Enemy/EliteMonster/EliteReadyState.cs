using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteReadyState : StateMachineBehaviour
{
    Transform enemyTransform;
    EliteMonster enemy;
    int selectedIndex = 1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ready");
        enemy = animator.GetComponent<EliteMonster>();
        enemyTransform = animator.GetComponent<Transform>();
        animator.ResetTrigger("Attack");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector2.Distance(enemy.player.position, enemyTransform.position) <= 3f && enemy.atkDelay <= 0)
        {
            DetermineNextAttack(animator);
        }

        if (Vector2.Distance(enemy.player.position, enemyTransform.position) > 3f)
        {
            animator.SetBool("isFollow", true);
        }

        enemy.DirectionEnemy(enemy.player.position);
    }

    private void DetermineNextAttack(Animator animator)
    {
        if (selectedIndex == 0 && enemy.skill1Delay <= 0)
        {
            selectedIndex = 1;
        }
        else
        {
            selectedIndex = 0;
        }

        enemy.atkDelay = enemy.atkCoolTime;
        animator.SetInteger("AttackIndex", selectedIndex);
        animator.SetTrigger("Attack");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
