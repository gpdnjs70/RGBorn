using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]

public class EnemyData : ScriptableObject
{
    public string EnemyName;
    [TextArea]
    public string EnemyDescription;

    public float baseHP;
    public float baseSpeed;
}
