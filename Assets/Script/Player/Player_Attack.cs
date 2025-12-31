using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.Member;
using static UnityEngine.EventSystems.EventTrigger;

public class Player_Attack : MonoBehaviour
{
    Entity entity;
    Animator animator;
    Rigidbody2D myrigidbody;

    void Start()
    {
        animator = GetComponent<Animator>();
        myrigidbody = GetComponent<Rigidbody2D>();
    }

    private float curTime = 0;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;

    [SerializeField] private Player_Movement movement;
    [SerializeField] private float attackLockTime = 0.35f;

    public float damage = 2f;

    [SerializeField] private Player_WeaponController weaponController;
    [SerializeField] private GameObject bowBulletPrefab;
    [SerializeField] private Transform bowBulletPos;

    [SerializeField] private GameObject staffBulletPrefab;
    [SerializeField] private Transform staffBulletPos;

    [SerializeField] private AttributeDatabase attributeDB;
    
    bool IsAdvantage(ColorAttributeData attacker, ColorAttributeData defender)
    {
        return attacker.strongAgainst.Contains(defender.colorMode);
    }

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!entity.CanAct)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UpdateAttackBoxPosition();

        if (curTime <= 0)
        {
            // 평타
            if (Input.GetMouseButtonDown(0))
            {
                DoAttack(damage);
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }        
    }

    void DoAttack(float damage)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (!collider.CompareTag("Enemy")) continue;

            Enemy_HP enemyHP = collider.GetComponent<Enemy_HP>();
            if (enemyHP == null) continue;

            Entity attacker = entity;
            Entity intendedTarget = enemyHP;
            Entity actualTarget = attacker.ResolveAttackTarget(intendedTarget);

            ColorAttribute atkAttrComp = attacker.GetComponent<ColorAttribute>();

            ColorAttribute defAttrComp = actualTarget.GetComponent<ColorAttribute>();

            float finalDamage = damage;

            if (atkAttrComp != null && defAttrComp != null)
            {
                ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);

                ColorAttributeData defAttr = attributeDB.Get(defAttrComp.colorMode);

                // 상성 계산
                if (atkAttr.strongAgainst.Contains(defAttr.colorMode))
                {
                    finalDamage *= (1f + atkAttr.attackBonusPercent);
                }

                if (atkAttr.weakAgainst.Contains(defAttr.colorMode))
                {
                    finalDamage *= (1f - atkAttr.attackMinusPercent);
                }
            }

            if (UnityEngine.Random.value < 0.3f)
            {
                attacker.ApplyAttributeDebuffTo(actualTarget, 3f);
            }

            actualTarget.TakeDamage(finalDamage);
        }

        StartCoroutine(Attack());
    }

    public void OnBowShoot()
    {
        if (weaponController != null &&
            weaponController.currentWeapon == WeaponType.Bow)
        {
            SpawnBowBullet();
        }
    }

    private void SpawnBowBullet()
    {
        if (bowBulletPrefab == null || bowBulletPos == null)
            return;

        GameObject b = Instantiate(bowBulletPrefab, bowBulletPos.position, Quaternion.identity);

        Bullet bulletScript = b.GetComponent<Bullet>();
        if (bulletScript != null && movement != null)
        {
            bulletScript.SetDirection(movement.lastDir);
            bulletScript.Init(GetComponent<Entity>());
        }
    }

    public void OnStaffShoot()
    {
        if (weaponController != null &&
            weaponController.currentWeapon == WeaponType.Staff)
        {
            SpawnStaffBullet();
        }
    }

    private void SpawnStaffBullet()
    {
        if (staffBulletPrefab == null || staffBulletPos == null)
            return;

        GameObject b = Instantiate(staffBulletPrefab, staffBulletPos.position, Quaternion.identity);

        Bullet bulletScript = b.GetComponent<Bullet>();
        if (bulletScript != null && movement != null)
        {
            bulletScript.SetDirection(movement.lastDir);
            bulletScript.Init(GetComponent<Entity>());
        }
    }

    private IEnumerator Attack()
    {
        movement.SetAttacking(true);
        animator.SetTrigger("Attack");

        curTime = coolTime;

        myrigidbody.velocity = Vector2.zero;

        yield return new WaitForSeconds(attackLockTime);

        movement.SetAttacking(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    private void UpdateAttackBoxPosition()
    {
        if (movement == null) return;

        Vector2 dir = movement.lastDir;
        float sign = dir.x >= 0 ? 1f : -1f;

        // 근접 공격 박스 위치
        if (pos != null)
        {
            Vector3 local = pos.localPosition;
            local.x = Mathf.Abs(local.x) * sign;
            pos.localPosition = local;
        }

        // 활 화살 발사 위치도 좌우 반전
        if (bowBulletPos != null)
        {
            Vector3 bLocal = bowBulletPos.localPosition;
            bLocal.x = Mathf.Abs(bLocal.x) * sign;
            bowBulletPos.localPosition = bLocal;
        }
    }
}
