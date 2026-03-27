using System.Collections;
using UnityEngine;

public class DialogueMovement : MonoBehaviour
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
    private float dirX;
    private Vector2 move;              // 현재 프레임 이동 입력 (정규화)
    public Vector2 lastDir = Vector2.left; // 마지막 비영(0) 방향(대시 방향)

    private bool isDashing;
    public bool IsMovementLocked => isDashing;

    public TutorialDialogue dialogue;
    GameObject scanObject;

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
        h = dialogue.isAction ? 0 : Input.GetAxisRaw("Horizontal");
        v = dialogue.isAction ? 0 : Input.GetAxisRaw("Vertical");

        move = new Vector2(h, v);
        if (move.sqrMagnitude > 0.0001f)
            lastDir = move.normalized; // 0이 아닐 때만 갱신

        // 이동 여부 -> 애니메이터
        bool isMove = (h != 0 || v != 0);
        anim.SetBool("isMove", isMove);

        // 방향 파라미터
        dirX = (isMove ? lastDir.x : lastDir.x);   // 정지 시에도 마지막 방향 유지
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

        if (Input.GetButtonDown("Jump") && scanObject != null)
        {
            dialogue.Action(scanObject);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        float finalSpeed = entity.Speed;
        // Debug.Log($"현재 속도는 {finalSpeed}");
        rigid.velocity = move.normalized * finalSpeed; // 대각선 속도 보정

        // Ray
        Debug.DrawRay(rigid.position, new Vector3(dirX, 0f, 0f) * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, new Vector3(dirX, 0f, 0f), 0.7f, LayerMask.GetMask("NPC"));

        if (rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else
        {
            scanObject = null;
        }
    }

    private IEnumerator Dash()
    {
        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        // 종료
        rigid.velocity = Vector2.zero;
        isDashing = false;
    }
}
