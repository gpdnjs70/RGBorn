using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType
{
    Normal,
    Skill1,
    Skill2,
    Skill3
}

public class Enemy : MonoBehaviour
{
    Animator animator;
    public Transform player;
    public Vector2 home;

    public float atkCoolTime = 4;
    public float atkDelay;

    public Transform target;
    public Enemy_HP hp;
    public Entity entity;
    public Entity targetHP;

    public EnemyAttackType currentAttackType { get; private set; }
    private Coroutine attackTypeRoutine;
    private Coroutine burnRoutine;

    public float powerBonusRate = 0.1f;
    public float attackBonusRate { get; private set; } = 0f;

    public Transform pos;
    public Vector2 boxSize;

    protected virtual void Start()
    {
        EnemyManager.Instance.Register(this);

        animator = GetComponent<Animator>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        home = transform.position;
    }

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    protected virtual void Update()
    {
        if(atkDelay >= 0)
        {
            atkDelay -= Time.deltaTime;
            if (atkDelay < 0) atkDelay = 0;
        }

        animator.SetBool("Shock", !entity.CanAct);
    }

    public void DirectionEnemy(float target, float baseobj)
    {
        if(target < baseobj)
        {
            animator.SetFloat("Direction", -1);
        }
        else
        {
            animator.SetFloat("Direction", 1);
        }
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
            attacker.ApplyAttributeDebuffTo(actualTarget, debuffDuration);
        }
        
        actualTarget.TakeDamage(damage);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    public void UpdateAttackBoxPosition()
    {
        // 이동 방향 계산 (플레이어가 오른쪽에 있으면 1, 왼쪽에 있으면 -1)
        float sign = (player.position.x - transform.position.x) >= 0 ? 1f : -1f;

        if (pos != null)
        {
            Vector3 local = pos.localPosition;
            local.x = Mathf.Abs(local.x) * sign; // 원래 위치의 절대값에 방향 곱하기
            pos.localPosition = local;
        }
    }

    public void OnAttackEvent() // 애니메이션 이벤트에서 호출
    {
        Collider2D hit = Physics2D.OverlapBox(pos.position, boxSize, 0, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            int damage = 2;
            if (UnityEngine.Random.value < 0.1f)
            {
                damage = 5;
            }

            DealDamageToTarget(damage, 3f);
        }
    }
}
