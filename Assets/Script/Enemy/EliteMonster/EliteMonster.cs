using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonster : Enemy
{
    public float skill1CoolTime = 8f;
    public float skill1Delay;

    protected override void Update()
    {
        if (skill1Delay > 0)
        {
            skill1Delay -= Time.deltaTime;
        }
        else
        {
            skill1Delay = 0;
        }

        base.Update();
    }
}
