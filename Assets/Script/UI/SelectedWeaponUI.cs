using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedWeaponUI : MonoBehaviour
{
    [SerializeField] private Player_WeaponController weaponController;
    [SerializeField] private Image weaponIconImage;

    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite bowSprite;
    [SerializeField] private Sprite staffSprite;

    private WeaponType lastWeapon = (WeaponType)(-1);

    private void Start()
    {
        UpdateIcon();
    }

    private void Update()
    {
        if (weaponController == null || weaponIconImage == null)
            return;

        // 무기가 바뀌었을 때만 갱신
        if (weaponController.currentWeapon != lastWeapon)
        {
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        if (weaponController == null || weaponIconImage == null)
            return;

        lastWeapon = weaponController.currentWeapon;

        switch (lastWeapon)
        {
            case WeaponType.Sword:
                weaponIconImage.sprite = swordSprite;
                break;
            case WeaponType.Bow:
                weaponIconImage.sprite = bowSprite;
                break;
            case WeaponType.Staff:
                weaponIconImage.sprite = staffSprite;
                break;
        }

        // 스프라이트가 없을 수도 있으니까 방어코드
        weaponIconImage.enabled = (weaponIconImage.sprite != null);
    }
}
