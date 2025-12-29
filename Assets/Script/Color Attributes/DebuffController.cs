using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffController : MonoBehaviour
{
    private Entity target;
    private Coroutine currentDebuff;

    private void Awake()
    {
        target = GetComponent<Entity>();
    }

    public void ApplyDebuff(DebuffType type, Entity source, float duration)
    {
        DebuffBase debuff = CreateDebuff(type);

        if (debuff == null) return;

        debuff.Init(source, target, duration);
        StartCoroutine(debuff.Run());
    }

    private DebuffBase CreateDebuff(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.Burn:
                return new BurnDebuff();
            case DebuffType.LifeSteal:
                return new LifeStealDebuff();
            case DebuffType.Poison:
                return new PoisonDebuff();
            case DebuffType.Corrosion:
                return new CorrosionDebuff();
        }

        return null;
    }
}
