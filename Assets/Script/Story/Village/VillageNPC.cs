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
    private const int INDEX_MAIN_AFTER_BOOK = 3;

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
        villageManager.HideControlGuide();

        int targetIndex = INDEX_IDLE;

        // --- NPC 3 특수 로직 ---
        if (npcID == 3)
        {
            if (villageManager.talkedToNPC3)
            {
                // 이미 모든 과정을 마쳤다면 반복 대사
                yield return StartCoroutine(villageManager.PlayDialogue(this.gameObject, INDEX_IDLE));
            }
            else
            {
                // 1. 첫 번째 메인 대사 실행
                yield return StartCoroutine(villageManager.PlayDialogue(this.gameObject, INDEX_MAIN));

                // 2. 동화책 연출 실행
                yield return StartCoroutine(villageManager.PlayFullStory());

                // 3. 동화책 끝난 후 두 번째 메인 대사 실행
                yield return StartCoroutine(villageManager.PlayDialogue(this.gameObject, INDEX_MAIN_AFTER_BOOK));

                // 플래그 업데이트 (모든 대사가 끝난 후 완료 처리)
                villageManager.OnNPCTalkComplete(npcID);
            }
        }
        // --- NPC 1 & 2 로직 (기존 유지) ---
        else
        {
            if (npcID == 1)
            {
                targetIndex = villageManager.talkedToNPC1 ? INDEX_IDLE : INDEX_MAIN;
            }
            else if (npcID == 2)
            {
                if (villageManager.talkedToNPC2) targetIndex = INDEX_IDLE;
                else if (villageManager.talkedToNPC1 && villageManager.talkedToNPC3) targetIndex = INDEX_MAIN;
                else targetIndex = INDEX_LOCKED;
            }

            yield return StartCoroutine(villageManager.PlayDialogue(this.gameObject, targetIndex));

            if (targetIndex == INDEX_MAIN)
            {
                villageManager.OnNPCTalkComplete(npcID);
            }
        }

        isTalking = false;
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
