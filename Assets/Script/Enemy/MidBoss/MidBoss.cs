using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : EliteMonster
{
    public int phase = 1;
    public float passiveSkillCoolTime = 30f;
    public float passiveSkillDelay;

    public int passiveSkillCount = 0;
    public int maxPassiveCount = 3;

    protected override void Update()
    {
        base.Update();

        if (passiveSkillDelay > 0)
        {
            passiveSkillDelay -= Time.deltaTime;
        }
        else
        {
            passiveSkillDelay = 0;
        }

        // 체력 비율에 따라 페이즈 전환 (50% 이하일 때 2페이즈)
        if (hp != null)
        {
            float hpRatio = hp.HP / hp.MaxHP;

            if (hpRatio <= 0.5f)
            {
                phase = 2;
                // Debug.Log("phase change 2");
            }
            else
            {
                phase = 1;
                // Debug.Log("phase change 1");
            }
        }
    }
}
