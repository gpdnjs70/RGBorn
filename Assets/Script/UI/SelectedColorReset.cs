using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedColorReset : MonoBehaviour
{
    [SerializeField] private ColorButtonController colorController;
    [SerializeField] private Image selectedWeaponImage;

    public void OnClickReset()
    {
        // 색 선택/섞기 상태 전부 초기화
        if (colorController != null)
        {
            colorController.ResetColorSelection();
        }

        Debug.Log("SelectedWeapon 클릭 → 색/선택 상태 초기화");
    }
}
