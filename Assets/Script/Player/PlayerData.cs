using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public float baseHP = 50f;
    public float damageMultiplier = 1f;

    public bool stage2Applied = false;
}
