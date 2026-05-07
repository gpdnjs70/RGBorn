using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillCoolTime : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string skillName;
    [TextArea][SerializeField] private string skillDescription;

    [SerializeField] private TextMeshProUGUI textCooldownTime;
    [SerializeField] private Image imageCooldownTime;

    [SerializeField] private GameObject tooltipObject; // 설명 UI 부모 객체
    [SerializeField] private Text textTooltipDescription; // 설명창의 본문 텍스트

    public float maxCooldownTime;
    private float currentCooldownTime;
    private bool isCooldown;

    public bool IsCooldown => isCooldown;

    private void Awake()
    {
        SetCooldownIs(false);
        if (tooltipObject != null) tooltipObject.SetActive(false); // 시작 시 설명창 끄기
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

    public void SetMaxCooldown(float time)
    {
        maxCooldownTime = time;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipObject == null) return;

        // 설명창 내용 세팅
        if (textTooltipDescription != null) textTooltipDescription.text = skillDescription;

        tooltipObject.SetActive(true);
    }

    // 마우스가 아이콘 밖으로 나갔을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipObject != null) tooltipObject.SetActive(false);
    }
}
