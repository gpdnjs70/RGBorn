using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HP : MonoBehaviour
{
    [SerializeField] private character character;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    private RectTransform rectTrans;
    private Camera mainCam;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    void Update()
    {
        // HP 값, 텍스트 업데이트
        if (sliderHP != null)
        {
            sliderHP.value = Utils.Percent(character.HP, character.MaxHP);
        }
        if (textHP != null)
        {
            textHP.text = $"{character.HP:F0}/{character.MaxHP:F0}";
        }

        // HP 바 위치 이동
        if (target != null && rectTrans != null && mainCam != null)
        {
            Vector3 worldPos = target.position + worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
            rectTrans.position = screenPos;
        }
    }
}
