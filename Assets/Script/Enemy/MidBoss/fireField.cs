using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireField : MonoBehaviour
{
    private Entity source;
    private float damage;
    private Collider2D col;

    void Awake() => col = GetComponent<Collider2D>();

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
        // (여기에 실제 불꽃 이펙트를 켜는 코드 추가 가능)

        yield return new WaitForSeconds(duration);

        // 3. 소멸 단계
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Entity target = collision.GetComponent<Entity>();
        if (target != null && source != null)
        {
            source.ApplyAttributeDebuffTo(target, 3f);
        }
    }
}
