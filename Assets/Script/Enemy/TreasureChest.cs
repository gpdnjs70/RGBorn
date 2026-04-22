using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 플레이어가 근처에 있고, 아직 열지 않았으며, E 키를 눌렀을 때
        if (isPlayerNearby && !isOpened && MonsterWaveManager.Instance.CanInteractWithChest())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(OpenSequence());
            }
        }
    }

    private System.Collections.IEnumerator OpenSequence()
    {
        isOpened = true;

        MonsterWaveManager.Instance.SetPlayerControl(false);

        // 1. 상자 열기 애니메이션
        if (anim != null) anim.SetTrigger("Open");

        Debug.Log("아이템 획득!");

        // 2. UI 끄기 및 보상 처리 (매니저 호출)
        MonsterWaveManager.Instance.OnChestOpened();
        yield return new WaitForSeconds(3.0f);

        MonsterWaveManager.Instance.SetPlayerControl(true);
        Debug.Log("연출 종료, 플레이어 이동 가능");

        // 3. 잠시 후 상자가 다시 닫히는 애니메이션 (필요 시)
        if (anim != null) anim.SetTrigger("Close");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MonsterWaveManager.Instance.SetPlayerSkillControl(false);

            isPlayerNearby = true;
            // 상자 근처에 오면 조작법 패널 켜기
            MonsterWaveManager.Instance.ShowControlPanel(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MonsterWaveManager.Instance.SetPlayerSkillControl(true);

            isPlayerNearby = false;
            // 상자에서 멀어지면 조작법 패널 끄기
            MonsterWaveManager.Instance.ShowControlPanel(false);
        }
    }
}
