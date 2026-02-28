using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerInteraction : MonoBehaviour
{
    public GameObject selectionUI; // 연결할 UI 오브젝트
    public GameObject colorSelectionUI;
    public Vector3 uiOffset;

    private CharacterBattle characterBattle;

    void Awake()
    {
        characterBattle = GetComponent<CharacterBattle>();
    }

    void Start()
    {
        /*
        if (selectionUI == null)
        {
            selectionUI = GameObject.Find("PlayerSelection");

            // 찾은 직후에는 일단 꺼둠
            if (selectionUI != null) selectionUI.SetActive(false);
        }
        */
    }

    // 특정 UI 하나를 업데이트하는 범용 메서드
    private void SetUIPosition(GameObject ui)
    {
        if (ui == null) return;

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        RectTransform canvasRect = ui.transform.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPos))
        {
            ui.GetComponent<RectTransform>().anchoredPosition = localPos + (Vector2)uiOffset;
        }
    }

    public void UpdateUIPosition()
    {
        // 필요할 때마다 호출
        SetUIPosition(selectionUI);
        SetUIPosition(colorSelectionUI);
    }
}
