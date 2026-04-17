using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Minion : Enemy
{
    private Enemy_HP myHP;

    // 보스가 소환 후 이 함수를 호출해줍니다.
    public void InitMinion()
    {
        // 1. 씬에서 플레이어 태그로 플레이어 오브젝트를 찾습니다.
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            // 2. Enemy 스크립트 변수들 할당
            this.player = playerObj.transform;       // Player-transform
            this.target = playerObj.transform;       // Target-transform
            this.targetHP = playerObj.GetComponent<Entity>(); // TargetHP (Entity)

            // 3. Enemy_HP에 PlayerAttribute 할당
            if (myHP == null) myHP = GetComponent<Enemy_HP>();
            if (myHP != null)
            {
                // 플레이어가 가진 PlayerAttribute 컴포넌트를 직접 가져와서 할당
                myHP.playerAttribute = playerObj.GetComponent<PlayerAttribute>();

                if (myHP.data != null)
                {
                    myHP.stats.MaxHP.BaseValue = myHP.data.baseHP;
                    myHP.stats.Speed.BaseValue = myHP.data.baseSpeed;

                    // 현재 HP를 새로운 MaxHP로 꽉 채움
                    myHP.HP = myHP.MaxHP;
                }
            }

            Debug.Log("잡몹 초기화 완료: 플레이어 정보 로드 성공");
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없어 잡몹 초기화에 실패했습니다!");
        }
    }
}