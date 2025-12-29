using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance;

    [Header("UI")]
    [SerializeField] private Image fadeImage;   // 검은 이미지
    [SerializeField] private Image tutorialImage;   // 게임 조작법 안내 이미지

    [Header("Tutorial")]
    [SerializeField] private bool showTutorialOnStart = true;
    [SerializeField] private bool hideTutorialOnAnyInput = true;

    [Header("Fade")]
    [SerializeField] private float fadeOutTime = 0.4f;  // 까매지는 속도
    [SerializeField] private float fadeInTime = 0.4f;   // 밝아지는 속도
    [SerializeField] private float blackHoldTime = 0.05f; // 완전 까만 상태 유지(짧게)

    private bool isTransitioning = false;
    private bool tutorialShown = false;
    private bool tutorialClosed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // 씬 로드될 때마다 튜토리얼이 다시 뜨지 않도록 처리(원하면 바꿀 수 있음)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 시작 시 페이드 이미지가 투명하도록 보정
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // 시작 시 튜토리얼 띄우기
        if (showTutorialOnStart)
            ShowTutorial();
        else
            HideTutorial();
    }

    private void Update()
    {
        // 튜토리얼이 떠 있고, "아무 입력으로 닫기" 옵션이면 닫기
        if (hideTutorialOnAnyInput && tutorialShown && !tutorialClosed)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                HideTutorial();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 넘어가면 튜토리얼은 기본적으로 숨김
        // (튜토리얼 씬에서만 띄우고 싶다는 전제)
        HideTutorial();
    }

    private void ShowTutorial()
    {
        if (tutorialImage == null) return;

        tutorialImage.gameObject.SetActive(true);
        tutorialShown = true;
        tutorialClosed = false;
    }
    private void HideTutorial()
    {
        if (tutorialImage == null) return;

        tutorialImage.gameObject.SetActive(false);
        tutorialClosed = true;
    }
    public bool IsTutorialClosed()
    {
        return tutorialClosed;
    }

    public void FadeToScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        isTransitioning = true;

        // Fade Out (검게)
        yield return Fade(0f, 1f, fadeOutTime);

        // 완전 검정 잠깐 유지
        if (blackHoldTime > 0f)
            yield return new WaitForSeconds(blackHoldTime);

        // 씬 로드
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 로드 직후 Fade In (밝게)
        yield return Fade(1f, 0f, fadeInTime);

        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            c.a = a;
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}