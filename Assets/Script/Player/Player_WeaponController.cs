using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword = 0,
    Bow = 1,
    Staff = 2
}

public class Player_WeaponController : MonoBehaviour
{
    public WeaponType currentWeapon = WeaponType.Sword;

    [SerializeField] private Animator animator;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        SetWeaponAnimation(currentWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        // 1번 키 → Sword
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponType.Sword;
            SetWeaponAnimation(currentWeapon);
            Debug.Log("Weapon changed: Sword");
        }

        // 2번 키 → Bow
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.Bow;
            SetWeaponAnimation(currentWeapon);
            Debug.Log("Weapon changed: Bow");
        }

        // 3번 키 → Staff
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = WeaponType.Staff;
            SetWeaponAnimation(currentWeapon);
            Debug.Log("Weapon changed: Staff");
        }
    }

    private void SetWeaponAnimation(WeaponType wt)
    {
        if (animator == null) return;

        animator.SetInteger("Weapon", (int)wt);
        Debug.Log($"Weapon changed: {wt}");
    }
}
