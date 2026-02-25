using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Monster : character, IDebuffTarget2
{
    public character character => this;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private AttributeDatabase attributeDB;
    [SerializeField] public PlayerAttribute playerAttribute;
    [SerializeField] private EnemyAttribute enemyAttribute;

    [SerializeField] public HP hpUI;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        HP = 50f;
        base.Setup();
    }

    public override void TakeDamage(float damage)
    {
        float finalDamage = damage * DefenseMultiplier;

        Debug.Log($"데미지는 {finalDamage}");

        HP -= finalDamage;

        if (HP <= 0)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }

        StartCoroutine("HitAnimation");
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
