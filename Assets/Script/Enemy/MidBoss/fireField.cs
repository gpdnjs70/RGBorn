using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireField : MonoBehaviour
{
    private Entity source;
    private float damage;
    private Collider2D col;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Setup(Entity attacker, float duration)
    {
        source = attacker;
        StartCoroutine(FireSequence(duration));
    }

    IEnumerator FireSequence(float duration)
    {
        // 1. 예고 단계: 콜라이더를 끄고 예고 애니메이션/색상 재생
        col.enabled = false;
        // (여기에 예고 이펙트를 켜는 코드 추가 가능)
        yield return new WaitForSeconds(1.0f); // 1초간 예고

        // 2. 활성화 단계: 콜라이더를 켜고 불꽃 애니메이션 재생
        col.enabled = true;

        yield return new WaitForSeconds(duration);

        col.enabled = false; // 사라지는 중에는 데미지를 주지 않도록 콜라이더 끔
        if (anim != null)
        {
            anim.SetTrigger("Expire");

            yield return new WaitForEndOfFrame();
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
        }

        // 3. 소멸 단계
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player_HP target = collision.GetComponent<Player_HP>();
        if (target != null)
        {
            // Debug.Log("화상 시작");
            target.ApplyBurn(5f, 2f, 5f);
        }
    }
}
