using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttribute : MonoBehaviour
{
    private ColorAttribute attribute;
    private int currentModeIndex = 0;

    public Image playerAttributeImage;
    public Sprite[] modeSprites;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();

        if (playerAttributeImage == null) playerAttributeImage = GetComponent<Image>();

        SetColor((ColorMode)currentModeIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleColor();
        }
    }

    private void CycleColor()
    {
        currentModeIndex++;

        if (currentModeIndex > 3)
        {
            currentModeIndex = 1;
        }

        SetColor((ColorMode)currentModeIndex);
    }

    private void SetColor(ColorMode mode)
    {
        attribute.SetColor(mode);
        // Debug.Log($"PlayerAttribute color updated: {mode}");

        int index = (int)mode;

        if (modeSprites != null && index < modeSprites.Length && modeSprites[index] != null)
        {
            playerAttributeImage.sprite = modeSprites[index];
        }
        else
        {
            Debug.LogWarning($"{mode}에 해당하는 Sprite가 modeSprites 배열에 등록되지 않았습니다.");
        }
    }
}
