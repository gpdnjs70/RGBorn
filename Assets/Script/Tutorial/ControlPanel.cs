using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public GameObject Panel;
    public RectTransform PanelRect;

    public Transform playerTransform;

    public Vector3 offset = new Vector3(0, 2.0f, 0);

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            UpdatePanelPosition();
        }
    }

    void UpdatePanelPosition()
    {
        Vector3 worldPos = playerTransform.position + offset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        PanelRect.position = screenPos;
    }
}
