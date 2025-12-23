using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    public ColorMode currentMode { get; private set; }

    [SerializeField] private ColorButtonController colorUI;

    private void Awake()
    {
        colorUI.OnColorChanged += SetColor;
    }

    private void SetColor(ColorMode mode)
    {
        currentMode = mode;
        Debug.Log($"PlayerAttribute color updated: {mode}");
    }
}
