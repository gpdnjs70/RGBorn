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

    public abstract void OnTurnStart();

    public virtual void OnForceRemove() { }

    public character GetSource() => source;

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
            Debug.Log($"{target.name}의 HP를 {DRAIN_AMOUNT}만큼 흡수합니다.");

            // source의 HP 회복
            if (source != null)
            {
                source.HP += DRAIN_AMOUNT;
                Debug.Log($"{source.name}이 {DRAIN_AMOUNT}만큼 회복하여 현재 HP: {source.HP}가 되었습니다.");
            }

            RemainingTurns--;
        }
    }

    public override void OnForceRemove()
    {
        Debug.Log($"{target.name}의 흡혈 디버프가 해제되었습니다.");
    }
}

public class BDebuff : DebuffBase2
{
    private bool hasApplied = false;

    public override void OnTurnStart()
    {
        // 대상 판별
        if (!hasApplied)
        {
            if (target is Monster monster)
            {
                // 몬스터: 공격 카운트다운을 2턴 뒤로 밀음
                monster.DelayAttack(2);
                Debug.Log($"{target.name} (몬스터)의 공격이 2턴 지연되었습니다.");
            }
            else
            {
                // 플레이어
                Debug.Log($"{target.name} 플레이어 턴 지연! 다음 캐릭터로 넘깁니다.");

                BattleHandler bh = Object.FindObjectOfType<BattleHandler>();
                if (bh != null)
                {
                    RemainingTurns--;

                    // 즉시 다음 캐릭터 선택 호출
                    bh.EndTurn();
                    return;
                }
            }
            hasApplied = true;
        }

        // 턴 소모
        RemainingTurns--;

        // 만료 시 처리
        if (IsExpired)
        {
            OnForceRemove();
        }
    }

    public override void OnForceRemove()
    {

    }
}

public class YDebuff : DebuffBase2
{
    // 2턴동안 마비시키기 위해서는 CharacterAttack에서 duration을 3으로 설정해야함

    public override void OnTurnStart()
    {
        // 이미 ApplyDebuff에서 Lock을 걸었으므로 여기선 턴만 깎습니다.
        RemainingTurns--;

        // 턴이 다 되면 해제
        if (IsExpired)
        {
            OnForceRemove();
        }
    }

    public override void OnForceRemove()
    {
        // 캐릭터가 잠겨있을 때만 해제 (카운트 -1)
        if (!target.CanAct)
        {
            target.UnlockAction();
        }
    }
}

public class PDebuff : DebuffBase2
{
    public override void OnTurnStart()
    {

    }
}

public class CDebuff : DebuffBase2
{
    public override void OnTurnStart()
    {

    }
}