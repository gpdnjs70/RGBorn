using System.Collections;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float speed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private SkillCoolTime dashCooldownUI;

    private Rigidbody2D rigid;
    private Animator anim;
    private Entity entity;

    private float h;
    private float v;
    private Vector2 move;              // 현재 프레임 이동 입력 (정규화)
    public Vector2 lastDir = Vector2.left; // 마지막 비영(0) 방향(대시 방향)

    private bool isDashing;
    private bool isAttacking = false;
    public bool isKnockBack;
    public bool IsMovementLocked => isDashing || isAttacking;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        entity = GetComponent<Entity>();

        entity.Speed = speed;
    }

    private void Update()
    {
        if (IsMovementLocked) return;

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
        float dirX = (isMove ? lastDir.x : lastDir.x);   // 정지 시에도 마지막 방향 유지
        anim.SetFloat("Direction", Mathf.Clamp(dirX, -1f, 1f));

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

        float finalSpeed = entity.MoveSpeed;
        // Debug.Log($"현재 속도는 {finalSpeed}");
        rigid.velocity = move.normalized * finalSpeed; // 대각선 속도 보정
    }

    private IEnumerator Dash()
    {
        tr.emitting = true;
        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        // 종료
        tr.emitting = false;
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
