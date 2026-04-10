using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuffTarget
{
    Entity Entity { get; }
}

public class DebuffInstance
{
    public DebuffBase debuff;
    public Entity source;
    public Coroutine coroutine;
}

public abstract class DebuffBase
{
    protected Entity target;    // 디버프를 당하는 쪽
    protected Entity source;   // 디버프를 건 쪽
    public float RemainingDuration { get; protected set; }

    public void Init(Entity source, Entity target, float duration)
    {
        this.source = source;
        this.target = target;
        this.RemainingDuration = duration;
    }

    public void Renew(float duration)
    {
        this.RemainingDuration = duration; // 혹은 누적하려면 += duration
    }

    public virtual void OnForceRemove() { }

    public abstract IEnumerator Run();
}

public class BurnDebuff : DebuffBase
{
    private float tickInterval = 2f;
    private float damage = 5f;

    public override IEnumerator Run()
    {
        Debug.Log("화상 디버프 시작");
        float tickTimer = 0f;

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime; // 전체 시간 차감
            tickTimer += Time.deltaTime;         // 틱 타이머 증가

            if (tickTimer >= tickInterval)
            {
                target.TakeDamage(damage);
                tickTimer -= tickInterval; // 타이머 초기화 (초과분 보존을 위해 -= 사용)
            }

            yield return null; // 매 프레임 대기
        }

        Debug.Log("화상 디버프 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }
}

public class SlowDebuff : DebuffBase
{
    private float slowRate = 0.4f; // 40% 슬로우
    private float appliedModifier;

    public override IEnumerator Run()
    {
        // baseSpeed 기준으로 감소량 계산
        float baseSpeed = target.stats.Speed.BaseValue;
        appliedModifier = -baseSpeed * slowRate;

        target.stats.Speed.AddModifier(appliedModifier);
        Debug.Log("슬로우 시작");

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime;
            yield return null;
        }

        target.stats.Speed.RemoveModifier(appliedModifier);
        Debug.Log("슬로우 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }

    public override void OnForceRemove()
    {
        target.stats.Speed.RemoveModifier(appliedModifier);
    }
}

public class LifeStealDebuff : DebuffBase
{
    private float tickInterval = 2f;
    private float stealAmount = 5f;

    public override IEnumerator Run()
    {
        Debug.Log("흡혈 디버프 시작");
        float tickTimer = 0f;

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime; // 전체 시간 차감
            tickTimer += Time.deltaTime;         // 틱 타이머 증가

            // 틱 인터벌(2초)이 지날 때마다 흡혈 실행
            if (tickTimer >= tickInterval)
            {
                // 타겟에게 데미지
                target.TakeDamage(stealAmount);

                // 시전자 체력 회복
                source.HP += stealAmount;

                tickTimer -= tickInterval; // 타이머 초기화 (초과분 보존)
            }

            yield return null; // 매 프레임 대기
        }

        Debug.Log("흡혈 디버프 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }
}

public class ShockDebuff : DebuffBase
{
    public override IEnumerator Run()
    {
        // 행동 차단
        target.LockAction();
        Debug.Log("Shock 시작");

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime;
            yield return null;
        }

        // 행동 복구
        target.UnlockAction();
        Debug.Log("Shock 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }

    public override void OnForceRemove()
    {
        target.UnlockAction();
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

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime;
            yield return null;
        }

        // 원상 복구
        target.RemoveDefenseModifier(defenseReduction);
        Debug.Log("부식 디버프 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }

    public override void OnForceRemove()
    {
        target.RemoveDefenseModifier(defenseReduction);
    }
}

public class PoisonDebuff : DebuffBase
{
    public override IEnumerator Run()
    {
        target.AddAttackRedirect(this);
        Debug.Log("중독 디버프 시작");

        while (RemainingDuration > 0)
        {
            RemainingDuration -= Time.deltaTime;
            yield return null;
        }

        target.RemoveAttackRedirect(this);
        Debug.Log("중독 디버프 종료");

        target.GetComponent<DebuffController>().OnDebuffFinished(this);
    }

    public override void OnForceRemove()
    {
        target.RemoveAttackRedirect(this);
    }
}

