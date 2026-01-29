using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeDash : MonoBehaviour
{
    public float lifeTime;
    private Enemy enemy;
    private MidBossAttackEvent bossScript;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Enemy owner)
    {
        enemy = owner;
        bossScript = owner.GetComponent<MidBossAttackEvent>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy == null || bossScript == null || bossScript.dashHitRegistered) return;

        if (collision.CompareTag("Player"))
        {
            Player_HP playerHP = collision.GetComponent<Player_HP>();

            if (playerHP != null)
            {
                bossScript.RegisterDashHit();

                enemy.SetAttackType(EnemyAttackType.Skill4);

                float currentHp = playerHP.HP;
                float damage = 10f;

                enemy.DealDamageToTarget(damage, 3f);

                Debug.Log($"Blaze Dash 타격 성공! 데미지: {damage}");
            }
        }
    }
}
