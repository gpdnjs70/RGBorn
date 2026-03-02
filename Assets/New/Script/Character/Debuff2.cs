using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType2
{
    None,
    RDebuff,
    GDebuff,
    BDebuff,
    YDebuff,
    PDebuff,
    CDebuff,
}

public interface IDebuffTarget2
{
    character character { get; }
}

public abstract class DebuffBase2
{
    protected character target;
    protected character source;
    public int RemainingTurns { get; protected set; } 

    public void Init(character source, character target, int duration)
    {
        this.source = source;
        this.target = target;
        this.RemainingTurns = duration;
    }

    // 디버프 생성되자마자 즉시 실행
    public virtual void OnApply() { }

    // 매 턴 정산 시점에 실행
    public abstract void OnTurnStart();

    public virtual void OnForceRemove() { }

    public void SetSource(character source) => this.source = source;
    public character GetSource() => source;

    public void RefreshDuration(int duration) => this.RemainingTurns = duration;

    // 디버프가 끝났는지 확인
    public bool IsExpired => RemainingTurns <= 0;
}

public class RDebuff : DebuffBase2
{
    private const float FIXED_DAMAGE = 5f;

    public override void OnTurnStart()
    {
        if (RemainingTurns > 0)
        {
            target.Burn(FIXED_DAMAGE);
            Debug.Log($"{target.name}에게 화상 고정 대미지 {FIXED_DAMAGE} 발생! (남은 턴: {RemainingTurns - 1})");

            RemainingTurns--;
        }
    }
}

public class GDebuff : DebuffBase2
{
    private const float DRAIN_AMOUNT = 5f; // 매 턴 흡수할 고정 수치

    public override void OnTurnStart()
    {
        if (RemainingTurns > 0)
        {
            // target의 HP 감소
            target.TakeDamage(DRAIN_AMOUNT);

            // source의 HP 회복
            if (source != null)
            {
                source.HP += DRAIN_AMOUNT;
                Debug.Log($"{target.name}의 HP를 {DRAIN_AMOUNT}만큼 흡수, {source.name}이 회복하여 현재 HP: {source.HP}가 됨");
            }

            RemainingTurns--;
        }
    }

    public override void OnForceRemove()
    {
        Debug.Log($"{target.name}의 흡혈 디버프 해제");
    }
}

public class BDebuff : DebuffBase2
{
    public override void OnApply()
    {
        // 걸리는 즉시 몬스터 카운트를 밀어버림 (시각적 즉각성)
        if (target is Monster monster)
        {
            monster.DelayAttack(2);
            Debug.Log($"{target.name} 공격 2턴 지연 (OnApply)");
        }
    }

    public override void OnTurnStart()
    {
        if (target is not Monster)
        { // 플레이어라면 턴 자체를 스킵
            BattleHandler bh = Object.FindObjectOfType<BattleHandler>();
            if (bh != null) bh.EndTurn();

            Debug.Log($"{target.name} 공격 2턴 지연 (OnTurnStart)");
        }
        RemainingTurns--; // 지속시간만 감소
    }
}

public class YDebuff : DebuffBase2
{
    public override void OnApply()
    {
        target.LockAction(); // 즉시 마비 상태 돌입
        Debug.Log($"{target.name} 마비 상태 (OnApply)");
    }

    public override void OnTurnStart()
    {
        RemainingTurns--;
        if (IsExpired) OnForceRemove();
    }

    public override void OnForceRemove()
    {
        if (!target.CanAct) target.UnlockAction();
        Debug.Log($"{target.name} 마비 해제");
    }
}

public class PDebuff : DebuffBase2
{
    public override void OnTurnStart()
    {
        // 몬스터의 공격 컴포넌트에서 데미지 수치를 가져옴
        CharacterAttack atkComp = target.GetComponent<CharacterAttack>();
        if (atkComp != null)
        {
            float baseDamage = atkComp.GetAttackDamage(); // Getter 추가 필요
            float reflectDamage = baseDamage * 0.5f;

            target.TakeDamage(reflectDamage);
            Debug.Log($"<color=purple>[P디버프]</color> {target.name} 공격 반사! {reflectDamage} 피해 발생.");
        }

        RemainingTurns--;
    }
}

public class CDebuff : DebuffBase2
{
    public override void OnTurnStart()
    {
        if (RemainingTurns > 0)
        {
            // source의 공격 컴포넌트를 가져옴
            if (source != null)
            {
                CharacterAttack atkComp = source.GetComponent<CharacterAttack>();
                if (atkComp != null)
                {
                    // source의 공격력 50% 계산
                    float sourceAtk = atkComp.GetAttackDamage();
                    float extraDamage = sourceAtk * 0.5f;

                    // target에게 추가 피해
                    target.TakeDamage(extraDamage);

                    Debug.Log($"[C디버프] {source.name}의 공격에 의한 추가 피해 {extraDamage} 발생!");
                }
            }

            RemainingTurns--;
        }
    }
}