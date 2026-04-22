using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealSkill : ISkill
{
    public string SkillName => "LifeSteal";
    public float Cooldown => 40.0f;
    public float Duration => 5.0f;
    public float HealRatio = 0.2f; // 가한 대미지의 20% 회복

    private Entity playerEntity;
    private MonoBehaviour mono;

    public LifeStealSkill(GameObject player)
    {
        playerEntity = player.GetComponent<Entity>();
        mono = player.GetComponent<MonoBehaviour>();
    }

    public void Execute()
    {
        mono.StartCoroutine(LifeStealRoutine());
    }

    private IEnumerator LifeStealRoutine()
    {
        Debug.Log("체력 흡수 활성화!");
        playerEntity.IsLifeStealActive = true;
        playerEntity.LifeStealRatio = HealRatio;

        Player_HP pHP = playerEntity.GetComponent<Player_HP>();
        if (pHP != null) pHP.UpdateVisuals();

        yield return new WaitForSeconds(Duration);

        playerEntity.IsLifeStealActive = false;
        if (pHP != null) pHP.UpdateVisuals();
        Debug.Log("체력 흡수 종료");
    }
}