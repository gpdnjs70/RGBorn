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
    private bool isContacting = false;
    private List<Entity> hitEnemies = new List<Entity>();
    public Vector3[] directionOffsets = new Vector3[8];

    [SerializeField] private AttributeDatabase attributeDB;

    public AudioClip clip;

    /**
    [Header("Debuff Settings")]
    [Range(0f, 1f)] public float baseDebuffChance = 0.25f; // 기본 25% 확률
    private int failCount = 0; // 실패 횟수 카운트
    public int maxFailStack = 4; // 4번 실패하면 5번째는 확정 (보정치)
    **/

    bool IsAdvantage(ColorAttributeData attacker, ColorAttributeData defender)
    {
        return attacker.strongAgainst.Contains(defender.colorMode);
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        myrigidbody = GetComponent<Rigidbody2D>();

        LoadAttackData();
    }

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.IsPaused) return;
        if (!entity.CanAct) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UpdateAttackBoxPosition();

        if (curTime > 0) curTime -= Time.deltaTime;

        if (curTime <= 0 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }

        if (isContacting)
        {
            CheckCollision();
        }
    }

    private void LoadAttackData()
    {
        if (SaveManager.Instance != null)
        {
            SaveData data = SaveManager.Instance.LoadGame();
            if (data != null)
            {
                // JSON에 저장된 누적 배율(예: 1.2, 1.44 등)을 기본 데미지에 곱함
                // 주의: damage 변수가 스크립트 상에서 계속 변하지 않게 관리해야 함
                Debug.Log($"공격력 배율 로드 완료: {data.attackMultiplier}");
            }
        }
    }

    private IEnumerator Attack()
    {
        movement.SetAttacking(true);
        animator.SetTrigger("Attack");
        SoundManager.instance.SFXPlay("Attack", clip);

        curTime = coolTime;

        myrigidbody.velocity = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            // 일시정지 중이면 루프를 돌며 대기 (시간 계산 안 함)
            if (PauseManager.IsPaused)
            {
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        movement.SetAttacking(false);
    }

    public void StartAttackContact()
    {
        isContacting = true;
        hitEnemies.Clear(); // 새로운 공격이 시작될 때 리스트 초기화
    }

    public void EndAttackContact()
    {
        isContacting = false;
    }

    void CheckCollision()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (!collider.CompareTag("Enemy")) continue;

            SlimeAI slimeAI = collider.GetComponent<SlimeAI>();
            Enemy_HP enemyHP = collider.GetComponent<Enemy_HP>();

            if (slimeAI != null && slimeAI.enabled)
            {
                slimeAI.InvokeOnHit();
                if (enemyHP != null)
                {
                    hitEnemies.Add(enemyHP);
                }
                continue;
            }

            if (enemyHP == null || !enemyHP.enabled) continue;

            // 이미 이번 휘두르기에 맞은 적이면 스킵 (다단히트 방지)
            if (hitEnemies.Contains(enemyHP)) continue;

            // 데미지 계산 및 적용 로직 (기존 DoAttack의 내용을 여기에 넣으세요)
            ApplyDamage(enemyHP);

            // 맞은 목록에 추가
            hitEnemies.Add(enemyHP);
        }
    }

    void ApplyDamage(Enemy_HP target)
    {
        if (target == null || !target.enabled) return;

        SaveData data = SaveManager.Instance.LoadGame();
        float multiplier = (data != null) ? data.attackMultiplier : 1f;

        // 스테이지 이름 기반 하드코딩 삭제 -> 저장된 고정 배율 사용
        float finalDamage = damage * multiplier;

        // 3. 상성 계산 (ColorAttribute)
        Entity attacker = entity; // 플레이어 본인
        Entity actualTarget = attacker.ResolveAttackTarget(target);

        ColorAttribute atkAttrComp = attacker.GetComponent<ColorAttribute>();
        ColorAttribute defAttrComp = actualTarget.GetComponent<ColorAttribute>();

        if (atkAttrComp != null && defAttrComp != null)
        {
            ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);
            ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

            // 강점 상성
            if (atkAttr.strongAgainst.Contains(defAttr.colorMode))
            {
                finalDamage *= (1f + atkAttr.attackBonusPercent);
            }
            // 약점 상성
            if (atkAttr.weakAgainst.Contains(defAttr.colorMode))
            {
                finalDamage *= (1f - atkAttr.attackMinusPercent);
            }
        }

        // 4. 최종 데미지 입히기
        actualTarget.TakeDamage(finalDamage);

        if (entity.IsLifeStealActive)
        {
            float healAmount = finalDamage * entity.LifeStealRatio;
            entity.Heal(healAmount);
        }
    }

    /**
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
    **/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    private void UpdateAttackBoxPosition()
    {
        if (movement == null || pos == null) return;

        int dirIndex = GetDirectionIndex(movement.dirX, movement.dirY);

        // 해당 방향에 설정된 좌표로 박스 이동
        pos.localPosition = directionOffsets[dirIndex];
    }

    // Player_Attack.cs

    private int GetDirectionIndex(float x, float y)
    {
        // 정밀한 판단을 위해 bool 값으로 먼저 추출
        bool right = x > 0.1f;
        bool left = x < -0.1f;
        bool up = y > 0.1f;
        bool down = y < -0.1f;

        // 8방향 판단 (순서 중요: 대각선을 먼저 체크해야 합니다)
        if (right && up) return 4;   // 우상
        if (right && down) return 5; // 우하
        if (left && up) return 6;    // 좌상
        if (left && down) return 7;  // 좌하

        if (up) return 0;    // 상
        if (down) return 1;  // 하
        if (left) return 2;  // 좌
        if (right) return 3; // 우

        return 1; // 기본값 (보통 캐릭터 정면인 '하'로 설정)
    }
}
