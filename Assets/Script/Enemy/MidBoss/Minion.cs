using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private Enemy master;
    private Transform playerTransform;

    // 필요한 컴포넌트들
    private Enemy_HP hpScript;
    private Enemy enemyScript;
    private Collider2D col;
    private Animator anim;

    private void Awake()
    {
        hpScript = GetComponent<Enemy_HP>();
        enemyScript = GetComponent<Enemy>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    public void Setup(Enemy boss)
    {
        master = boss;
        playerTransform = boss.player;

        if (hpScript != null)
        {
            hpScript.playerAttribute = boss.hp.playerAttribute;
        }

        ResetMinion();
    }

    private void ResetMinion()
    {
        DebuffController debuffController = GetComponent<DebuffController>();
        if (debuffController != null)
        {
            debuffController.ClearAllDebuffs();
        }

        if (hpScript != null)
        {
            // 체력 및 스크립트 활성화
            hpScript.HP = hpScript.MaxHP;
            hpScript.enabled = true;

            // 리플렉션으로 private 변수인 isDead를 false로 강제 변경
            FieldInfo isDeadField = typeof(Enemy_HP).GetField("isDead", BindingFlags.NonPublic | BindingFlags.Instance);
            if (isDeadField != null)
            {
                isDeadField.SetValue(hpScript, false);
            }

            if (hpScript.hpUI != null) hpScript.hpUI.gameObject.SetActive(true);
        }

        if (enemyScript != null) enemyScript.enabled = true;
        if (col != null) col.enabled = true;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        // OnEnable도 혹시 모르니 유지 (다른 초기화 로직이 있을 수 있음)
        hpScript.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
