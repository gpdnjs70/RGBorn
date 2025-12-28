using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HP : Entity
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Enemy enemy;

    [SerializeField] private AttributeDatabase attributeDB;
    [SerializeField] private PlayerAttribute playerAttribute;
    [SerializeField] private EnemyAttribute enemyAttribute;

    [SerializeField] private UI_HP hpUI;
    private bool isDead = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        base.Setup();
    }

    private void Update()
    {
        
    }

    public override float MaxHP => 200;
    public override float HPRecovery => 15;

    public override void TakeDamage(float damage)
    {
        ColorAttributeData atkAttr = attributeDB.Get(playerAttribute.currentMode);
        ColorAttributeData defAttr = attributeDB.Get(enemyAttribute.colorMode);

        float finalDamage = damage;

        if (enemy != null &&
        enemy.currentAttackType == EnemyAttackType.Skill1)
        {
            float reduction = defAttr.damageReductionRate;

            if (defAttr.strongAgainst.Contains(atkAttr.colorMode))
            {
                reduction += 0.1f;
            }

            if (defAttr.weakAgainst.Contains(atkAttr.colorMode))
            {
                reduction -= 0.1f;
            }

            finalDamage *= (1f - reduction);
            Debug.Log("Skill1 중 → 피해 감소");
        }

        Debug.Log($"데미지는 {finalDamage}");

        HP -= finalDamage;

        if (HP <= 0)
        {
            Die();
            return;
        }

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

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy Dead");

        // 충돌 제거
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // 이동/공격 중단
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.enabled = false;
        }

        if (EnemyManager.Instance != null && enemy != null)
            EnemyManager.Instance.OnEnemyDied(enemy);

        // 죽는 애니메이션
        if (animator != null)
            animator.SetTrigger("Damaged");

        // HP UI 제거
        if (hpUI != null)
            Destroy(hpUI.gameObject, 0.5f);

        // Enemy 제거
        Destroy(gameObject, 0.5f); // 애니메이션 길이에 맞춰 조절
    }
}
