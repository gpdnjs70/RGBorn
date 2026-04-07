using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager_0 : MonoBehaviour
{
    // 서사 흐름에 맞춘 단계 정의
    public enum TutorialStep { None, MoveOnly, DashEnabled, All }

    [Header("References")]
    [SerializeField] private GameObject player;
    private Player_Movement playerMoveScript;
    private Player_Attack attackScript;
    private Rigidbody2D rigid;

    [SerializeField] private TutorialDialogue tutorialDialogue;
    [SerializeField] private GameObject playerSelfObject; // 주인공 독백용 (주인공 오브젝트 연결)

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Player Opening Move Settings")]
    [SerializeField] private List<Transform> targetPositions = new List<Transform>(); // 플레이어가 걸어갈 목표 위치
    [SerializeField] private float moveSpeed = 3f;     // 컷신 중 플레이어 이동 속도

    [Header("퀘스트 창")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private List<Sprite> tutorialSprites = new List<Sprite>(); // 0:WASD, 1:상자, 2:문 등
    [TextArea(2, 10)]
    [SerializeField] private List<string> tutorialMessages = new List<string>();

    [Header("조작법 가이드")]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Image controlImage;
    [SerializeField] private List<Sprite> controlSprites = new List<Sprite>(); // 0:WASD키, 1:Shift키 등

    [Header("Interactive Objects")]
    [SerializeField] private GameObject SceneChange;

    [Header("Animation State")]
    [SerializeField] private AnimatorOverrideController colorController;

    private int boxContactCount = 0;
    public bool isNearBox => boxContactCount > 0;// 플레이어가 상자 근처에 있는지
    private bool isNearDoor = false; // 문 근처에 있는지 확인

    private CanvasGroup panelCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        playerMoveScript = player.GetComponent<Player_Movement>();
        attackScript = player.GetComponent<Player_Attack>();
        rigid = player.GetComponent<Rigidbody2D>();

        if (tutorialPanel != null)
        {
            panelCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null) panelCanvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
        }

        // 초기 상태 설정
        if (controlPanel != null) controlPanel.SetActive(false);
    }

    private void Start()
    {
        // 집 안에서 깨어나는 시퀀스 바로 시작
        StartCoroutine(HouseTutorialSequence());
    }

    private void SetPlayerControllable(TutorialStep step)
    {
        // 모두 끄기
        playerMoveScript.enabled = false;
        if (attackScript) attackScript.enabled = false;
        if (rigid) rigid.velocity = Vector2.zero;

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isMove", false);
        }

        switch (step)
        {
            case TutorialStep.MoveOnly:
                playerMoveScript.enabled = true;
                // playerMoveScript.canDash = false; // 이동 스크립트 내 대시 변수 제어 필요 시 추가
                break;
            case TutorialStep.DashEnabled:
                playerMoveScript.enabled = true;
                // playerMoveScript.canDash = true;
                break;
            case TutorialStep.All:
                playerMoveScript.enabled = true;
                break;
            case TutorialStep.None:
            default:
                // 대사 중에는 모두 꺼진 상태 유지
                break;
        }
    }

    private IEnumerator HouseTutorialSequence()
    {
        SetPlayerControllable(TutorialStep.None);

        SetupGuideUI(0);
        // --- 1단계: 깨어남 & 이동 (WASD) ---
        yield return StartCoroutine(PlayDialogue(0));
        yield return StartCoroutine(FadeOutPanel());

        yield return StartCoroutine(PlayDialogue(1));

        SetupGuideUI(1); // WASD 가이드 표시
        SetPlayerControllable(TutorialStep.MoveOnly);

        Vector3 startPos = player.transform.position;
        // 일정 거리 이상 움직이면 가이드 숨김
        yield return new WaitUntil(() => Vector3.Distance(player.transform.position, startPos) > 5f);
        yield return StartCoroutine(FadeOutPanel());

        // --- 2단계: 상자 조사 유도 (Shift) ---
        yield return StartCoroutine(PlayDialogue(2));

        SetupGuideUI(2); // Shift 대시 가이드 표시
        SetPlayerControllable(TutorialStep.DashEnabled);

        startPos = player.transform.position;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOutPanel());

        // --- 3단계: 성물 발견 ---
        yield return StartCoroutine(PlayDialogue(3));
        SetPlayerControllable(TutorialStep.All);

        yield return new WaitUntil(() => isNearBox);
        bool isGuiActive = false; // 현재 UI가 켜져 있는지 체크하는 플래그

        while (true)
        {
            if (isNearBox && !isGuiActive)
            {
                SetupGuideUI(3);
                isGuiActive = true;
            }
            else if (!isNearBox && isGuiActive)
            {
                tutorialPanel.SetActive(false);
                if (controlPanel != null) controlPanel.SetActive(false);
                isGuiActive = false;
            }

            if (isNearBox && Input.GetKeyDown(KeyCode.E))
            {
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(FadeOutPanel());
        SetPlayerControllable(TutorialStep.None);

        // --- 4단계: 팔찌 발견 ---
        yield return StartCoroutine(PlayDialogue(4));

        // --- 5단계: 팔찌 장착 ---
        if (colorController != null)
        {
            Animator anim = player.GetComponent<Animator>();

            // 1. 교체 전 현재 방향(DirX, DirY) 값을 미리 저장
            float lastDirX = anim.GetFloat("DirX");
            float lastDirY = anim.GetFloat("DirY");
            bool lastIsMove = anim.GetBool("isMove"); // 이동 중이었는지 여부도 저장하면 좋습니다.

            // 2. 컨트롤러 교체 (이때 모든 파라미터가 초기화됨)
            anim.runtimeAnimatorController = colorController;

            // 3. 저장해둔 방향 값을 새 컨트롤러에 즉시 주입
            anim.SetFloat("DirX", lastDirX);
            anim.SetFloat("DirY", lastDirY);
            anim.SetBool("isMove", lastIsMove);

            // 교체 직후 파티클이나 효과음을 넣으면 더 극적입니다.
            // Instantiate(colorAwakeEffect, player.transform.position, Quaternion.identity);
        }

        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
        yield return StartCoroutine(PlayDialogue(5));
       
        isNearDoor = false;
        SetPlayerControllable(TutorialStep.All);
        yield return new WaitUntil(() => isNearDoor);
        yield return StartCoroutine(PlayDialogue(6));

        if (targetPositions.Count > 0)
        {
            yield return StartCoroutine(MovePlayerToTarget(targetPositions[0]));
        }

        SceneChange.SetActive(true);
    }

    private IEnumerator MovePlayerToTarget(Transform target)
    {
        if (target == null) yield break;

        SetPlayerControllable(TutorialStep.None);

        Animator anim = player.GetComponent<Animator>();
        if (anim != null) anim.SetBool("isMove", true); // 걷기 시작

        // 이동 중 조작 금지
        SetPlayerControllable(TutorialStep.None);

        while (Vector3.Distance(player.transform.position, target.position) > 0.05f)
        {
            // 방향 계산 (8방향 애니메이션 반영)
            Vector3 direction = (target.position - player.transform.position).normalized;
            if (anim != null)
            {
                anim.SetFloat("DirX", direction.x);
                anim.SetFloat("DirY", direction.y);
                anim.SetBool("isMove", true);
            }

            // 실제 좌표 이동
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 도착 후 정리
        player.transform.position = target.position;
        if (anim != null) anim.SetBool("isMove", false);
    }

    // 기존 PlayDialogue 로직 유지
    private IEnumerator PlayDialogue(int index)
    {
        SetPlayerControllable(TutorialStep.None);

        if (rigid != null)
        {
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = 0f; // 회전이 있다면 회전도 정지
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isMove", false);
        }

        // 주인공 독백 데이터 설정
        ObjData data = playerSelfObject.GetComponent<ObjData>();
        data.SetDialogue(index);

        int totalCalls = data.dialogue.lines.Length + 1;
        for (int i = 0; i < totalCalls; i++)
        {
            tutorialDialogue.Action(playerSelfObject);
            if (i < data.dialogue.lines.Length)
            {
                yield return StartCoroutine(WaitForUserInput());
            }
        }
    }

    // 퀘스트 창과 조작법 창을 동시에 설정하는 기존 함수 유지
    private void SetupGuideUI(int index)
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = 1f;

        tutorialImage.sprite = tutorialSprites[index];
        if (tutorialMessages.Count > index) tutorialText.text = tutorialMessages[index];

        if (controlPanel != null)
        {
            controlPanel.SetActive(true);
            if (controlSprites.Count > index) controlImage.sprite = controlSprites[index];
        }
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) break;
            yield return null;
        }
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            Camera.main.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

    private IEnumerator FadeOutPanel()
    {
        if (controlPanel != null) controlPanel.SetActive(false);

        if (panelCanvasGroup == null)
        {
            tutorialPanel.SetActive(false);
            yield break;
        }

        float currentTime = 0;
        float startAlpha = panelCanvasGroup.alpha;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, currentTime / fadeDuration);
            yield return null;
        }
        tutorialPanel.SetActive(false);
        
        panelCanvasGroup.alpha = 1f; // 다음 사용을 위해 리셋
    }

    // 상자(RelicBox)에 닿았을 때 외부에서 호출해줄 함수
    public void PlayerReachedBox()
    {
        boxContactCount++;
    }

    public void PlayerLeftBox()
    {
        boxContactCount = Mathf.Max(0, boxContactCount - 1);
    }

    public void PlayerReachedDoor()
    {
        isNearDoor = true;
    }
}