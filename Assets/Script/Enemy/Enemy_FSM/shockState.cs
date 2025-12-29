using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockState : StateMachineBehaviour
{
    override public void OnStateEnter(
        Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Shock State Enter");
    }

    override public void OnStateUpdate(
        Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 아무 것도 안 함
    }

    override public void OnStateExit(
        Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Shock State Exit");
    }
}
