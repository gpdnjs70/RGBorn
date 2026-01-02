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

    float wheelCooldown = 0.15f;
    float lastWheelTime = 0f;

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
        if (Time.time - lastWheelTime < wheelCooldown) return;

        float wheel = Input.GetAxis("Mouse ScrollWheel");

        if (wheel > 0f)
        {
            ChangeWeapon(1);
            lastWheelTime = Time.time;
        }
        else if (wheel < 0f)
        {
            ChangeWeapon(-1);
            lastWheelTime = Time.time;
        }
    }

    void ChangeWeapon(int direction)
    {
        int weaponCount = System.Enum.GetValues(typeof(WeaponType)).Length;

        int nextIndex = (int)currentWeapon + direction;

        // 순환 처리
        if (nextIndex >= weaponCount)
            nextIndex = 0;
        else if (nextIndex < 0)
            nextIndex = weaponCount - 1;

        currentWeapon = (WeaponType)nextIndex;

        SetWeaponAnimation(currentWeapon);
        Debug.Log($"Weapon changed: {currentWeapon}");
    }


    private void SetWeaponAnimation(WeaponType wt)
    {
        if (animator == null) return;

        animator.SetInteger("Weapon", (int)wt);
        Debug.Log($"Weapon changed: {wt}");
    }
}
