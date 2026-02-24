using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        start, playerTurn, enemyTrun, win
    }

    public State state;
    public bool isLive; // 적 생존 여부

    private void Awake()
    {
        state = State.start; // 전투 시작 알림
        BattleStart();
    }

    void BattleStart()
    {
        // 전투 시작 시 캐릭터 등장 애니메이션 등 효과를 넣고 싶으면 넣기
            
            
        state = State.playerTurn; // 플레이어나 적에게 턴 넘기기
    }

    public void PlayerAttackButton()
    {
        // 공격 스킬, 데미지 등 코드 작성

            
        if(state != State.playerTurn) // 버튼이 계속 눌리는 것을 방지하기 위함
        {
            return;
        }
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("플레이어 공격");
        // 공격 스킬, 데미지 등 코드 작성
        
        if (!isLive) // 적 죽었으면 전투 종료
        {
            state = State.win;
            EndBattle();
        }
        else // 적 살았으면 적에게 턴 넘기기
        {
            state = State.enemyTrun;
            StartCoroutine(EnemyTurn());
        }
    }

    void EndBattle()
    {
        Debug.Log("전투 종료");
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        // 적 공격 코드

        // 적 공격 끝났으면 플레이어에게 턴 넘기기
        state = State.playerTurn;
    }
}
