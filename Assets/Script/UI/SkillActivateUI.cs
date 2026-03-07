using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class SkillActivateUI : MonoBehaviour
{
    [SerializeField] private GameObject outline;
    [SerializeField] private Transform target;
    
    private RectTransform rectTrans;
    private Camera mainCam;
    private List<Image> childImages = new List<Image>();

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCam = Camera.main;

        if (outline != null)
        {
            foreach (Transform child in outline.transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null) childImages.Add(img);
            }
        }
    }

    public virtual void Update()
    {
        // UI 위치 이동
        if (target != null && rectTrans != null && mainCam != null)
        {
            Vector3 worldPos = target.position;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
            rectTrans.position = screenPos;
        }
    }

    public void Show(bool active) => outline.SetActive(active);

    public void SetChildSprite(int index, Sprite sprite)
    {
        if (index >= 0 && index < childImages.Count)
        {
            childImages[index].sprite = sprite;
        }
    }

    public int ChildCount => childImages.Count;
}
