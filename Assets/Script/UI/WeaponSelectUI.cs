using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    [SerializeField] private Player_WeaponController weaponController;
    [SerializeField] private WeaponSlotUI[] slots;

    private WeaponType lastWeapon = (WeaponType)(-1);

    private void Start()
    {
        // 시작할 때 한 번 강제로 갱신
        UpdateSlots();
    }

    private void Update()
    {
        if (weaponController == null || slots == null || slots.Length == 0)
            return;

        if (weaponController.currentWeapon != lastWeapon)
        {
            UpdateSlots();
        }
    }

    private void UpdateSlots()
    {
        if (weaponController == null) return;

        lastWeapon = weaponController.currentWeapon;
        int currentIndex = (int)lastWeapon;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            bool isSelected = (i == currentIndex);
            slots[i].SetSelected(isSelected);
        }
    }
}
