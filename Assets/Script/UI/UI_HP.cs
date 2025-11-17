using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HP : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    void Update()
    {
        if(sliderHP != null)
        {
            sliderHP.value = Utils.Percent(entity.HP, entity.MaxHP);
        }
        if (textHP != null)
        {
            textHP.text = $"{entity.HP:F0}/{entity.MaxHP:F0}";
        }
    }
}
