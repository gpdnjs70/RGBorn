using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageManager : MonoBehaviour
{
    private Rigidbody2D rigid;

    [Header("NPC Flags")]
    public bool talkedToNPC1 = false;
    public bool talkedToNPC3 = false;
    public bool talkedToNPC2 = false; // 최종 NPC

    [Header("References")]
    [SerializeField] private GameObject player;
    public TutorialDialogue dialogueSystem; // NPC가 참조할 수 있도록 public

    private Player_Movement playerMoveScript;

    // 퀘스트 창/조작 가이드 UI (TutorialManager_0의 기능)
    [SerializeField] public GameObject controlPanel;
    [SerializeField] private Image controlImage;
    [SerializeField] private List<Sprite> controlSprites = new List<Sprite>(); // 0: E키 등

    [Header("동화책 전용 UI")]
    [SerializeField] private GameObject storyBookPanel; // 동화책 전체 패널
    [SerializeField] private Animator storyBookContentAnimator; // 애니메이션이 실행되는 자식 오브젝트의 애니메이터
    [SerializeField] private int totalPageCount = 5; // 애니메이터에 설정된 총 페이지(상태) 수

    [SerializeField] private GameObject SavePoint;

    public bool isDialogueActive { get; private set; }

    private void Awake()
    {
        rigid = player.GetComponent<Rigidbody2D>();
        playerMoveScript = player.GetComponent<Player_Movement>();
    }

    private void Start()
    {
        if (controlPanel != null) controlPanel.SetActive(false);
        StartCoroutine(VillageStorySequence());
    }

    private IEnumerator VillageStorySequence()
    {
        if (SavePoint != null) SavePoint.SetActive(false);

        yield return new WaitUntil(() => talkedToNPC1 && talkedToNPC3);
        Debug.Log("모든 조건 충족! 이제 NPC 2와 대화하면 다음 씬으로 넘어갑니다.");

        yield return new WaitUntil(() => talkedToNPC2);

        Debug.Log("모든 대화 완료! 세이브 포인트 오브젝트를 활성화합니다.");
        if (SavePoint != null)
        {
            SavePoint.SetActive(true);
        }
    }

    public void SetPlayerControllable(bool canControl)
    {
        if (playerMoveScript) playerMoveScript.enabled = canControl;

        if (!canControl && rigid != null)
        {
            rigid.velocity = Vector2.zero; // 물리 멈춤
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null && !canControl)
        {
            anim.SetBool("isMove", false); // 걷기 애니메이션 강제 종료
        }
    }

    // NPC 근처에 갔을 때 가이드 UI 표시
    public void ShowControlGuide(int spriteIndex)
    {
        if (controlPanel == null) return;
        controlPanel.SetActive(true);
        controlImage.sprite = controlSprites[spriteIndex];
    }

    public void HideControlGuide()
    {
        if (controlPanel != null) controlPanel.SetActive(false);
    }

    public void OnNPCTalkComplete(int npcID)
    {
        switch (npcID)
        {
            case 1: talkedToNPC1 = true; break;
            case 3: talkedToNPC3 = true; break;
            case 2: talkedToNPC2 = true; break;
        }
    }

    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex)
    {
        if (isDialogueActive) yield break;

        isDialogueActive = true;

        // 2. 플레이어 조작 정지 및 애니메이션 초기화
        SetPlayerControllable(false);

        // 3. NPC의 ObjData 가져오기 및 대사 SO 설정
        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObj.name + "에 ObjData 스크립트가 없습니다!");
            SetPlayerControllable(true);
            isDialogueActive = false;
            yield break;
        }

        npcData.SetDialogue(dialogueIndex);

        // --- 핵심 수정 부분 ---
        // VillageManager의 for 루프와 WaitForUserInput을 제거하고,
        // TutorialDialogue 시스템이 내부적으로 입력을 처리하게 둡니다.

        // 대화 시작 신호 (한 번만 호출)
        dialogueSystem.Action(npcObj);

        // TutorialDialogue가 대화를 모두 마치고 isDialogueFinished를 true로 바꿀 때까지 대기
        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);
        // -----------------------

        // 대화가 완전히 끝난 후 약간의 여유 (연타 방지)
        yield return new WaitForSeconds(0.1f);

        SetPlayerControllable(true);
        isDialogueActive = false;
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null; // 입력 중복 방지
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                break;
            yield return null;
        }
    }

    public IEnumerator PlayFullStory()
    {
        SetPlayerControllable(false);

        // 1. 배경이 포함된 전체 패널 활성화
        if (storyBookPanel != null) storyBookPanel.SetActive(true);

        // 2. 페이지 수만큼 루프
        for (int i = 0; i < totalPageCount; i++)
        {
            Debug.Log($"현재 동화책 {i + 1} / {totalPageCount} 페이지 대기 중");

            // 3. 사용자 입력을 기다림 (스페이스바/클릭)
            yield return StartCoroutine(WaitForUserInput());

            // 4. 마지막 페이지에서 입력을 받았다면 루프 종료
            if (i == totalPageCount - 1) break;

            // 5. 다음 페이지 애니메이션 실행
            if (storyBookContentAnimator != null)
            {
                storyBookContentAnimator.SetTrigger("NextPage");

                // 6. 애니메이션이 시작되고 전환되는 동안 아주 잠깐 대기 (연타 방지)
                yield return new WaitForSeconds(0.1f);

                // 만약 애니메이션이 완전히 끝날 때까지 입력을 막고 싶다면 아래 주석을 해제하세요.

                yield return new WaitUntil(() =>
                    storyBookContentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
                    && !storyBookContentAnimator.IsInTransition(0));
            }
        }

        // 7. 모든 과정이 끝나면 배경 패널까지 비활성화
        if (storyBookPanel != null) storyBookPanel.SetActive(false);
    }
}
