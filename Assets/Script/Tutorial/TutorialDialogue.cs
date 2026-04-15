using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject talkPanel;
    public RectTransform talkPanelRect;

    public Transform playerTransform;
    private Transform npcTransform;
    private Transform currentTargetTransform;

    public Vector3 offset = new Vector3(0, 2.0f, 0);
    private Vector3 currentLineOffset;

    public Image portraitImg;
    public Text talkerName;
    public Text talkText;

    public bool isAction;
    public int talkIndex;
    private DialogueData currentDialogue;

    public bool isDialogueFinished = false;

    [Header("Typing Effect")]
    public AudioClip typeClip;
    public float typingSpeed = 0.05f; // 글자 출력 속도
    private Coroutine typingCoroutine; // 현재 실행 중인 코루틴 관리
    private bool isTyping; // 현재 타이핑 중인지 확인
    private string fullSentence; // 현재 전체 문장 저장

    void Update()
    {
        // 대화 중일 때 입력 처리 (마우스 왼쪽 클릭 또는 스페이스바)
        if (isAction && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            HandleInput();
        }
    }

    public void Action(GameObject scanObj)
    {
        Debug.Log("Action Called");

        if (!isAction)
        {
            npcTransform = scanObj.transform;
            currentDialogue = scanObj.GetComponent<ObjData>().dialogue;
            talkIndex = 0;
            isAction = true; // 상태 먼저 변경
            isDialogueFinished = false;
            talkPanel.SetActive(true);
            Talk();
        }
    }

    void HandleInput()
    {
        if (isTyping)
        {
            // 1. 타이핑 중일 때 클릭하면 -> 즉시 전체 문장 출력
            isTyping = false;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            talkText.text = fullSentence;
            if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
        }
        else
        {
            // 2. 타이핑이 끝났을 때 클릭하면 -> 다음 대화로
            Talk();
        }
    }

    void Talk()
    {
        // 대화가 끝났는지 확인
        if (currentDialogue == null || talkIndex >= currentDialogue.lines.Length)
        {
            isAction = false;
            isDialogueFinished = true;
            talkIndex = 0;
            talkPanel.SetActive(false);
            return;
        }

        // 현재 인덱스의 대화 데이터 가져오기
        TalkLine currentLine = currentDialogue.lines[talkIndex];

        switch (currentLine.talkerType)
        {
            case TalkerType.Player:
                currentTargetTransform = playerTransform;
                break;
            case TalkerType.NPC:
                currentTargetTransform = npcTransform;
                break;
            default:
                currentTargetTransform = npcTransform; // 기본값
                break;
        }

        currentLineOffset = (currentLine.customOffset == Vector3.zero) ? offset : currentLine.customOffset;

        // 텍스트 및 초상화 설정
        talkerName.text = currentLine.talkerName;
        portraitImg.sprite = currentLine.portrait;
        portraitImg.color = new Color(1, 1, 1, currentLine.portrait != null ? 1 : 0);

        // --- 타이핑 효과 시작 ---
        fullSentence = currentLine.context;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(fullSentence));

        talkIndex++;
    }

    IEnumerator TypeSentence(string sentence)
    {
        talkText.text = ""; // 텍스트 초기화
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            if (!isTyping) break;

            talkText.text += letter; // 한 글자씩 추가

            if (SoundManager.instance != null && typeClip != null)
            {
                // [변경] 매번 생성하지 않는 새로운 함수를 호출하거나, 직접 제어
                SoundManager.instance.PlayVariableSFX(typeClip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
    }

    void LateUpdate()
    {
        if (isAction && currentTargetTransform != null)
        {
            UpdatePanelPosition();
        }
    }

    void UpdatePanelPosition()
    {
        if (currentTargetTransform == null || talkPanelRect == null) return;

        // 1. 타겟의 월드 좌표를 스크린 좌표로 변환
        Vector3 worldPos = currentTargetTransform.position + currentLineOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. 만약 카메라 뒤에 있다면 (카메라 시야 밖 처리)
        if (screenPos.z < 0)
        {
            talkPanel.SetActive(false);
            return;
        }

        // 3. 화면 경계값 계산 (패널의 피벗과 크기를 고려)
        // talkPanelRect의 피벗이 중앙(0.5, 0.5)이라고 가정할 때의 계산입니다.
        float panelWidth = talkPanelRect.rect.width * talkPanelRect.lossyScale.x;
        float panelHeight = talkPanelRect.rect.height * talkPanelRect.lossyScale.y;

        // 화면의 가로세로 범위 (0 ~ Screen.width / 0 ~ Screen.height)
        float minX = panelWidth / 2;
        float maxX = Screen.width - (panelWidth / 2);
        float minY = panelHeight / 2;
        float maxY = Screen.height - (panelHeight / 2);

        // 4. 좌표가 화면 밖으로 나가지 않도록 Clamp 처리
        float clampedX = Mathf.Clamp(screenPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(screenPos.y, minY, maxY);

        talkPanelRect.position = new Vector3(clampedX, clampedY, screenPos.z);
    }
}
