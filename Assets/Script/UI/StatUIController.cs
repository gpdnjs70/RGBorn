using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUIController : MonoBehaviour
{
    public static StatUIController Instance;

    [Header("UI References")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Image contentImage;      // 중간에 바뀔 이미지
    [SerializeField] private Text contentText;       // Legacy Text
    [SerializeField] private Button confirmButton;   // 확인 버튼

    [Header("Tutorial Data")]
    public List<Sprite> spriteList = new List<Sprite>();
    [TextArea(3, 10)]
    public List<string> textList = new List<string>();

    public bool isDisplaying = false;
    private int currentIndex = 0;

    private Action onSequenceComplete; // 종료 후 실행할 함수 보관용

    [SerializeField] private AudioClip levelUpSound;

    private void Awake()
    {
        Instance = this;
        // 시작 시 UI 숨기기
        panelGroup.alpha = 0;
        panelGroup.blocksRaycasts = false;
        panelGroup.gameObject.SetActive(false);

        confirmButton.onClick.RemoveAllListeners(); // 기존 연결 싹 다 제거
        confirmButton.onClick.AddListener(ShowNextContent);
    }

    private void Update()
    {
        // UI가 떠 있을 때 스페이스바를 누르면 다음으로
        if (isDisplaying && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextContent();
        }
    }

    // 외부(savePoint 등)에서 호출할 시작 함수
    public void StartUISequence(List<Sprite> newSprites, List<string> newTexts, Action onComplete = null)
    {
        spriteList.Clear();
        spriteList.AddRange(newSprites);

        textList.Clear();
        textList.AddRange(newTexts);

        // 이전 호출자가 남겨둔 콜백이 실행되지 않도록 무조건 덮어씌웁니다.
        onSequenceComplete = onComplete;

        if (spriteList.Count == 0 || textList.Count == 0) return;

        isDisplaying = true;
        currentIndex = 0;

        panelGroup.gameObject.SetActive(true);
        panelGroup.alpha = 1;
        panelGroup.blocksRaycasts = true;

        UpdateUI();
    }

    public void ShowNextContent()
    {
        currentIndex++;

        // 리스트 끝에 도달했는지 체크
        if (currentIndex < spriteList.Count && currentIndex < textList.Count)
        {
            UpdateUI();
        }
        else
        {
            CloseUI();
        }
    }

    private void UpdateUI()
    {
        contentImage.sprite = spriteList[currentIndex];
        contentText.text = textList[currentIndex];

        if (levelUpSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.SFXPlay("levelUp", levelUpSound);
        }
    }

    private void CloseUI()
    {
        isDisplaying = false;
        panelGroup.alpha = 0;
        panelGroup.blocksRaycasts = false;
        panelGroup.gameObject.SetActive(false);

        if (onSequenceComplete != null)
        {
            Debug.Log($"[StatUI] 콜백 실행됨: {onSequenceComplete.Method.Name}");
            var tempAction = onSequenceComplete;
            onSequenceComplete = null;
            tempAction.Invoke();
        }
    }
}
