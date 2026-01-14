using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject fireFieldEffectPrefab;
    [SerializeField] private GameObject infernoEffectPrefab;

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

        // inferno Effect
        Vector3 pos = enemy.pos.transform.position;

        // 이동 방향 계산 (플레이어가 오른쪽에 있으면 1, 왼쪽에 있으면 -1)
        float sign = (enemy.player.position.x - transform.position.x) >= 0 ? 1f : -1f;
        pos += new Vector3(sign * 0.5f, 0, 0);

        pos -= Vector3.right * 2.5f;
        pos += Vector3.up * 0.3f;
        GameObject effectObj = Instantiate(infernoEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }
}
