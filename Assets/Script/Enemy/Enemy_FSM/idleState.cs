using UnityEngine;

public class idleState : StateMachineBehaviour
{
    Transform enemyTransform;
    Enemy enemy;
    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("idle");
        enemy = animator.GetComponent<Enemy>();
        enemyTransform = animator.GetComponent<Transform>();
        timer = Random.Range(2f, 5f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dist = Vector2.Distance(enemyTransform.position, enemy.player.position);

        // 공격 조건 (사거리 안 + 쿨타임 완료)
        if (dist <= 1.4f && enemy.atkDelay <= 0)
        {
            enemy.atkDelay = enemy.atkCoolTime;
            enemy.DirectionEnemy(enemy.player.position);

            animator.SetTrigger("Attack");
            return; // 공격 시작하면 아래 로직 무시
        }

        // 추적 조건 (공격 사거리는 아니지만 인식 범위 안)
        if (dist > 1.2f && dist <= 9f)
        {
            animator.SetBool("isWander", false);
            animator.SetBool("isFollow", true);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            animator.SetBool("isFollow", false);
            animator.SetBool("isWander", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
