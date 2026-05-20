using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStep { None, Attack, Attribute, All }

    [Header("References")]
    [SerializeField] private GameObject player;
    private Player_Movement playerMoveScript;
    private Player_Attack attackScript;
    private PlayerAttribute playerAttributeScript;
    private Player_Skill playerSkillScript;
    private Rigidbody2D rigid;

    [SerializeField] private TutorialDialogue tutorialDialogue;
    [SerializeField] private GameObject npcObject;

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Player Opening Move Settings")]
    [SerializeField] private List<Transform> targetPositions = new List<Transform>(); // 플레이어가 걸어갈 목표 위치
    [SerializeField] private float moveSpeed = 3f;     // 컷신 중 플레이어 이동 속도

    [Header("퀘스트 창")]
    [SerializeField] private GameObject tutorialPanel; // UI 전체를 켜고 끌 부모 객체
    [SerializeField] private Text tutorialText;
    [TextArea(2, 10)]
    [SerializeField] private List<string> tutorialMessages = new List<string>();
    [SerializeField] private Image tutorialImage;      // 스프라이트가 바뀔 이미지 컴포넌트
    [SerializeField] private List<Sprite> tutorialSprites = new List<Sprite>(); // 무한정 추가 가능한 리스트

    [Header("조작법")]
    [SerializeField] private GameObject controlPanel; // UI 전체를 켜고 끌 부모 객체
    [SerializeField] private Image controlImage;      // 스프라이트가 바뀔 이미지 컴포넌트
    [SerializeField] private List<Sprite> controlSprites = new List<Sprite>(); // 무한정 추가 가능한 리스트

    [Header("Stat UI Settings")]
    [SerializeField] private List<Sprite> GetItemSprites; // Stat UI에 띄울 이미지들
    [SerializeField] private List<string> GetItemTexts;   // Stat UI에 띄울 설명들
    [SerializeField] private GameObject statePanel;

    [Header("State UI")]
    [SerializeField] private List<Image> SkillIcon;
    [SerializeField] private List<GameObject> SkillLock;

    [Header("Etc")]
    [SerializeField] private GameObject tutorialSlime1;
    [SerializeField] private RuntimeAnimatorController SlimeController1;
    private Enemy_HP enemyHPScript1;
    private SlimeAI slimeAI1;

    [SerializeField] private GameObject tutorialSlime2;
    [SerializeField] private RuntimeAnimatorController SlimeController2;
    [SerializeField] private RuntimeAnimatorController SlimeController3;
    private Enemy_HP enemyHPScript2;
    private SlimeAI slimeAI2;

    private GameObject hittedSlime = null;

    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup panelCanvasGroup;

    [SerializeField] private GameObject SavePoint;

    private bool isDialogueActive = false;

    [SerializeField] private GameObject imageUIPanel;

    private void Awake()
    {
        playerMoveScript = player.GetComponent<Player_Movement>();
        attackScript = player.GetComponent<Player_Attack>();
        playerAttributeScript = player.GetComponent<PlayerAttribute>();
        playerSkillScript = player.GetComponent<Player_Skill>();
        rigid = player.GetComponent<Rigidbody2D>();

        enemyHPScript1 = tutorialSlime1.GetComponent<Enemy_HP>();
        slimeAI1 = tutorialSlime1.GetComponent<SlimeAI>();

        enemyHPScript2 = tutorialSlime2.GetComponent<Enemy_HP>();
        slimeAI2 = tutorialSlime2.GetComponent<SlimeAI>();

        if (slimeAI1 != null)
        {
            slimeAI1.OnHit += (obj) => hittedSlime = obj;
        }

        if (slimeAI2 != null)
        {
            slimeAI2.OnHit += (obj) => hittedSlime = obj;
        }

        if (tutorialPanel != null)
        {
            panelCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;

            // 플레이어 초기 설정
            if (player != null)
            {
                SetPlayerControllable(TutorialStep.None);

                Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
                if (rigid != null)
                {
                    rigid.velocity = Vector2.zero;
                }

                playerMoveScript.dirX = 1f;
                playerMoveScript.lastDir = Vector2.right;

                Animator anim = player.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetFloat("DirX", 1f);
                    anim.SetFloat("DirY", -1f);
                    anim.SetFloat("Direction", 1f);
                    anim.SetBool("isMove", false);
                }
            }

            StartCoroutine(TutorialOpeningSequence());
        }
    }

    private void SetPlayerControllable(TutorialStep step)
    {
        // 일단 모두 끄기 (초기화)
        if (playerMoveScript) playerMoveScript.enabled = false;
        if (attackScript) attackScript.enabled = false;
        if (playerAttributeScript) playerAttributeScript.enabled = false;
        if (playerSkillScript) playerSkillScript.enabled = false;

        if (rigid) rigid.velocity = Vector2.zero;

        // 단계별로 필요한 것만 켜기
        switch (step)
        {
            case TutorialStep.Attack:
                playerMoveScript.enabled = true;
                attackScript.enabled = true;
                break;

            case TutorialStep.Attribute:
                playerMoveScript.enabled = true;
                attackScript.enabled = true;
                playerAttributeScript.enabled = true;
                break;

            case TutorialStep.All:
                playerMoveScript.enabled = true;
                attackScript.enabled = true;
                playerAttributeScript.enabled = true;
                playerSkillScript.enabled = true;
                break;

            case TutorialStep.None:
            default:
                // 대사 중에는 모두 꺼진 상태 유지
                break;
        }
    }

    private IEnumerator TutorialOpeningSequence()
    {
        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
        yield return StartCoroutine(PlayDialogue(0));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MovePlayerToTarget(targetPositions[0]));
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(PlayDialogue(1));

        // 7. 조작법 설명 진입
        StartControlTutorial();
    }

    // 플레이어를 목표 위치로 자연스럽게 걷게 만드는 코루틴
    private IEnumerator MovePlayerToTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("목표 위치(targetPosition)가 설정되지 않았습니다!");
            yield break;
        }

        Animator anim = player.GetComponent<Animator>();

        // 걷기 애니메이션 켜기
        if (anim != null) anim.SetBool("isMove", true);

        // 플레이어의 현재 위치와 목표 위치의 거리가 0.05f보다 클 때까지 계속 이동
        while (Vector3.Distance(player.transform.position, target.position) > 0.05f)
        {
            // [일시정지 체크 추가]
            if (PauseManager.IsPaused)
            {
                if (anim != null) anim.speed = 0f; // 애니메이션 정지
                if (SoundManager.instance != null) SoundManager.instance.PauseVariableSFX(); // 사운드 일시정지
                yield return null;
                continue; // 아래 이동/사운드 로직 실행 안 함
            }

            // 일시정지 해제 시 복구
            if (anim != null) anim.speed = 1f;
            if (SoundManager.instance != null) SoundManager.instance.ResumeVariableSFX();

            Vector3 direction = (target.position - player.transform.position).normalized;
            if (anim != null)
            {
                anim.SetFloat("DirX", direction.x);
                anim.SetFloat("DirY", direction.y);
            }

            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            playerMoveScript.PlayGroundSound();

            yield return null; // 다음 프레임까지 대기
        }

        // 목표 위치에 정확히 안착시키고 걷기 애니메이션 끄기
        player.transform.position = target.position;
        if (anim != null)
        {
            anim.SetFloat("DirX", 1f);
            anim.SetFloat("DirY", 0f);
            anim.SetBool("isMove", false);
        }

        player.transform.position = target.position;
        if (anim != null) { anim.speed = 1f; anim.SetBool("isMove", false); }
        if (SoundManager.instance != null) SoundManager.instance.StopVariableSFX();
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null;

        while (true)
        {
            if (!PauseManager.IsPaused && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 일시정지 중이면 흔들림 계산을 건너뜀
            if (PauseManager.IsPaused)
            {
                yield return null;
                continue;
            }

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

    private void StartControlTutorial()
    {
        Debug.Log("조작법 튜토리얼 시작!");
        statePanel.SetActive(true);

        StartCoroutine(SlimeActivationObserver());
        StartCoroutine(ControlTutorialSequence());
    }

    // --- 메인 조작 튜토리얼 루프 ---
    private IEnumerator ControlTutorialSequence()
    {
        // --- 1단계: 평타 공격 ---
        if (tutorialSprites.Count > 0)
        {
            SetupGuideUI(0);
            yield return new WaitForSeconds(0.5f);

            SetPlayerControllable(TutorialStep.Attack);

            bool hasAttacked = false;
            yield return new WaitUntil(() => {
                if (!PauseManager.IsPaused && Input.GetMouseButtonDown(0)) hasAttacked = true;
                return hasAttacked && (slimeAI1.enabled == false);
            });

            if (tutorialSlime1 != null)
            {
                Animator slimeAnim = tutorialSlime1.GetComponent<Animator>();

                slimeAnim.SetFloat("DirX", -1);
                slimeAnim.SetFloat("DirY", 0);

                slimeAnim.Update(0);
            }

            if (enemyHPScript1 != null) enemyHPScript1.LockAction();

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(FadeOutPanel());
        }

        // --- 2단계: 속성 전환 ---
        if (tutorialSprites.Count > 1)
        {
            // NPC 대사 리스트의 1번 SO 재생
            yield return StartCoroutine(PlayDialogue(2));
            if (enemyHPScript1 != null) enemyHPScript1.UnlockAction();

            SetupGuideUI(1);
            SetPlayerControllable(TutorialStep.Attribute);

            float initialHP = enemyHPScript1.HP;

            yield return new WaitUntil(() => enemyHPScript1.HP < initialHP);

            Debug.Log("속성 전환 확인 완료!");
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(FadeOutPanel());
        }

        // --- 3단계: 속성 상성 설명 ---
        if (tutorialSprites.Count > 2)
        {
            SetupGuideUI(2);
            SetPlayerControllable(TutorialStep.Attribute);

            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(FadeOutPanel());
        }

        yield return new WaitUntil(() =>
            (tutorialSlime1 == null || enemyHPScript1.HP <= 0)
        );
        SetPlayerControllable(TutorialStep.None);
        yield return new WaitForSeconds(0.5f);

        // --- 4단계: 불안정한 속성에 대한 설명 ---
        yield return StartCoroutine(MovePlayerToTarget(targetPositions[0]));
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("DirX", 1);
            playerAnim.SetFloat("DirY", 0);
        }

        if (slimeAI2 != null && SlimeController3 != null)
        {
            // 새로 만든 함수 호출
            slimeAI2.UpdateAppearance(SlimeController3);
        }

        yield return StartCoroutine(PlayDialogue(3));
        yield return new WaitForSeconds(0.5f);

        if (SkillIcon != null)
        {
            foreach (Image iconImage in SkillIcon)
            {
                if (iconImage != null)
                {
                    iconImage.raycastTarget = true;
                }
            }
        }

        if (SkillLock != null)
        {
            foreach (GameObject lockObj in SkillLock)
            {
                if (lockObj != null)
                {
                    lockObj.SetActive(false); // 잠금 이미지나 자물쇠 아이콘 숨기기
                }
            }
        }

        SetupGuideUI(3);
        SetPlayerControllable(TutorialStep.All);

        if (slimeAI2 != null && slimeAI2.enabled)
        {
            slimeAI2.ChangeSlimeAnimator(SlimeController2);
            slimeAI2.OnHit -= (obj) => hittedSlime = obj;
        }

        yield return new WaitUntil(() =>
            (tutorialSlime2 == null || enemyHPScript2.HP <= 0)
        ); 
        yield return StartCoroutine(FadeOutPanel());
        yield return new WaitForSeconds(0.5f);

        // --- 마무리 단계: NPC에게 이동 및 최종 대화 ---
        if (player != null)
        {
            Player_HP pHP = player.GetComponent<Player_HP>();
            if (pHP != null)
            {
                pHP.ResetAllVisualsAndStates();
            }
        }
        yield return new WaitForSeconds(1.0f);

        if (playerAttributeScript != null)
        {
            playerAttributeScript.SetNoneColor();
        }
        SetPlayerControllable(TutorialStep.None);

        // npc idle 상태로 전환
        NPCController npcCtrl = npcObject.GetComponent<NPCController>();
        if (npcCtrl != null)
        {
            npcCtrl.PlayIdleAnimation(Vector2.left);
        }
        statePanel.SetActive(false);

        yield return StartCoroutine(MovePlayerToTarget(targetPositions[1]));

        // 마지막 대화 방향 설정
        Vector2 dirToNPC = (npcObject.transform.position - player.transform.position).normalized;

        playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("DirX", dirToNPC.x);
            playerAnim.SetFloat("DirY", dirToNPC.y);
        }

        if (npcCtrl != null)
        {
            // 플레이어의 반대 방향을 바라보게 함
            npcCtrl.PlayIdleAnimation(-dirToNPC);
        }

        // NPC 대사 리스트의 3번 SO 재생
        yield return StartCoroutine(PlayDialogue(4));
        yield return new WaitForSeconds(1.0f);

        if (imageUIPanel != null)
        {
            imageUIPanel.SetActive(true);

            yield return null;

            bool isClosed = false;

            yield return new WaitUntil(() =>
            {
                if (!PauseManager.IsPaused && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                {
                    isClosed = true;
                }
                return isClosed;
            });

            imageUIPanel.SetActive(false); // 입력 확인 후 패널 비활성화
        }
        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(PlayDialogue(5));
        yield return new WaitForSeconds(0.5f);

        // Debug.Log("조작법 및 구출 튜토리얼 완전 종료!");
        if (SavePoint != null)
        {
            SavePoint.SetActive(true);
            Debug.Log("모든 튜토리얼 단계 완료: 세이브 포인트 활성화");
        }

        SetPlayerControllable(TutorialStep.Attack);
    }

    // NPC의 dialogueList 중 하나를 끝까지 재생하고 창을 닫는 공용 함수
    private IEnumerator PlayDialogue(int index)
    {
        // 중복 실행 방지
        if (isDialogueActive) yield break;
        isDialogueActive = true;

        // 플레이어 조작 및 물리 정지
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

        // NPC 데이터 설정 및 대사 시작
        ObjData npcData = npcObject.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObject.name + "에 ObjData가 없습니다!");
            isDialogueActive = false;
            yield break;
        }

        npcData.SetDialogue(index); // 대사 SO 교체

        // 대화 시스템 실행 (Action 한 번 호출로 시스템에 제어권을 넘김)
        tutorialDialogue.Action(npcObject);

        // TutorialDialogue 스크립트가 스스로 대화를 끝낼 때까지 대기
        yield return new WaitUntil(() => tutorialDialogue.isDialogueFinished);

        // 대화 종료 후 약간의 여유 시간
        yield return new WaitForSeconds(0.1f);
        isDialogueActive = false;
    }

    private void SetupGuideUI(int index)
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        tutorialImage.sprite = tutorialSprites[index];
        if (tutorialMessages.Count > index) tutorialText.text = tutorialMessages[index];

        if (controlPanel != null) controlPanel.SetActive(true);
        controlImage.sprite = controlSprites[index];
    }

    // 계속해서 실행되며 활성화되는 slime을 찾음
    private IEnumerator SlimeActivationObserver()
    {
        while (true)
        {
            yield return new WaitUntil(() => hittedSlime != null);

            ActivateSlimeUI(hittedSlime);
            hittedSlime = null;
            yield return null;
        }
    }

    private void ActivateSlimeUI(GameObject target)
    {
        if (target == tutorialSlime1 && slimeAI1.enabled)
        {
            slimeAI1.ChangeSlimeAnimator(SlimeController1);
            slimeAI1.OnHit -= (obj) => hittedSlime = obj;
        }
    }

    private IEnumerator FadeOutPanel()
    {
        if (panelCanvasGroup == null)
        {
            tutorialPanel.SetActive(false);
            yield break;
        }

        float currentTime = 0;
        float startAlpha = panelCanvasGroup.alpha;

        while (currentTime < fadeDuration)
        {
            // 일시정지 중이면 페이드 진행 중단
            if (PauseManager.IsPaused)
            {
                yield return null;
                continue;
            }

            currentTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, currentTime / fadeDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);

        controlPanel.SetActive(false);

        // 다음 가이드를 위해 알파값 초기화 (필요시)
        panelCanvasGroup.alpha = 1f;
    }
}