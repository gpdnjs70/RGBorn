using System.Collections;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float Speed = 5f;

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
    private Vector2 lastDir = Vector2.left; // 마지막 비영(0) 방향(대시 방향)

    private bool canDash = true;
    private bool isDashing;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 대시 중엔 입력/애니메이션 갱신 차단
        if (isDashing) return;

        // 입력
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        move = new Vector2(h, v);
        if (move.sqrMagnitude > 0.0001f)
            lastDir = move.normalized; // 0이 아닐 때만 갱신

        // 이동 여부 -> 애니메이터
        bool isMove = (h != 0 || v != 0);
        anim.SetBool("isMove", isMove);

        // 방향 파라미터 (필요시 유지)
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

        // 대시 입력
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return; // 대시 중 일반 이동 막기
        rigid.velocity = move.normalized * Speed; // 대각선 속도 보정
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        string dashStateName = lastDir.x > 0 ? "Right_dash" : "Left_dash";
        anim.CrossFade(dashStateName, 0f);

        anim.SetFloat("dashX", lastDir.x);
        anim.SetBool("isDashing", true);

        // 대시 시작
        tr.emitting = true;
        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        // 대시 종료
        tr.emitting = false;
        isDashing = false;
        rigid.velocity = Vector2.zero;

        anim.SetBool("isDashing", false);

        // 쿨다운
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
