using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : ISkill
{
    public string SkillName => "Fireball";
    public float Cooldown => 3.0f;

    private GameObject bulletPrefab;
    private Transform firePos;
    private GameObject owner;
    private Player_Movement movement;
    private Animator animator;
    private Entity entity;

    // 생성자를 통해 필요한 참조를 미리 받아둡니다.
    public Fireball(GameObject player, GameObject bullet, Transform pos)
    {
        owner = player;
        bulletPrefab = bullet;
        firePos = pos;

        movement = player.GetComponent<Player_Movement>();
        animator = player.GetComponent<Animator>();
        entity = player.GetComponent<Entity>();
    }

    public void Execute()
    {
        UpdateFirePosition();

        if (animator != null)
        {
            animator.SetTrigger("Skill");
        }

        Debug.Log($"{SkillName} 발사!");
    }

    private void UpdateFirePosition()
    {
        if (movement == null || firePos == null) return;

        float sign = movement.dirX >= 0 ? 1f : -1f;

        Vector3 local = firePos.localPosition;
        local.x = Mathf.Abs(local.x) * sign;
        firePos.localPosition = local;
    }
}
