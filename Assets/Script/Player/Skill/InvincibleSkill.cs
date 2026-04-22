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

        playerHP.UpdateVisuals(); // 상태가 변했으니 비주얼 갱신

        yield return new WaitForSeconds(Duration);

        playerHP.IsInvincible = false;
        playerHP.UpdateVisuals(); // 복구
        Debug.Log("무적 모드 종료");
    }
}
