using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    public float coolTime = 1.0f;
    public Transform pos;
    public Vector2 boxSize;

    [SerializeField] private Player_Movement movement;

    public float damage = 2f;

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

            attacker.ApplyAttributeDebuffTo(actualTarget, 10f);

            actualTarget.TakeDamage(finalDamage);
        }

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        movement.SetAttacking(true);
        animator.SetTrigger("Attack");

        curTime = coolTime;

        myrigidbody.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

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
        
        float sign = movement.dirX >= 0 ? 1f : -1f;

        // 근접 공격 박스 위치
        if (pos != null)
        {
            Vector3 local = pos.localPosition;
            local.x = Mathf.Abs(local.x) * sign;
            pos.localPosition = local;
        }
    }
}
