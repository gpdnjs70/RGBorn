using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public int phaseIndex;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 작동했거나 플레이어가 아니면 무시
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true; // 중복 실행 방지

            // 매니저에게 "몇 번째 페이즈 입구에 플레이어가 들어왔다"고 알림
            MonsterWaveManager.Instance.OnPlayerEnterPhase(phaseIndex);

            // 트리거 오브젝트 비활성화 (필요 시)
            gameObject.SetActive(false); 
        }
    }
}
