using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackEvent : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Attack()
    {
        if (enemy == null || enemy.targetHP == null) return;

        enemy.SetAttackType(EnemyAttackType.Normal);

        int damage = 10;
        enemy.targetHP.TakeDamage(damage);
    }

    public void Skill1()
    {
        if (enemy == null || enemy.targetHP == null) return;

        enemy.SetAttackType(EnemyAttackType.Skill1, 3f);

        int damage = 20;
        enemy.targetHP.TakeDamage(damage);

        KnockBack knockBack =
            enemy.targetHP.GetComponent<KnockBack>();

        if (knockBack != null)
        {
            knockBack.PlayFeedBack(enemy.gameObject);
        }
    }

    public void Skill2()
    {
        // 이미 실행 중이면 무시
        if (enemy.currentAttackType == EnemyAttackType.Skill2)
            return;

        enemy.SetAttackType(EnemyAttackType.Skill2);
        enemy.StartBurn(10f, 2f);

        if (enemy != null || enemy.targetHP != null)
        {
            float currentHp = enemy.targetHP.HP;
            float damage = currentHp * 0.05f;

            enemy.targetHP.TakeDamage(damage);
        }
    }
}
