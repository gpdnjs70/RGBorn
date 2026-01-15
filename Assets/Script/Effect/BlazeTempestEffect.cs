using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeTempestEffect : MonoBehaviour
{
    public float lifeTime;
    private Enemy enemy;
    private bool hasDealtDamage = false; // 중복 데미지 방지

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        
    }

    public void Setup(Enemy owner)
    {
        enemy = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 데미지를 줬거나, 주인 정보가 없거나, 플레이어가 아니면 리턴
        if (hasDealtDamage || enemy == null || !collision.CompareTag("Player")) return;

        // 플레이어의 HP 컴포넌트 가져오기
        Player_HP playerHP = collision.GetComponent<Player_HP>();

        if (playerHP != null)
        {
            // 공격 타입 설정
            enemy.SetAttackType(EnemyAttackType.Skill2);

            // 기존 데미지 로직: 현재 체력의 5%
            float currentHp = playerHP.HP;
            float damage = currentHp * 0.05f;

            // 데미지 적용
            enemy.DealDamageToTarget(damage, 3f);

            hasDealtDamage = true; // 타격 성공 시 플래그 세우기
            Debug.Log($"Blaze Tempest 타격 성공! 데미지: {damage}");
        }
    }
}
