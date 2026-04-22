using UnityEngine;
using UnityEngine.AI;

public class Golem_followState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Enemy enemy;
    private Entity entity;
    private Transform enemyTransform;

    private Vector3 dashDirection;
    private float maxMoveDistance = 10f;
    private float stateTime; // 상태 지속 시간 체크

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        entity = enemy.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();
        enemyTransform = animator.transform;

        stateTime = 0; // 초기화
        enemy.atkDelay = enemy.atkCoolTime;

        // 1. 에이전트 즉시 초기화
        agent.isStopped = false;
        agent.speed = entity.Speed;
        agent.velocity = Vector3.zero; // 이전 상태의 관성 제거

        // 2. 목적지 계산
        dashDirection = (enemy.player.position - enemyTransform.position).normalized;
        Vector3 rawTargetPos = enemyTransform.position + (dashDirection * maxMoveDistance);

        // 3. NavMesh 위 유효 좌표 찾기 (반경을 넉넉히 3.0f로 설정)
        NavMeshHit hit;
        Vector3 targetNavPosition;
        if (NavMesh.SamplePosition(rawTargetPos, out hit, 3.0f, NavMesh.AllAreas))
        {
            targetNavPosition = hit.position;
        }
        else
        {
            targetNavPosition = rawTargetPos;
        }

        agent.SetDestination(targetNavPosition);

        // 4. 경로 계산을 강제로 즉시 시작하도록 유도
        enemy.DirectionEnemy(targetNavPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stateTime += Time.deltaTime;

        // --- 공통 체크 (플레이어 접촉/벽 충돌) ---
        float distToPlayer = Vector2.Distance(enemyTransform.position, enemy.player.position);
        if (distToPlayer <= 1.1f)
        {
            StopAndFall(animator);
            enemy.OnAttackEvent();
            return;
        }

        if (enemy.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Wall")))
        {
            StopAndFall(animator);
            return;
        }

        // --- NavMesh 이동 로직 체크 (시작 후 0.1초 뒤부터 검사) ---
        // 경로 계산 중(pathPending)이 아닐 때만 체크
        if (stateTime > 0.1f && !agent.pathPending)
        {
            // A. 목적지 도달 체크
            if (agent.remainingDistance <= agent.stoppingDistance + 0.3f)
            {
                StopAndFall(animator);
                return;
            }

            // B. 경로 우회/꺾임 체크
            // desiredVelocity가 유효할 때만 체크
            if (agent.desiredVelocity.sqrMagnitude > 0.1f)
            {
                Vector3 currentMoveDir = agent.desiredVelocity.normalized;
                float dot = Vector3.Dot(dashDirection, currentMoveDir);

                if (dot < 0.5f) // 좀 더 완만한 0.5(60도)로 조정
                {
                    StopAndFall(animator);
                    return;
                }
            }

            // C. 제자리 걸음/끼임 체크 (어느 정도 가속 시간이 지난 후 검사)
            if (stateTime > 0.5f)
            {
                // 목적지는 남았는데 속도가 거의 없는 경우
                if (agent.velocity.sqrMagnitude < 0.01f && agent.remainingDistance > 0.5f)
                {
                    StopAndFall(animator);
                    return;
                }
            }
        }

        // 애니메이션 방향 업데이트
        // 에이전트가 이동 중이면 steeringTarget을, 아니면 목적지를 바라봄
        Vector3 lookPos = (agent.steeringTarget != Vector3.zero) ? agent.steeringTarget : enemy.player.position;
        enemy.DirectionEnemy(lookPos);
    }

    private void StopAndFall(Animator animator)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        animator.SetTrigger("isFall");
        animator.SetBool("isFollow", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
    }
}