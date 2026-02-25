using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject skillEffectPrefab;

    [SerializeField] private Vector2 normalAttackSize;
    [SerializeField] private Vector2 Skill1AttackSize;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Attack()
    {
        if (enemy == null || enemy.targetHP == null) return;

        Collider2D hit = Physics2D.OverlapBox(enemy.pos.position, normalAttackSize, 0, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            enemy.SetAttackType(EnemyAttackType.Normal);

            float damage = 7 * (1f + enemy.attackBonusRate);
            enemy.DealDamageToTarget(damage, 3f);
        }
    }

    public void Skill1()
    {
        if (enemy == null || enemy.targetHP == null) return;

        Collider2D hit = Physics2D.OverlapBox(enemy.pos.position, Skill1AttackSize, 0, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            enemy.SetAttackType(EnemyAttackType.Skill1, 3f);

            float damage = 5 * (1f + enemy.attackBonusRate);
            enemy.DealDamageToTarget(damage, 3f);

            KnockBack knockBack = enemy.targetHP.GetComponent<KnockBack>();

            if (knockBack != null)
            {
                Debug.Log("≥ÀπÈ¿Ã »£√‚µ ");
                knockBack.PlayFeedBack(enemy.gameObject);
            }
        }        
    }

    public void Skill2()
    {
        Debug.Log("Skill2");
        // punishment Effect
        Vector3 pos = enemy.player.transform.position;
        pos += Vector3.up * 0.5f;
        GameObject effectObj = Instantiate(skillEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            float damage = enemy.player.GetComponent<Player_HP>().HP * 0.05f;
            effectScript.Setup(enemy, EnemyAttackType.Skill3, damage);
        }
    }
}
