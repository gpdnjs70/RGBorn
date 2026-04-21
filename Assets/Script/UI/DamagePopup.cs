using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    // Create a Damage Popup
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        GameObject prefab = Resources.Load<GameObject>("DamagePopup");

        if (prefab == null)
        {
            Debug.LogError("Resources 폴더에 'DamagePopup' 프리팹을 찾을 수 없습니다.");
            return null;
        }

        GameObject damagePopupTransform = Instantiate(prefab, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());

        if (!isCriticalHit)
        {
            // Normal Hit
            ColorUtility.TryParseHtmlString("#FFFFFF", out textColor);
        }
        else
        {
            // Critical Hit
            ColorUtility.TryParseHtmlString("#0ABB0A", out textColor);
        }
        textMesh.color = textColor;

        disappearTimer = 0.5f;
    }

    private void Update()
    {
        // float moveYSpeed = 20f;
        // transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}