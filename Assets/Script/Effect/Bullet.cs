using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector2 dir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    public float damage = 20f;
    
    private ColorButtonController colorController;

    [SerializeField] private AttributeDatabase attributeDB;
    private PlayerAttribute playerAttribute;

    void Start()
    {
        Invoke("DestroyBullet", 2);
    }

    public void Init(PlayerAttribute playerAttr)
    {
        playerAttribute = playerAttr;
    }

    private void Awake()
    {
        colorController = FindFirstObjectByType<ColorButtonController>();
    }

    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "Enemy")
            {
                Enemy_HP enemy_HP = ray.collider.GetComponent<Enemy_HP>();
                EnemyAttribute enemyAttr = ray.collider.GetComponent<EnemyAttribute>();
                ColorAttributeData defAttr = attributeDB.Get(enemyAttr.colorMode);

                ColorAttributeData atkAttr = attributeDB.Get(playerAttribute.currentMode);

                float finalDamage = damage;

                int matchCount = 0;
                float bonusSum = 0f;
                if (atkAttr.strongAgainst.Contains(defAttr.colorMode))
                {
                    bonusSum += atkAttr.attackBonusPercent;
                    matchCount++;
                }
                if (matchCount > 0)
                {
                    float avgBonus = bonusSum / matchCount; // 결국 attackBonusPercent
                    finalDamage *= (1f + avgBonus);

                    Debug.Log($"평균 보너스 +{avgBonus * 100f}% → {finalDamage}");
                }

                int weakCount = 0;
                float penaltySum = 0f;
                if (atkAttr.weakAgainst.Contains(defAttr.colorMode))
                {
                    penaltySum += atkAttr.attackMinusPercent;
                    weakCount++;
                }
                if (weakCount > 0)
                {
                    float avgPenalty = penaltySum / weakCount;
                    finalDamage *= (1f - avgPenalty);

                    Debug.Log($"평균 마이너스 +{avgPenalty * 100f}% → {finalDamage}");
                }

                if (enemy_HP != null)
                {
                    enemy_HP.TakeDamage(finalDamage);
                    Debug.Log($"데미지는 {finalDamage}");
                }
            }

            DestroyBullet();
        }

        transform.Translate(dir * speed * Time.deltaTime);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        float sign = direction.x >= 0 ? 1f : -1f;
        dir = new Vector2(sign, 0f);

        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }
}
