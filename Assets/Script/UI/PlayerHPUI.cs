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

    [SerializeField] private Image fillImage;
    [SerializeField] private List<Color> attributeColors = new List<Color>();
    private ColorAttribute playerAttribute;
    private ColorMode lastMode = (ColorMode)(-1);

    private void Awake()
    {
        if (entity != null)
        {
            playerAttribute = entity.GetComponent<ColorAttribute>();
            if (playerAttribute == null)
                Debug.LogError($"{entity.name}에 ColorAttribute 컴포넌트가 없습니다!");
        }
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

        UpdateColorByAttribute();
    }

    private void UpdateColorByAttribute()
    {
        if (playerAttribute == null || fillImage == null || attributeColors == null)
        {
            return;
        }

        if (playerAttribute.colorMode != lastMode)
        {
            int index = (int)playerAttribute.colorMode;

            if (attributeColors.Count > 0 && index >= 0 && index < attributeColors.Count)
            {
                fillImage.color = attributeColors[index];
                lastMode = playerAttribute.colorMode;
            }
            else
            {
                lastMode = playerAttribute.colorMode;
            }
        }
    }
}
