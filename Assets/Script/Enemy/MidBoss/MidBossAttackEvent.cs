using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAttackEvent : MonoBehaviour
{
    private Enemy enemy;
    private Rigidbody2D rigid;

    [SerializeField] private GameObject fireFieldEffectPrefab;
    [SerializeField] private GameObject infernoEffectPrefab;
    [SerializeField] private GameObject fireRainEffectPrefab;
    [SerializeField] private GameObject BlazeTempestEffectPrefab;
    [SerializeField] private GameObject meteorShowerEffectPrefab;

    // Blaze Dash
    [SerializeField] private GameObject BlazeDashEffectPrefab; // 지나간 길에 남길 프리팹
    public float distanceInterval = 0.5f;
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    private bool isDashing = false;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigid = GetComponent<Rigidbody2D>();
    }

    public void PassiveSkill()
    {
        Debug.Log("PassiveSkill");
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

    public void fireRain()
    {
        Debug.Log("fireRain");

        // fireRain Effect
        Vector3 pos = enemy.player.transform.position;
        pos += new Vector3(0, 1.5f, 0);
        GameObject effectObj = Instantiate(fireRainEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }

    public void BlazeDash()
    {
        Debug.Log("Blaze Dash");

        if (isDashing) return;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        isDashing = true;

        // 대시 시작 시점의 플레이어 위치를 향한 방향 계산
        Vector2 dashDir = Vector2.right; // 기본값
        if (enemy.player != null)
        {
            dashDir = (enemy.player.position - transform.position).normalized;
            dashDir = new Vector2(dashDir.x, 0f);
        }

        Vector3 lastSpawnPos = transform.position; // 마지막 생성 위치 저장
        rigid.velocity = dashDir * dashingPower;

        float elapsed = 0f;
        while (elapsed < dashingTime)
        {
            // 마지막 생성 지점으로부터 현재까지의 거리 계산
            float dist = Vector2.Distance(lastSpawnPos, transform.position);

            if (dist >= distanceInterval)
            {
                SpawnPathEffect();
                lastSpawnPos = transform.position; // 생성한 현재 위치를 마지막 위치로 갱신
            }

            yield return null; // 거리 체크를 위해 매 프레임 대기
            elapsed += Time.deltaTime;
        }

        // 대시 종료
        rigid.velocity = Vector2.zero;
        isDashing = false;
    }

    private void SpawnPathEffect()
    {
        if (BlazeDashEffectPrefab == null) return;

        // 현재 위치에 생성
        GameObject pathObj = Instantiate(BlazeDashEffectPrefab, transform.position, Quaternion.identity);
        
        var effectScript = pathObj.GetComponent<BlazeDash>();
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

    public void meteorShower()
    {
        Debug.Log("meteorShower");

        // meteorShower Effect
        Vector3 pos = enemy.player.transform.position;
        //pos += new Vector3(0, 1.5f, 0);
        GameObject effectObj = Instantiate(meteorShowerEffectPrefab, pos, Quaternion.identity);

        PunishmentEffect effectScript = effectObj.GetComponent<PunishmentEffect>();
        if (effectScript != null)
        {
            effectScript.Setup(enemy);
        }
    }
}
