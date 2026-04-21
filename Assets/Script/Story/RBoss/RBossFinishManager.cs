using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RBossFinishManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bossNPC;      // 대화할 보스 NPC
    [SerializeField] private string nextSceneName;    // 대화 종료 후 이동할 마을/다음맵
    public TutorialDialogue dialogueSystem;

    private Player_Movement playerMoveScript;
    private Player_Attack attackScript;
    private PlayerAttribute playerAttributeScript;
    private Player_Skill playerSkillScript;
    private Rigidbody2D rigid;
    private Animator playerAnim;

    private void Awake()
    {
        // 씬에 배치된 플레이어의 컴포넌트들을 가져옵니다.
        rigid = player.GetComponent<Rigidbody2D>();
        playerMoveScript = player.GetComponent<Player_Movement>();
        attackScript = player.GetComponent<Player_Attack>();
        playerAttributeScript = player.GetComponent<PlayerAttribute>();
        playerSkillScript = player.GetComponent<Player_Skill>();

        playerAnim = player.GetComponent<Animator>();
    }

    private void Start()
    {
        // 씬이 시작되자마자 대화 시퀀스 실행
        StartCoroutine(FullDialogueSequence());
    }

    private IEnumerator FullDialogueSequence()
    {
        // 1. 시작하자마자 모든 조작 금지
        SetPlayerControllable(false);

        if (playerAnim != null)
        {
            playerAnim.SetFloat("DirX", -1);
            playerAnim.SetFloat("DirY", 1);
        }

        // 2. 약간의 페이드 인 시간 등을 고려한 대기
        yield return new WaitForSeconds(1.0f);

        // 3. 대화 실행 (이미 씬에 bossNPC가 배치되어 있다고 가정)
        if (bossNPC != null)
        {
            // PlayDialogue는 기존 코드 그대로 사용
            yield return StartCoroutine(PlayDialogue(bossNPC, 0));
        }

        if (bossNPC != null)
        {
            Animator RBossAnim = bossNPC.GetComponent<Animator>();
            RBossAnim.SetTrigger("Death");
        }

        yield return new WaitForSeconds(1.0f);

        // 4. 대화가 다 끝나면 다음 장소로 이동
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeTransition.Instance.FadeToScene(nextSceneName);
        }
    }

    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex)
    {
        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData != null)
        {
            npcData.SetDialogue(dialogueIndex);
            dialogueSystem.Action(npcObj);

            // dialogueSystem의 끝남 플래그를 기다림
            yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void SetPlayerControllable(bool canControl)
    {
        if (playerMoveScript) playerMoveScript.enabled = canControl;
        if (attackScript) attackScript.enabled = canControl;
        if (playerAttributeScript) playerAttributeScript.enabled = canControl;
        if (playerSkillScript) playerSkillScript.enabled = canControl;

        if (!canControl && rigid != null)
        {
            rigid.velocity = Vector2.zero;
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.SetBool("isMove", false);
        }
    }
}
