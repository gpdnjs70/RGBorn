using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Mark : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    [Header("StoryScene Manager Reference")]
    [SerializeField] private VillageManager villageManager; // 매니저 스크립트 등록
    [SerializeField] private TutorialManager_0 tutorialManager_0; // 매니저 스크립트 등록
    
    [Header("Sprites per NPC Status")]
    [SerializeField] private Sprite defaultSprite;     // 대화 전 기본 이미지
    [SerializeField] private Sprite completedSprite;   // 대화 완료 후 이미지 (선택 사항)
    [SerializeField] private int npcID = 1;

    private RectTransform rectTrans;
    private Camera mainCam;
    private Image markImage;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        mainCam = Camera.main;
        markImage = GetComponent<Image>();
    }

    private void Start()
    {
        villageManager = FindFirstObjectByType<VillageManager>();
        tutorialManager_0 = FindFirstObjectByType<TutorialManager_0>();
    }

    private void LateUpdate()
    {
        if (target == null || rectTrans == null || mainCam == null) return;

        // 1. NPC 상태에 따라 Source Image 변경하는 핵심 로직
        UpdateMarkerSprite();

        // 2. 좌표 변환 및 스크린 위치 이동
        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        // 3. 카메라 뒤쪽 예외 처리
        if (screenPos.z < 0)
        {
            if (markImage != null) markImage.enabled = false;
            return;
        }

        if (markImage != null) markImage.enabled = true;
        rectTrans.position = screenPos;
    }

    private void UpdateMarkerSprite()
    {
        if (markImage == null) return;

        bool isTalked = false;

        // 1. 마을 씬일 때 처리
        if (villageManager != null)
        {
            switch (npcID)
            {
                case 1: isTalked = villageManager.talkedToNPC1; break;
                case 2: isTalked = villageManager.talkedToNPC2; break;
                case 3: isTalked = villageManager.talkedToNPC3; break;
            }
        }
        else if (tutorialManager_0 != null)
        {
            switch (npcID)
            {
                case 1: isTalked = tutorialManager_0.talkedToBook; break;
                case 2: isTalked = tutorialManager_0.openedBox; break;
            }
        }

        // 조건에 따른 이미지 교체
        if (isTalked)
        {
            markImage.sprite = completedSprite;
        }
        else
        {
            markImage.sprite = defaultSprite;
        }
    }
}
