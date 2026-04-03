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
        Vector3 worldPos = currentTargetTransform.position + currentLineOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        talkPanelRect.position = screenPos;
    }
}
