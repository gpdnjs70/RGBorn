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
        hpBar.value = targetCharacter.HP / targetCharacter.MaxHP;
        hpText.text = $"{(int)targetCharacter.HP} / {(int)targetCharacter.MaxHP}";
        // 사망 시 흐리게 처리하는 등의 추가 로직
    }
}
