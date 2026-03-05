using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    private void Awake()
    {

    }

    public virtual void Update()
    {
        // HP 값, 텍스트 업데이트
        if (sliderHP != null)
        {
            sliderHP.value = Utils.Percent(entity.HP, entity.MaxHP);
        }
        if (textHP != null)
        {
            textHP.text = $"{entity.HP:F0}/{entity.MaxHP:F0}";
        }
    }
}
