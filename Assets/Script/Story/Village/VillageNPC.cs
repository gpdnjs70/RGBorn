using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageNPC : MonoBehaviour
{
    public int npcID; // 1, 2, 3
    public VillageManager villageManager;

    // 인덱스 규칙: 0 = 기본(반복), 1 = 찐(진행), 2 = NPC2 전용 거절
    private const int INDEX_IDLE = 0;
    private const int INDEX_MAIN = 1;
    private const int INDEX_LOCKED = 2;

    private bool isPlayerNear = false;
    private bool isTalking = false;

    private void Update()
    {
        // 근처에 있고 대화 중이 아닐 때 E키 입력 시 상호작용
        if (isPlayerNear && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ProcessInteraction());
        }
    }

    private IEnumerator ProcessInteraction()
    {
        isTalking = true;
        villageManager.HideControlGuide(); // 대화 시작 시 E키 가이드 숨김

        int targetIndex = INDEX_IDLE; // 기본값은 '기본 대사'

        // --- NPC별 대사 분기 로직 ---

        if (npcID == 1)
        {
            // NPC 1: 찐 대사 전이면 1번, 했으면 0번
            targetIndex = villageManager.talkedToNPC1 ? INDEX_IDLE : INDEX_MAIN;
        }
        else if (npcID == 3)
        {
            // NPC 3: 찐 대사 전이면 1번, 했으면 0번
            targetIndex = villageManager.talkedToNPC3 ? INDEX_IDLE : INDEX_MAIN;
        }
        else if (npcID == 2)
        {
            // NPC 2 (특수 조건)
            if (villageManager.talkedToNPC2)
            {
                // 이미 찐 대화 완료 시 -> 기본 대사
                targetIndex = INDEX_IDLE;
            }
            else if (villageManager.talkedToNPC1 && villageManager.talkedToNPC3)
            {
                // 조건 충족 시 -> 찐 대사
                targetIndex = INDEX_MAIN;
            }
            else
            {
                // 조건 미달 시 -> 거절 대사
                targetIndex = INDEX_LOCKED;
            }
        }

        // VillageManager를 통해 대화 실행
        yield return StartCoroutine(villageManager.PlayDialogue(this.gameObject, targetIndex));

        // --- 대화 종료 후 플래그 업데이트 ---
        // 방금 진행한 대사가 '찐 대사(1번)'였다면 매니저에게 완료 보고
        if (targetIndex == INDEX_MAIN)
        {
            villageManager.OnNPCTalkComplete(npcID);
        }

        isTalking = false;
        // 대화가 끝난 직후에는 가이드를 띄우지 않음 (TriggerExit 후 Re-enter 시에만 뜸)
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            // 대화 중이 아닐 때만 진입 시 가이드 표시
            if (!isTalking) villageManager.ShowControlGuide(0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            villageManager.HideControlGuide();
        }
    }
}
