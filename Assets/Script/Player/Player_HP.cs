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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

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

    public override void TakeDamage(float damage)
    {
        float finalDamage = damage * DefenseMultiplier;

        Debug.Log($"데미지는 {finalDamage}");
        DamagePopup.Create(transform.position + Vector3.up, (int)finalDamage, false);

        HP -= finalDamage;

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

    public override void Burn(float damage)
    {
        HP -= damage;
    }
}
