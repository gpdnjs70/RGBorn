using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelection : MonoBehaviour
{
    public System.Action<ColorMode> OnColorChanged;

    // R, G, B 버튼에 각각 연결할 함수들
    public void OnClickRed() => SelectColor(ColorMode.Yellow);
    public void OnClickGreen() => SelectColor(ColorMode.Purple);
    public void OnClickBlue() => SelectColor(ColorMode.Cyan);

    private void SelectColor(ColorMode mode)
    {
        Debug.Log($"[ColorSelection] 선택된 색상 모드: {mode}");

        OnColorChanged?.Invoke(mode); // 구독 중인 플레이어에게 색상 전달
        gameObject.SetActive(false); // 선택 후 UI 닫기
    }
}