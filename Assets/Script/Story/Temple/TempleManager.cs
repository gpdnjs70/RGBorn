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

    [Header("UI & Effect")]
    [SerializeField] private GameObject SceneChange;

    // 대화 상태 확인용
    public bool isDialogueActive { get; private set; }
    private bool hasTriggeredPaladin = false; // 중복 실행 방지

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
    }

    // --- 외부(Trigger 오브젝트)에서 호출할 함수 ---
    public void TriggerPaladinEvent()
    {
        if (!hasTriggeredPaladin)
        {
            hasTriggeredPaladin = true;
            StartCoroutine(PaladinDialogueSequence());
        }
    }

    private IEnumerator PaladinDialogueSequence()
    {
        // 1. 성기사 대화 시작
        SetPlayerControllable(false);
        yield return StartCoroutine(PlayDialogue(paladinNPC, 0));

        // 2. 보스 등장 및 이동
        if (finalBoss != null)
        {
            finalBoss.SetActive(true); // 보스 활성화
            yield return StartCoroutine(MoveBoss(finalBoss, bossTargetPos.position, 2.0f));

            // 3. 보스 대화 시작 (이동 완료 후 바로 실행)
            yield return StartCoroutine(PlayDialogue(finalBoss, 0));

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(PlayDialogue(finalBoss, 1));
        }

        // 4. 모든 이벤트 종료 후 연출
        if (SceneChange != null) SceneChange.SetActive(true);
        SetPlayerControllable(true);
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
            boss.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = targetPos;
        if (bossAnim != null) bossAnim.SetBool("isMove", false);
    }

    // VillageManager와 동일한 방식의 대화 처리 루틴
    public IEnumerator PlayDialogue(GameObject npcObj, int dialogueIndex)
    {
        SetPlayerControllable(false); // 대화 시작 시 조작 금지

        ObjData npcData = npcObj.GetComponent<ObjData>();
        if (npcData == null)
        {
            Debug.LogError(npcObj.name + "에 ObjData가 없습니다!");
            SetPlayerControllable(true);
            yield break;
        }

        npcData.SetDialogue(dialogueIndex); // 대사 SO 교체

        // 핵심: TutorialManager처럼 Action을 한 번만 호출하고 내부 로직이 끝날 때까지 기다립니다.
        // 이렇게 하면 TutorialDialogue 스크립트의 Update문이 입력을 정상적으로 처리합니다.
        dialogueSystem.Action(npcObj);

        // TutorialDialogue가 스스로 isDialogueFinished를 true로 만들 때까지 대기
        yield return new WaitUntil(() => dialogueSystem.isDialogueFinished);

        yield return new WaitForSeconds(0.1f);
    }

    public void SetPlayerControllable(bool canControl)
    {
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
}
