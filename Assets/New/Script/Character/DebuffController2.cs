using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffController2 : MonoBehaviour
{
    private character target;
    private List<DebuffBase2> activeDebuffs = new();

    private void Awake()
    {
        target = GetComponent<character>();
    }

    // [핵심] 현재 턴인 캐릭터(source)가 과거에 걸어둔 디버프들을 찾아 실행
    public void ProcessEffectsBySource(character sourceCharacter)
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var d = activeDebuffs[i];

            // 이 디버프를 건 사람(Source)이 지금 턴 주인이라면?
            if (d.GetSource() == sourceCharacter)
            {
                // R, G 같은 대미지형 디버프는 여기서 실행
                if (d is RDebuff || d is GDebuff)
                {
                    d.OnTurnStart();
                    if (d.IsExpired) CleanUpDebuff(i);
                }
            }
        }
    }

    // [제어형] 마비(Y), 지연(B)은 기존처럼 자기 차례에 처리
    public void ProcessTurnControl()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var d = activeDebuffs[i];
            if (d is YDebuff || d is BDebuff)
            {
                d.OnTurnStart();
                if (d.IsExpired) CleanUpDebuff(i);
            }
        }
    }

    // ApplyDebuff 로직 (기존과 동일하지만 YDebuff는 신규 시 LockAction 유지)
    public void ApplyDebuff(DebuffType2 type, character source, int duration)
    {
        DebuffBase2 existing = activeDebuffs.Find(d => GetDebuffType(d) == type);
        if (existing != null) { existing.Init(source, target, duration); return; }

        DebuffBase2 debuff = CreateDebuff(type);
        if (debuff == null) return;
        debuff.Init(source, target, duration);

        if (type == DebuffType2.YDebuff) target.LockAction();
        activeDebuffs.Add(debuff);
    }

    private void CleanUpDebuff(int index)
    {
        activeDebuffs[index].OnForceRemove();
        activeDebuffs.RemoveAt(index);
    }

    private DebuffType2 GetDebuffType(DebuffBase2 debuff)
    {
        if (debuff is RDebuff) return DebuffType2.RDebuff;
        if (debuff is GDebuff) return DebuffType2.GDebuff;
        if (debuff is BDebuff) return DebuffType2.BDebuff;
        if (debuff is YDebuff) return DebuffType2.YDebuff;
        if (debuff is PDebuff) return DebuffType2.PDebuff;
        if (debuff is CDebuff) return DebuffType2.CDebuff;
        return DebuffType2.None;
    }

    private DebuffBase2 CreateDebuff(DebuffType2 type)
    {
        switch (type)
        {
            case DebuffType2.RDebuff:
                return new RDebuff();
            case DebuffType2.GDebuff:
                return new GDebuff();
            case DebuffType2.BDebuff:
                return new BDebuff();
            case DebuffType2.YDebuff:
                return new YDebuff();
            case DebuffType2.PDebuff:
                return new PDebuff();
            case DebuffType2.CDebuff:
                return new CDebuff();
        }

        return null;
    }
}
