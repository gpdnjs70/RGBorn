using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HP : MonoBehaviour
{
    public int HP = 3;

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }
}
