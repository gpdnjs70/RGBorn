using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private character character;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    [SerializeField] private TextMeshProUGUI turnText;

    private RectTransform rectTrans;
    private Camera mainCam;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (target != null && rectTrans != null && mainCam != null)
        {
            Vector3 worldPos = target.position + worldOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
            rectTrans.position = screenPos;
        }
    }

    public void UpdateTurnText(int count)
    {
        if (turnText != null)
        {
            if (count >= 0)
            {
                // 0 이상이면 UI를 보여주고 숫자 업데이트
                gameObject.SetActive(true);
                turnText.text = count.ToString();
            }
            else
            {
                // 0이 되면 UI를 아예 숨김
                gameObject.SetActive(false);
            }
        }
    }
}
