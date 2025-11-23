using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField] private string skillName;
    [SerializeField] private float maxCooldownTime;
    [SerializeField] private TextMeshProUGUI textSkillData;
    [SerializeField] private TextMeshProUGUI textCooldownTime;
    [SerializeField] private Image imageCooldownTime;

    private float currentCooldownTime;
    private bool isCooldown;

    private void Awake()
    {
        SetCooldownIs(false);
    }

    public void UseSkill()
    {
        // 이미 스킬을 사용해서 재사용 대기 시간이 남아있으면 종료
        if (isCooldown)
        {
            textSkillData.text = $"[{skillName}] Cooldown Time : {currentCooldownTime:f1}";
            return;
        }

        textSkillData.text = $"Use Skill : {skillName}";

        StartCoroutine(nameof(OnCooldownTime), maxCooldownTime);
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
