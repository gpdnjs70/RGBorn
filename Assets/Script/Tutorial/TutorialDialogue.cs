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

    public void Action(GameObject scanObj)
    {
        npcTransform = scanObj.transform;
        ObjData objData = scanObj.GetComponent<ObjData>();

        // 새로운 대화를 시작할 때만 데이터를 세팅
        if (!isAction)
        {
            currentDialogue = scanObj.GetComponent<ObjData>().dialogue;
            talkIndex = 0;
        }

        Talk();

        UpdatePanelPosition();
        talkPanel.SetActive(isAction);
    }

    void Talk()
    {
        // 대화가 끝났는지 확인
        if (currentDialogue == null || talkIndex >= currentDialogue.lines.Length)
        {
            isAction = false;
            talkIndex = 0;
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
        talkText.text = currentLine.context;
        talkerName.text = currentLine.talkerName;
        portraitImg.sprite = currentLine.portrait;
        portraitImg.color = new Color(1, 1, 1, currentLine.portrait != null ? 1 : 0);

        isAction = true;
        talkIndex++;
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
