using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RBoss_AttackEvent : MonoBehaviour
{
    private Enemy enemy;
    private Rigidbody2D rigid;

    [SerializeField] private GameObject fireFieldEffectPrefab;
    [SerializeField] private GameObject fireBallEffectPrefab;
    [SerializeField] private GameObject fireMineEffectPrefab;

    [SerializeField] private GameObject[] minionPrefabs;
    private int currentMinionIndex = 0;

    private Dictionary<string, float> lastExecutionTimes = new Dictionary<string, float>();

    [Header("Attack Sounds")]
    [SerializeField] private AudioClip fireFieldClip;
    [SerializeField] private AudioClip fireBallClip;
    [SerializeField] private AudioClip fireMineClip;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip summonClip;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void PlayBossSFX(string actionName, AudioClip clip)
    {
        if (clip != null && SoundManager.instance != null)
        {
            // 작성하신 SoundManager의 SFXPlay는 새로운 GameObject를 생성해 재생하고 파괴합니다.
            SoundManager.instance.SFXPlay(actionName, clip);
        }
    }

    private Vector3 GetValidNavMeshPos(Vector3 targetPos, float range = 1.0f)
    {
        NavMeshHit hit;
        // 2D 평면(X-Y)에서 targetPos 주변 반경 내의 유효한 '파란색 영역'을 찾습니다.
        if (NavMesh.SamplePosition(targetPos, out hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }

    public void fireField()
    {
        if (IsDuplicateInvocation("fireField")) return;

        PlayBossSFX("FireField", fireFieldClip);

        // Debug.Log("파이어 필드");

        int spawnCount = 8;
        float range = 4.0f; // 랜덤 분산 범위 (조금 줄여서 밀집도 높임)
        float spacing = 1.0f;
        float forwardOffset = 3.0f; // 플레이어 쪽으로 얼마나 전진해서 생성할 것인지

        int obstacleLayerMask = LayerMask.GetMask("fireField", "Enemy", "Wall");

        // 1. 보스 위치와 플레이어 위치 파악
        Vector3 center = transform.position;
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 태그로 찾기
        Vector3 targetCenter = center;

        if (player != null)
        {
            // 2. 플레이어를 향하는 방향 벡터 (정규화)
            Vector3 dirToPlayer = (player.transform.position - center).normalized;

            // 3. 생성 중심점을 플레이어 방향으로 전진시킴
            targetCenter = center + (dirToPlayer * forwardOffset);
        }

        int currentSpawned = 0;
        int attempts = 0;
        int maxAttempts = 100; // 공간이 부족할 경우를 대비한 안전장치

        while (currentSpawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // 1. 랜덤 좌표 생성
            Vector2 randomCircle = Random.insideUnitCircle * range;
            Vector3 rawSpawnPos = targetCenter + new Vector3(randomCircle.x, randomCircle.y, 0);

            // 2. NavMesh 위 유효한 위치인지 먼저 확인 (Bake된 파란 영역 안으로 보정)
            Vector3 navPos = GetValidNavMeshPos(rawSpawnPos, 2.0f);

            if (navPos != Vector3.zero)
            {
                // 3. NavMesh 위 좌표를 Spacing(그리드)에 맞춰 정렬
                float finalX = Mathf.Round(navPos.x / spacing) * spacing;
                float finalY = Mathf.Round(navPos.y / spacing) * spacing;
                Vector3 finalPos = new Vector3(finalX, finalY, 0);

                // 4. 중복 위치 확인 (이미 fireField가 설치되었는지)
                // NavMesh를 썼으므로 "Wall" 체크는 제외하고 "fireField" 중복만 체크합니다.
                Collider2D hit = Physics2D.OverlapCircle(finalPos, 0.7f, obstacleLayerMask);

                if (hit == null)
                {
                    // 5. 생성 및 셋업
                    GameObject tileObj = Instantiate(fireFieldEffectPrefab, finalPos, Quaternion.identity);
                    fireField tileScript = tileObj.GetComponent<fireField>();
                    if (tileScript != null)
                    {
                        Entity attackerEntity = enemy.GetComponent<Enemy_HP>();
                        tileScript.Setup(attackerEntity, 10f);
                    }
                    currentSpawned++;
                }
            }
        }
    }

    public void fireBall()
    {
        if (IsDuplicateInvocation("fireBall")) return;

        PlayBossSFX("FireBall", fireBallClip);
        // Debug.Log("파이어 볼");

        if (enemy == null || enemy.attackPoints == null) return;

        Entity attackerEntity = enemy.GetComponent<Enemy_HP>();
        if (attackerEntity == null) return;

        // 보스의 중심 위치 (발사 원점)
        Vector3 centerPos = transform.position;

        for (int i = 0; i < enemy.attackPoints.Length; i++)
        {
            Transform spawnPoint = enemy.attackPoints[i];
            GameObject arrowObj = Instantiate(fireBallEffectPrefab, spawnPoint.position, spawnPoint.rotation);

            arrow arrowScript = arrowObj.GetComponent<arrow>();
            if (arrowScript != null)
            {
                arrowScript.Init(attackerEntity);

                Vector2 radialDir = (spawnPoint.position - centerPos).normalized;

                if (radialDir == Vector2.zero) radialDir = spawnPoint.right;

                arrowScript.SetDirection(radialDir);
            }
        }
    }

    public void fireMine()
    {
        if (IsDuplicateInvocation("fireMine")) return;

        PlayBossSFX("FireMine", fireMineClip);
        // Debug.Log("불 지뢰");

        // fireMine Effect
        if (enemy == null) return;
        Entity attackerEntity = enemy.GetComponent<Enemy_HP>();
        if (attackerEntity == null) return;

        int spawnCount = 5;       // 맵에 뿌릴 지뢰 개수
        float range = 6.0f;       // 보스 주변 스폰 반경
        float spacing = 1.0f;     // 그리드 간격 (fireField와 동일하게 적용)
        float safetyRadius = 0.7f;

        int obstacleLayerMask = LayerMask.GetMask("Wall", "fireMine");

        Vector3 center = transform.position; // 보스의 현재 위치

        int currentSpawned = 0;
        int attempts = 0;
        int maxAttempts = 50; // 무한 루프 방지

        while (currentSpawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // 1. 랜덤 좌표 생성
            Vector2 randomCircle = Random.insideUnitCircle * range;
            Vector3 rawSpawnPos = center + new Vector3(randomCircle.x, randomCircle.y, 0);

            // 2. NavMesh 위 유효한 위치 찾기
            Vector3 navPos = GetValidNavMeshPos(rawSpawnPos, 2.0f);

            if (navPos != Vector3.zero)
            {
                // 3. Spacing 적용 (그리드 정렬)
                float finalX = Mathf.Round(navPos.x / spacing) * spacing;
                float finalY = Mathf.Round(navPos.y / spacing) * spacing;
                Vector3 finalPos = new Vector3(finalX, finalY, 0);

                // 4. 안전 거리 체크 (OverlapCircle 사용)
                // '점'이 아니라 '원'으로 체크하여 보스 테두리에 찰싹 붙는 것을 방지합니다.
                Collider2D hit = Physics2D.OverlapCircle(finalPos, safetyRadius, obstacleLayerMask);

                if (hit == null)
                {
                    // 5. 생성 및 셋업
                    GameObject mineObj = Instantiate(fireMineEffectPrefab, finalPos, Quaternion.identity);
                    fireMine mineScript = mineObj.GetComponent<fireMine>();

                    if (mineScript != null)
                    {
                        float burstDamage = 15f;
                        mineScript.Setup(attackerEntity, burstDamage);
                    }
                    currentSpawned++;
                }
            }
        }
    }

    public void Teleport()
    {
        if (IsDuplicateInvocation("Teleport")) return;
    PlayBossSFX("Teleport", teleportClip);

    Vector2 playerPos = enemy.player.position;
    Vector3 finalPos = transform.position;
    bool foundPos = false;

    for (int i = 0; i < 15; i++)
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDist = Random.Range(2.0f, 3.5f);
        
        // 2D 위치 계산
        Vector3 potentialPos = new Vector3(
            playerPos.x + Mathf.Cos(randomAngle) * randomDist,
            playerPos.y + Mathf.Sin(randomAngle) * randomDist,
            0
        );

        // NavMeshPlus가 구워놓은 '갈 수 있는 영역'인지 확인
        Vector3 validPos = GetValidNavMeshPos(potentialPos, 1.0f);

        if (validPos != Vector3.zero)
        {
            finalPos = validPos;
            foundPos = true;
            break;
        }
    }

    // NavMeshAgent2D 등을 사용 중이라면 Warp가 가장 안전합니다.
    var agent = GetComponent<NavMeshAgent>();
    if (agent != null)
    {
        agent.Warp(finalPos);
    }
    else
    {
        transform.position = finalPos;
    }
    }

    public void SummonMinions()
    {
        if (IsDuplicateInvocation("SummonMinions")) return;

        PlayBossSFX("Summon", summonClip);
        if (minionPrefabs == null || minionPrefabs.Length == 0) return;

        GameObject prefabToSpawn = minionPrefabs[currentMinionIndex];

        if (prefabToSpawn != null)
        {
            int countPerType = 3; // 한 번에 소환할 마릿수
            float summonRange = 3.0f;

            for (int i = 0; i < countPerType; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * summonRange;
                GameObject minionObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                // 초기화 로직 실행
                Minion minionScript = minionObj.GetComponent<Minion>();
                if (minionScript != null)
                {
                    minionScript.InitMinion();
                }
            }

            Debug.Log($"{prefabToSpawn.name} 3마리 소환 완료 (Index: {currentMinionIndex})");
        }

        currentMinionIndex = (currentMinionIndex + 1) % minionPrefabs.Length;
    }

    private bool IsDuplicateInvocation(string methodName)
    {
        // fireBall은 중복 호출이 심하므로 쿨타임을 더 넉넉하게 잡습니다.
        float customDebounce = (methodName == "fireBall") ? 0.4f : 0.1f;

        if (lastExecutionTimes.ContainsKey(methodName))
        {
            float timeSinceLast = Time.time - lastExecutionTimes[methodName];
            if (timeSinceLast < customDebounce)
            {
                // Debug.Log($"<color=red>{methodName}</color> 중복 호출 차단됨 (간격: {timeSinceLast})");
                return true;
            }
        }

        lastExecutionTimes[methodName] = Time.time;
        return false;
    }
}
