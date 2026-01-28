using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private Enemy master; // 보스 참조
    private Transform playerTransform;

    public void Setup(Enemy boss)
    {
        master = boss;
        playerTransform = boss.player; // 보스가 이미 알고 있는 플레이어 정보를 가져옴

        // 여기서 씬에 있는 다른 요소들도 보스를 통해 가져올 수 있습니다.
        // 예: this.targetPoint = boss.someScenePoint;

        ResetMinion();
    }

    private void ResetMinion()
    {
        // 체력 회복, 상태 초기화 로직 (풀링에서 재사용될 때 필수)
    }

    // 소환수가 죽거나 보스가 사라질 때 호출
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
