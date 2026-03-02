using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelection : MonoBehaviour
{
    public System.Action<ColorMode> OnColorChanged;
    private character currentOwner; // 누가 이 UI를 열었는지 저장할 변수

    public void Open(character owner)
    {
        currentOwner = owner;
        this.gameObject.SetActive(true);
    }

    private void SelectColor(ColorMode mode)
    {
        // 1. 이벤트 먼저 날리기
        OnColorChanged?.Invoke(mode);

        // 2. 미리 저장해둔 주인(currentOwner)으로 정산하기
        if (currentOwner != null)
        {
            BattleHandler bh = Object.FindObjectOfType<BattleHandler>();
            if (bh != null)
            {
                Debug.Log($"[정산 시작] 주인: {currentOwner.name}");
                bh.ProcessMyAppliedDebuffs(currentOwner);
            }
        }
        /*
        else
        {
            Debug.LogError("UI의 주인이 설정되지 않았습니다! Open(character)을 사용했나요?");
        }
        */

        gameObject.SetActive(false);
        currentOwner = null; // 초기화
    }

    // 버튼 연결용
    public void OnClickRed() => SelectColor(ColorMode.Red);
    public void OnClickGreen() => SelectColor(ColorMode.Green);
    public void OnClickBlue() => SelectColor(ColorMode.Blue);
}