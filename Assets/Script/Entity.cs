using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private Stats stats;    // 캐릭터 정보
    public Entity target;   // 공격 대상(플레이어의 경우 대상을 클릭하는 방식으로 설정)

    // 체력(HP) 프로퍼티 : 0~MaxHP 사이의 값을 넘어갈 수 없도록 설정
    public float HP
    {
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
        get => stats.HP;
    }

    // 현재 프로퍼티는 추상(abstract)으로 선언되었기 때문에
    // 실제 작동하는 내용은 플레이어, 적과 같은 파생 클래스에서 정의
    public abstract float MaxHP         { get; }    // 최대 체력
    public abstract float HPRecovery    { get; }    // 초당 체력 회복량

    // 속도(Speed) 프로퍼티
    public float Speed
    {
        set => stats.Speed = Mathf.Clamp(value, 0, MaxSpeed);
        get => stats.Speed;
    }

    public abstract float MaxSpeed { get; }    // 최대 속도

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

        // StartCoroutine("Recovery");
    }

    // 초당 체력 회복
    protected IEnumerator Recovery()
    {
        while (true)
        {
            if (HP < MaxHP) HP += HPRecovery;

            yield return new WaitForSeconds(1);
        }
    }

    // 상대방을 공격할 때 상대방의 TakeDamage() 호출
    // 매개변수 damage는 공격하는 본인 공격력
    public abstract void TakeDamage(float damage);

    // 화상 디버프
    public virtual void Burn(float damage = 5f)
    {
        HP -= damage;
    }

    // 슬로우 디버프
    public float MoveSpeed
    {
        get
        {
            float result = Speed;

            foreach (float mod in moveSpeedModifiers)
                result *= (1f - mod);

            return Mathf.Max(0.1f, result); // 완전 정지 방지
        }
    }

    public void AddMoveSpeedModifier(float value)
    {
        moveSpeedModifiers.Add(value);
    }

    public void RemoveMoveSpeedModifier(float value)
    {
        moveSpeedModifiers.Remove(value);
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
}

[System.Serializable]
public struct Stats
{
    // 이름, 레벨, 직업, 스탯(힘, 지능 등), 체력/마나, 경험치 등의 캐릭터 수치
    [HideInInspector] public float HP;
    [HideInInspector] public float Speed;
}