using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBoss : Enemy
{
    [Header("Boss HP Patterns")]
    public int currentPattern = 0;
    private bool hp70Reached = false;
    private bool hp40Reached = false;
    private bool hp15Reached = false;
    private bool isSpecialPatternQueued = false;

    protected override void Update()
    {
        base.Update(); // 부모의 쿨타임 감소, Shock 체크 로직 실행

        if (hp != null) // Enemy_HP 참조 확인
        {
            CheckBossHP();
        }
    }

    // 1. 애니메이션 이벤트에서 호출할 함수 (애니메이션 끝부분에 배치)
    public void OnSkillAnimationEnd()
    {
        // 다음 스킬로 강제 전이
        animator.SetTrigger("NextStep");
    }

    public void StartPattern()
    {
        // 1. 공격 시작 직전에 현재 패턴 번호를 애니메이터에 전달
        animator.SetInteger("patternID", currentPattern);

        // 2. 전달 직후, 미리 다음 패턴 번호를 계산해둠 (0 -> 1 -> 0 ...)
        if (isSpecialPatternQueued)
        {
            isSpecialPatternQueued = false;
            // 소환이 끝나면 다시 기본 0번 패턴으로 돌아가도록 셋팅
            currentPattern = 0;
        }
        else
        {
            // 일반적인 상황: 0번과 1번 패턴을 번갈아 사용
            currentPattern = (currentPattern == 0) ? 1 : 0;
        }

        // 3. 애니메이션 실행
        animator.SetBool("isAttacking", true);
        animator.SetTrigger("Attack");

        Debug.Log($"공격 시작! 이번 패턴 번호: {animator.GetInteger("patternID")}, 다음 예정 패턴: {currentPattern}");
    }

    public void OnPatternEnd()
    {
        // 이제 여기서는 패턴 교체를 하지 않고 상태만 해제합니다.
        animator.SetBool("isAttacking", false);
    }

    private void CheckBossHP()
    {
        // Enemy_HP(hp)는 Entity를 상속받았으므로 HP/MaxHP에 접근 가능
        float hpPercent = hp.HP / hp.MaxHP;

        if (!hp70Reached && hpPercent <= 0.7f)
        {
            QueueSummonPattern();
            hp70Reached = true;
        }
        else if (!hp40Reached && hpPercent <= 0.4f)
        {
            QueueSummonPattern();
            hp40Reached = true;
        }
        else if (!hp15Reached && hpPercent <= 0.15f)
        {
            QueueSummonPattern();
            hp15Reached = true;
        }
    }

    private void QueueSummonPattern()
    {
        // 2번을 잡몹 소환 패턴ID로 가정
        currentPattern = 2;
        isSpecialPatternQueued = true;

        // 특수 패턴 상황이므로 다음 공격 쿨타임을 즉시 초기화하여 바로 실행되게 함
        atkDelay = 0;

        Debug.Log($"<color=red>보스 체력 임계치 도달! 소환 패턴 예약됨 (HP: {hp.HP})</color>");
    }
}
