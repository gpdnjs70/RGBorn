using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class character : MonoBehaviour
{
    public character target;

    [SerializeField] private float currentHP;
    public float MaxHP = 100f;
    public float HP
    {
        get => currentHP;
        set => currentHP = Mathf.Clamp(value, 0, MaxHP);
    }

    // 방어력
    private List<float> defenseModifiers = new();
    // 공격 대상
    private List<object> attackRedirectors = new();
    // 속도
    private List<float> moveSpeedModifiers = new();
    // 마비
    private int actionLockCount = 0;
    public bool CanAct { get; private set; } = true;

    protected void Setup()
    {
        HP = MaxHP;
    }

    public abstract void TakeDamage(float damage);

    // 화상 디버프
    public virtual void Burn(float damage = 5f)
    {
        HP -= damage;
    }

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
    public void ApplyAttributeDebuffTo(character target, float duration)
    {
        if (target == null) return;

        ColorAttribute attackerAttr = GetComponent<ColorAttribute>();

        DebuffController debuff = target.GetComponent<DebuffController>();

        if (attackerAttr == null || debuff == null) return;

        DebuffType type = attackerAttr.GetDebuffType();
        if (type == DebuffType.None) return;

        // debuff.ApplyDebuff(type, this, duration);
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

    public character ResolveAttackTarget(character intendedTarget)
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
}
