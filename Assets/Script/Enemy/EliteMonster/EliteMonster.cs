using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonster : Enemy
{
    protected override void Start()
    {
        base.Start();
        EnemyManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);
    }
}
