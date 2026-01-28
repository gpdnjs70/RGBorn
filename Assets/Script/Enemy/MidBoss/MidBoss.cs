using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : EliteMonster
{
    public int phase = 1;

    protected override void Update()
    {
        base.Update();

        // 체력 비율에 따라 페이즈 전환 (50% 이하일 때 2페이즈)
        if (hp != null)
        {
            float hpRatio = hp.HP / hp.MaxHP;

            if (hpRatio <= 0.5f)
            {
                phase = 2;
            }
            else
            {
                phase = 1;
            }
        }
    }
}
