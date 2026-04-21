using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    private Animator anim;
    public float speed;
    private Vector2 dir = Vector2.right;

    public float distance;
    public LayerMask isLayer;

    public float damage = 20f;
    private Entity source;

    [SerializeField] private AttributeDatabase attributeDB;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

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

        transform.Translate(dir * speed * Time.deltaTime, Space.World);
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

        /**
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
        **/

        /**
        if (UnityEngine.Random.value < 0.3f)
        {
            attacker.ApplyAttributeDebuffTo(target, 3f);
        }
        **/

        target.TakeDamage(finalDamage);

        if (attacker != null && attacker.IsLifeStealActive)
        {
            float healAmount = finalDamage * attacker.LifeStealRatio;
            attacker.Heal(healAmount);
        }
    }


    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        dir = direction.normalized;

        if (anim != null)
        {
            // 애니메이터의 파라미터에 방향 값 전달
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }

        // 레이캐스트가 날아가는 방향을 위해 Rotation은 맞춰주는 것이 좋습니다.
        // 다만, 비주얼은 애니메이터가 담당하므로 SpriteRenderer의 회전은 끌 수도 있습니다.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
