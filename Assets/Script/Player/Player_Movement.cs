using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private SkillCoolTime dashCooldownUI;
    [SerializeField] private AudioClip dashClip;

    private Rigidbody2D rigid;
    private Animator anim;
    private Entity entity;

    private Vector2 moveInput;              // 현재 프레임 이동 입력 (정규화)
    public Vector2 lastDir = Vector2.down; // 마지막 비영(0) 방향(대시 방향)
    public float dirX;
    public float dirY;

    private bool isDashing;
    private bool isAttacking = false;
    public bool isKnockBack;
    public bool IsMovementLocked => isDashing || isAttacking;

    [Header("Footsteps")]
    [SerializeField] private AudioClip grassClip;
    [SerializeField] private AudioClip stoneClip;
    [SerializeField] private AudioClip woodClip;

    /**
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
    **/

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
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);

        if (!IsMovementLocked)
        {
            if (moveInput != Vector2.zero)
            {
                dirX = x;
                dirY = y;

                //---------------------------

                // 8방향 파라미터 업데이트
                anim.SetFloat("DirX", moveInput.x);
                anim.SetFloat("DirY", moveInput.y);
                anim.SetBool("isMove", true);

                moveInput.Normalize();
                lastDir = moveInput; // 마지막 방향 저장 (대시용)

                PlayGroundSound();
            }
            else
            {
                anim.SetBool("isMove", false);
                SoundManager.instance.StopVariableSFX();
            }
        }

        // 대시 입력
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && !isAttacking)
        {
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
        rigid.velocity = moveInput * finalSpeed; // 대각선 속도 보정
    }

    private IEnumerator Dash()
    {
        isDashing = true;

        if (dashClip != null)
        {
            SoundManager.instance.SFXPlay("Dash", dashClip);
        }

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

    public void PlayGroundSound()
    {
        // 1. 내 발 위치의 좌표를 잡습니다. (보통 캐릭터 발치인 중앙 하단)
        Vector2 footPos = new Vector2(transform.position.x, transform.position.y - 1.36f);

        // 2. 그 지점에 있는 'Ground' 레이어의 콜라이더를 하나 가져옵니다.
        Collider2D hit = Physics2D.OverlapPoint(footPos, LayerMask.GetMask("Ground"));

        // Debug.Log($"발밑 검사 위치: {footPos}");

        if (hit != null)
        {
            string tag = hit.tag;
            AudioClip clipToPlay = null;

            // 태그 판별
            switch (tag)
            {
                case "Grass": clipToPlay = grassClip; break;
                case "Stone": clipToPlay = stoneClip; break;
                case "Wood": clipToPlay = woodClip; break;
            }

            if (clipToPlay != null && SoundManager.instance != null)
            {
                SoundManager.instance.PlayVariableSFX(clipToPlay);
                // Debug.Log($"[발소리] 현재 밟은 곳: {tag}");
            }
        }
    }

    public void SetControlEnabled(bool enabled)
    {
        // 조작 허용/차단 시 속도 초기화
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        if (!enabled && rigid != null) rigid.velocity = Vector2.zero;

        // 애니메이터 정지 (선택 사항)
        Animator anim = GetComponent<Animator>();
        if (!enabled && anim != null) anim.SetBool("isMove", false);

        // 각 컴포넌트 활성/비활성
        if (TryGetComponent(out Player_Movement move)) move.enabled = enabled;
        if (TryGetComponent(out Player_Attack attack)) attack.enabled = enabled;
        if (TryGetComponent(out PlayerAttribute attr)) attr.enabled = enabled;
        if (TryGetComponent(out Player_Skill skill)) skill.enabled = enabled;
    }
}
