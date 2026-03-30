using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayDamagedAnimation(float dir)
    {
        if (anim != null)
        {
            // 0보다 크면 오른쪽(1), 작으면 왼쪽(-1)으로 명확하게 고정해주는 것이 좋습니다.
            float finalDir = (dir > 0) ? 1.0f : -1.0f;

            anim.SetFloat("Direction", finalDir);
            anim.SetTrigger("Damaged");

            // 디버그 로그로 값이 실제로 들어오는지 확인해보세요!
            // Debug.Log($"Hit Direction: {finalDir}");
        }
    }

    public void PlayIdleAnimation(Vector2 dir)
    {
        if (anim != null)
        {
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);

            anim.SetTrigger("Finish"); 
        }
    }
}
