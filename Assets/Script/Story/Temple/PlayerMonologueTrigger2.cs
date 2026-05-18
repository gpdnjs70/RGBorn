using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonologueTrigger2 : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 닿았고, 아직 실행 전이라면
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // TempleManager를 찾아 독백 트리거 실행
            TempleManager tm = FindObjectOfType<TempleManager>();
            if (tm != null)
            {
                tm.TriggerPlayerMonologue();
            }

            // 필요하다면 트리거 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}
