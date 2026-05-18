using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempleManager : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Player_Movement playerMoveScript;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject paladinNPC; // 성기사 NPC 오브젝트
    [SerializeField] private GameObject finalBoss;
    [SerializeField] private Transform bossTargetPos;
    public TutorialDialogue dialogueSystem;
    [SerializeField] private AnimatorOverrideController newOverrideController;
    [SerializeField] private GameObject imageUIPanel;

    [SerializeField] private Transform playerTargetPos;

    [Header("UI & Effect")]
    [SerializeField] private GameObject SceneChange;

    // 대화 상태 확인용
    public bool isDialogueActive { get; private set; }
    private bool hasTriggeredPaladin = false; // 중복 실행 방지

    private bool hasPlayedIntro = false;

    [Header("조작 UI")]
    [SerializeField] public GameObject controlPanel;
    [SerializeField] private Image controlImage;
    [SerializeField] private List<Sprite> controlSprites = new List<Sprite>(); // 0: E키 등

    [Header("Camera Event")]
    [SerializeField] private CameraFollow cameraFollow; // 기존 카메라 스크립트 참조
    [SerializeField] private Transform cameraEventPos;  // 카메라가 도달할 목표 지점
    private GameObject cameraDummy;                     // 코드에서 생성할 임시 타겟

    private void Awake()
    {
        if (player != null)
        {
            rigid = player.GetComponent<Rigidbody2D>();
            playerMoveScript = player.GetComponent<Player_Movement>();
        }
    }

    private void Start()
    {
        if (SceneChange != null) SceneChange.SetActive(false);
        if (finalBoss != null) finalBoss.SetActive(false);
        if (player != null)
        {
            Animator anim = player.GetComponent<Animator>();

            anim.SetFloat("DirX", 0);
            anim.SetFloat("DirY", 1);
        }
    }

    // --- 외부(Trigger 오브젝트)에서 호출할 함수 ---
    public void TriggerPaladinEvent()
    {
        if (!hasTriggeredPaladin)
        {
            if (PauseManager.IsPaused) return;

            hasTriggeredPaladin = true;
            StartCoroutine(PaladinDialogueSequence());
        }
    }

    private IEnumerator PaladinDialogueSequence()
    {
        SetPlayerControllable(false);

        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopVariableSFX();
        }

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("DirX", 0);
            playerAnim.SetFloat("DirY", 1);
        }

        if (cameraFollow != null && cameraEventPos != null)
        {
            // 현재 플레이어 위치에 더미 생성
            cameraDummy = new GameObject("CameraDummy");
            cameraDummy.transform.position = player.transform.position;

            Transform originalTarget = cameraFollow.target; // 원래 타겟(플레이어) 저장
            cameraFollow.target = cameraDummy.transform;    // 카메라 타겟을 더미로 변경

            // 1. 카메라 서서히 올리기 (2초 동안)
            yield return StartCoroutine(MoveCameraDummy(cameraEventPos.position, 2.0f));
        }

        // 1. 성기사 대화 시작
        
        yield return StartCoroutine(PlayDialogue(paladinNPC, 0));

        // 2. 보스 등장 및 이동
        if (finalBoss != null)
        {
            finalBoss.SetActive(true); // 보스 활성화
            yield return StartCoroutine(MoveBoss(finalBoss, bossTargetPos.position, 2.0f));

            // 3. 보스 대화 시작 (이동 완료 후 바로 실행)
            yield return StartCoroutine(PlayDialogue(finalBoss, 0));

            yield return new WaitForSecondsRealtime(1f);

            yield return StartCoroutine(PlayDialogue(finalBoss, 1));
        }

        // 4. 모든 이벤트 종료 후 연출
        if (SceneChange != null) SceneChange.SetActive(true);
        if (playerTargetPos != null)
        {
            // 카메라가 다시 플레이어(혹은 플레이어를 따라가는 더미)를 비추도록 설정
            // 만약 카메라가 플레이어를 바로 따라가게 하고 싶다면 cameraFollow.target = player.transform;

            yield return StartCoroutine(MovePlayerToPosition(playerTargetPos.position, 2.5f));
        }
    }

    private IEnumerator MoveCameraDummy(Vector3 targetPos, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = cameraDummy.transform.position;

        while (elapsed < duration)
        {
            if (!PauseManager.IsPaused)
            {
                elapsed += Time.deltaTime;
                cameraDummy.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            }
            yield return null;
        }
        cameraDummy.transform.position = targetPos;
    }

    private IEnumerator MovePlayerToPosition(Vector3 targetPos, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = player.transform.position;
        Animator playerAnim = player.GetComponent<Animator>();

        // 애니메이션 설정 (걷기 애니메이션이 있다면)
        if (playerAnim != null)
        {
            playerAnim.SetBool("isMove", true);
            // 이동 방향에 따라 DirX, DirY 설정 (예: 위로 이동 시 DirY = 1)
            Vector2 dir = (targetPos - startPos).normalized;
            playerAnim.SetFloat("DirX", dir.x);
            playerAnim.SetFloat("DirY", dir.y);
        }

        while (elapsed < duration)
        {
            if (!PauseManager.IsPaused)
            {
                elapsed += Time.deltaTime;
                player.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);

                // 애니메이션 속도 조절 (일시정지 시 멈춤)
                if (playerAnim != null) playerAnim.speed = 1f;

                if (playerMoveScript != null)
                {
                    playerMoveScript.PlayGroundSound();
                }
            }
            else
            {
                // 일시정지 시 애니메이션 멈춤
                if (playerAnim != null) playerAnim.speed = 0f;
                if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
            }
            yield return null;
        }

        player.transform.position = targetPos;

        if (playerAnim != null)
        {
            playerAnim.speed = 1f;
            playerAnim.SetBool("isMove", false);
        }

        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
    }

    private IEnumerator MoveBoss(GameObject boss, Vector3 targetPos, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = boss.transform.position;

        // 보스에게 애니메이터가 있다면 여기서 걷기 애니메이션 실행 가능
        Animator bossAnim = boss.GetComponent<Animator>();
        if (bossAnim != null) bossAnim.SetBool("isMove", true);

        while (elapsed < duration)
        {
            if (!PauseManager.IsPaused)
            {
                elapsed += Time.deltaTime;
                boss.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                if (bossAnim != null) bossAnim.speed = 1f;

                if (playerMoveScript != null)
                {
                    playerMoveScript.PlayGroundSound();
                }
            }
            else
            {
                if (bossAnim != null) bossAnim.speed = 0f;
                if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
            }
            yield return null;
        }

        boss.transform.position = targetPos;
        if (bossAnim != null)
        {
            bossAnim.speed = 1f;
            bossAnim.SetBool("isMove", false);
        }

        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
    }

    // VillageManager와 동일한 방식의 대화 처리 루틴
    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex, bool restoreControlAtEnd = true)
    {
        if (isDialogueActive) yield break;

        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

        isDialogueActive = true;

        SetPlayerControllable(false); // 대화 시작 시 조작 금지

        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();

        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObj.name + "에 ObjData가 없습니다!");
            SetPlayerControllable(true);
            isDialogueActive = false;
            yield break;
        }

        npcData.SetDialogue(dialogueIndex); // 대사 SO 교체

        // 핵심: TutorialManager처럼 Action을 한 번만 호출하고 내부 로직이 끝날 때까지 기다립니다.
        // 이렇게 하면 TutorialDialogue 스크립트의 Update문이 입력을 정상적으로 처리합니다.
        dialogueSystem.Action(npcObj);

        // TutorialDialogue가 스스로 isDialogueFinished를 true로 만들 때까지 대기
        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);

        isDialogueActive = false;
    }

    public void SetPlayerControllable(bool canControl)
    {
        if (PauseManager.IsPaused && canControl) return;

        if (playerMoveScript) playerMoveScript.enabled = canControl;

        if (!canControl && rigid != null)
        {
            rigid.velocity = Vector2.zero;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null && !canControl)
        {
            anim.SetBool("isMove", false);
        }
    }

    public void ShowControlGuide(int spriteIndex)
    {
        if (controlPanel == null) return;
        controlPanel.SetActive(true);
        controlImage.sprite = controlSprites[spriteIndex];
    }

    public void HideControlGuide()
    {
        if (controlPanel != null) controlPanel.SetActive(false);
    }

    public void TriggerPlayerMonologue()
    {
        if (hasPlayedIntro) return;
        hasPlayedIntro = true;

        StartCoroutine(PlayerMonologueSequence());
    }

    private IEnumerator PlayerMonologueSequence()
    {
        // 1. 조작 복구 옵션을 false로 주어 독백이 끝나도 플레이어가 움직이지 못하게 함
        yield return StartCoroutine(PlayDialogue(player, 0, false));

        // [일시정지 예외처리] 대기 시간 중에도 일시정지 체크
        float timer = 0;
        while (timer < 0.5f)
        {
            if (!PauseManager.IsPaused) timer += Time.deltaTime;
            yield return null;
        }

        if (imageUIPanel != null) imageUIPanel.SetActive(true);
        ShowControlGuide(0);

        yield return null; // 연타 방지를 위해 한 프레임 쉬고 시작
        while (true)
        {
            if (!PauseManager.IsPaused)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (player != null)
                    {
                        Animator playerAnim = player.GetComponent<Animator>();
                        if (playerAnim != null && newOverrideController != null)
                        {
                            playerAnim.runtimeAnimatorController = newOverrideController;
                        }
                    }

                    break; // E키 누르면 루프 탈출
                }
            }
            yield return null;
        }

        if (imageUIPanel != null) imageUIPanel.SetActive(false);
        HideControlGuide();

        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);
        yield return new WaitForSeconds(0.2f);

        SetPlayerControllable(true);
    }
}
