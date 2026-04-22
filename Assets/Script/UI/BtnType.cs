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
    Quit
}

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;
    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

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
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
