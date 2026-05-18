using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RBossSceneManager : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Player_Movement playerMoveScript;
    private Player_Attack attackScript;
    private PlayerAttribute playerAttributeScript;
    private Player_Skill playerSkillScript;
    [SerializeField] private GameObject statePanel;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bossNPC;     // 대화용 보스 (NPC 레이어)
    [SerializeField] private GameObject bossEnemy;   // 실제 전투용 보스 (Enemy 레이어)
    [SerializeField] private Transform playerStandPos; // 대화 시 플레이어가 서 있을 위치
    [SerializeField] private Transform bossSpawnPos;  // 보스가 등장할 위치
    [SerializeField] private AnimatorOverrideController newOverrideController;

    [SerializeField] private float cutsceneMoveSpeed = 1.5f;

    public TutorialDialogue dialogueSystem;
    private bool isDialogueActive = false;

    [SerializeField] private string nextSceneName;

    private void Awake()
    {
        rigid = player.GetComponent<Rigidbody2D>();
        playerMoveScript = player.GetComponent<Player_Movement>();
        attackScript = player.GetComponent<Player_Attack>();
        playerAttributeScript = player.GetComponent<PlayerAttribute>();
        playerSkillScript = player.GetComponent<Player_Skill>();
    }

    private void Start()
    {
        // 처음에는 전투용 보스를 꺼두고 대화용 보스만 활성화하거나, 둘 다 꺼둠
        if (bossEnemy != null) bossEnemy.SetActive(false);

        // 씬 시작 시 자동으로 시퀀스 실행 (혹은 트리거를 통해 호출)
        StartCoroutine(BossIntroductionSequence());
    }

    private IEnumerator BossIntroductionSequence()
    {
        // 1. 플레이어 조작 정지 및 지정 위치로 이동 (선택 사항)
        SetPlayerControllable(false);
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("DirX", 0f);
            anim.SetFloat("DirY", 1f); // 위를 바라봄
        }

        yield return new WaitForSeconds(0.5f);

        if (playerStandPos != null)
        {
            yield return StartCoroutine(MoveObject(player, playerStandPos.position, cutsceneMoveSpeed));
        }

        // 2. 보스 등장 (예: 위에서 내려오거나 서서히 나타남)
        if (bossNPC != null)
        {
            bossNPC.SetActive(true);
        }

        // 3. 보스 대화 시작 (PlayDialogue 함수 재사용)
        yield return StartCoroutine(PlayDialogue(bossNPC, 0));
        yield return new WaitForSecondsRealtime(0.5f);

        StartCoroutine(CameraShake(0.5f, 0.1f));
        if (player != null)
        {
            Animator playerAnim = player.GetComponent<Animator>();
            if (playerAnim != null && newOverrideController != null)
            {
                playerAnim.runtimeAnimatorController = newOverrideController;

                playerAnim.SetFloat("DirX", 0);
                playerAnim.SetFloat("DirY", 1);
            }
        }
        yield return StartCoroutine(PlayDialogue(bossNPC, 1));

        yield return new WaitForSecondsRealtime(2.0f);

        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

        StartBossBattle();
    }

    private void StartBossBattle()
    {
        // 대화용 NPC 제거 및 실제 적 개체 활성화
        if (bossNPC != null) bossNPC.SetActive(false);
        if (bossEnemy != null) bossEnemy.SetActive(true);

        playerAttributeScript.SetRGBColor(ColorMode.Red);
        if (statePanel != null) statePanel.SetActive(true);

        // 플레이어 조작 권한 반환
        SetPlayerControllable(true);

        // 여기에 BGM 변경이나 UI 활성화 로직 추가
    }

    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex)
    {
        // 중복 실행 방지
        if (isDialogueActive) yield break;

        if (PauseManager.IsPaused) yield return new WaitUntil(() => !PauseManager.IsPaused);

        isDialogueActive = true;
        // 1. 플레이어 조작 정지 및 물리 초기화 (확실히 고정)
        SetPlayerControllable(false);

        // 2. NPC 데이터 설정
        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObj.name + "에 ObjData가 없습니다!");
            isDialogueActive = false;
            yield break;
        }
        npcData.SetDialogue(dialogueIndex);

        // 3. 대화 시스템 실행 (Action 한 번 호출로 시스템에 제어권을 넘김)
        dialogueSystem.Action(npcObj);

        // 4. TutorialDialogue가 스스로 대화를 끝낼 때까지 대기
        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);

        // 5. 대화 종료 후 약간의 여유 (연타 방지)
        yield return new WaitForSecondsRealtime(0.1f);
        isDialogueActive = false;
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 targetPos, float speed)
    {
        Animator anim = obj.GetComponent<Animator>();
        if (anim != null) anim.SetBool("isMove", true);

        while (Vector3.Distance(obj.transform.position, targetPos) > 0.05f)
        {
            if (!PauseManager.IsPaused)
            {
                if (anim != null) anim.speed = 1f;

                obj.transform.position = Vector3.MoveTowards(
                    obj.transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );

                if (obj == player && playerMoveScript != null)
                    playerMoveScript.PlayGroundSound();
            }
            else
            {
                // 일시정지 중이면 애니메이션 멈춤
                if (anim != null) anim.speed = 0f;
            }
            yield return null;
        }

        obj.transform.position = targetPos;
        if (anim != null)
        {
            anim.speed = 1f;
            anim.SetBool("isMove", false);
        }
    }

    public void SetPlayerControllable(bool canControl)
    {
        if (player == null) return;

        if (PauseManager.IsPaused && canControl) return;

        // 이동, 공격, 속성, 스킬 스크립트 전체 제어
        if (playerMoveScript) playerMoveScript.enabled = canControl;
        if (attackScript) attackScript.enabled = canControl;
        if (playerAttributeScript) playerAttributeScript.enabled = canControl;
        if (playerSkillScript) playerSkillScript.enabled = canControl;

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

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            if (!PauseManager.IsPaused)
            {
                Camera.main.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * magnitude;
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

    public void OnAllEnemiesDefeated()
    {
        StartCoroutine(EndBattleSequence());
    }

    private IEnumerator EndBattleSequence()
    {
        Debug.Log("전투 종료 시퀀스 시작");

        // 전투 중이던 UI(패널 등)를 끄고 싶다면 여기에 추가
        if (statePanel != null) statePanel.SetActive(false);

        // 플레이어 조작 다시 정지 (대화 씬으로 넘어가기 전 안전장치)
        SetPlayerControllable(false);

        yield return new WaitForSeconds(1.5f); // 적들이 사라지는 것을 볼 시간

        // 씬 전환
        if (FadeTransition.Instance != null)
        {
            // 인스펙터에서 설정한 nextSceneName으로 페이드 전환 시작
            FadeTransition.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            // 혹시 모르니 페이드 매니저가 없을 때의 예외 처리
            Debug.LogWarning("FadeTransition Instance를 찾을 수 없습니다. 바로 씬을 전환합니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}
