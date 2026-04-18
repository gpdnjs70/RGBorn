using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBossSceneManager : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Player_Movement playerMoveScript;
    private Player_Attack attackScript;
    private PlayerAttribute playerAttributeScript;
    private Player_Skill playerSkillScript;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bossNPC;     // 대화용 보스 (NPC 레이어)
    [SerializeField] private GameObject bossEnemy;   // 실제 전투용 보스 (Enemy 레이어)
    [SerializeField] private Transform playerStandPos; // 대화 시 플레이어가 서 있을 위치
    [SerializeField] private Transform bossSpawnPos;  // 보스가 등장할 위치

    [SerializeField] private float cutsceneMoveSpeed = 1.5f;

    public TutorialDialogue dialogueSystem;

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

        yield return new WaitForSeconds(1.0f);

        StartBossBattle();
    }

    private void StartBossBattle()
    {
        // 대화용 NPC 제거 및 실제 적 개체 활성화
        if (bossNPC != null) bossNPC.SetActive(false);
        if (bossEnemy != null) bossEnemy.SetActive(true);

        // 플레이어 조작 권한 반환
        SetPlayerControllable(true);

        // 여기에 BGM 변경이나 UI 활성화 로직 추가
    }

    // --- 기존 매니저에서 사용한 공용 함수들 ---

    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex)
    {
        ObjData npcData = npcObj.GetComponent<ObjData>();
        npcData.SetDialogue(dialogueIndex);

        dialogueSystem.Action(npcObj);
        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 targetPos, float speed)
    {
        Animator anim = obj.GetComponent<Animator>();
        if (anim != null) anim.SetBool("isMove", true);

        while (Vector3.Distance(obj.transform.position, targetPos) > 0.05f)
        {
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            // 플레이어인 경우 발소리 재생 등 추가 가능
            if (obj == player && playerMoveScript != null)
                playerMoveScript.PlayGroundSound();

            yield return null;
        }

        obj.transform.position = targetPos;
        if (anim != null) anim.SetBool("isMove", false);
    }

    public void SetPlayerControllable(bool canControl)
    {
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
}
