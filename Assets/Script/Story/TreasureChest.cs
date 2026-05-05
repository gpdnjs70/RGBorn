using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    [SerializeField] private List<Sprite> itemSprites;
    [SerializeField] private List<string> itemTexts;

    [SerializeField] private AudioClip acquisitionSound;

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
        if (acquisitionSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.SFXPlay("acquisitionSound", acquisitionSound);
        }
        yield return new WaitForSeconds(3.0f);

        if (StatUIController.Instance != null)
        {
            StatUIController.Instance.StartUISequence(itemSprites, itemTexts, () =>
            {
                // UI가 완전히 닫힌 후 실행될 콜백 (Action)
                EndSequence();
            });
        }
        else
        {
            // UI가 없을 경우 바로 종료 처리
            EndSequence();
        }
    }

    private void EndSequence()
    {
        // 4. 플레이어 조작 복구 및 후처리
        MonsterWaveManager.Instance.SetPlayerControl(true);

        if (anim != null) anim.SetTrigger("Close");

        Debug.Log("아이템 획득 연출 종료");
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
