using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeTempestEffect : MonoBehaviour
{
    public float lifeTime;
    public float moveSpeed;
    private Enemy enemy;
    private Vector3 moveDirection;
    private bool hasDealtDamage = false;
    private bool isStopped = false; // 이동 정지 상태 플래그

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 무언가에 부딪혔다면 이동 계산 스킵
        if (isStopped) return;

        // 설정된 방향으로 매 프레임 지속적으로 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void Setup(Enemy owner)
    {
        enemy = owner;

        if (enemy != null && enemy.player != null)
        {
            moveDirection = (enemy.player.position - transform.position).normalized;

            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (moveDirection.x >= 0 ? -1f : 1f);
            transform.localScale = localScale;
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
            enemy.SetAttackType(EnemyAttackType.Skill2);

            // 기존 데미지 로직: 현재 체력의 5%
            float currentHp = playerHP.HP;
            float damage = currentHp * 0.05f;

            // 데미지 적용
            enemy.DealDamageToTarget(damage, 3f);

            hasDealtDamage = true; // 타격 성공 시 플래그 세우기
            Debug.Log($"Punishment 타격 성공! 데미지: {damage}");

            Destroy(gameObject, 0.5f);
        }
    }
}