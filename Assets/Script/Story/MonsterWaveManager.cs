using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterWaveManager : MonoBehaviour
{
    public static MonsterWaveManager Instance;

    [System.Serializable]
    public class WavePhase
    {
        public string phaseName; // 인스펙터 식별용
        public List<GameObject> monsters; // 해당 단계에 나타날 몬스터들
        public GameObject bossMonster;
        public GameObject[] phaseWalls;
        public GameObject entryCollider;
    }

    [Header("Wave Structure")]
    [SerializeField] private WavePhase[] phases = new WavePhase[3]; // 3단계를 위한 배열
    private int currentPhaseIndex = 0;

    [Header("UI References")]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Image controlImage;

    [Header("Wave Objects")]
    [SerializeField] private GameObject mapParent;
    [SerializeField] private Sprite chestInteractionSprite;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    [Header("Dialogue & Player Control")]
    [SerializeField] private GameObject playerObject;
    private Player_Movement playerMove;
    private Player_Attack playerAttack;
    private Rigidbody2D playerRigid;
    private Animator playerAnim;

    private int aliveCount;
    private bool isWaveStarted = false;

    [SerializeField] private GameObject boxPrefab;
    private bool allMonstersDead = false;

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
        if (controlPanel != null) controlPanel.SetActive(false);
        foreach (var phase in phases)
        {
            foreach (var wall in phase.phaseWalls) if (wall != null) wall.SetActive(false);
            foreach (var mon in phase.monsters) if (mon != null) mon.SetActive(false);
            if (phase.bossMonster != null) phase.bossMonster.SetActive(false);
        }
    }

    public void SetPlayerControl(bool canControl)
    {
        if (playerObject == null) return;

        if (playerMove != null) playerMove.enabled = canControl;
        if (playerAttack != null) playerAttack.enabled = canControl;

        if (!canControl)
        {
            // 움직임 멈추기: 속도 초기화 및 애니메이션 정지
            if (playerRigid != null) playerRigid.velocity = Vector2.zero;
            if (playerAnim != null) playerAnim.SetBool("isMove", false);
        }
    }

    public void OnPlayerEnterPhase(int index)
    {
        // 순서대로 진행하기 위해 현재 페이즈 인덱스와 맞는지 확인
        if (index == currentPhaseIndex && !isWaveStarted)
        {
            StartCoroutine(StartPhaseSequence(index));
        }
    }

    private IEnumerator StartPhaseSequence(int index)
    {
        isWaveStarted = true;
        WavePhase cp = phases[index];

        // 1. 입구 콜라이더는 이제 끔 (중복 실행 방지)
        if (cp.entryCollider != null) cp.entryCollider.SetActive(false);

        // 2. 연출: 화면 흔들림과 함께 벽이 활성화
        if (mapParent != null) StartCoroutine(MapShake(mapParent, shakeDuration, shakeMagnitude));

        foreach (var wall in cp.phaseWalls)
        {
            if (wall != null) wall.SetActive(true);
        }

        yield return new WaitForSeconds(0.3f); // 살짝의 시간차 연출

        // 3. 몬스터 등장
        aliveCount = cp.monsters.Count;
        foreach (var mon in cp.monsters)
        {
            if (mon != null) mon.SetActive(true);
        }
    }

    public void OnMonsterDied(GameObject monster)
    {
        WavePhase cp = phases[currentPhaseIndex];

        if (monster == cp.bossMonster)
        {
            SpawnTreasure(monster.transform.position);
        }

        if (!cp.monsters.Contains(monster) && monster != cp.bossMonster) return;

        aliveCount--;

        // 2. 모든 몬스터가 죽었을 때
        if (aliveCount <= 0)
        {
            if (currentPhaseIndex < phases.Length - 1)
            {
                // 중간 페이즈는 평소대로 벽 제거
                RemoveCurrentWalls();
                currentPhaseIndex++;
                isWaveStarted = false;
            }
            else
            {
                // 마지막 페이즈: 이제부터 상호작용이 가능하다고 표시
                allMonstersDead = true;
                Debug.Log("<color=cyan>모든 적 처치! 이제 상자를 열 수 있습니다.</color>");
            }
        }
    }

    private void EndCurrentPhase()
    {
        // 현재 페이즈 벽 해제
        foreach (var wall in phases[currentPhaseIndex].phaseWalls)
        {
            if (wall != null) wall.SetActive(false);
        }

        Debug.Log($"<color=green>{phases[currentPhaseIndex].phaseName} 클리어!</color>");

        currentPhaseIndex++;
        isWaveStarted = false; // 다음 페이즈 입구 진입을 대기하기 위해 false로

        if (currentPhaseIndex >= phases.Length)
        {
            Debug.Log("<color=yellow>전체 웨이브 종료!</color>");
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

    private void SpawnTreasure(Vector3 spawnPos)
    {
        if (boxPrefab != null)
        {
            // 몬스터 위치에 상자 생성
            Instantiate(boxPrefab, spawnPos, Quaternion.identity);
            Debug.Log("<color=orange>보스가 보물상자를 남겼습니다!</color>");
        }
    }

    public void ShowControlPanel(bool show)
    {
        // 마지막 페이즈이고, 모든 몬스터가 죽었을 때만 UI를 보여줌
        if (currentPhaseIndex == phases.Length - 1 && allMonstersDead)
        {
            if (controlPanel != null)
            {
                controlPanel.SetActive(show);
                if (show && controlImage != null) controlImage.sprite = chestInteractionSprite;
            }
        }
    }

    public bool CanInteractWithChest()
    {
        return allMonstersDead;
    }

    public void OnChestOpened()
    {
        ShowControlPanel(false);
        RemoveCurrentWalls();
        Debug.Log("<color=yellow>모든 퀘스트 완료! 벽이 사라집니다.</color>");

        currentPhaseIndex++; // 웨이브 최종 종료 처리
    }

    private void RemoveCurrentWalls()
    {
        foreach (var wall in phases[currentPhaseIndex].phaseWalls)
        {
            if (wall != null) wall.SetActive(false);
        }
    }
}