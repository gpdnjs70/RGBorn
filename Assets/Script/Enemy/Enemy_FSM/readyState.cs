using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readyState : StateMachineBehaviour
{
    Enemy enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("ready");
        if (enemy == null) enemy = animator.GetComponent<Enemy>();
        if (enemy != null && enemy.readySound != null)
        {
            SoundManager.instance.SFXPlay(enemy.gameObject.name + "_Ready", enemy.readySound);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PauseManager.IsPaused) return;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
