using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorAttribute : MonoBehaviour
{
    public ColorMode colorMode { get; private set; }

    public void SetColor(ColorMode mode)
    {
        colorMode = mode;
        Debug.Log($"{name} attribute set to {mode}");
    }

    public DebuffType GetDebuffType()
    {
        switch (colorMode)
        {
            case ColorMode.Red:
                return DebuffType.Burn;

            case ColorMode.Blue:
                return DebuffType.Slow;

            case ColorMode.Green:
                return DebuffType.LifeSteal;

            case ColorMode.Yellow:
                return DebuffType.Shock;

            case ColorMode.Purple:
                return DebuffType.Poison;

            case ColorMode.Cyan:
                return DebuffType.Corrosion;

            default:
                return DebuffType.None;
        }
    }
}
