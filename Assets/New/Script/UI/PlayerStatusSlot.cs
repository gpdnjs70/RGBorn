using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSlot : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image RImage;
    [SerializeField] private Image RImage_de;
    [SerializeField] private Image GImage;
    [SerializeField] private Image GImage_de;
    [SerializeField] private Image BImage;
    [SerializeField] private Image BImage_de;

    private character targetCharacter;

    // 캐릭터와 UI를 연결
    public void Bind(character target)
    {
        targetCharacter = target;
        if (playerImage != null && targetCharacter.CharacterPortrait != null)
        {
            playerImage.sprite = targetCharacter.CharacterPortrait;
            playerImage.preserveAspect = true;
        }

        // 초기 UI 세팅
        UpdateUI();
    }

    private void Update()
    {
        if (targetCharacter != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (targetCharacter == null) return;

        // HP 처리
        if (hpBar != null)
        {
            hpBar.value = targetCharacter.HP / targetCharacter.MaxHP;
        }
        if (hpText != null)
        {
            hpText.text = $"{Mathf.Max(0, targetCharacter.HP):F0} / {targetCharacter.MaxHP:F0}";
        }
        /*
        // 사망 시 처리
        if (playerImage != null)
        {
            playerImage.color = targetCharacter.HP <= 0 ? new Color(0.5f, 0.5f, 0.5f, 0.8f) : Color.white;
        }
        */

        // 색상 선택 UI 처리
        PlayerColorAttribute colorAttr = targetCharacter.GetComponent<PlayerColorAttribute>();
        if (colorAttr != null)
        {
            bool isRedActive = colorAttr.RedTurns > 0;
            RImage.gameObject.SetActive(isRedActive);
            RImage_de.gameObject.SetActive(!isRedActive);

            bool isGreenActive = colorAttr.GreenTurns > 0;
            GImage.gameObject.SetActive(isGreenActive);
            GImage_de.gameObject.SetActive(!isGreenActive);

            bool isBlueActive = colorAttr.BlueTurns > 0;
            BImage.gameObject.SetActive(isBlueActive);
            BImage_de.gameObject.SetActive(!isBlueActive);
        }
    }
}
