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
    [SerializeField] private EnemyAttributeMono enemyAttributeMono;

    [SerializeField] public UI_HP hpUI;
    private bool isDead = false;

    [SerializeField] public EnemyData data;

    public float detectionRange = 10f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        enemyAttribute = GetComponent<EnemyAttribute>();
        enemyAttributeMono = GetComponent<EnemyAttributeMono>();

        stats = new Stats();
        stats.MaxHP.BaseValue = data.baseHP;
        stats.Speed.BaseValue = data.baseSpeed;

        base.Setup();
    }

    public override void TakeDamage(float damage, GameObject sender = null)
    {
        // float skill1Multiplier = IsSkill1Active();

        float finalDamage = damage * DefenseMultiplier; //* skill1Multiplier;

        Debug.Log($"데미지는 {finalDamage}");
        DamagePopup.Create(transform.position + Vector3.up, (int)finalDamage, false);

        HP -= finalDamage;
        
        if (HP <= 0)
        {
            Die();
            return;
        }

        if (enemy != null && enemy.damagedSound != null)
        {
            SoundManager.instance.SFXPlay(gameObject.name + "_Hit", enemy.damagedSound);
        }

        if (animator != null)
        {
            // animator.SetTrigger("Damaged");
        }

        // StartCoroutine("HitAnimation");
    }

    /**
    private float IsSkill1Active()
    {
        // 스킬 사용 중이 아니면 배율 1 (대미지 변화 없음)
        if (enemy == null || enemy.currentAttackType != EnemyAttackType.Skill1)
            return 1f;

        // 공격자(Player)와 방어자(Enemy)의 컴포넌트 가져오기
        ColorAttribute defAttrComp = enemyAttribute.GetComponent<ColorAttribute>();
        ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

        // --- 상성 관련 로직(if문들)을 여기서 삭제합니다 ---
        float reduction = defAttr.damageReductionRate;

        Debug.Log($"Skill1 중 -> 순수 속성 방어력({reduction}) 적용");

        // 최종 배율 반환 (예: reduction이 0.3이면 0.7배의 대미지만 받음)
        return 1f - reduction;
    }
    **/

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

        if (enemy != null && enemy.dieSound != null)
        {
            SoundManager.instance.SFXPlay(gameObject.name + "_Die", enemy.dieSound);
        }

        Debug.Log("Enemy Dead");

        // 충돌 제거
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // 이동/공격 중단
        if (enemy != null)
        {
            enemy.enabled = false;
        }

        if (EnemyManager.Instance != null && enemy != null && enemy is EliteMonster)
            EnemyManager.Instance.OnEnemyDied(enemy);

        if (MonsterWaveManager.Instance != null)
        {
            MonsterWaveManager.Instance.OnMonsterDied(this.gameObject);
        }

        // 죽는 애니메이션
        if (animator != null)
            animator.SetTrigger("Death");

        bool isMinion = gameObject.name.Contains("Minion");

        if (isMinion)
        {
            // 따까리는 파괴하지 않고 비활성화만!
            StartCoroutine(DeactivateMinion(0.5f));
        }
        else
        {
            // 일반 몬스터는 원래대로 파괴
            if (hpUI != null) Destroy(hpUI.gameObject);
            Destroy(gameObject, 1.5f);
        }
    }

    protected override void FindTarget()
    {
        // 이미 유효한 타겟이 있다면 새로 찾지 않음
        if (IsTargetValid()) return;

        // 주변의 'Player' 레이어를 가진 객체 검색
        Collider2D col = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        if (col != null)
        {
            target = col.GetComponent<Entity>();
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
