using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorAttribute : MonoBehaviour
{
    [SerializeField] public ColorSelection colorSelectionUI;
    private ColorAttribute attribute;
    private playerInteraction interaction;
    private BattleHandler battleHandler;

    private int redTurns = 0;
    private int greenTurns = 0;
    private int blueTurns = 0;

    public int RedTurns => redTurns;
    public int GreenTurns => greenTurns;
    public int BlueTurns => blueTurns;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        interaction = GetComponent<playerInteraction>();
        battleHandler = FindObjectOfType<BattleHandler>();
    }

    public void ActivateColorSelection()
    {
        colorSelectionUI.OnColorChanged -= SetColor;
        colorSelectionUI.OnColorChanged += SetColor;

        colorSelectionUI.Open(this.GetComponent<character>());

        interaction.UpdateUIPosition();
    }

    public void DeactivateColorSelection()
    {
        if (colorSelectionUI != null)
        {
            // 이벤트 구독 해제 (중복 호출 방지)
            colorSelectionUI.OnColorChanged -= SetColor;

            // UI 오브젝트 비활성화
            colorSelectionUI.gameObject.SetActive(false);

            // Debug.Log("[ColorUI] 색상 선택 창을 강제로 닫았습니다.");
        }
    }

    private void SetColor(ColorMode newMode)
    {
        Debug.Log($"선택된 색상: {newMode}");

        // 새롭게 선택한 기본 색상의 타이머를 3으로 리셋
        if (newMode == ColorMode.Red) redTurns = 3;
        else if (newMode == ColorMode.Green) greenTurns = 3;
        else if (newMode == ColorMode.Blue) blueTurns = 3;

        // 현재 타이머 상황에 맞춰 최종 색상 결정
        UpdateFinalColor();

        colorSelectionUI.OnColorChanged -= SetColor;

        if (battleHandler != null)
        {
            battleHandler.EndTurn();
        }
    }

    // 색상 조합 규칙 정의 함수
    // 현재 남은 턴수들을 조합하여 최종 Attribute 결정
    private void UpdateFinalColor()
    {
        bool hasRed = redTurns > 0;
        bool hasGreen = greenTurns > 0;
        bool hasBlue = blueTurns > 0;

        ColorMode resultMode = ColorMode.None;

        // 세 가지가 다 섞인 경우
        if (hasRed && hasGreen && hasBlue)
        {
            // resultMode = ColorMode.Purple;
        }
        // 두 가지 조합
        else if (hasRed && hasBlue) resultMode = ColorMode.Purple;
        else if (hasBlue && hasGreen) resultMode = ColorMode.Cyan;
        else if (hasGreen && hasRed) resultMode = ColorMode.Yellow;
        // 단일 색상
        else if (hasRed) resultMode = ColorMode.Red;
        else if (hasGreen) resultMode = ColorMode.Green;
        else if (hasBlue) resultMode = ColorMode.Blue;
        // 아무것도 없음
        else resultMode = ColorMode.None;

        if (attribute.colorMode != resultMode)
        {
            Debug.Log($"[색상 변화] {attribute.colorMode} -> {resultMode}");
            attribute.SetColor(resultMode);
        }
    }

    public void DecayTurn()
    {
        if (redTurns > 0) redTurns--;
        if (greenTurns > 0) greenTurns--;
        if (blueTurns > 0) blueTurns--;

        Debug.Log($"{gameObject.name} 타이머 현황 - R:{redTurns}, G:{greenTurns}, B:{blueTurns}");

        // 턴이 깎인 후 다시 최종 색상 계산
        UpdateFinalColor();
    }
}
