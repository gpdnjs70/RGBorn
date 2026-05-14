using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polyCollider;

    private Sprite lastSprite;

    public float speed;
    private Vector2 dir = Vector2.right;

    public LayerMask isLayer;

    public float damage = 20f;
    private Entity source;

    [SerializeField] private AttributeDatabase attributeDB;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
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
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    private void LateUpdate()
    {
        UpdateColliderShape();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 충돌한 대상의 레이어가 isLayer에 포함되어 있는지 확인
        if (((1 << collision.gameObject.layer) & isLayer) != 0)
        {
            Entity target = collision.GetComponent<Entity>();

            if (target != null && source != null)
            {
                // 자신(발사자)과는 충돌하지 않도록 예외 처리
                if (target == source) return;

                Entity actualTarget = source.ResolveAttackTarget(target);
                DealDamageWithAttribute(source, actualTarget);
            }

            // 레이어에 걸렸으므로(적 혹은 벽 등) 총알 파괴
            DestroyBullet();
        }
    }

    void DealDamageWithAttribute(Entity attacker, Entity target)
    {
        // 1. 세이브 데이터에서 누적 공격력 배율 로드
        SaveData data = SaveManager.Instance.LoadGame();
        float multiplier = (data != null) ? data.attackMultiplier : 1f;

        // 2. 기존 Stage 이름 기반 하드코딩을 삭제하고 배율 적용
        float finalDamage = damage * multiplier;

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

    private void UpdateColliderShape()
    {
        // 현재 렌더러의 스프라이트가 이전 프레임과 다를 때만 갱신
        if (spriteRenderer.sprite != lastSprite)
        {
            lastSprite = spriteRenderer.sprite;

            // 스프라이트가 없으면 처리 안 함
            if (lastSprite == null) return;

            // 기존 콜라이더의 경로 개수를 현재 스프라이트의 외곽선 개수로 맞춤
            polyCollider.pathCount = lastSprite.GetPhysicsShapeCount();

            // 각 경로(Path)를 돌며 새로운 모양 대입
            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < polyCollider.pathCount; i++)
            {
                path.Clear();
                lastSprite.GetPhysicsShape(i, path);
                polyCollider.SetPath(i, path.ToArray());
            }
        }
    }

    void DestroyBullet()
    {
        CancelInvoke("DestroyBullet");
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
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
