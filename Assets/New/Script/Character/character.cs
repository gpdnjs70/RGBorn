using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class character : MonoBehaviour
{
    // Player UI 관련 변수들
    [SerializeField] private Sprite characterPortrait;
    public Sprite CharacterPortrait => characterPortrait;

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
        TakeDamage(damage);
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
    public void ApplyAttributeDebuffTo(character target, int duration)
    {
        if (target == null) return;

        ColorAttribute attackerAttr = GetComponent<ColorAttribute>();

        DebuffController2 debuff = target.GetComponent<DebuffController2>();

        if (attackerAttr == null || debuff == null) return;

        DebuffType2 type = attackerAttr.GetDebuffType();
        if (type == DebuffType2.None) return;

        debuff.ApplyDebuff(type, this, duration);
    }

    public virtual character GetAttackTarget(character intendedTarget)
    {
        return intendedTarget;
    }

    // 마비 디버프
    public void LockAction()
    {
        actionLockCount++;
        CanAct = false;
        Debug.Log($"[LOCK] {gameObject.name} 현재 카운트: {actionLockCount}");
    }

    public void UnlockAction()
    {
        actionLockCount--;
        if (actionLockCount <= 0)
        {
            actionLockCount = 0;
            CanAct = true;
        }
        Debug.Log($"[UNLOCK] {gameObject.name} 현재 카운트: {actionLockCount}, CanAct: {CanAct}");
    }
}
