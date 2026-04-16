using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MidBossAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    private Rigidbody2D rigid;

    [SerializeField] private GameObject minionPrefab;
    private List<GameObject> minionPool = new List<GameObject>(); // 리스트로 풀 관리
    [SerializeField] private Transform[] summonPoints; // 소환할 위치
    private GameObject[] activeMinions; // 각 위치별로 소환된 미니언을 기억할 배열

    private bool isFieldFired = false;
    [SerializeField] private GameObject fireFieldEffectPrefab;

    private bool isBallFired = false;
    [SerializeField] private GameObject fireBallEffectPrefab;
    
    private bool isMineFired = false;
    [SerializeField] private GameObject fireMineEffectPrefab;

    // Blaze Dash
    [SerializeField] private GameObject BlazeDashEffectPrefab; // 지나간 길에 남길 프리팹
    public float distanceInterval = 0.5f;
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    private bool isDashing = false;
    public bool dashHitRegistered { get; private set; }

    private bool isTempestFired = false;
    [SerializeField] private GameObject BlazeTempestEffectPrefab;

    private bool isMeteorFired = false;
    [SerializeField] private GameObject meteorShowerEffectPrefab;

    private bool isTeleportFired = false;

    private void Start()
    {
        // 소환 지점 개수만큼 관리용 배열 크기 설정
        if (summonPoints != null && summonPoints.Length > 0)
        {
            activeMinions = new GameObject[summonPoints.Length];
        }
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigid = GetComponent<Rigidbody2D>();
    }

    public void PassiveSkill()
    {
        Debug.Log("PassiveSkill");

        for (int i = 0; i < summonPoints.Length; i++)
        {
            // 해당 위치의 미니언이 없거나(null), 비활성화(죽음) 상태인지 체크
            if (activeMinions[i] == null || !activeMinions[i].activeSelf)
            {
                // 죽었거나 없는 경우에만 새로 소환
                SpawnMinionAt(i);
            }
            else
            {
                // 살아있다면 아무것도 하지 않고 넘어감
                Debug.Log($"{i}번 위치의 enemy는 아직 살아있음.");
            }
        }
    }

    private void SpawnMinionAt(int index)
    {
        // 풀에서 꺼내기
        GameObject minionObj = GetMinionFromPool();

        // 위치 설정 후 활성화
        minionObj.transform.position = summonPoints[index].position;
        minionObj.SetActive(true);

        // Minion 스크립트를 가져와서 리셋 및 정보 주입
        Minion minionComponent = minionObj.GetComponent<Minion>();
        if (minionComponent != null)
        {
            minionComponent.Setup(this.enemy);
        }

        // 관리 배열에 등록
        activeMinions[index] = minionObj;

        Debug.Log($"{index}번 위치에 enemy 리셋 및 소환 완료");
    }

    private GameObject GetMinionFromPool()
    {
        // 비활성화된 객체가 있다면 반환
        foreach (var obj in minionPool)
        {
            if (!obj.activeSelf) return obj;
        }

        // 없다면 새로 생성해서 풀에 추가
        GameObject newObj = Instantiate(minionPrefab);
        minionPool.Add(newObj);
        return newObj;
    }

    public void fireField()
    {
        Debug.Log("파이어 필드");

        if (isFieldFired) return;
        isFieldFired = true;

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
        Debug.Log("파이어 볼");

        if (isBallFired) return;
        isBallFired = true;

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
        Debug.Log("불 지뢰");

        if (isMineFired) return;
        isMineFired = true;

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
        Debug.Log("순간이동");

        if (isTeleportFired) return;
        isTeleportFired = true;

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

    public void BlazeDash()
    {
        Debug.Log("Blaze Dash");

        if (isDashing) return;
        isDashing = true;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        dashHitRegistered = false;

        // 대시 시작 시점의 플레이어 위치를 향한 방향 계산
        Vector2 dashDir = Vector2.right; // 기본값
        if (enemy.player != null)
        {
            dashDir = (enemy.player.position - transform.position).normalized;
            dashDir = new Vector2(dashDir.x, 0f);
        }

        Vector3 lastSpawnPos = transform.position; // 마지막 생성 위치 저장
        rigid.velocity = dashDir * dashingPower;

        float elapsed = 0f;
        while (elapsed < dashingTime)
        {
            // 마지막 생성 지점으로부터 현재까지의 거리 계산
            float dist = Vector2.Distance(lastSpawnPos, transform.position);

            if (dist >= distanceInterval)
            {
                SpawnPathEffect();
                lastSpawnPos = transform.position; // 생성한 현재 위치를 마지막 위치로 갱신
            }

            yield return null; // 거리 체크를 위해 매 프레임 대기
            elapsed += Time.deltaTime;
        }

        // 대시 종료
        rigid.velocity = Vector2.zero;
        isDashing = false;
    }

    // 이펙트에서 호출할 함수
    public void RegisterDashHit()
    {
        dashHitRegistered = true;
    }

    private void SpawnPathEffect()
    {
        if (BlazeDashEffectPrefab == null) return;

        // 현재 위치에 생성
        Vector3 pos = transform.position;
        pos -= new Vector3(0, 1.5f, 0);

        GameObject pathObj = Instantiate(BlazeDashEffectPrefab, pos, Quaternion.identity);
        
        var effectScript = pathObj.GetComponent<BlazeDash>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }

    public void BlazeTempest()
    {
        Debug.Log("Blaze Tempest");

        if (isTempestFired) return;
        isTempestFired = true;

        Vector3 pos = enemy.transform.position;
        float sign = (enemy.player.position.x - transform.position.x) >= 0 ? 1f : -1f;
        pos += new Vector3(sign * 0.5f, 0, 0);

        // 이펙트 생성
        GameObject effectObj = Instantiate(BlazeTempestEffectPrefab, pos, Quaternion.identity);

        // Flip 처리 (방향 전환)
        Vector3 localScale = effectObj.transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (sign * -1); // sign이 -1이면 왼쪽, 1이면 오른쪽
        effectObj.transform.localScale = localScale;

        // 스크립트 설정
        BlazeTempestEffect effectScript = effectObj.GetComponent<BlazeTempestEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }

    public void meteorShower()
    {
        Debug.Log("meteorShower");

        if (isMeteorFired) return;
        isMeteorFired = true;

        // meteorShower Effect
        Vector3 pos = enemy.player.transform.position;
        pos += new Vector3(0, 0.8f, 0);
        GameObject effectObj = Instantiate(meteorShowerEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            float damage = enemy.player.GetComponent<Player_HP>().HP * 0.07f;
            effectScript.Setup(enemy, EnemyAttackType.Skill6, damage);
        }
    }

    public void ResetAllAttackFlags()
    {
        isFieldFired = false;
        isBallFired = false;
        isMineFired = false;
        isTempestFired = false;
        isMeteorFired = false;
        isTeleportFired = false;
        Debug.Log("모든 공격 플래그 리셋됨 (스테이트 종료)");
    }
}
