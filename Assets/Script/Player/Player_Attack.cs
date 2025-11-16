using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        UpdateAttackBoxPosition();

        if (curTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                   if(collider.tag == "Enemy")
                    {
                        collider.GetComponent<Enemy_HP>().TakeDamage(1);
                    }
                }

                StartCoroutine(Attack());
            }
        }
        else
        {
            curTime -= Time.deltaTime;
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
        if (movement == null || pos == null) return;

        Vector2 dir = movement.lastDir;
        float sign = dir.x >= 0 ? 1f : -1f;

        Vector3 local = pos.localPosition;
        local.x = Mathf.Abs(local.x) * sign;
        pos.localPosition = local;
    }
}
