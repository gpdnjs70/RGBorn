using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType
{
    None,
    Burn,      // 화상(지속적으로 hp 감소)
    Slow,      // 슬로우(느려짐)
    LifeSteal, // 흡혈(이 피흡)
    Shock,     // 마비(스킬 사용 불가)
    Poison,    // 중독(지속적으로 hp 감소, 혼란(자기때림))
    Corrosion  // 부식(방어력 감소)
}

[CreateAssetMenu(fileName = "ColorAttribute", menuName = "ColorAttributes/ColorAttributeData")]
public class ColorAttributeData : ScriptableObject
{
    public ColorMode colorMode;

    // 공격 시 추가 데미지 비율
    public float attackBonusPercent;
    // 피격 시 적용되는 데미지 배율
    public float receiveDamageMultiplier;

    // 디버프
    public DebuffType debuffType;
    public float debuffChance;
    public float debuffDuration;
}
