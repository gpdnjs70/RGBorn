using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireMine : MonoBehaviour
{
    private Entity attacker;
    private float burstDamage;
    private bool isExploded = false;

    private Animator anim;
    private Collider2D col;

    public float lifeTime = 10f; // 지뢰 유지 시간

    [Header("Audio Clips")]
    [SerializeField] private AudioClip explodeClip; // 폭발 사운드 클립
    [SerializeField] private AudioClip expireClip;  // 소화/만료 사운드 클립

    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Setup(Entity source, float damage)
    {
        this.attacker = source;
        this.burstDamage = damage;

        StartCoroutine(LifeTimeRoutine());
    }

    private IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!isExploded)
        {
            ExpireMine();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploded || !collision.CompareTag("Player")) return;

        Entity targetEntity = collision.GetComponent<Entity>();
        if (targetEntity != null)
        {
            ExplodeOnPlayer(targetEntity);
        }
    }

    private void ExplodeOnPlayer(Entity target)
    {
        isExploded = true;
        col.enabled = false; // 중복 밟기 방지를 위해 콜라이더 즉시 끄기

        target.TakeDamage(burstDamage, attacker != null ? attacker.gameObject : null);

        anim.SetTrigger("Explode");
    }

    private void ExpireMine()
    {
        isExploded = true;
        col.enabled = false; // 소화 중일 때는 밟아도 반응하지 않게 콜라이더 끄기

        anim.SetTrigger("Expire");
    }

    public void DestroyMineObject()
    {
        Destroy(gameObject);
    }

    public void PlayExplodeSound()
    {
        if (SoundManager.instance != null && explodeClip != null)
        {
            SoundManager.instance.SFXPlay("MineExplode", explodeClip);
        }
    }

    public void PlayExpireSound()
    {
        if (SoundManager.instance != null && expireClip != null)
        {
            SoundManager.instance.SFXPlay("MineExpire", expireClip);
        }
    }
}
