using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject SceneChange;

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
            else
            {
                Debug.Log("PortalTrigger가 EnemyManager에 연결되지 않음");
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

    public void ShowAllEnemiesUI()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ActivateSkillUI();
            }
        }
    }
}
