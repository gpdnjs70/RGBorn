using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAttackState : StateMachineBehaviour
{
    private bool hasFired = false;
    public int overrideIndex = 0;

    [Header("공격 발사 타이밍 (0~1)")]
    [Range(0f, 1f)]
    public float fireTime = 0.5f; // 0.5는 애니메이션의 절반 지점

    // 스테이트에 진입할 때 호출 (공격 시작)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasFired = false; // 발사 여부 초기화

        // 이전 질문에서 제안한 doAttack 파라미터가 있다면 여기서 false로 잠금
        animator.SetBool("doAttack", false);
    }

    // 매 프레임 호출
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex)) return;

        if (!hasFired && stateInfo.normalizedTime >= fireTime)
        {
            hasFired = true;
            MidBossAttackEvent attackEvent = animator.GetComponent<MidBossAttackEvent>();
            if (attackEvent != null)
            {
                // overrideIndex가 설정되어 있으면 그걸 쓰고, 아니면 파라미터 인덱스를 가져옴
                int index = (overrideIndex != 0) ? overrideIndex : animator.GetInteger("AttackIndex");

                ExecuteAttackByIndex(attackEvent, index);
            }
        }
    }

    // 공격 스테이트를 빠져나갈 때 호출 (공격 종료)
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    private void ExecuteAttackByIndex(MidBossAttackEvent ev, int index)
    {
        // 인덱스 번호는 사용자의 Animator 설정에 맞게 수정하세요
        switch (index)
        {
            case 1: ev.fireField(); break;
            case 2: ev.fireBall(); break;
            case 3: ev.fireMine(); break;
            case 4: ev.Teleport(); break;
            case 5: ev.BlazeDash(); break;
            case 6: ev.BlazeTempest(); break;
            case 7: ev.meteorShower(); break;
            default: Debug.LogWarning("알 수 없는 AttackIndex입니다."); break;
        }
    }
}
