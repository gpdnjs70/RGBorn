using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_HP : Entity, IDebuffTarget
{
    public Entity Entity => this;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private float baseHP = 50f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        stats = new Stats();
        stats.Speed.BaseValue = 5f;

        string sceneName = SceneManager.GetActiveScene().name;
        float hpByScene = baseHP;

        switch (sceneName)
        {
            case "Stage1":
                hpByScene = 50f;
                break;
            case "Stage2":
                hpByScene = 100f;
                break;
            case "Stage3":
                hpByScene = 200f;
                break;
            default:
                hpByScene = baseHP; // 예외 상황 대비
                break;
        }

        stats.MaxHP.BaseValue = hpByScene;

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
