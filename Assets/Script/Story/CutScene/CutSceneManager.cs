using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneScene
    {
        public Sprite illustration;     // 보여줄 이미지
        [TextArea(3, 5)]
        public string dialogue;        // 표시할 텍스트
        public float displayTime = 3f; // 이 장면이 유지될 시간
    }

    [Header("UI References (Legacy)")]
    [SerializeField] private Image imageDisplay;
    [SerializeField] private Text textDisplay;

    [Header("Settings")]
    [SerializeField] private CutsceneScene[] cutsceneScenes;
    [SerializeField] private string nextSceneName;

    private Coroutine cutsceneRoutine;

    private void Start()
    {
        if (cutsceneScenes != null && cutsceneScenes.Length > 0)
        {
            cutsceneRoutine = StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        for (int i = 0; i < cutsceneScenes.Length; i++)
        {
            // 이미지와 텍스트 교체
            if (imageDisplay != null) imageDisplay.sprite = cutsceneScenes[i].illustration;
            if (textDisplay != null) textDisplay.text = cutsceneScenes[i].dialogue;

            // 설정된 시간만큼 대기
            yield return new WaitForSeconds(cutsceneScenes[i].displayTime);
        }

        EndCutscene();
    }

    private void EndCutscene()
    {
        Debug.Log("컷씬 종료");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeTransition.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            // 다음 씬 이름이 없으면 컷씬 UI만 비활성화
            gameObject.SetActive(false);
        }
    }

    public void SkipToTutorial()
    {
        // 1. 진행 중인 코루틴이 있다면 중단 (중복 호출 방지)
        if (cutsceneRoutine != null)
        {
            StopCoroutine(cutsceneRoutine);
        }

        // 2. 즉시 튜토리얼로 이동
        Debug.Log("컷씬 스킵: Tutorial로 이동");
        FadeTransition.Instance.FadeToScene("Tutorial_Inside");
    }

    public void SkipToMainMenuFinal()
    {
        // 1. 진행 중인 코루틴이 있다면 중단 (중복 호출 방지)
        if (cutsceneRoutine != null)
        {
            StopCoroutine(cutsceneRoutine);
        }

        // 2. 즉시 메인 메뉴로 이동
        Debug.Log("컷씬 스킵: MainMenu_Final로 이동");
        FadeTransition.Instance.FadeToScene("MainMenu_Final");
    }
}
