using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HP : Entity
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        base.Setup();
    }

    private void Update()
    {
        
    }

    public override float MaxHP => 200;
    public override float HPRecovery => 15;

    public override void TakeDamage(float damage)
    {
        HP -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }

        // StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.1f);

        color.a = 1;
        spriteRenderer.color = color;
    }
}
