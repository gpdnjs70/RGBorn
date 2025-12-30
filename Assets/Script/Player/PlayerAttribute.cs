using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    [SerializeField] private ColorButtonController colorUI;
    private ColorAttribute attribute;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        colorUI.OnColorChanged += SetColor;
    }

    private void SetColor(ColorMode mode)
    {
        attribute.SetColor(mode);
        Debug.Log($"PlayerAttribute color updated: {mode}");
    }
}
