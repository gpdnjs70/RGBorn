using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HP : Entity
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Entity에 정의되어 있는 Setup() 메소드 호출
        base.Setup();
    }

    // 기본 체력 + 스탯 보너스 + 버프 등과 같이 계산
    public override float MaxHP => MaxHPBasic + MaxHPAttrBonus;
    // 100 + 현재 레벨 * 30
    public float MaxHPBasic     => 100 + 1 * 30;
    // 힘 * 10
    public float MaxHPAttrBonus => 10 * 10;

    public override float HPRecovery => 10;

    public override void TakeDamage(float damage)
    {
        HP -= damage;

        StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.1f);

        color.a = 1;
        spriteRenderer.color = color;
    }
}
