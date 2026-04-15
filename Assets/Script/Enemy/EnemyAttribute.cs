using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttribute : MonoBehaviour
{
    [SerializeField] private ColorMode mode;
    [SerializeField] private AnimatorOverrideController[] colorOverrides;

    private Animator animator;
    private ColorAttribute attribute;

    private Coroutine changeAttributeRoutine;
    private int remainingChangeCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attribute = GetComponent<ColorAttribute>();
    }

    private void OnEnable()
    {
        // 오브젝트가 켜질 때마다 상태 초기화
        remainingChangeCount = 0;

        // 처음 나타날 때 즉시 랜덤 속성 부여 (원치 않으면 생략 가능)
        ChangeAttribute();

        // 기존에 돌아가던 루프가 있다면 멈추고 새로 시작
        if (changeAttributeRoutine != null) StopCoroutine(changeAttributeRoutine);
        changeAttributeRoutine = StartCoroutine(AttributeChangeLoop());
    }

    private void OnDisable()
    {
        // 오브젝트가 꺼질 때 코루틴을 안전하게 멈춤
        if (changeAttributeRoutine != null)
        {
            StopCoroutine(changeAttributeRoutine);
            changeAttributeRoutine = null;
        }
    }

    private IEnumerator AttributeChangeLoop()
    {
        while (true)
        {
            // 랜덤 대기 (2f ~ 4.1f)
            yield return new WaitForSeconds(Random.Range(3f, 5.1f));

            ChangeAttribute();
        }
    }

    private void ChangeAttribute()
    {
        int randomIndex = Random.Range(1, 4);
        ColorMode newMode = (ColorMode)randomIndex;

        this.mode = newMode;
        attribute.SetColor(newMode);

        if (animator != null && colorOverrides.Length >= 3)
        {
            animator.runtimeAnimatorController = colorOverrides[randomIndex - 1];
        }
    }
}
