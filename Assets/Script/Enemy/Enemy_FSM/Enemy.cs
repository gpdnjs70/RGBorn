using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAttackType
{
    Normal,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5,
    Skill6,
}

public class Enemy : MonoBehaviour
{
    Animator animator;
    public Transform player;
    public Transform target;
    public Enemy_HP hp;
    public Entity entity;
    public Entity targetHP;

    private bool isAttacking = false;
    public float atkCoolTime = 4;
    public float atkDelay;
    public EnemyAttackType currentAttackType { get; private set; }
    private Coroutine attackTypeRoutine;
    private Coroutine burnRoutine;

    public float powerBonusRate = 0.1f;
    public float attackBonusRate { get; private set; } = 0f;

    public Transform[] attackPoints;
    public Vector2 boxSize;
    private int currentDirIndex = 0;

    // 원거리 공격 몬스터 관련 변수
    public GameObject bullet;
    private float lastShootTime = 0f;
    [SerializeField] private float shootCooldown = 0.1f;
    private Vector2 lockedAttackDir;

    private bool lastShockState = false;
    NavMeshAgent agent;

    public int currentPattern = 0;

    [Header("Damage Settings")]
    [SerializeField] private float meleeDamage = 2f;      // 기본 근접 데미지
    [SerializeField] private float criticalMeleeDamage = 5f; // 크리티컬 근접 데미지 (10% 확률)

    [Header("Audio Settings")]
    // public AudioClip followSound; // 추적 시작 시 소리
    // public float stepInterval = 0.5f;
    public AudioClip readySound;
    public AudioClip damagedSound; // 피격 시 소리
    public AudioClip dieSound; // 사망 시 소리

