using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pointer : MonoBehaviour, IPointerClickHandler
{
    private static GameObject currentActivePointer;
    public static Transform SelectedTarget { get; private set; }
    private static Transform lastTarget;

    [SerializeField] public GameObject pointerImage; // 연결할 UI 오브젝트
    public Vector3 uiOffset;

    [SerializeField] private Transform target;

    private RectTransform imageRectTrans;
    private Camera mainCam;

    void Awake()
    {
        if (pointerImage != null)
        {
            imageRectTrans = pointerImage.GetComponent<RectTransform>();
        }
        mainCam = Camera.main;
    }

    void Start()
    {

    }

    void Update()
    {
        if (target != null && pointerImage != null && pointerImage.activeSelf)
        {
            Vector3 worldPos = target.position + uiOffset;
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

            imageRectTrans.position = screenPos;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BattleHandler battleHandler = FindObjectOfType<BattleHandler>();
            if (battleHandler.GetState() != BattleHandler.State.playerTurn) return;

            SelectThis();

            Debug.Log($"{target.name} 선택됨 (포인터 고정)");
        }
    }

    public void SelectThis()
    {
        if (currentActivePointer != null && currentActivePointer != pointerImage)
            currentActivePointer.SetActive(false);

        pointerImage.SetActive(true);
        currentActivePointer = pointerImage;
        SelectedTarget = target;
        lastTarget = target; // 선택할 때마다 기억함
    }

    public static void ClearSelection()
    {
        if (currentActivePointer != null) currentActivePointer.SetActive(false);
        currentActivePointer = null;
        SelectedTarget = null;
    }

    // 기억하고 있던 타겟의 포인터를 다시 활성화하는 함수
    public static void RestoreLastSelection()
    {
        if (lastTarget != null)
        {
            Pointer p = lastTarget.GetComponent<Pointer>();
            if (p != null) p.SelectThis();
        }
    }
}