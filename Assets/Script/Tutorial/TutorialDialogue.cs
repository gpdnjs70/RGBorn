using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject talkPanel;
    public RectTransform talkPanelRect;
    public Vector3 offset = new Vector3(0, 2.0f, 0);
    private GameObject currentScanObject; // 현재 대화중인 오브젝트 저장

    public Image portraitImg;
    public Text talkText;

    public bool isAction;
    public int talkIndex;
    private DialogueData currentDialogue;

    public void Action(GameObject scanObj)
    {
        currentScanObject = scanObj;
        ObjData objData = scanObj.GetComponent<ObjData>();

        // 새로운 대화를 시작할 때만 데이터를 세팅
        if (!isAction)
        {
            currentDialogue = objData.dialogue;
            talkIndex = 0;
        }

        Talk(objData.isNPC);

        UpdatePanelPosition();
        talkPanel.SetActive(isAction);
    }

    void Talk(bool isNPC)
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

        if (isNPC)
        {
            talkText.text = currentLine.context;
            portraitImg.sprite = currentLine.portrait;

            // 초상화가 있으면 보여주고 없으면 숨기기
            bool hasPortrait = currentLine.portrait != null;
            portraitImg.color = new Color(1, 1, 1, hasPortrait ? 1 : 0);
        }
        else
        {
            talkText.text = currentLine.context;
            portraitImg.color = new Color(1, 1, 1, 0);
        }

        isAction = true;
        talkIndex++;
    }

    void LateUpdate()
    {
        if (isAction && currentScanObject != null)
        {
            UpdatePanelPosition();
        }
    }

    void UpdatePanelPosition()
    {
        // 1. 캐릭터의 월드 좌표 + 머리 위 오프셋
        Vector3 worldPos = currentScanObject.transform.position + offset;

        // 2. 월드 좌표를 스크린 좌표(픽셀)로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 3. UI 패널의 위치를 스크린 좌표로 설정
        talkPanelRect.position = screenPos;
    }
}
