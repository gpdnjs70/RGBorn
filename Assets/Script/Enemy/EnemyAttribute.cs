using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttribute : MonoBehaviour
{
    [SerializeField] private ColorMode mode;
    private ColorAttribute attribute;

    [SerializeField] private Sprite[] randomSprites;

    private SkillActivateUI skillUI;
    private Coroutine skillRoutine;

    private void Awake()
    {
        attribute = GetComponent<ColorAttribute>();
        attribute.SetColor(mode);

        skillUI = GetComponentInChildren<SkillActivateUI>(true);
    }

    public void ActivateSkill()
    {
        if (skillRoutine != null) StopCoroutine(skillRoutine);
        skillRoutine = StartCoroutine(SkillSequenceRoutine());
    }

    private IEnumerator SkillSequenceRoutine()
    {
        if (skillUI == null || randomSprites.Length == 0) yield break;

        skillUI.Show(true);

        for (int i = 0; i < skillUI.ChildCount; i++)
        {
            skillUI.SetChildSprite(i, randomSprites[0]);
        }

        float initialWait = Random.Range(2f, 4.1f);
        yield return new WaitForSeconds(initialWait);

        int changeCount = 6;
        int currentChildIdx = 0;

        for (int i = 0; i < changeCount; i++)
        {
            int randomIndex = Random.Range(1, 4);
            ColorMode newMode = (ColorMode)randomIndex;

            this.mode = newMode;
            attribute.SetColor(newMode);

            if (randomIndex < randomSprites.Length)
            {
                skillUI.SetChildSprite(currentChildIdx, randomSprites[randomIndex]);
            }

            currentChildIdx = (currentChildIdx + 1) % skillUI.ChildCount;

            if (i < changeCount - 1)
            {
                yield return new WaitForSeconds(Random.Range(2f, 4.1f));
            }
        }

        yield return new WaitForSeconds(1.0f);

        skillUI.Show(false);
        skillRoutine = null;
    }
}