    protected virtual void Start()
    {
        EnemyManager.Instance.Register(this);

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Awake()
    {
        entity = GetComponent<Entity>();
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    protected virtual void Update()
    {
        if (atkDelay >= 0)
        {
            atkDelay -= Time.deltaTime;
            if (atkDelay < 0) atkDelay = 0;
        }

        bool currentShock = !entity.CanAct;
        if (currentShock != lastShockState)
        {
            animator.SetBool("Shock", currentShock);
            lastShockState = currentShock;
        }
    }

    public void DirectionEnemy(Vector3 targetPos)
    {
        Vector2 direction = (targetPos - transform.position).normalized;

        animator.SetFloat("DirX", direction.x);
        animator.SetFloat("DirY", direction.y);

        currentDirIndex = Get8DirectionIndex(direction);
    }

    private int Get8DirectionIndex(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return currentDirIndex;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 45도 단위로 나누어 인덱스화
        int baseIndex = Mathf.RoundToInt(angle / 45f) % 8;

        // 변환표: Right(0)->2, UpRight(1)->1, Up(2)->0, UpLeft(3)->7, Left(4)->6, DownLeft(5)->5, Down(6)->4, DownRight(7)->3
        int[] conversionTable = { 2, 1, 0, 7, 6, 5, 4, 3 };

        return conversionTable[baseIndex];
    }

    public void SetAttackType(EnemyAttackType type, float duration = 0f)
    {
        // 이전 타이머가 있으면 중단
        if (attackTypeRoutine != null)
        {
            StopCoroutine(attackTypeRoutine);
            attackTypeRoutine = null;
        }

        currentAttackType = type;

        // duration > 0 이면 자동 리셋 예약
        if (duration > 0f)
        {
            attackTypeRoutine = StartCoroutine(ResetAttackTypeAfter(duration));
        }
    }

    private IEnumerator ResetAttackTypeAfter(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAttackType();
    }

    public void ResetAttackType()
    {
        Debug.Log("type 리셋");
        currentAttackType = EnemyAttackType.Normal;
        attackTypeRoutine = null;
    }

    public void AddAttackBonus(float rate)
    {
        attackBonusRate += rate;
        Debug.Log($"{name} 공격 배율 증가 → {attackBonusRate * 100f}%");
    }

    public void DealDamageToTarget(float damage, float debuffDuration)
    {
        if (targetHP == null) return;

        Entity attacker = hp;
        Entity intendedTarget = targetHP;
        Entity actualTarget = attacker.ResolveAttackTarget(intendedTarget);

        if (UnityEngine.Random.value < 0.3f)
        {
            // attacker.ApplyAttributeDebuffTo(actualTarget, debuffDuration);
        }

        actualTarget.TakeDamage(damage, this.gameObject);
    }

    private void OnDestroy()
    {
        DebuffController[] debuffs = FindObjectsOfType<DebuffController>();
        foreach (var d in debuffs)
        {
            d.RemoveDebuffsFromSource(entity);
        }

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);
    }

    private void OnDrawGizmos()
    {
        if (attackPoints == null || attackPoints.Length == 0) return;

        for (int i = 0; i < attackPoints.Length; i++)
        {
            if (attackPoints[i] == null) continue;

            // 현재 활성화된 공격 범위는 빨간색, 나머지는 노란색
            Gizmos.color = (i == currentDirIndex && Application.isPlaying) ? Color.red : Color.yellow;
            Gizmos.DrawWireCube(attackPoints[i].position, boxSize);
        }
    }

    public void OnAttackEvent() // 애니메이션 이벤트에서 호출
    {
        if (isAttacking) return;
        StartCoroutine(ResetAttackFlag());

        if (attackPoints == null || attackPoints.Length <= currentDirIndex) return;

        Transform currentPos = attackPoints[currentDirIndex];
        Collider2D hit = Physics2D.OverlapBox(currentPos.position, boxSize, 0, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            float damage = meleeDamage;
            if (UnityEngine.Random.value < 0.1f)
            {
                damage = criticalMeleeDamage;
            }

            DealDamageToTarget(damage, 3f);
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.1f); // 0.1초 동안은 추가 판정 무시
        isAttacking = false;
    }

    public void CopySettingsFrom(Enemy boss)
    {
        // 1. 보스가 보고 있는 플레이어 정보를 내 정보에 대입
        this.player = boss.player;
        this.target = boss.player;

        // 2. 내 HP 스크립트에도 보스가 가진 타겟 정보를 대입
        Enemy_HP myHP = GetComponent<Enemy_HP>();
        if (myHP != null)
        {
            myHP.target = boss.hp.target; // 보스가 때리던 대상의 HP 정보
            myHP.playerAttribute = boss.hp.playerAttribute; // 플레이어 속성 정보
        }
    }

    public void SetLockedDirection(Vector3 targetPos)
    {
        lockedAttackDir = (targetPos - transform.position).normalized;
    }

    public void Shoot()
    {
        if (Time.time - lastShootTime < shootCooldown) return;
        lastShootTime = Time.time;

        if (bullet == null) return;
        
        Transform spawnPoint = (attackPoints != null && attackPoints.Length > currentDirIndex)
                               ? attackPoints[currentDirIndex]
                               : transform;

        GameObject b = Instantiate(bullet, spawnPoint.position, Quaternion.identity);

        arrow arrowScript = b.GetComponent<arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(lockedAttackDir);

            if (entity != null)
            {
                arrowScript.Init(entity);
            }
        }
    }

    // 1. 애니메이션 이벤트에서 호출할 함수 (애니메이션 끝부분에 배치)
    public void OnSkillAnimationEnd()
    {
        // 다음 스킬로 강제 전이
        animator.SetTrigger("NextStep");
    }

    public void StartPattern()
    {
        // 1. 공격 시작 직전에 현재 패턴 번호를 애니메이터에 전달
        animator.SetInteger("patternID", currentPattern);

        // 2. 전달 직후, 미리 다음 패턴 번호를 계산해둠 (0 -> 1 -> 0 ...)
        currentPattern = (currentPattern == 0) ? 1 : 0;

        // 3. 애니메이션 실행
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Attack");

        Debug.Log($"공격 시작! 이번 패턴 번호: {animator.GetInteger("patternID")}, 다음 예정 패턴: {currentPattern}");
    }

    public void OnPatternEnd()
    {
        // 이제 여기서는 패턴 교체를 하지 않고 상태만 해제합니다.
        animator.SetBool("isAttacking", false);
    }
}