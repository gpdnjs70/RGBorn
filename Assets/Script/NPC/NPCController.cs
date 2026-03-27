using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float detectionRange = 3f;     // 플레이어 감지 거리
    public Transform playerTransform;     // 플레이어 위치 (비어있으면 Tag로 찾음)

    private Animator anim;
    private TutorialDialogue dialogueSystem; // 대화 중인지 확인용

    void Awake()
    {
        anim = GetComponent<Animator>();
        dialogueSystem = FindObjectOfType<TutorialDialogue>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (dialogueSystem != null && dialogueSystem.isAction) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange)
        {
            // 범위 내에 있으면 플레이어를 바라봄
            UpdateLookDirection();
        }
        else
        {
            // 범위를 벗어나면 기본 방향으로 복귀
            ReturnToDefault();
        }
    }

    void UpdateLookDirection()
    {
        if (playerTransform == null) return;
        
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        anim.SetFloat("DirX", direction.x);
        anim.SetFloat("DirY", direction.y);
    }

    // 에디터에서 범위를 시각적으로 확인
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void ReturnToDefault()
    {
        // 설정해둔 기본 방향(예: 0, -1)으로 애니메이션 파라미터 고정
        anim.SetFloat("DirX", 0f);
        anim.SetFloat("DirY", -1f);
    }
}
