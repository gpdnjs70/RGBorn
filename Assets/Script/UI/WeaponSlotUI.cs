using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image blackMask;

    private void Awake()
    {
        // 기본 상태: 선택 안 됨 → 검은 마스크 켜둠
        if (blackMask != null)
            blackMask.enabled = true;
    }

    // 선택/비선택에 따라 마스크 켜고 끄기
    public void SetSelected(bool isSelected)
    {
        if (blackMask == null) return;

        // 선택되면 검은 덮개 OFF, 선택 안 되면 ON
        blackMask.enabled = !isSelected;
    }
}
