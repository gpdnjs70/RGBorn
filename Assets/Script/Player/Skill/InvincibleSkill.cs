using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleSkill : ISkill
{
    public string SkillName => "Invincible";
    public float Cooldown => 300.0f;
    public float Duration => 3.0f; // 무적 유지 시간

    private Player_HP playerHP;
    private SpriteRenderer spriteRenderer;
    private MonoBehaviour mono; // 코루틴 실행용

    public InvincibleSkill(GameObject player)
    {
        playerHP = player.GetComponent<Player_HP>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        mono = player.GetComponent<MonoBehaviour>();
    }

    public void Execute()
    {
        if (mono != null)
        {
            mono.StartCoroutine(InvincibleRoutine());
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        Debug.Log("무적 모드 활성화!");
        playerHP.IsInvincible = true;

        // 시각적 효과 (반투명하게)
        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;

        yield return new WaitForSeconds(Duration);

        // 상태 복구
        playerHP.IsInvincible = false;
        c.a = 1.0f;
        spriteRenderer.color = c;
        Debug.Log("무적 모드 종료");
    }
}
