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

        // 시각적 피드백 (예: 캐릭터 색상을 보라색이나 빨간색으로 변경)
        SpriteRenderer sr = playerEntity.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = new Color(0.5f, 1.0f, 0.5f); // 붉은 기운

        yield return new WaitForSeconds(Duration);

        sr.color = originalColor;
        playerEntity.IsLifeStealActive = false;
        Debug.Log("체력 흡수 종료");
    }
}