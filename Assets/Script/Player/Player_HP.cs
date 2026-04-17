using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_HP : Entity, IDebuffTarget
{
    public Entity Entity => this;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float baseHP = 50f;

    private KnockBack knockback;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<KnockBack>();

        stats = new Stats();
        stats.Speed.BaseValue = 5f;

        string sceneName = SceneManager.GetActiveScene().name;
        float hpByScene = baseHP;

        switch (sceneName)
        {
            case "Stage1":
                hpByScene = 50f;
                break;
            case "Stage2":
                hpByScene = 100f;
                break;
            case "Stage3":
                hpByScene = 200f;
                break;
            default:
                hpByScene = baseHP; // 예외 상황 대비
                break;
        }

        stats.MaxHP.BaseValue = hpByScene;

        base.Setup();
    }

    protected override void Update()
    {
        base.Update(); // 부모의 Update 실행 (상태 체크 등)

        // 마우스 왼쪽 클릭 시 타겟팅 시도
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetByMouse();
        }

        // 타겟이 죽었다면 타겟 해제
        if (target != null && target.HP <= 0)
        {
            target = null;
        }
    }

    private void SetTargetByMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
        {
            // 클릭한 대상이 Entity 컴포넌트를 가지고 있는지 확인
            if (hit.collider.TryGetComponent(out Entity enemy))
            {
                target = enemy;
                Debug.Log($"타겟 변경: {enemy.name}");
            }
        }
        else
        {
            // 빈 공간 클릭 시 타겟 해제 (선택 사항)
            // currentTarget = null;
        }
    }

    protected override void FindTarget()
    {
        // 이미 타겟이 있고, 그 타겟이 살아있다면 굳이 새로 찾지 않음 (선택 사항)
        if (IsTargetValid()) return;

        float detectionRange = 15f; // 탐지 범위
        LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // 적 레이어만 검출

        // 1. 주변의 모든 적 콜라이더 가져오기
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        
        Entity closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var targetCollider in targets)
        {
            if (targetCollider.TryGetComponent(out Entity enemy))
            {
                // 이미 죽은 적은 제외
                if (enemy.HP <= 0) continue;

                // 거리 계산 (제곱근 계산을 피하기 위해 sqrMagnitude 사용 권장)
                float distance = (targetCollider.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        // 최종적으로 가장 가까운 적을 타겟으로 설정
        target = closestEnemy;
    }

    public override void TakeDamage(float damage, GameObject sender = null)
    {
        // 1. 기본 방어 배율 가져오기 (1.0 +- 버프/디버프)
        float multiplier = DefenseMultiplier;

        // 2. 공격자(sender)와의 상성 체크
        if (sender != null)
        {
            // 내 속성과 공격자의 속성을 가져옴
            ColorAttribute myAttrComp = GetComponent<ColorAttribute>();
            ColorAttribute attackerAttrComp = sender.GetComponent<ColorAttribute>();

            if (myAttrComp != null && attackerAttrComp != null)
            {
                // DB에서 내 속성 데이터를 가져옴
                ColorAttributeData myData = AttributeDatabase.Instance.Get(myAttrComp.colorMode);
                ColorMode attackerMode = attackerAttrComp.colorMode;

                if (myData != null)
                {
                    // 내가 공격자에게 강한 상성일 때 (우위: 대미지 감소)
                    if (myData.strongAgainst.Contains(attackerMode))
                    {
                        // ReductionRate가 0.3이면 multiplier에서 0.3을 빼서 0.7배가 됨
                        multiplier -= Mathf.Abs(myData.damageReductionRate);
                    }
                    // 내가 공격자에게 약한 상성일 때 (열위: 대미지 증가)
                    else if (myData.weakAgainst.Contains(attackerMode))
                    {
                        // ReductionRate가 0.3이면 multiplier에 0.3을 더해서 1.3배가 됨
                        multiplier += Mathf.Abs(myData.damageReductionRate);
                    }
                }
            }
        }

        float finalDamage = damage * Mathf.Clamp(multiplier, 0.1f, 2f);

        // --- 기존 로직 수행 ---
        // Debug.Log($"최종 데미지는 {finalDamage} (배율: {multiplier})");
        DamagePopup.Create(transform.position + Vector3.up, (int)finalDamage, false);

        HP -= finalDamage;

        if (knockback != null && sender != null)
        {
            knockback.PlayFeedBack(sender);
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
