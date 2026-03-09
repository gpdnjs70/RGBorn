using System.Collections;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private SkillCoolTime dashCooldownUI;

    private Rigidbody2D rigid;
    private Animator anim;
    private Entity entity;

    private float h;
    private float v;
    public float dirX;
    private Vector2 move;              // 현재 프레임 이동 입력 (정규화)
    public Vector2 lastDir = Vector2.right; // 마지막 비영(0) 방향(대시 방향)

    private bool isDashing;
    private bool isAttacking = false;
    public bool isKnockBack;
    public bool IsMovementLocked => isDashing || isAttacking;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        entity = GetComponent<Entity>();
    }

    private void Update()
    {
        if (!entity.CanAct)
            return;

        if (IsMovementLocked) return;

        // 입력
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        move = new Vector2(h, v);
        if (Mathf.Abs(h) > 0.0001f)
        {
            dirX = h;
        }// 좌우 방향 이동이 0이 아닐 때만 갱신

        // 이동 여부 -> 애니메이터
        bool isMove = (move.sqrMagnitude > 0);
        anim.SetBool("isMove", isMove);

        // 방향 파라미터
        anim.SetFloat("Direction", dirX);

        if (move.sqrMagnitude > 0.0001f)
            lastDir = move.normalized;

        // 대시 입력
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            if (dashCooldownUI != null && !dashCooldownUI.TryUseSkill())
            {
                // 쿨타임 중이면 대시 안 됨
                return;
            }

            // 여기까지 왔으면 대시 가능
            anim.SetTrigger("Dash");   // 애니메이션 점화
            isDashing = true;          // 로직 잠금
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isKnockBack)
            return;

        if (isAttacking)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        float finalSpeed = entity.Speed;
        // Debug.Log($"현재 속도는 {finalSpeed}");
        rigid.velocity = move.normalized * finalSpeed; // 대각선 속도 보정
    }

    private IEnumerator Dash()
    {
        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        // 종료
        rigid.velocity = Vector2.zero;
        isDashing = false;
    }

    public void SetAttacking(bool value)
    {
        if (isAttacking == value) return;
        isAttacking = value;

        // 이동 즉시 정지 & 애니메이터 플래그(선택)
        if (value) rigid.velocity = Vector2.zero;
    }

    public void SetKnockBack(bool value)
    {
        isKnockBack = value;
    }
}
