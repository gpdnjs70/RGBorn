using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;
using static UnityEngine.EventSystems.EventTrigger;

public class Player_Attack : MonoBehaviour
{
    Entity entity;
    Animator animator;
    Rigidbody2D myrigidbody;

    private float curTime = 0;
    public float coolTime = 1.0f;
    public Transform pos;
    public Vector2 boxSize;

    [SerializeField] private Player_Movement movement;

    public float damage = 2f;

    [SerializeField] private AttributeDatabase attributeDB;

    [Header("Debuff Settings")]
    [Range(0f, 1f)] public float baseDebuffChance = 0.25f; // 기본 25% 확률
    private int failCount = 0; // 실패 횟수 카운트
    public int maxFailStack = 4; // 4번 실패하면 5번째는 확정 (보정치)

    bool IsAdvantage(ColorAttributeData attacker, ColorAttributeData defender)
    {
        return attacker.strongAgainst.Contains(defender.colorMode);
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        myrigidbody = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!entity.CanAct) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UpdateAttackBoxPosition();

        if (curTime <= 0)
        {
            // 평타
            if (Input.GetMouseButtonDown(0))
            {
                DoAttack(damage);
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }        
    }

    void DoAttack(float damage)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (!collider.CompareTag("Enemy")) continue;

            // 튜토리얼 씬을 위한 코드
            SlimeAI slimeAI = collider.GetComponent<SlimeAI>();
            if (slimeAI != null)
            {
                slimeAI.InvokeOnHit();
            }

            Enemy_HP enemyHP = collider.GetComponent<Enemy_HP>();
            if (enemyHP == null || !enemyHP.enabled) continue;

            Entity attacker = entity;
            Entity intendedTarget = enemyHP;
            Entity actualTarget = attacker.ResolveAttackTarget(intendedTarget);

            ColorAttribute atkAttrComp = attacker.GetComponent<ColorAttribute>();

            ColorAttribute defAttrComp = actualTarget.GetComponent<ColorAttribute>();

            string sceneName = SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case "Stage1":
                    damage *= 1.2f;
                    break;
                case "Stage2":
                    damage *= 1.44f;
                    break;
                case "Stage3":
                    damage *= 1.728f;
                    break;
                default:
                    break;
            }

            float finalDamage = damage;

            if (atkAttrComp != null && defAttrComp != null)
            {
                ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);

                ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

                // 상성 계산
                if (atkAttr.strongAgainst.Contains(defAttr.colorMode))
                {
                    finalDamage *= (1f + atkAttr.attackBonusPercent);
                }

                if (atkAttr.weakAgainst.Contains(defAttr.colorMode))
                {
                    finalDamage *= (1f - atkAttr.attackMinusPercent);
                }
            }

            // --- 디버프 발동 로직 ---
            if (ShouldApplyDebuff())
            {
                attacker.ApplyAttributeDebuffTo(actualTarget, 10f);
                Debug.Log("디버프 발동!");
                failCount = 0; // 발동 성공 시 스택 초기화
            }
            else
            {
                failCount++;
                Debug.Log($"디버프 실패.. (현재 스택: {failCount})");
            }

            actualTarget.TakeDamage(finalDamage);
        }

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        movement.SetAttacking(true);
        animator.SetTrigger("Attack");

        curTime = coolTime;

        myrigidbody.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        movement.SetAttacking(false);
    }

    bool ShouldApplyDebuff()
    {
        // 1. 최대 실패 스택에 도달했는지 확인
        if (failCount >= maxFailStack) return true;

        // 2. 랜덤 확률 판정 (0.0 ~ 1.0)
        float randomValue = Random.value;

        // 기본 확률 + (실패할 때마다 보너스 확률 추가 가능)
        // 여기서는 단순하게 기본 확률로 판정하고, 스택은 확정 발동용으로만 사용
        return randomValue <= baseDebuffChance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    private void UpdateAttackBoxPosition()
    {
        if (movement == null) return;
        
        float sign = movement.dirX >= 0 ? 1f : -1f;

        // 근접 공격 박스 위치
        if (pos != null)
        {
            Vector3 local = pos.localPosition;
            local.x = Mathf.Abs(local.x) * sign;
            pos.localPosition = local;
        }
    }
}
