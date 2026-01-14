using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject skillEffectPrefab;

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
        GameObject effectObj = Instantiate(skillEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }
}
