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

    [Header("동화책 전용 UI")]
    [SerializeField] private GameObject storyBookPanel; // 동화책 전체 패널
    [SerializeField] private Animator storyBookContentAnimator; // 애니메이션이 실행되는 자식 오브젝트의 애니메이터
    [SerializeField] private int totalPageCount = 5; // 애니메이터에 설정된 총 페이지(상태) 수

    [Header("Interactive Objects")]
    [SerializeField] private boxTrigger targetBox;
    [SerializeField] private GameObject SceneChange;

    [Header("Animation State")]
    [SerializeField] private AnimatorOverrideController colorController;

    private int bookContactCount = 0;
    public bool isNearBook => bookContactCount > 0;

    private int boxContactCount = 0;
    public bool isNearBox => boxContactCount > 0;// 플레이어가 상자 근처에 있는지
    private bool isNearDoor = false; // 문 근처에 있는지 확인

    private CanvasGroup panelCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isDialogueActive = false;

    [Header("Color Sprites")]
    [SerializeField] private Sprite ColorPanelSprite;
    [SerializeField] private Sprite ColorNametagSprite;
    [SerializeField] private Sprite ColorFrameSprite;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip MiracleClip;

    private void Awake()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(false);
            Debug.Log("첫 방문: 기본 세이브 생성");
        }

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
        if (SaveManager.Instance != null)
        {
            // 1. 새로운 세이브 데이터 객체 생성
            SaveData firstSave = new SaveData();
            Entity playerEntity = player.GetComponent<Entity>();

            // 2. 인스펙터에 설정된 플레이어의 기본 스탯을 데이터에 주입
            if (playerEntity != null)
            {
                firstSave.lastSceneName = "Tutorial_Inside";
                firstSave.currentMaxHP = playerEntity.stats.MaxHP.BaseValue;
                firstSave.currentSpeed = playerEntity.stats.Speed.BaseValue; // 인스펙터 값이 5라면 5가 들어감
                firstSave.attackMultiplier = 1.0f;
            }

            // 3. 이 데이터를 가지고 저장 (updatedData 인자로 전달)
            SaveManager.Instance.SaveGame(false, null, firstSave);
        }

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

        yield return new WaitUntil(() =>
             !PauseManager.IsPaused && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
             Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        );
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOutPanel());

        // --- 2단계: 상자 조사 유도 (Shift) ---
        yield return StartCoroutine(PlayDialogue(2));

        SetupGuideUI(2); // Shift 대시 가이드 표시
        SetPlayerControllable(TutorialStep.DashEnabled);
        
        yield return new WaitUntil(() => !PauseManager.IsPaused && Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOutPanel());

        // --- 3단계: 책 발견 ---
        yield return StartCoroutine(PlayDialogue(3));
        SetPlayerControllable(TutorialStep.All);

        yield return new WaitUntil(() => isNearBook);

        bool isBookGuiActive = false;
        while (true)
        {
            if (isNearBook)
            {
                if (!isBookGuiActive)
                {
                    // 책 근처에 오면 UI 표시 (index 3이 책 가이드라고 가정)
                    SetupGuideUI(3);
                    isBookGuiActive = true;
                    // Debug.Log("Book UI Activated");
                }

                // 책 근처에서 E키를 누르면 다음 단계로 진행
                if (Input.GetKeyDown(KeyCode.E))
                {
                    break;
                }
            }
            else
            {
                if (isBookGuiActive)
                {
                    // 책에서 멀어지면 UI 숨기기
                    tutorialPanel.SetActive(false);
                    if (controlPanel != null) controlPanel.SetActive(false);
                    isBookGuiActive = false;
                    // Debug.Log("Book UI Deactivated");
                }
            }
            yield return null;
        }

        yield return StartCoroutine(FadeOutPanel());

        // 동화책 관련 이야기
        yield return StartCoroutine(PlayDialogue(4));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayFullStory());
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PlayDialogue(5));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PlayDialogue(6));
        SetPlayerControllable(TutorialStep.All);

        yield return new WaitUntil(() => isNearBox);
        bool isGuiActive = false; // 현재 UI가 켜져 있는지 체크하는 플래그

        while (true)
        {
            if (isNearBox)
            {
                if (!isGuiActive)
                {
                    // 박스에 닿았을 때 UI 활성화
                    SetupGuideUI(3);
                    isGuiActive = true;
                    // Debug.Log("Box UI Activated"); // 디버깅용
                }

                // 박스 근처에서 E키를 누르면 탈출
                if (!PauseManager.IsPaused && Input.GetKeyDown(KeyCode.E))
                {                    
                    break;
                }
            }
            else
            {
                if (isGuiActive)
                {
                    // 박스에서 멀어지면 UI 비활성화
                    tutorialPanel.SetActive(false);
                    if (controlPanel != null) controlPanel.SetActive(false);
                    isGuiActive = false;
                    Debug.Log("Box UI Deactivated"); // 디버깅용
                }
            }

            yield return null;
        }

        yield return StartCoroutine(FadeOutPanel());
        SetPlayerControllable(TutorialStep.None);

        // --- 4단계: 팔찌 발견 ---
        Animator anim = player.GetComponent<Animator>();

        anim.SetFloat("DirX", -1);
        anim.SetFloat("DirY", 0);
        
        yield return StartCoroutine(PlayDialogue(7));

        if (targetBox != null) targetBox.PlayOpenAnimation();
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(PlayDialogue(8));

        if (targetBox != null) targetBox.PlayFinishAnimation();

        // --- 5단계: 팔찌 장착 ---
        if (colorController != null)
        {
            anim = player.GetComponent<Animator>();

            // 1. 교체 전 현재 방향(DirX, DirY) 값을 미리 저장
            float lastDirX = anim.GetFloat("DirX");
            float lastDirY = anim.GetFloat("DirY");
            bool lastIsMove = anim.GetBool("isMove"); // 이동 중이었는지 여부도 저장하면 좋습니다.

            // 2. 컨트롤러 교체 (이때 모든 파라미터가 초기화됨)
            anim.runtimeAnimatorController = colorController;

            if (!PauseManager.IsPaused && SoundManager.instance != null && MiracleClip != null)
            {
                SoundManager.instance.SFXPlay("WallSpawn", MiracleClip);
            }

            // 3. 저장해둔 방향 값을 새 컨트롤러에 즉시 주입
            anim.SetFloat("DirX", lastDirX);
            anim.SetFloat("DirY", lastDirY);
            anim.SetBool("isMove", lastIsMove);

            // 교체 직후 파티클이나 효과음을 넣으면 더 극적입니다.
            // Instantiate(colorAwakeEffect, player.transform.position, Quaternion.identity);
        }

        // talkPanel Color Sprite로 교체
        if (tutorialDialogue.talkPanel.GetComponent<Image>() != null)
        {
            tutorialDialogue.talkPanel.GetComponent<Image>().sprite = ColorPanelSprite;
        }
        if (tutorialDialogue.Nametag.GetComponent<Image>() != null)
        {
            tutorialDialogue.Nametag.GetComponent<Image>().sprite = ColorNametagSprite;
        }
        if (tutorialDialogue.Frame.GetComponent<Image>() != null)
        {
            tutorialDialogue.Frame.GetComponent<Image>().sprite = ColorFrameSprite;
        }

        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
        yield return StartCoroutine(PlayDialogue(9));
       
        isNearDoor = false;
        SetPlayerControllable(TutorialStep.All);
        yield return new WaitUntil(() => isNearDoor);
        yield return StartCoroutine(PlayDialogue(10));

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
            // 1. 일시정지 체크
            if (PauseManager.IsPaused)
            {
                if (anim != null) anim.speed = 0f;
                if (SoundManager.instance != null) SoundManager.instance.PauseVariableSFX();

                // 일시정지인 동안은 여기서 계속 대기하며 아래 로직을 실행하지 않음
                yield return null;
                continue;
            }

            // 2. 일시정지가 아닐 때 (정상 작동)
            if (anim != null && anim.speed != 1f) anim.speed = 1f;

            // 사운드 재개 (UnPause)
            if (SoundManager.instance != null) SoundManager.instance.ResumeVariableSFX();
            
            // 방향 계산 및 애니메이션 설정
            Vector3 direction = (target.position - player.transform.position).normalized;
            if (anim != null)
            {
                anim.SetFloat("DirX", direction.x);
                anim.SetFloat("DirY", direction.y);
                anim.SetBool("isMove", true);
            }

            // 실제 좌표 이동 (Time.timeScale이 0이면 deltaTime이 0이라 자동으로 멈추긴 함)
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            // 발소리 재생
            playerMoveScript.PlayGroundSound();

            yield return null;
        }

        // 3. 도착 후 정리
        player.transform.position = target.position;
        if (anim != null)
        {
            anim.speed = 1f;
            anim.SetBool("isMove", false);
        }

        // 도착했으므로 이제는 Pause가 아니라 Stop으로 사운드 완전히 끄기
        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopVariableSFX();
        }
    }

    // 기존 PlayDialogue 로직 유지
    private IEnumerator PlayDialogue(int index)
    {
        // 1. 중복 실행 방지
        if (isDialogueActive) yield break;
        isDialogueActive = true;

        // 2. 플레이어 조작 정지
        SetPlayerControllable(TutorialStep.None);

        if (rigid != null)
        {
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = 0f;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isMove", false);
        }

        // 3. 주인공 독백 데이터 설정
        ObjData data = playerSelfObject.GetComponent<ObjData>();
        if (data == null)
        {
            Debug.LogError("playerSelfObject에 ObjData가 없습니다!");
            isDialogueActive = false;
            yield break;
        }

        data.SetDialogue(index);

        // 4. 대화 시스템 실행 (제어권 양도)
        tutorialDialogue.Action(playerSelfObject);

        // 5. TutorialDialogue가 끝날 때까지 대기
        yield return new WaitUntil(() => tutorialDialogue.isDialogueFinished);

        // 6. 대화 종료 처리
        yield return new WaitForSeconds(0.1f);
        isDialogueActive = false;
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
            if (!PauseManager.IsPaused && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                break;
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

    // 책에 닿았을 때 호출
    public void PlayerReachedBook()
    {
        bookContactCount++;
    }

    // 책에서 멀어졌을 때 호출
    public void PlayerLeftBook()
    {
        bookContactCount = Mathf.Max(0, bookContactCount - 1);
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

    private IEnumerator PlayFullStory()
    {
        SetPlayerControllable(TutorialStep.None);

        // 1. 배경이 포함된 전체 패널 활성화
        if (storyBookPanel != null) storyBookPanel.SetActive(true);

        // 2. 페이지 수만큼 루프
        for (int i = 0; i < totalPageCount; i++)
        {
            Debug.Log($"현재 동화책 {i + 1} / {totalPageCount} 페이지 대기 중");

            // 3. 사용자 입력을 기다림 (스페이스바/클릭)
            yield return StartCoroutine(WaitForUserInput());

            // 4. 마지막 페이지에서 입력을 받았다면 루프 종료
            if (i == totalPageCount - 1) break;

            // 5. 다음 페이지 애니메이션 실행
            if (storyBookContentAnimator != null)
            {
                storyBookContentAnimator.SetTrigger("NextPage");

                // 6. 애니메이션이 시작되고 전환되는 동안 아주 잠깐 대기 (연타 방지)
                yield return new WaitForSeconds(0.1f);

                // 만약 애니메이션이 완전히 끝날 때까지 입력을 막고 싶다면 아래 주석을 해제하세요.
                
                yield return new WaitUntil(() => 
                    storyBookContentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f 
                    && !storyBookContentAnimator.IsInTransition(0));
            }
        }

        // 7. 모든 과정이 끝나면 배경 패널까지 비활성화
        if (storyBookPanel != null) storyBookPanel.SetActive(false);
    }
}