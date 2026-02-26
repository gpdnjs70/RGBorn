using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerInteraction : MonoBehaviour
{
    public GameObject selectionUI; // 연결할 UI 오브젝트
    public Vector3 uiOffset;

    private CharacterBattle characterBattle;

    void Awake()
    {
        characterBattle = GetComponent<CharacterBattle>();
    }

    void Start()
    {
        if (selectionUI == null)
        {
            selectionUI = GameObject.Find("PlayerSelection");

            // 찾은 직후에는 일단 꺼둠
            if (selectionUI != null) selectionUI.SetActive(false);
        }
    }

    public void UpdateUIPosition()
    {
        if (selectionUI == null) return;

        // 플레이어의 월드 좌표를 스크린 좌표로 변환
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        // 스크린 좌표를 캔버스(UI) 좌표로 변환
        RectTransform canvasRect = selectionUI.transform.parent.GetComponent<RectTransform>();
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            null,
            out localPos
        );

        // 변환된 좌표에 오프셋 적용
        selectionUI.GetComponent<RectTransform>().anchoredPosition = localPos + (Vector2)uiOffset;
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (characterBattle != null && !characterBattle.IsIdle())
            {
                return;
            }

            bool isActive = !selectionUI.activeSelf;
            selectionUI.SetActive(isActive);

            if (isActive)
            {
                // 플레이어의 월드 좌표를 스크린 좌표로 변환
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

                // 스크린 좌표를 캔버스(UI) 좌표로 변환
                RectTransform canvasRect = selectionUI.transform.parent.GetComponent<RectTransform>();
                Vector2 localPos;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPoint,
                    null,
                    out localPos
                );

                // 변환된 좌표에 오프셋 적용
                selectionUI.GetComponent<RectTransform>().anchoredPosition = localPos + (Vector2)uiOffset;

                Debug.Log("플레이어 우클릭: UI 켜짐");
            }
            else
            {
                Debug.Log("플레이어 우클릭: UI 꺼짐");
            }
        }
    }
    */
}
