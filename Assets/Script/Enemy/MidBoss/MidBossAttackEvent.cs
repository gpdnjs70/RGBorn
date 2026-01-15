using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject fireFieldEffectPrefab;
    [SerializeField] private GameObject infernoEffectPrefab;
    [SerializeField] private GameObject BlazeTempestEffectPrefab;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void fireField()
    {
        Debug.Log("fireField");

        // fireField Effect
        Vector3 pos = enemy.player.transform.position;
        pos -= Vector3.up * 0.3f;
        GameObject effectObj = Instantiate(fireFieldEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }

    public void Inferno()
    {
        Debug.Log("Inferno");

        // 위치 계산
        Vector3 pos = enemy.pos.transform.position;
        float sign = (enemy.player.position.x - transform.position.x) >= 0 ? 1f : -1f;
        pos += new Vector3(sign * 1.0f, 0.5f, 0);

        // 이펙트 생성
        GameObject effectObj = Instantiate(infernoEffectPrefab, pos, Quaternion.identity);

        // Flip 처리 (방향 전환)
        Vector3 localScale = effectObj.transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (sign * -1); // sign이 -1이면 왼쪽, 1이면 오른쪽
        effectObj.transform.localScale = localScale;

        // 스크립트 설정
        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }

    public void BlazeTempest()
    {
        Debug.Log("Blaze Tempest");

        Vector3 pos = enemy.pos.transform.position;
        float sign = (enemy.player.position.x - transform.position.x) >= 0 ? 1f : -1f;
        pos += new Vector3(sign * 0.5f, 0, 0);

        // 이펙트 생성
        GameObject effectObj = Instantiate(BlazeTempestEffectPrefab, pos, Quaternion.identity);

        // Flip 처리 (방향 전환)
        Vector3 localScale = effectObj.transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (sign * -1); // sign이 -1이면 왼쪽, 1이면 오른쪽
        effectObj.transform.localScale = localScale;

        // 스크립트 설정
        BlazeTempestEffect effectScript = effectObj.GetComponent<BlazeTempestEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }
}
