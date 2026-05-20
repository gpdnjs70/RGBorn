using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BTNType
{
    New,
    Continue,
    Option,
    Sound,
    Back,
    Quit,
    Scene,
    SceneBack,
    LeftButton,
    RightButton,
    Scene_Tutorial_Inside,
    Scene_Tutorial_Outside,
    Scene_Stage1,
    Scene_Town,
    Scene_Stage2,
    Scene_Temple,
    Scene_Stage3,
    Scene_RBoss,
    Scene_Final,
}

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;
    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    [Header("Scene Selection Groups")]
    public CanvasGroup sceneMenuParent;
    public CanvasGroup sceneGroup1;
    public CanvasGroup sceneGroup2;

    static bool isScene1Active = true;

    [Header("Page Image Settings")]
    public Image dotImage1;
    public Image dotImage2;
    public Sprite dotOnSprite;
    public Sprite dotOffSprite;

    public void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    bool isSound;
    public void OnBtnClick()
    {
        switch(currentType)
        {
            case BTNType.New:
                Debug.Log("새게임");
                FadeTransition.Instance.FadeToScene("Start");
                break;
            case BTNType.Continue:
                Debug.Log("이어하기");
                // 1. SaveManager를 통해 JSON 데이터 로드
                SaveData data = SaveManager.Instance.LoadGame();

                if (data != null)
                {
                    // 2. 저장된 데이터가 있다면 해당 씬 이름으로 이동
                    Debug.Log("저장된 씬으로 이동: " + data.lastSceneName);
                    FadeTransition.Instance.FadeToScene(data.lastSceneName);
                }
                else
                {
                    // 3. 저장 파일이 없을 경우 예외 처리 (예: 새 게임으로 시작)
                    Debug.Log("저장된 파일이 없습니다. 처음부터 시작합니다.");
                    FadeTransition.Instance.FadeToScene("Start");
                }
                break;
            case BTNType.Option:
                CanvasGroupOn(optionGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(optionGroup);
                break;
            case BTNType.Quit:
                Application.Quit();
                Debug.Log("앱 종료");
                break;
            case BTNType.Scene:
                CanvasGroupOn(sceneMenuParent);
                CanvasGroupOff(mainGroup);

                CanvasGroupOn(sceneGroup1);
                CanvasGroupOff(sceneGroup2);
                isScene1Active = true;

                UpdatePageImage();
                break;
            case BTNType.SceneBack:
                CanvasGroupOn(mainGroup);

                CanvasGroupOff(sceneMenuParent);
                CanvasGroupOff(sceneGroup1);
                CanvasGroupOff(sceneGroup2);
                isScene1Active = true;
                break;
            case BTNType.LeftButton:
            case BTNType.RightButton:
                // 현재 1번이 켜져 있다면 -> 2번을 켬
                if (isScene1Active)
                {
                    CanvasGroupOn(sceneGroup2);
                    CanvasGroupOff(sceneGroup1);
                    isScene1Active = false;
                }
                // 현재 2번이 켜져 있다면 -> 1번을 켬
                else
                {
                    CanvasGroupOn(sceneGroup1);
                    CanvasGroupOff(sceneGroup2);
                    isScene1Active = true;
                }

                UpdatePageImage();
                break;
            case BTNType.Scene_Tutorial_Inside:
                SaveAndLoadScene("Tutorial_Inside", 50.0f, 5.0f, 1.0f);
                break;
            case BTNType.Scene_Tutorial_Outside:
                SaveAndLoadScene("Tutorial_Outside", 50.0f, 5.0f, 1.0f);
                break;
            case BTNType.Scene_Stage1:
                SaveAndLoadScene("Stage1", 50.0f, 5.0f, 1.0f);
                break;
            case BTNType.Scene_Town:
                SaveAndLoadScene("Town", 50.0f, 5.0f, 1.2f);
                break;
            case BTNType.Scene_Stage2:
                SaveAndLoadScene("Stage1_after", 50.0f, 5.0f, 1.2f);
                break;
            case BTNType.Scene_Temple:
                SaveAndLoadScene("Temple", 100.0f, 5.0f, 1.44f);
                break;
            case BTNType.Scene_Stage3:
                SaveAndLoadScene("Stage3", 100.0f, 5.0f, 1.44f);
                break;
            case BTNType.Scene_RBoss:
                SaveAndLoadScene("RBoss", 100.0f, 5.0f, 1.44f);
                break;
            case BTNType.Scene_Final:
                SaveAndLoadScene("Finish", 100.0f, 5.0f, 1.44f);
                break;
        }
    }

    private void UpdatePageImage()
    {
        if(dotImage1 == null || dotImage2 == null) return;

        if (isScene1Active)
        {
            // 1페이지 활성화 상태: 1번은 켜고, 2번은 끎
            dotImage1.sprite = dotOnSprite;
            dotImage2.sprite = dotOffSprite;
        }
        else
        {
            // 2페이지 활성화 상태: 1번은 끄고, 2번은 켬
            dotImage1.sprite = dotOffSprite;
            dotImage2.sprite = dotOnSprite;
        }
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentType == BTNType.LeftButton ||
            currentType == BTNType.RightButton ||
            currentType == BTNType.SceneBack)
        {
            return; // 아무것도 하지 않고 함수를 빠져나감
        }

        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentType == BTNType.LeftButton ||
            currentType == BTNType.RightButton ||
            currentType == BTNType.SceneBack)
        {
            return; // 아무것도 하지 않고 함수를 빠져나감
        }

        buttonScale.localScale = defaultScale;
    }

    private void SaveAndLoadScene(string sceneName, float maxHP, float speed, float attackMult)
    {
        // 1. 새로운 저장 데이터 객체 생성 및 값 할당
        SaveData newData = new SaveData();

        newData.lastSceneName = sceneName;
        newData.isRespawn = false; // 요청하신 대로 false 고정

        // 하드코딩 혹은 전달받은 값 입력
        newData.currentMaxHP = maxHP;
        newData.currentSpeed = speed;
        newData.attackMultiplier = attackMult;

        // 2. SaveManager를 통해 JSON으로 저장
        // (SaveManager에 SaveGame(SaveData data) 메서드가 있다고 가정합니다)
        SaveManager.Instance.SaveGame(false, sceneName, newData);

        Debug.Log($"{sceneName} 데이터 저장 완료 후 이동");

        // 3. 씬 이동
        FadeTransition.Instance.FadeToScene(sceneName);
    }
}
