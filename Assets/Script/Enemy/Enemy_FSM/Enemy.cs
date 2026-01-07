using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType
{
    Normal,
    Skill1,
    Skill2
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
}
