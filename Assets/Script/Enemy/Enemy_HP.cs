using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HP : Entity, IDebuffTarget
{
    public Entity Entity => this;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Enemy enemy;

    [SerializeField] private AttributeDatabase attributeDB;
    [SerializeField] public PlayerAttribute playerAttribute;
    [SerializeField] private EnemyAttribute enemyAttribute;

    [SerializeField] public UI_HP hpUI;
    private bool isDead = false;

    [SerializeField] public EnemyData data;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        stats = new Stats();
        stats.MaxHP.BaseValue = data.baseHP;
        stats.Speed.BaseValue = data.baseSpeed;

        base.Setup();
    }

    public override void TakeDamage(float damage)
    {
        float skill1Multiplier = IsSkill1Active();

        float finalDamage = damage * DefenseMultiplier * skill1Multiplier;

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

    private float IsSkill1Active()
    {
        if (enemy == null || enemy.currentAttackType != EnemyAttackType.Skill1)
            return 1f;

        ColorAttribute atkAttrComp = playerAttribute.GetComponent<ColorAttribute>();
        ColorAttribute defAttrComp = enemyAttribute.GetComponent<ColorAttribute>();

        ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);
        ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

        float reduction = defAttr.damageReductionRate;

        if (defAttr.strongAgainst.Contains(atkAttr.colorMode))
            reduction += 0.1f;

        if (defAttr.weakAgainst.Contains(atkAttr.colorMode))
            reduction -= 0.1f;

        Debug.Log("Skill1 중 → 피해 감소");

        // reduction 0.3 → multiplier 0.7
        return 1f - reduction;
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

        if (EnemyManager.Instance != null && enemy != null && enemy is EliteMonster)
            EnemyManager.Instance.OnEnemyDied(enemy);

        // 죽는 애니메이션
        if (animator != null)
            animator.SetTrigger("Damaged");

        bool isMinion = gameObject.name.Contains("Minion");

        if (isMinion)
        {
            // 따까리는 파괴하지 않고 비활성화만!
            StartCoroutine(DeactivateMinion(0.5f));
        }
        else
        {
            // 일반 몬스터는 원래대로 파괴
            if (hpUI != null) Destroy(hpUI.gameObject, 0.5f);
            Destroy(gameObject, 0.5f);
        }
    }

    private IEnumerator DeactivateMinion(float delay)
    {
        yield return new WaitForSeconds(delay);

        // HP UI 파괴하지 말고 비활성화
        if (hpUI != null) hpUI.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
