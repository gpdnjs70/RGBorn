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

    [Header("조작 UI")]
    [SerializeField] public GameObject controlPanel;
    [SerializeField] private Image controlImage;
    [SerializeField] private List<Sprite> controlSprites = new List<Sprite>(); // 0: E키 등

    [Header("동화책 전용 UI")]
    [SerializeField] private GameObject storyBookPanel; // 동화책 전체 패널
    [SerializeField] private Animator storyBookContentAnimator; // 애니메이션이 실행되는 자식 오브젝트의 애니메이터
    [SerializeField] private int totalPageCount = 5; // 애니메이터에 설정된 총 페이지(상태) 수

    [Header("Stat UI Settings")]
    [SerializeField] private List<Sprite> itemSprites;
    [SerializeField] private List<string> itemTexts;

    [SerializeField] private GameObject SavePoint;

    public bool isDialogueActive { get; private set; }
    private bool hasPlayedIntro = false;

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

        yield return new WaitUntil(() => !PauseManager.IsPaused && talkedToNPC1 && talkedToNPC3);
        Debug.Log("모든 조건 충족! 이제 NPC 2와 대화하면 다음 씬으로 넘어갑니다.");

        yield return new WaitUntil(() => !PauseManager.IsPaused && talkedToNPC2);

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

    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex, bool restoreControlAtEnd = true)
    {
        if (isDialogueActive) yield break;

        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

        isDialogueActive = true;
        // 2. 플레이어 조작 정지 및 애니메이션 초기화
        SetPlayerControllable(false);

        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();

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
        yield return new WaitForSecondsRealtime(0.1f);

        if (restoreControlAtEnd)
        {
            if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);
            SetPlayerControllable(true);
        }
        isDialogueActive = false;
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null; // 입력 중복 방지
        while (true)
        {
            if (!PauseManager.IsPaused)
            {
                if (!PauseManager.IsPaused)
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                        break;
                }
            }
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
                if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

                storyBookContentAnimator.SetTrigger("NextPage");

                // 6. 애니메이션이 시작되고 전환되는 동안 아주 잠깐 대기 (연타 방지)
                yield return new WaitForSecondsRealtime(0.1f);

                // 만약 애니메이션이 완전히 끝날 때까지 입력을 막고 싶다면 아래 주석을 해제하세요.

                yield return new WaitUntil(() =>
                    !PauseManager.IsPaused && // 일시정지가 아닐 때만 진행 상황 체크
                    storyBookContentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                    !storyBookContentAnimator.IsInTransition(0));            
            }
        }

        // 7. 모든 과정이 끝나면 배경 패널까지 비활성화
        if (storyBookPanel != null) storyBookPanel.SetActive(false);
    }

    public void TriggerPlayerMonologue()
    {
        if (hasPlayedIntro) return;
        hasPlayedIntro = true;

        StartCoroutine(PlayerMonologueSequence());
    }

    private IEnumerator PlayerMonologueSequence()
    {
        // 1. 조작 복구 옵션을 false로 주어 독백이 끝나도 플레이어가 움직이지 못하게 함
        yield return StartCoroutine(PlayDialogue(player, 0, false));

        // [일시정지 예외처리] 대기 시간 중에도 일시정지 체크
        float timer = 0;
        while (timer < 0.5f)
        {
            if (!PauseManager.IsPaused) timer += Time.deltaTime;
            yield return null;
        }

        bool isStatUIFinished = false;

        if (StatUIController.Instance != null)
        {
            StatUIController.Instance.StartUISequence(itemSprites, itemTexts, () => {
                isStatUIFinished = true;
            });

            // UI가 열려있는 동안에는 StatUIController 내부에서 PauseManager.IsPaused를 처리하므로
            // 여기서는 단순히 종료 플래그만 기다리면 됩니다.
            yield return new WaitUntil(() => isStatUIFinished);
        }

        // 모든 시퀀스가 진짜로 끝났을 때만 조작 해제
        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

        yield return new WaitForSeconds(0.2f);
        SetPlayerControllable(true);

        Debug.Log("마을 입구 독백 및 아이템 장착 연출 완료");
    }
}
