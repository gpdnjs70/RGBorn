using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWaveManager : MonoBehaviour
{
    public static MonsterWaveManager Instance;

    [System.Serializable]
    public class WavePhase
    {
        public string phaseName; // 인스펙터 식별용
        public List<GameObject> monsters; // 해당 단계에 나타날 몬스터들
    }

    [Header("Wave Structure")]
    [SerializeField] private WavePhase[] phases = new WavePhase[3]; // 3단계를 위한 배열
    private int currentPhaseIndex = 0;

    [Header("Wave Objects")]
    [SerializeField] private GameObject mapParent;
    [SerializeField] private GameObject[] treeWalls;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    [Header("Dialogue & Player Control")]
    [SerializeField] private TutorialDialogue tutorialDialogue;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private int startWaveDialogueIndex = 0;

    private Player_Movement playerMove;
    private Player_Attack playerAttack;
    private Rigidbody2D playerRigid;
    private Animator playerAnim;

    private int aliveCount;
    private bool isWaveStarted = false;

    private void Awake()
    {
        Instance = this;
        if (playerObject != null)
        {
            playerMove = playerObject.GetComponent<Player_Movement>();
            playerAttack = playerObject.GetComponent<Player_Attack>();
            playerRigid = playerObject.GetComponent<Rigidbody2D>();
            playerAnim = playerObject.GetComponent<Animator>();
        }
    }

    void Start()
    {
        foreach (var wall in treeWalls) wall.SetActive(false);

        // 모든 단계의 모든 몬스터를 일단 꺼둡니다.
        foreach (var phase in phases)
        {
            foreach (var mon in phase.monsters) mon.SetActive(false);
        }
    }

    public void StartWave()
    {
        if (isWaveStarted) return;
        StartCoroutine(WaveStartSequence());
    }

    private IEnumerator WaveStartSequence()
    {
        isWaveStarted = true;
        currentPhaseIndex = 0;

        SetPlayerControl(false);

        if (mapParent != null) StartCoroutine(MapShake(mapParent, shakeDuration, shakeMagnitude));
        foreach (var wall in treeWalls) wall.SetActive(true);

        // 첫 시작 대사 연출 (필요 시 주석 해제)
        // yield return StartCoroutine(PlayWaveDialogue(startWaveDialogueIndex));

        SetPlayerControl(true);

        // 첫 번째 페이즈 시작
        StartPhase(currentPhaseIndex);
        yield return null;
    }

    private void StartPhase(int index)
    {
        if (index >= phases.Length) return;

        Debug.Log($"<color=cyan>{phases[index].phaseName} 시작!</color>");
        aliveCount = phases[index].monsters.Count;

        foreach (var mon in phases[index].monsters)
        {
            mon.SetActive(true);
            // 몬스터가 켜질 때 EnemyAttribute가 있다면 초기화 로직이 작동할 것입니다.
        }
    }

    public void OnMonsterDied(GameObject monster)
    {
        // 현재 페이즈의 몬스터인지 확인
        if (!phases[currentPhaseIndex].monsters.Contains(monster)) return;

        aliveCount--;
        Debug.Log($"몬스터 처치! 남은 수: {aliveCount}");

        if (aliveCount <= 0)
        {
            currentPhaseIndex++;

            if (currentPhaseIndex < phases.Length)
            {
                // 다음 페이즈로 넘어가기 전 짧은 대기 시간 (연출용)
                Invoke("NextPhase", 1.0f);
            }
            else
            {
                EndWave();
            }
        }
    }

    private void NextPhase()
    {
        // 다음 단계가 시작될 때 지진 효과를 살짝 다시 주면 좋습니다.
        if (mapParent != null) StartCoroutine(MapShake(mapParent, 0.4f, 0.1f));
        StartPhase(currentPhaseIndex);
    }

    void EndWave()
    {
        foreach (var wall in treeWalls) wall.SetActive(false);
        isWaveStarted = false;
        Debug.Log("<color=yellow>모든 웨이브 클리어!</color>");
    }

    // 플레이어의 움직임과 공격을 끄고 켜는 함수
    private void SetPlayerControl(bool canControl)
    {
        if (playerObject == null) return;

        if (playerMove != null) playerMove.enabled = canControl;
        if (playerAttack != null) playerAttack.enabled = canControl;

        if (!canControl)
        {
            // 움직임을 멈출 때 속도 초기화 및 애니메이션 정지
            if (playerRigid != null) playerRigid.velocity = Vector2.zero;
            if (playerAnim != null) playerAnim.SetBool("isMove", false);
        }
    }

    private IEnumerator PlayWaveDialogue(int index)
    {
        ObjData playerFileData = playerObject.GetComponent<ObjData>();
        if (playerFileData != null && tutorialDialogue != null)
        {
            playerFileData.SetDialogue(index);
            int totalCalls = playerFileData.dialogue.lines.Length + 1;

            for (int i = 0; i < totalCalls; i++)
            {
                tutorialDialogue.Action(playerObject);

                if (i < playerFileData.dialogue.lines.Length)
                {
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
                    yield return null;
                }
            }
        }
    }

    private IEnumerator MapShake(GameObject target, float duration, float magnitude)
    {
        Vector3 originalPos = target.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            target.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.transform.position = originalPos;
    }
}