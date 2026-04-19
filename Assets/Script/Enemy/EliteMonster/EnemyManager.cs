using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject SceneChange;
    [SerializeField] private RBossSceneManager bossSceneManager;

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

        if (enemies.Count == 0)
        {
            Debug.Log("모든 몬스터 처치!");

            if (SceneChange != null)
            {
                SceneChange.SetActive(true);
            }

            if (bossSceneManager != null)
            {
                bossSceneManager.OnAllEnemiesDefeated();
            }
        }
    }

    // Enemy 사망 시 호출
    public void OnEnemyDied(Enemy deadEnemy)
    {
        foreach (var enemy in enemies)
        {
            if (enemy == deadEnemy) continue;

            enemy.AddAttackBonus(enemy.powerBonusRate);
        }
    }
}
