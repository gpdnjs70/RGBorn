using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject skillEffectPrefab;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Attack()
    {
        if (enemy == null || enemy.targetHP == null) return;

        enemy.SetAttackType(EnemyAttackType.Normal);

        float damage = 10 * (1f + enemy.attackBonusRate);
        enemy.DealDamageToTarget(damage, 3f);
    }

    public void Skill1()
    {
        if (enemy == null || enemy.targetHP == null) return;

        enemy.SetAttackType(EnemyAttackType.Skill1, 3f);

        float damage = 20 * (1f + enemy.attackBonusRate);
        enemy.DealDamageToTarget(damage, 3f);

        KnockBack knockBack = enemy.targetHP.GetComponent<KnockBack>();

        if (knockBack != null)
        {
            knockBack.PlayFeedBack(enemy.gameObject);
        }
    }

    public void Skill2()
    {
        // punishment Effect
        Vector3 pos = enemy.player.transform.position;
        pos += Vector3.up * 0.5f;
        Instantiate(skillEffectPrefab, pos, Quaternion.identity);

        if (enemy != null && enemy.targetHP != null)
        {
            float currentHp = enemy.targetHP.HP;
            float damage = currentHp * 0.05f;

            enemy.DealDamageToTarget(damage, 3f);
        }

        enemy.SetAttackType(EnemyAttackType.Skill2);
    }
}
