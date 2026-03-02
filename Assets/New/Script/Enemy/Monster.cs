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
    [SerializeField] private TurnUI turnUI;

    [SerializeField] private int attackInterval = 3; // 몇 턴마다 공격할지 (예: 3턴마다)
    private int currentTurnWait = 0; // 현재 쌓인 턴 카운트

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        HP = 50f;
        base.Setup();

        UpdateMonsterTurnUI();
    }

    public override void TakeDamage(float damage)
    {
        float finalDamage = damage * DefenseMultiplier;

        Debug.Log($"데미지는 {finalDamage}");

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0);

        // 30% 확률로 크리티컬 연출 (예시)
        bool isCritical = Random.Range(0, 100) < 30;

        DamagePopup.Create(spawnPos, (int)finalDamage, isCritical);

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

    public bool CheckTurnAndPrepareAttack()
    {
        currentTurnWait++;

        UpdateMonsterTurnUI();

        if (currentTurnWait >= attackInterval)
        {
            currentTurnWait = 0; // 카운트 초기화

            if (!CanAct) // 마비 상태
            {
                Debug.Log($"{gameObject.name}이 공격할 차례지만 마비되어 공격을 씹었습니다!");
                UpdateMonsterTurnUI();
                return false; // 공격 안 함
            }
           
            // 여기서 애니메이션이나 UI 연출을 미리 실행할 수 있음
            return true; // 공격
        }

        // 공격 안 할 때는 UI 숫자만 갱신하거나 기 모으는 연출
        return false; // 공격 아님
    }

    public void UpdateMonsterTurnUI()
    {
        if (turnUI != null)
        {
            // 남은 턴 계산
            int remainingTurn = attackInterval - currentTurnWait - 1;
            turnUI.UpdateTurnText(remainingTurn);
        }
    }

    public void DelayAttack(int amount)
    {
        // currentTurnWait이 작아질수록 남은 턴(attackInterval - currentTurnWait)은 늘어납니다.
        currentTurnWait -= amount;

        // UI 업데이트 (남은 턴 숫자가 즉시 늘어남)
        UpdateMonsterTurnUI();
    }
}
