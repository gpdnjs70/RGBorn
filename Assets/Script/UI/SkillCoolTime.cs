using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] private string skillName;
    [SerializeField] private float maxCooldownTime;
    [SerializeField] private TextMeshProUGUI textCooldownTime;
    [SerializeField] private Image imageCooldownTime;

    private float currentCooldownTime;
    private bool isCooldown;

    public bool IsCooldown => isCooldown;

    private void Awake()
    {
        SetCooldownIs(false);
    }

    public void UseSkill()
    {
        TryUseSkill();
    }

    // True면 스킬 사용 성공, False면 쿨타임
    public bool TryUseSkill()
    {
        if (isCooldown)
        {
            Debug.Log($"[{skillName}] Cooldown Time : {currentCooldownTime:f1}");
            return false;
        }

        Debug.Log($"Use Skill : {skillName}");
        StartCooldown();
        return true;
    }

    private void StartCooldown()
    {
        StopAllCoroutines();
        StartCoroutine(OnCooldownTime(maxCooldownTime));
    }

    private IEnumerator OnCooldownTime(float maxCooldownTime)
    {
        currentCooldownTime = maxCooldownTime;

        SetCooldownIs(true);

        while(currentCooldownTime > 0)
        {
            currentCooldownTime -= Time.deltaTime;

            // Image UI의 fillAmount를 조절해 채워지는 이미지 모양 설정
            imageCooldownTime.fillAmount = currentCooldownTime / maxCooldownTime;

            // Text UI에 쿨다운 시간 표시
            textCooldownTime.text = currentCooldownTime.ToString("F1");

            yield return null;
        }

        SetCooldownIs(false);
    }

    private void SetCooldownIs(bool boolean)
    {
        isCooldown = boolean;
        textCooldownTime.enabled = boolean;
        imageCooldownTime.enabled = boolean;
    }
}
