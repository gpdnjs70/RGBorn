using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentEffect : MonoBehaviour
{
    public float lifeTime;
    private Enemy enemy;
    private EnemyAttackType attackType = EnemyAttackType.Skill2;
    private float damageValue = 5f;

    private bool hasDealtDamage = false; // 중복 데미지 방지
    private Collider2D effectCollider;

    private void Awake()
    {
        effectCollider = GetComponent<Collider2D>();
        if (effectCollider != null)
        {
            effectCollider.enabled = false;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Enemy owner, EnemyAttackType type, float damage)
    {
        enemy = owner;
        attackType = type;
        damageValue = damage;
    }

    public void EnableCollider()
    {
        if (effectCollider != null)
        {
            effectCollider.enabled = true;
        }
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
            enemy.SetAttackType(attackType);

            // 기존 데미지 로직: 현재 체력의 5%
            float currentHp = playerHP.HP;
            float damage = damageValue;

            // 데미지 적용
            enemy.DealDamageToTarget(damage, 3f);

            hasDealtDamage = true; // 타격 성공 시 플래그 세우기
            Debug.Log($"Punishment 타격 성공! 데미지: {damage}");

            effectCollider.enabled = false;
        }
    }
}
