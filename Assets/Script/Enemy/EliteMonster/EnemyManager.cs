using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    // Enemy ªÁ∏¡ Ω√ »£√‚
    public void OnEnemyDied(Enemy deadEnemy)
    {
        foreach (var enemy in enemies)
        {
            if (enemy == deadEnemy) continue;

            enemy.AddAttackBonus(enemy.powerBonusRate);
        }
    }
}
