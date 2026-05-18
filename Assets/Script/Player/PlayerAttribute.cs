using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public bool hasGear = false; // 장비 획득 여부
    private const int GEAR_OFFSET = 4; // 장비 장착 시 건너뛸 인덱스 거리

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        if (animator == null) animator = GetComponent<Animator>();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Tutorial_Inside" || sceneName == "Tutorial_Outside" || sceneName == "Town" || sceneName == "Stage1_after" || sceneName == "Temple" || sceneName == "Stage3" || sceneName == "RBoss")
        {
            currentModeIndex = 0;
        }
        else
        {
            currentModeIndex = 1;
        }

        SetColor((ColorMode)currentModeIndex);
    }

    private void Update()
    {
        if (PauseManager.IsPaused) return;

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
        int baseIndex = (int)mode;
        // Debug.Log($"PlayerAttribute color updated: {mode}");

        // 장비를 가졌다면 인덱스에 4를 더함 (예: 1번 속성 -> 5번 속성 오버라이드)
        int targetIndex = hasGear ? baseIndex + GEAR_OFFSET : baseIndex;

        if (attributeOverrides != null && targetIndex < attributeOverrides.Length && attributeOverrides[targetIndex] != null)
        {
            animator.runtimeAnimatorController = attributeOverrides[targetIndex];
        }

        UpdateUIAlpha(baseIndex);
    }

    public void SetNoneColor()
    {
        currentModeIndex = 0; // None 인덱스
        SetColor(ColorMode.None);
    }

    public void SetRGBColor(ColorMode targetMode)
    {
        int index = (int)targetMode;
        if (index < 0 || index > 3)
        {
            return;
        }

        currentModeIndex = index;
        SetColor(targetMode);
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

    public void OnEquipGear()
    {
        hasGear = true;
        // 장비를 얻자마자 현재 모드의 강화된 버전으로 즉시 갱신
        SetColor((ColorMode)currentModeIndex);
    }
}
