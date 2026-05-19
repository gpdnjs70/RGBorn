using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    [SerializeField] private GameObject skipButton;

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
            // 1. 현재 장면 설정
            if (imageDisplay != null) imageDisplay.sprite = cutsceneScenes[i].illustration;
            if (textDisplay != null) textDisplay.text = cutsceneScenes[i].dialogue;

            float timer = 0;
            bool skipRequested = false;

            // 2. 대기 루프 (시간 기반 + 입력 감지)
            while (timer < cutsceneScenes[i].displayTime)
            {
                // 일시정지 상태가 아닐 때만 동작
                if (Time.timeScale > 0)
                {
                    timer += Time.deltaTime;

                    // 스페이스바 입력 감지
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        skipRequested = true;
                        break;
                    }

                    // 마우스 좌클릭(0) 감지
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 클릭한 위치에 스킵 버튼 UI가 있는지 검사
                        if (IsPointerOverSkipButtons())
                        {
                            // 스킵 버튼을 누른 것이라면 장면 넘기기를 무시하고 대기 (버튼 고유의 스킵 함수만 실행되도록)
                            yield return null;
                            continue;
                        }

                        // 스킵 버튼 영역이 아니라면 정상적으로 다음 장면으로 진행
                        skipRequested = true;
                        break;
                    }
                }
                yield return null;
            }

            // 입력 겹침 방지를 위해 한 프레임 정도 짧게 대기 (선택 사항)
            if (skipRequested)
            {
                yield return null;
            }
        }

        EndCutscene();
    }

    private bool IsPointerOverSkipButtons()
    {
        if (EventSystem.current == null || skipButton == null)
            return false;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == skipButton || result.gameObject.transform.IsChildOf(skipButton.transform))
                {
                    return true; // 스킵 버튼 영역이 맞으므로 true 리턴
                }
            }
        }
        return false;
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
