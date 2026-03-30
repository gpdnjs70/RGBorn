using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    public Animator animator;

    public float idleDuration = 2.0f;
    public float attackRecoveryTime = 1.0f;

    public NPCController targetNPC;


    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        StartCoroutine(SlimeBehaviorLoop());
    }

    private IEnumerator SlimeBehaviorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleDuration);

            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackRecoveryTime);
        }
    }

    public void TriggerDamageEvent()
    {
        if (targetNPC != null)
        {
            targetNPC.PlayDamagedAnimation(); // NPC에게 맞았다고 알려줌
        }
    }
}
