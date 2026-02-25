using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AttributeDatabase attributeDB; // 상성 데이터베이스 연결 필요

    public void ExecuteAttack(character attacker, character target, Action onComplete = null)
    {
        if (attacker == null || target == null) return;

        // 상태 이상에 따른 타겟 재설정 (혼란 등)
        character finalTarget = attacker.ResolveAttackTarget(target);

        // 공격 가능 상태인지 확인 (마비 등)
        if (!attacker.CanAct)
        {
            Debug.Log($"{attacker.name}은(는) 현재 행동할 수 없는 상태입니다.");
            onComplete?.Invoke();
            return;
        }

        //데미지 계산
        float calculatedDamage = attackDamage;

        // 속성 상성 계산
        ColorAttribute atkAttrComp = attacker.GetComponent<ColorAttribute>();
        ColorAttribute defAttrComp = finalTarget.GetComponent<ColorAttribute>();

        if (atkAttrComp != null && defAttrComp != null && attributeDB != null)
        {
            ColorAttributeData atkAttr = attributeDB.Get(atkAttrComp.colorMode);

            // 상성 유리 (공격 보너스)
            if (atkAttr.strongAgainst.Contains(defAttrComp.colorMode))
            {
                calculatedDamage *= (1f + atkAttr.attackBonusPercent);
            }
            // 상성 불리 (데미지 감소)
            if (atkAttr.weakAgainst.Contains(defAttrComp.colorMode))
            {
                calculatedDamage *= (1f - atkAttr.attackMinusPercent);
            }
        }

        Debug.Log($"{attacker.name}이(가) {finalTarget.name}에게 {calculatedDamage} 데미지 공격!");

        // 데미지 입히기
        finalTarget.TakeDamage(calculatedDamage);

        // 속성 디버프 적용 (10초로 적용)
        attacker.ApplyAttributeDebuffTo(finalTarget, 10f);

        // 이펙트 생성
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, finalTarget.transform.position, Quaternion.identity);
        }

        onComplete?.Invoke();
    }
}
