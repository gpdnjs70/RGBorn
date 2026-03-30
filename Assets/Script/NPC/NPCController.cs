using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayDamagedAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("Damaged");
        }
    }
}
