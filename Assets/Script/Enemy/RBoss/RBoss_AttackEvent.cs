using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void fireField()
    {
        if (IsDuplicateInvocation("fireField")) return;

        PlayBossSFX("FireField", fireFieldClip);

        // Debug.Log("파이어 필드");

        int spawnCount = 8;
        float range = 4.0f; // 랜덤 분산 범위 (조금 줄여서 밀집도 높임)
        float spacing = 1.0f;
        float forwardOffset = 3.0f; // 플레이어 쪽으로 얼마나 전진해서 생성할 것인지

        int obstacleLayerMask = LayerMask.GetMask("Wall", "fireField");

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

        for (int i = 0; i < spawnCount; i++)
        {
            // 4. 이동된 targetCenter 주변으로 랜덤 좌표 생성
            Vector2 randomCircle = Random.insideUnitCircle * range;
            Vector3 spawnPos = targetCenter + new Vector3(randomCircle.x, randomCircle.y, 0);

            spawnPos.x = Mathf.RoundToInt(spawnPos.x / spacing) * spacing;
            spawnPos.y = Mathf.RoundToInt(spawnPos.y / spacing) * spacing;

            Collider2D hitObstacle = Physics2D.OverlapPoint(spawnPos, obstacleLayerMask);

            if (hitObstacle == null)
            {
                GameObject tileObj = Instantiate(fireFieldEffectPrefab, spawnPos, Quaternion.identity);
                fireField tileScript = tileObj.GetComponent<fireField>();
                if (tileScript != null)
                {
                    Entity attackerEntity = enemy.GetComponent<Enemy_HP>();
                    tileScript.Setup(attackerEntity, 10f);
                }
            }
            else
            {
                i--;
                // 무한 루프 방지 카운트 (i가 너무 작아지면 중단)
                if (i < -50) break;
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

        int obstacleLayerMask = LayerMask.GetMask("Wall", "fireMine", "Enemy");

        Vector3 center = transform.position; // 보스의 현재 위치

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * range;
            Vector3 spawnPos = center + new Vector3(randomCircle.x, randomCircle.y, 0);

            // 일정 간격(그리드)에 맞게 위치 조정
            spawnPos.x = Mathf.RoundToInt(spawnPos.x / spacing) * spacing;
            spawnPos.y = Mathf.RoundToInt(spawnPos.y / spacing) * spacing;

            Collider2D hitObstacle = Physics2D.OverlapPoint(spawnPos, obstacleLayerMask);
            // 벽이 아닌 빈 공간에만 생성
            if (hitObstacle == null)
            {
                // Debug.Log($"지뢰 생성 위치: {spawnPos} - 벽 없음. 생성 시도!");
                GameObject mineObj = Instantiate(fireMineEffectPrefab, spawnPos, Quaternion.identity);

                fireMine mineScript = mineObj.GetComponent<fireMine>();
                if (mineScript != null)
                {
                    // fireBall처럼 큰 단발성 대미지 수치 지정
                    float burstDamage = 15f;
                    mineScript.Setup(attackerEntity, burstDamage);
                }
            }
            else
            {
                // 벽에 걸려서 생성을 실패했다면 루프 횟수를 복구하여 지정된 개수를 무조건 채움
                // Debug.LogWarning($"지뢰 생성 실패: {spawnPos} 위치에 벽 감지됨! (i 값: {i})");
                i--;

                if (i < -50)
                {
                    // Debug.LogError("지뢰 생성 무한 루프 발생! 강제 종료");
                    break;
                }
            }
        }
    }

    public void Teleport()
    {
        if (IsDuplicateInvocation("Teleport")) return;

        PlayBossSFX("Teleport", teleportClip);
        // Debug.Log("순간이동");

        if (enemy.player == null) return;

        Vector2 playerPos = enemy.player.position;
        Vector3 targetPos = transform.position;

        bool foundPos = false;
        int attempts = 0;

        // 플레이어 주변 2~3 유닛 정도 떨어진 곳을 목표로 함
        float minDistance = 2.0f;
        float maxDistance = 3.5f;

        while (!foundPos && attempts < 15)
        {
            attempts++;

            // 1. 플레이어를 중심으로 랜덤한 각도와 거리 계산
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomDist = Random.Range(minDistance, maxDistance);

            Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDist;
            Vector2 potentialPos = playerPos + offset;

            // 2. 해당 위치가 벽 내부가 아닌지 확인
            Collider2D hitWall = Physics2D.OverlapCircle(potentialPos, 0.5f, LayerMask.GetMask("Wall"));

            if (hitWall == null)
            {
                targetPos = potentialPos;
                foundPos = true;
            }
        }

        // 만약 플레이어 주변이 온통 벽이라 자리를 못 찾았다면, 최후의 수단으로 플레이어 위치 그대로 이동
        if (!foundPos)
        {
            targetPos = playerPos;
        }

        transform.position = targetPos;

        // 텔레포트 후 즉시 플레이어를 바라보게 설정
        enemy.DirectionEnemy(playerPos);
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
