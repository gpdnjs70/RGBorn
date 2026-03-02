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

    public DebuffType2 GetDebuffType()
    {
        switch (colorMode)
        {
            case ColorMode.Red:
                return DebuffType2.RDebuff;

            case ColorMode.Blue:
                return DebuffType2.BDebuff;

            case ColorMode.Green:
                return DebuffType2.GDebuff;

            case ColorMode.Yellow:
                return DebuffType2.YDebuff;

            case ColorMode.Purple:
                return DebuffType2.PDebuff;

            case ColorMode.Cyan:
                return DebuffType2.CDebuff;

            default:
                return DebuffType2.None;
        }
    }
}
