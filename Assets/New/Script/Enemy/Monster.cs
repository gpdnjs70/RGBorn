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
            HP = 0;
            Die();
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

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망");

        // 죽는 애니메이션 실행
        if (animator != null)
        {
            // animator.SetTrigger("Die"); 
        }

        // 전투 핸들러에게 이 캐릭터가 죽었음을 알림
        BattleHandler battleHandler = FindObjectOfType<BattleHandler>();
        if (battleHandler != null)
        {
            battleHandler.OnCharacterDeath(GetComponent<CharacterBattle>());
        }

        // 삭제
        Destroy(gameObject);
    }
}
