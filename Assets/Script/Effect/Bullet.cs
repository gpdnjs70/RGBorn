using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Vector2 dir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    public float damage = 20f;
    private Entity source;

    [SerializeField] private AttributeDatabase attributeDB;    

    void Start()
    {
        Invoke("DestroyBullet", 2);
    }

    public void Init(Entity sourceEntity)
    {
        source = sourceEntity;
    }

    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if(ray.collider != null)
        {
            Entity target = ray.collider.GetComponent<Entity>();
            if (target != null && source != null)
            {
                Entity actualTarget = source.ResolveAttackTarget(target);

                // 데미지 계산 + 디버프 전부 Entity에게 위임
                DealDamageWithAttribute(source, actualTarget);
            }

            DestroyBullet();
            return;
        }

        transform.Translate(dir * speed * Time.deltaTime);
    }

    void DealDamageWithAttribute(Entity attacker, Entity target)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "Stage1":
                damage *= 1.2f;
                break;
            case "Stage2":
                damage *= 1.44f;
                break;
            case "Stage3":
                damage *= 1.728f;
                break;
            default:
                break;
        }

        float finalDamage = damage;

        ColorAttribute atkAttrComp = attacker.GetComponent<ColorAttribute>();

        ColorAttribute defAttrComp = target.GetComponent<ColorAttribute>();

        if (atkAttrComp != null && defAttrComp != null)
        {
            ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);

            ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

            if (atkAttr.strongAgainst.Contains(defAttr.colorMode))
                finalDamage *= (1f + atkAttr.attackBonusPercent);

            if (atkAttr.weakAgainst.Contains(defAttr.colorMode))
                finalDamage *= (1f - atkAttr.attackMinusPercent);
        }

        if (UnityEngine.Random.value < 0.3f)
        {
            attacker.ApplyAttributeDebuffTo(target, 3f);
        }

        target.TakeDamage(finalDamage);
    }


    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void SetDirection(float direction)
    {
        float sign = direction >= 0 ? 1f : -1f;
        dir = new Vector2(sign, 0f);

        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }
}
