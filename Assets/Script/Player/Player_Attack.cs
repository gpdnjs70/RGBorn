using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Attack : MonoBehaviour
{
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

    [SerializeField] public float damage = 2f;

    [SerializeField] private Player_WeaponController weaponController;
    [SerializeField] private GameObject bowBulletPrefab;
    [SerializeField] private Transform bowBulletPos;

    // Update is called once per frame
    void Update()
    {
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
            if (collider.tag == "Enemy")
            {
                Enemy_HP enemy_HP = collider.GetComponent<Enemy_HP>();
                if(enemy_HP != null)
                {
                    enemy_HP.TakeDamage(damage);
                }
            }
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
