using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerInteraction : MonoBehaviour, IPointerClickHandler
{
    public GameObject selectionUI; // 연결할 UI 오브젝트
    public Vector3 uiOffset;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 오른쪽 마우스 클릭인지 확인
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // UI 활성화
            selectionUI.SetActive(true);

            // 1. 플레이어의 월드 좌표를 스크린 좌표로 변환
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            // 2. 스크린 좌표를 캔버스(UI) 좌표로 변환 (Scale 문제를 해결해줌)
            RectTransform canvasRect = selectionUI.transform.parent.GetComponent<RectTransform>();
            Vector2 localPos;

            // 이 함수가 캔버스의 Scale이 무엇이든 상관없이 정확한 UI 위치를 찾아줍니다.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                null, // Overlay 모드일 때는 null을 넣습니다.
                out localPos
            );

            // 3. 변환된 좌표에 오프셋 적용 (이제 Vector2 값을 쓰면 됩니다)
            selectionUI.GetComponent<RectTransform>().anchoredPosition = localPos + (Vector2)uiOffset;

            Debug.Log("플레이어 우클릭됨!");
        }
    }
}
