using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float Speed;

    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    private Rigidbody2D rigid;
    private Animator anim;

    private float h;
    private float v;
    private Vector2 move;              // 현재 프레임 이동 입력 (정규화)
    private Vector2 lastDir = Vector2.right; // 마지막 비영(0) 방향(대시 방향)
    private bool canDash = true;
    private bool isDashing;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDashing) return;

        // Move Value
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 0.0001f)   // 0이 아닐 때만
            lastDir = input.normalized;

        // 움직임 여부
        bool isMove = (h != 0 || v != 0);
        anim.SetBool("isMove", isMove);

        // 방향(좌/우) 파라미터 업데이트
        if (anim.GetInteger("hAxisRaw") != (int)h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != (int)v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
        {
            anim.SetBool("isChange", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        rigid.velocity = new Vector2(h, v) * Speed;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // 대시 시작
        tr.emitting = true;
        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        // 대시 종료
        tr.emitting = false;
        isDashing = false;
        rigid.velocity = Vector2.zero;

        // 쿨다운
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
