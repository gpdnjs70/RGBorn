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

    [SerializeField] private Image fillImage;
    [SerializeField] private List<Color> attributeColors = new List<Color>();

    private ColorAttribute monsterAttribute;
    private ColorMode lastMode = (ColorMode)(-1);

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    private RectTransform rectTrans;
    private Camera mainCam;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCam = Camera.main;

        if (entity != null)
        {
            monsterAttribute = entity.GetComponent<ColorAttribute>();
        }
    }

    public virtual void Update()
    {
        // HP 값, 텍스트 업데이트
        if(sliderHP != null)
        {
            sliderHP.value = Utils.Percent(entity.HP, entity.MaxHP);
        }
        if (textHP != null)
        {
            textHP.text = $"{entity.HP:F0}/{entity.MaxHP:F0}";
        }

        UpdateColorByAttribute();

        // HP 바 위치 이동
        if (target != null && rectTrans != null && mainCam != null)
        {
            Vector3 worldPos = target.position + worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
            rectTrans.position = screenPos;
        }
    }

    private void UpdateColorByAttribute()
    {
        if (monsterAttribute == null || fillImage == null || attributeColors.Count == 0) return;

        if (monsterAttribute.colorMode != lastMode)
        {
            lastMode = monsterAttribute.colorMode;
            int index = (int)lastMode;

            if (index >= 0 && index < attributeColors.Count)
            {
                fillImage.color = attributeColors[index];
            }
        }
    }
}
