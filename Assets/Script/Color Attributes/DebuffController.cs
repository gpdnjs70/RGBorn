using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
