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

    public void ProcessEffectsBySource(character sourceCharacter)
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var d = activeDebuffs[i];

            if (d.GetSource() == sourceCharacter)
            {
                if (d is CDebuff)
                {
                    d.OnTurnStart();
                    if (d.IsExpired) CleanUpDebuff(i);
                }
            }
        }
    }

    // 현재 턴인 캐릭터(source)가 과거에 걸어둔 디버프들을 찾아 실행
    public void ProcessTickDebuffs()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var d = activeDebuffs[i];

            // R, G 같은 대미지형 디버프는 여기서 실행
            if (d is RDebuff || d is GDebuff)
            {
                d.OnTurnStart();
                if (d.IsExpired) CleanUpDebuff(i);
            }
        }
    }

    // 마비(Y), 지연(B)은 자기 차례에 처리
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

    public void ExecuteCDebuff()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            // C(Cyan) 디버프인 경우에만 발동
            if (activeDebuffs[i] is CDebuff)
            {
                activeDebuffs[i].OnTurnStart(); // 대미지 발생
                if (activeDebuffs[i].IsExpired) CleanUpDebuff(i);
            }
        }
    }

    public void ExecutePDebuff()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            if (activeDebuffs[i] is PDebuff)
            {
                activeDebuffs[i].OnTurnStart(); // 여기서 반사 피해 발생
                if (activeDebuffs[i].IsExpired) CleanUpDebuff(i);
            }
        }
    }

    // ApplyDebuff 로직
    public void ApplyDebuff(DebuffType2 type, character source, int duration)
    {
        // 이미 있는 디버프인지 확인
        DebuffBase2 existing = activeDebuffs.Find(d => GetDebuffType(d) == type);

        if (existing != null)
        {
            existing.RefreshDuration(duration);
            existing.OnApply();
            Debug.Log($"{type} 디버프 갱신 및 효과 재발동!");
        }
        else
        {
            // 새 디버프 생성
            DebuffBase2 debuff = CreateDebuff(type);
            debuff.Init(source, target, duration);
            activeDebuffs.Add(debuff);

            // 즉시 실행
            debuff.OnApply();
        }
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
