using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    private ColorAttribute attribute;
    private int currentModeIndex = 0;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
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
    }
}
