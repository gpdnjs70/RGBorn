using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{
    [SerializeField] EnemyData _enemyData;

    // Start is called before the first frame update
    void Start()
    {
        GetEnemyData();
    }

    void GetEnemyData()
    {
        print($"Name : {_enemyData.EnemyName}\n" +
            $"HP : {_enemyData.baseHP}\n" +
            // $"Damage : {_enemyData.Damage}\n" +
            $"MoveSpeed : {_enemyData.baseSpeed}\n" // +
            // $"Description : {_enemyData.EnemyDescription}"
            );
    }
}
