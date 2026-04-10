using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebuffController : MonoBehaviour
{
    private Entity target;
    private List<DebuffInstance> activeDebuffs = new();

    private void Awake()
    {
        target = GetComponent<Entity>();
    }

    public void ApplyDebuff(DebuffType type, Entity source, float duration)
    {
        var existing = activeDebuffs.Find(instance => GetDebuffTypeFromInstance(instance.debuff) == type);

        if (existing != null)
        {
            existing.debuff.Renew(duration);
            Debug.Log($"{type} 디버프 갱신됨 (남은 시간 초기화)");
            return;
        }

        DebuffBase debuff = CreateDebuff(type);

        if (debuff == null) return;

        debuff.Init(source, target, duration);
        Coroutine c = StartCoroutine(debuff.Run());

        activeDebuffs.Add(new DebuffInstance
        {
            debuff = debuff,
            source = source,
            coroutine = c
        });
    }

    private DebuffType GetDebuffTypeFromInstance(DebuffBase debuff)
    {
        return debuff switch
        {
            BurnDebuff => DebuffType.Burn,
            SlowDebuff => DebuffType.Slow,
            LifeStealDebuff => DebuffType.LifeSteal,
            ShockDebuff => DebuffType.Shock,
            PoisonDebuff => DebuffType.Poison,
            CorrosionDebuff => DebuffType.Corrosion,
            _ => DebuffType.None
        };
    }

    public void RemoveDebuffsFromSource(Entity source)
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            if (activeDebuffs[i].source == source)
            {
                activeDebuffs[i].debuff.OnForceRemove();
                StopCoroutine(activeDebuffs[i].coroutine);
                activeDebuffs.RemoveAt(i);
            }
        }
    }

    // 모든 디버프 삭제
    public void ClearAllDebuffs()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            CleanUpDebuff(i);
        }
    }

    private void CleanUpDebuff(int index)
    {
        activeDebuffs[index].debuff.OnForceRemove();
        if (activeDebuffs[index].coroutine != null)
            StopCoroutine(activeDebuffs[index].coroutine);
        activeDebuffs.RemoveAt(index);
    }

    private DebuffBase CreateDebuff(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.Burn:
                return new BurnDebuff();
            case DebuffType.Slow:
                return new SlowDebuff();
            case DebuffType.LifeSteal:
                return new LifeStealDebuff();
            case DebuffType.Shock:
                return new ShockDebuff();
            case DebuffType.Poison:
                return new PoisonDebuff();
            case DebuffType.Corrosion:
                return new CorrosionDebuff();
        }

        return null;
    }

    public void OnDebuffFinished(DebuffBase debuff)
    {
        activeDebuffs.RemoveAll(d => d.debuff == debuff);
    }
}
