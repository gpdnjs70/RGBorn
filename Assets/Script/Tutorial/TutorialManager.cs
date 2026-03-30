using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] private Transform targetPosition; // 플레이어가 걸어갈 목표 위치
    [SerializeField] private float moveSpeed = 3f;     // 컷신 중 플레이어 이동 속도

    [SerializeField] private GameObject tutorialPanel; // UI 전체를 켜고 끌 부모 객체
    [SerializeField] private Text tutorialText;
    [SerializeField] private List<string> tutorialMessages = new List<string>();
    [SerializeField] private Image tutorialImage;      // 스프라이트가 바뀔 이미지 컴포넌트
    [SerializeField] private List<Sprite> tutorialSprites = new List<Sprite>(); // 무한정 추가 가능한 리스트

    [SerializeField] private GameObject tutorialSlime1;
    [SerializeField] private RuntimeAnimatorController SlimeController1;
    private Enemy_HP enemyHPScript1;
    private SlimeAI slimeAI1;

    [SerializeField] private GameObject tutorialSlime2;
    [SerializeField] private RuntimeAnimatorController SlimeController2;
    private Enemy_HP enemyHPScript2;
    private SlimeAI slimeAI2;

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
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MovePlayerToTarget());
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));

        // 4~6번 과정을 이 한 줄로 대체 (0번 SO 재생: "으아악!", "살려주세요!")
        yield return StartCoroutine(PlayDialogue(0));

        // 7. 조작법 설명 진입
        StartControlTutorial();
    }

    // 플레이어를 목표 위치로 자연스럽게 걷게 만드는 코루틴
    private IEnumerator MovePlayerToTarget()
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("목표 위치(targetPosition)가 설정되지 않았습니다!");
            yield break;
        }

        Animator anim = player.GetComponent<Animator>();

        // 걷기 애니메이션 켜기
        if (anim != null) anim.SetBool("isMove", true);

        // 목표 위치를 향한 방향 벡터 계산 (애니메이션 방향 설정용)
        Vector3 direction = (targetPosition.position - player.transform.position).normalized;
        if (anim != null)
        {
            anim.SetFloat("DirX", direction.x);
            anim.SetFloat("DirY", direction.y);
        }

        // 플레이어의 현재 위치와 목표 위치의 거리가 0.05f보다 클 때까지 계속 이동
        while (Vector3.Distance(player.transform.position, targetPosition.position) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // 다음 프레임까지 대기
        }

        // 목표 위치에 정확히 안착시키고 걷기 애니메이션 끄기
        player.transform.position = targetPosition.position;
        if (anim != null)
        {
            anim.SetFloat("DirX", 1f);
            anim.SetFloat("DirY", 0f);
            anim.SetBool("isMove", false);
        }
    }

    private IEnumerator WaitForUserInput()
    {
        yield return null;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
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

            // 유저가 실제로 공격(좌클릭)할 때까지 대기
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            if (slimeAI1 != null && SlimeController1 != null)
            {
                slimeAI1.ChangeSlimeAnimator(SlimeController1);
            }

            Debug.Log("평타 공격 확인 완료!");
            yield return new WaitForSeconds(0.5f);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        // --- 2단계: 속성 전환 ---
        if (tutorialSprites.Count > 1)
        {
            if (enemyHPScript1 != null) enemyHPScript1.LockAction();
            // NPC 대사 리스트의 1번 SO 재생
            yield return StartCoroutine(PlayDialogue(1));
            if (enemyHPScript1 != null) enemyHPScript1.UnlockAction();

            SetupGuideUI(1);
            SetPlayerControllable(TutorialStep.Attribute);

            // 유저가 속성 전환(Space)할 때까지 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            Debug.Log("속성 전환 확인 완료!");
            yield return new WaitForSeconds(0.5f);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        // --- 3단계: 속성 상성 설명 ---
        if (tutorialSprites.Count > 2)
        {
            SetupGuideUI(2);
            SetPlayerControllable(TutorialStep.All);

            yield return new WaitForSeconds(3f);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        yield return new WaitUntil(() => tutorialSlime1 == null || enemyHPScript1.HP <= 0);

        yield return new WaitForSeconds(1.0f);

        // 마지막 대화 방향 설정
        Vector2 dirToNPC = (npcObject.transform.position - player.transform.position).normalized;

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("DirX", dirToNPC.x);
            playerAnim.SetFloat("DirY", dirToNPC.y);
        }

        NPCController npcCtrl = npcObject.GetComponent<NPCController>();
        if (npcCtrl != null)
        {
            // 플레이어의 반대 방향을 바라보게 함
            npcCtrl.PlayIdleAnimation(-dirToNPC);
        }

        // NPC 대사 리스트의 3번 SO 재생
        yield return StartCoroutine(PlayDialogue(3));

        // Debug.Log("조작법 및 구출 튜토리얼 완전 종료!");

        SetPlayerControllable(TutorialStep.All);
    }

    // NPC의 dialogueList 중 하나를 끝까지 재생하고 창을 닫는 공용 함수
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

        ObjData npcData = npcObject.GetComponent<ObjData>();
        npcData.SetDialogue(index); // NPC에게 대사 SO 교체 명령

        // SO의 대사 줄 수만큼 + 창 닫기용 1번 = 총 횟수
        int totalCalls = npcData.dialogue.lines.Length + 1;

        for (int i = 0; i < totalCalls; i++)
        {
            tutorialDialogue.Action(npcObject);

            // 마지막 호출(창 닫기)이 아닐 때만 유저 입력을 기다림
            if (i < npcData.dialogue.lines.Length)
            {
                yield return StartCoroutine(WaitForUserInput());
            }
        }
    }

    private void SetupGuideUI(int index)
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        tutorialImage.sprite = tutorialSprites[index];
        if (tutorialMessages.Count > index) tutorialText.text = tutorialMessages[index];
    }
}