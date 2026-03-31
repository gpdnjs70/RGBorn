using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    public Animator animator;

    private SlimeAI slimeAIScript;
    private Enemy enemyScript;
    private Enemy_HP enemyHPScript;
    private EnemyAttributeMono enemyAttributeMonoScript;
    private Canvas enemyCanvas;

    public float idleDuration = 2.0f;
    public float attackRecoveryTime = 1.0f;
    public float StartedDirection;

    public NPCController targetNPC;

    public event Action<GameObject> OnHit;

    private void Awake()
    {
        slimeAIScript = GetComponent<SlimeAI>();
        enemyScript = GetComponent<Enemy>();
        enemyHPScript = GetComponent<Enemy_HP>();
        enemyAttributeMonoScript = GetComponent<EnemyAttributeMono>();
        enemyCanvas = GetComponentInChildren<Canvas>(true);
    }

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (enemyScript) enemyScript.enabled = false;
        if (enemyHPScript) enemyHPScript.enabled = false;
        if (enemyAttributeMonoScript) enemyAttributeMonoScript.enabled = false;
        if (enemyCanvas) enemyCanvas.gameObject.SetActive(false);

        animator.SetFloat("DirX", StartedDirection);

        StartCoroutine(SlimeBehaviorLoop());
    }

    private IEnumerator SlimeBehaviorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleDuration);

            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackRecoveryTime);
        }
    }

    public void TriggerDamageEvent()
    {
        if (targetNPC != null)
        {
            // NPC 위치에서 슬라임 위치를 뺀 '상대적 방향'을 구합니다.
            Vector2 direction = (transform.position - targetNPC.transform.position).normalized;

            // NPC에게 방향 전달
            targetNPC.PlayDamagedAnimation(direction.x);
        }
    }

    public void ChangeSlimeAnimator(RuntimeAnimatorController newController)
    {
        if (animator != null && newController != null)
        {
            animator.runtimeAnimatorController = newController;
            animator.SetFloat("DirX", StartedDirection);
            // Debug.Log($"{gameObject.name}의 애니메이터가 성공적으로 전환되었습니다.");
        }

        StopAllCoroutines();

        if (enemyCanvas) enemyCanvas.gameObject.SetActive(true);
        if (enemyScript) enemyScript.enabled = true;
        if (enemyHPScript) enemyHPScript.enabled = true;
        if (enemyAttributeMonoScript) enemyAttributeMonoScript.enabled = true;

        if (slimeAIScript) slimeAIScript.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something touched me: " + collision.gameObject.name + " / Tag: " + collision.tag);

        if (collision.CompareTag("Weapon"))
        {
            OnHit?.Invoke(gameObject);
        }
    }

    public void InvokeOnHit()
    {
        // 구독자(TutorialManager 등)가 있을 때만 이벤트를 실행
        OnHit?.Invoke(this.gameObject);
    }
}
