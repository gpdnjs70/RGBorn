using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : character, IDebuffTarget2
{
    public character character => this;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private float baseHP = 50f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        string sceneName = SceneManager.GetActiveScene().name;
        
        HP = baseHP;

        base.Setup();
    }

    public override void TakeDamage(float damage)
    {
        float finalDamage = damage * DefenseMultiplier;

        Debug.Log($"데미지는 {finalDamage}");

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0);

        // 30% 확률로 크리티컬 연출 (예시)
        bool isCritical = Random.Range(0, 100) < 30;

        DamagePopup.Create(spawnPos, (int)finalDamage, isCritical);

        HP -= finalDamage;

        StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.1f);

        color.a = 1;
        spriteRenderer.color = color;
    }
}
