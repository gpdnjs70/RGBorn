using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private SkillCoolTime dashCooldownUI;

    private Rigidbody2D rigid;
    private Animator anim;
    private Entity entity;

    private Vector2 moveInput;              // 현재 프레임 이동 입력 (정규화)
    public Vector2 lastDir = Vector2.down; // 마지막 비영(0) 방향(대시 방향)
    public float dirX;

    private bool isDashing;
    private bool isAttacking = false;
    public bool isKnockBack;
    public bool IsMovementLocked => isDashing || isAttacking;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[Player] {scene.name} 씬 로드 완료! 이동해야 할 좌표가 있나? : {GameData.hasSpawnPoint}");

        if (GameData.hasSpawnPoint)
        {
            // transform과 물리 엔진(rigid) 위치를 동시에 강제 이동
            transform.position = GameData.nextSpawnPoint;
            if (rigid != null) rigid.position = GameData.nextSpawnPoint;

            Debug.Log($"[Player] 위치 이동 완료! 적용된 위치: {GameData.nextSpawnPoint}");

            // 이동 완료 후 플래그 초기화
            GameData.hasSpawnPoint = false;
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        entity = GetComponent<Entity>();
    }

    private void Update()
    {
        if (!entity.CanAct) return;

        if (IsMovementLocked) return;

        // 입력
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (!IsMovementLocked)
        {
            if (moveInput != Vector2.zero)
            {
                // dirX를 위한 코드
                if (Mathf.Abs(moveInput.x) > 0.0001f)
                {
                    dirX = moveInput.x; // 좌우 방향 갱신
                }
                anim.SetFloat("Direction", dirX);
                //---------------------------

                // 8방향 파라미터 업데이트
                anim.SetFloat("DirX", moveInput.x);
                anim.SetFloat("DirY", moveInput.y);
                anim.SetBool("isMove", true);

                moveInput.Normalize();
                lastDir = moveInput; // 마지막 방향 저장 (대시용)
            }
            else
            {
                anim.SetBool("isMove", false);
            }
        }

        // 대시 입력
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isAttacking)
        {
            // 대시 쿨타임 확인
            if (dashCooldownUI == null || dashCooldownUI.TryUseSkill())
            {
                StartCoroutine(Dash());
            }
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
        rigid.velocity = moveInput * finalSpeed; // 대각선 속도 보정
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");

        rigid.velocity = lastDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

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
