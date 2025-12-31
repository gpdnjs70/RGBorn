using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public Stat MaxHP = new();
    public Stat Speed = new();
    public float HP;
}