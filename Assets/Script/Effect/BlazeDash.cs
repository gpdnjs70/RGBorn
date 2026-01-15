using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeDash : MonoBehaviour
{
    public float lifeTime;
    private Enemy enemy;
    private bool hasDealtDamage = false; // 중복 데미지 방지

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Enemy owner)
    {
        enemy = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDealtDamage || enemy == null || !collision.CompareTag("Player")) return;

        Player_HP playerHP = collision.GetComponent<Player_HP>();

        if (playerHP != null)
        {
            enemy.SetAttackType(EnemyAttackType.Skill2);

            float currentHp = playerHP.HP;
            float damage = currentHp * 0.05f;

            enemy.DealDamageToTarget(damage, 3f);

            hasDealtDamage = true;
            Debug.Log($"Blaze Dash 타격 성공! 데미지: {damage}");
        }
    }
}
