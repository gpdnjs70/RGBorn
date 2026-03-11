using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttribute : MonoBehaviour
{
    [SerializeField] private ColorMode mode;
    [SerializeField] private Sprite[] randomSprites;
    [SerializeField] private AnimatorOverrideController[] colorOverrides;

    private Animator animator;
    private ColorAttribute attribute;
    private SkillActivateUI skillUI;

    private Coroutine changeAttributeRoutine;
    private bool isSkillUIActive = false;
    private int remainingChangeCount = 0;
    private int currentChildIdx = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attribute = GetComponent<ColorAttribute>();
        skillUI = GetComponentInChildren<SkillActivateUI>(true);

        attribute.SetColor(mode);
    }

    private void Start()
    {
        // 첫 루프 시작
        changeAttributeRoutine = StartCoroutine(AttributeChangeLoop());
    }

    private IEnumerator AttributeChangeLoop()
    {
        while (true)
        {
            // 랜덤 대기 (2f ~ 4.1f)
            yield return new WaitForSeconds(Random.Range(2f, 4.1f));

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

        // 스킬 연출 중일 때만 UI 업데이트
        if (isSkillUIActive)
        {
            UpdateSkillUI(randomIndex);
        }
    }

    public void ActivateSkill()
    {
        if (skillUI == null || randomSprites.Length == 0) return;

        // 기존 루프 중단
        if (changeAttributeRoutine != null) StopCoroutine(changeAttributeRoutine);

        // UI 초기화 및 상태 설정
        isSkillUIActive = true;
        remainingChangeCount = 6;
        currentChildIdx = 0;
        skillUI.Show(true);

        // 초기 UI 세팅
        for (int i = 0; i < skillUI.ChildCount; i++)
        {
            skillUI.SetChildSprite(i, randomSprites[0]);
        }

        changeAttributeRoutine = StartCoroutine(AttributeChangeLoop());
    }

    private void UpdateSkillUI(int spriteIndex)
    {
        if (spriteIndex < randomSprites.Length)
        {
            skillUI.SetChildSprite(currentChildIdx, randomSprites[spriteIndex]);
        }

        currentChildIdx = (currentChildIdx + 1) % skillUI.ChildCount;
        remainingChangeCount--;

        if (remainingChangeCount <= 0)
        {
            isSkillUIActive = false;
            StartCoroutine(HideSkillUIAfterDelay());
        }
    }

    private IEnumerator HideSkillUIAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (!isSkillUIActive)
        {
            skillUI.Show(false);
        }
    }
}
