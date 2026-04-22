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
    [SerializeField] private GameObject storyBookPanel;
    [SerializeField] private Image storyBookImage;
    [SerializeField] private List<Sprite> storyBookSprites;

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
        isDialogueActive = true;

        // 1. 플레이어 조작 정지 및 애니메이션 초기화
        SetPlayerControllable(false);

        // 2. NPC의ObjData 가져오기 및 대사 SO 설정
        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObj.name + "에 ObjData 스크립트가 없습니다!");
            SetPlayerControllable(true);
            yield break;
        }
        npcData.SetDialogue(dialogueIndex);

        // 3. 대사 줄 수만큼 반복 (+ 창 닫기용 1회)
        int totalCalls = npcData.dialogue.lines.Length + 1;

        for (int i = 0; i < totalCalls; i++)
        {
            dialogueSystem.Action(npcObj);

            // 마지막 호출(창 닫기)이 아닐 때만 유저 입력을 기다림
            if (i < npcData.dialogue.lines.Length)
            {
                yield return StartCoroutine(WaitForUserInput());
            }
        }

        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished == true);
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
        SetPlayerControllable(false); // 조작 금지
        if (storyBookPanel != null) storyBookPanel.SetActive(true);

        for (int i = 0; i < storyBookSprites.Count; i++)
        {
            storyBookImage.sprite = storyBookSprites[i];
            yield return StartCoroutine(WaitForUserInput()); // 유저 입력 대기
        }

        if (storyBookPanel != null) storyBookPanel.SetActive(false);
        SetPlayerControllable(true); // 조작 해제
    }
}
