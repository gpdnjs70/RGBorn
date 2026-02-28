using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorAttribute : MonoBehaviour
{
    [SerializeField] public ColorSelection colorSelectionUI;
    private ColorAttribute attribute;
    private playerInteraction interaction;
    private BattleHandler battleHandler;

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

        colorSelectionUI.gameObject.SetActive(true);
        interaction.UpdateUIPosition();
    }

    private void SetColor(ColorMode mode)
    {
        Debug.Log($"SetColor 실행됨! 모드: {mode}");
        attribute.SetColor(mode);
        colorSelectionUI.OnColorChanged -= SetColor;

        if (battleHandler != null)
        {
            // Debug.Log("BattleHandler 찾음. EndTurn 실행.");
            battleHandler.EndTurn();
        }
    }
}
