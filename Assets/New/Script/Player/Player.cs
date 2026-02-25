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

    public override void Burn(float damage)
    {
        HP -= damage;
    }
}
