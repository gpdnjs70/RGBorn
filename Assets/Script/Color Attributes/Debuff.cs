using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuffTarget
{
    Entity Entity { get; }
}

public abstract class DebuffBase
{
    protected Entity target;    // 디버프를 당하는 쪽
    protected Entity source;   // 디버프를 건 쪽
    protected float duration;

    public void Init(Entity source, Entity target, float duration)
    {
        this.source = source;
        this.target = target;
        this.duration = duration;
    }

    public abstract IEnumerator Run();
}

public class BurnDebuff : DebuffBase
{
    private float tickInterval = 2f;
    private float damage = 5f;

    public override IEnumerator Run()
    {
        float elapsed = 0f;

        yield return new WaitForSeconds(tickInterval);

        while (elapsed < duration)
        {
            target.Burn(damage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }
}

public class SlowDebuff : DebuffBase
{
    private float slowRate = 0.4f; // 40% 슬로우

    public override IEnumerator Run()
    {
        target.AddMoveSpeedModifier(slowRate);
        Debug.Log("슬로우 시작");

        yield return new WaitForSeconds(duration);

        target.RemoveMoveSpeedModifier(slowRate);
        Debug.Log("슬로우 종료");
    }
}

public class LifeStealDebuff : DebuffBase
{
    private float tickInterval = 2f;
    private float stealAmount = 5f;

    public override IEnumerator Run()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 타겟에게 데미지
            target.TakeDamage(stealAmount);

            // 시전자 체력 회복
            source.HP += stealAmount;

            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }
}

public class CorrosionDebuff : DebuffBase
{
    private float defenseReduction = -0.3f;

    public override IEnumerator Run()
    {
        // 방어력 감소
        target.AddDefenseModifier(defenseReduction);
        Debug.Log("부식 디버프 시작");

        yield return new WaitForSeconds(duration);

        // 원상 복구
        target.RemoveDefenseModifier(defenseReduction);
        Debug.Log("부식 디버프 종료");
    }
}

public class PoisonDebuff : DebuffBase
{
    public override IEnumerator Run()
    {
        target.AddAttackRedirect(this);
        Debug.Log("혼란 디버프 시작");

        yield return new WaitForSeconds(duration);

        target.RemoveAttackRedirect(this);
        Debug.Log("혼란 디버프 종료");
    }
}

