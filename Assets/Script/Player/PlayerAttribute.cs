using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttribute : MonoBehaviour
{
    private ColorAttribute attribute;
    public Animator animator;
    public AnimatorOverrideController[] attributeOverrides;

    private int currentModeIndex = 0;

    public Image[] attributeImages;
    public float activeAlpha = 0f;
    public float inactiveAlpha = 0.8f;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        if (animator == null) animator = GetComponent<Animator>();

        SetColor((ColorMode)currentModeIndex);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
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

        if (attributeOverrides != null && index < attributeOverrides.Length && attributeOverrides[index] != null)
        {
            // 배열에 담긴 오버라이드 컨트롤러 에셋을 현재 애니메이터에 꽂아버립니다.
            animator.runtimeAnimatorController = attributeOverrides[index];
        }

        UpdateUIAlpha(index);
    }

    private void UpdateUIAlpha(int activeIndex)
    {
        for (int i = 0; i < attributeImages.Length; i++)
        {
            if (attributeImages[i] == null) continue;

            Color color = attributeImages[i].color;

            color.a = (i + 1 == activeIndex) ? activeAlpha : inactiveAlpha;

            attributeImages[i].color = color;
        }
    }
}
