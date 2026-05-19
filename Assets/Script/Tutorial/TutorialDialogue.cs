using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject talkPanel;
    public RectTransform talkPanelRect;

    public Transform playerTransform;
    private Transform npcTransform;
    private Transform currentTargetTransform;

    public Vector3 offset = new Vector3(0, 2.0f, 0);
    private Vector3 currentLineOffset;

    public GameObject Frame;
    public Image portraitImg;

    public GameObject Nametag;
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

    [Header("Skip Feature")]
    public GameObject skipButton; // UI에서 스킵 버튼 오브젝트 연결
    public int skipThreshold = 5; // 스킵 버튼이 나타날 최소 대사 줄 수

    void Update()
    {
        if (PauseManager.IsPaused) return;

        if (isAction)
        {
            // 1. 스페이스바는 언제나 정상 대사 진행
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleInput();
            }
            // 2. 마우스 좌클릭 처리
            else if (Input.GetMouseButtonDown(0))
            {
                // 스킵 버튼이 켜져 있고, 마우스가 정확히 무언가 UI를 가리키고 있을 때
                if (skipButton != null && skipButton.activeSelf && EventSystem.current != null)
                {
                    // [핵심 예외 처리] 마우스가 가리키고 있는 UI 오브젝트가 스킵 버튼 본인이거나 그 자식 오브젝트(텍스트 등)인 경우
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        PointerEventData eventData = new PointerEventData(EventSystem.current);
                        eventData.position = Input.mousePosition;
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(eventData, results);

                        foreach (RaycastResult result in results)
                        {
                            // 레이캐스트에 걸린 UI가 스킵 버튼이거나, 스킵 버튼의 하위 오브젝트라면
                            if (result.gameObject == skipButton || result.gameObject.transform.IsChildOf(skipButton.transform))
                            {
                                // 대사 넘기기를 실행하지 않고 리턴 (SkipDialogue 함수만 단독 실행되도록 보장)
                                return;
                            }
                        }
                    }
                }

                // 스킵 버튼 영역이 아니라면 정상적으로 다음 대사 진행/타이핑 스킵
                HandleInput();
            }
        }
    }

    void HandleInput()
    {
        if (isTyping)
        {
            isTyping = false;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            talkText.text = fullSentence;
            if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
        }
        else
        {
            Talk();
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

            if (skipButton != null)
            {
                // 대사 줄 수가 설정한 threshold보다 많을 때만 버튼 활성화
                skipButton.SetActive(currentDialogue.lines.Length >= skipThreshold);
            }

            Talk();
        }
    }

    void Talk()
    {
        // 대화가 끝났는지 확인
        if (currentDialogue == null || talkIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
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

    private void EndDialogue()
    {
        isAction = false;
        isDialogueFinished = true;
        talkIndex = 0;
        talkPanel.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false); // 종료 시 버튼 숨김
    }

    IEnumerator TypeSentence(string sentence)
    {
        talkText.text = ""; // 텍스트 초기화
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            while (PauseManager.IsPaused)
            {
                // 일시정지 중에는 소리도 나지 않게 하고 루프를 돌며 기다립니다.
                if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
                yield return null;
            }

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

        Vector3 worldPos = currentTargetTransform.position + currentLineOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
        {
            talkPanel.SetActive(false);
            return;
        }

        // 1. 부모 패널의 실제 크기
        float parentHalfW = (talkPanelRect.rect.width * talkPanelRect.lossyScale.x) * 0.5f;
        float parentHalfH = (talkPanelRect.rect.height * talkPanelRect.lossyScale.y) * 0.5f;

        // 2. 자식(nametag)에 의한 추가 오프셋 계산 
        // 이미지처럼 nametag가 부모 중심에서 벗어나 있다면 그만큼 여백을 더 줘야 합니다.
        // 여기서는 수동으로 적어주거나, 넉넉하게 Padding을 잡는 것이 정신건강에 좋습니다.

        float extraRight = 35f;  // nametag가 오른쪽으로 치우친 정도 (예시)
        float extraTop = 20f;    // nametag가 위쪽으로 치우친 정도 (이미지의 Pos Y: 20)
        float safePadding = 10f; // 화면 끝과의 최소 간격

        // 3. 최종 경계값 계산 (부모 크기 + 자식이 삐져나온 만큼)
        float minX = parentHalfW + safePadding;
        float maxX = Screen.width - (parentHalfW + extraRight + safePadding);
        float minY = parentHalfH + safePadding;
        float maxY = Screen.height - (parentHalfH + extraTop + safePadding);

        // 4. 제한(Clamp) 적용
        float clampedX = Mathf.Clamp(screenPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(screenPos.y, minY, maxY);

        talkPanelRect.position = new Vector3(clampedX, clampedY, screenPos.z);
    }

    public void SkipDialogue()
    {
        if (PauseManager.IsPaused) return;

        // 1. 타이핑 중이었다면 중단
        if (isTyping)
        {
            isTyping = false;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
        }

        // 2. 대화 강제 종료 처리
        isAction = false;
        isDialogueFinished = true;
        talkIndex = 0;
        talkPanel.SetActive(false);

        if (skipButton != null) skipButton.SetActive(false);

        Debug.Log("대화가 스킵되었습니다.");
    }
}
