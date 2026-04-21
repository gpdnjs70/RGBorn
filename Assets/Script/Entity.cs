using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public Stats stats = new();   // 캐릭터 정보
    public Entity target;

    // 체력(Hp) 프로퍼티
    public float MaxHP => stats.MaxHP.Value;

    public float HP
    {
        get => stats.HP;
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
    }
    // public abstract float HPRecovery    { get; }    // 초당 체력 회복량

    public bool IsInvincible { get; set; } = false; // 무적 상태 프로퍼티

    // 속도(Speed) 프로퍼티
    public float Speed => Mathf.Max(0.1f, stats.Speed.Value);

    // 방어력
    private List<float> defenseModifiers = new();
    // 공격 대상
    private List<object> attackRedirectors = new();
    // 속도
    private List<float> moveSpeedModifiers = new();
    // 마비
    private int actionLockCount = 0;
    public bool CanAct { get; private set; } = true;

    public bool IsLifeStealActive { get; set; } = false;
    public float LifeStealRatio { get; set; } = 0f;

    protected void Setup()
    {
        HP = MaxHP;

        // StartCoroutine("Recovery");
    }

    protected virtual void Update()
    {
        FindTarget();
    }

    protected abstract void FindTarget();

    protected bool IsTargetValid()
    {
        return target != null && target.HP > 0;
    }

    // 초당 체력 회복
    /**
    protected IEnumerator Recovery()
    {
        while (true)
        {
            if (HP < MaxHP) HP += HPRecovery;

            yield return new WaitForSeconds(1);
        }
    }
    **/

    // 상대방을 공격할 때 상대방의 TakeDamage() 호출
    // 매개변수 damage는 공격하는 본인 공격력
    public abstract void TakeDamage(float damage, GameObject sender = null);

    // 부식 디버프
    public float DefenseMultiplier
    {
        get
        {
            float total = 1f;
            foreach (var mod in defenseModifiers)
                total -= mod;

            return Mathf.Clamp(total, 0.1f, 2f);
        }
    }

    public void AddDefenseModifier(float value)
    {
        defenseModifiers.Add(value);
    }

    public void RemoveDefenseModifier(float value)
    {
        defenseModifiers.Remove(value);
    }

    // 디버프 적용
    public void ApplyAttributeDebuffTo(Entity target, float duration)
    {
        if (target == null) return;

        ColorAttribute attackerAttr = GetComponent<ColorAttribute>();

        DebuffController debuff = target.GetComponent<DebuffController>();

        if (attackerAttr == null || debuff == null) return;

        DebuffType type = attackerAttr.GetDebuffType();
        if (type == DebuffType.None) return;

        debuff.ApplyDebuff(type, this, duration);
    }

    public virtual Entity GetAttackTarget(Entity intendedTarget)
    {
        return intendedTarget;
    }

    // 중독 디버프
    public void AddAttackRedirect(object source)
    {
        attackRedirectors.Add(source);
    }

    public void RemoveAttackRedirect(object source)
    {
        attackRedirectors.Remove(source);
    }

    public Entity ResolveAttackTarget(Entity intendedTarget)
    {
        // 혼란 상태면 자기 자신
        if (attackRedirectors.Count > 0)
            return this;

        return intendedTarget;
    }

    // 마비 디버프
    public void LockAction()
    {
        actionLockCount++;
        CanAct = false;
    }

    public void UnlockAction()
    {
        actionLockCount--;

        if (actionLockCount <= 0)
        {
            actionLockCount = 0;
            CanAct = true;
        }
    }

    // 체력 회복 메서드
    public void Heal(float amount)
    {
        HP = Mathf.Min(HP + amount, stats.MaxHP.Value);
        // 필요 시 회복 팝업 출력
        DamagePopup.Create(transform.position + Vector3.up, (int)amount, true);
    }
}