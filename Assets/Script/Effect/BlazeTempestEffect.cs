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
        if (hasDealtDamage || enemy == null || !collision.CompareTag("Player")) return;

        // 플레이어와 부딪히는 순간 즉시 이동 멈춤
        isStopped = true;

        Player_HP playerHP = collision.GetComponent<Player_HP>();

        if (playerHP != null)
        {
            enemy.SetAttackType(EnemyAttackType.Skill2);

            float currentHp = playerHP.HP;
            float damage = currentHp * 0.05f;

            enemy.DealDamageToTarget(damage, 3f);

            hasDealtDamage = true;
            Debug.Log($"Blaze Tempest 타격 성공! 데미지: {damage}");

            // 부딪힌 위치에서 0.4초 동안 이펙트를 보여준 뒤 삭제
            Destroy(gameObject, 0.4f);
        }
    }
}