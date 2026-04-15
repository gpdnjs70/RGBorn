using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifySkill : ISkill
{
    public string SkillName => "Identify";
    public float Cooldown => 30.0f;

    public IdentifySkill() { }

    public void Execute()
    {
        // EnemyManager에 있는 모든 적의 UI 활성화 함수 호출
        if (EnemyManager.Instance != null)
        {
            Debug.Log("Identify 스킬 호출");
            // EnemyManager.Instance.ShowAllEnemiesUI();
        }
        else
        {
            Debug.LogWarning("EnemyManager 인스턴스를 찾을 수 없습니다!");
        }
    }
}
