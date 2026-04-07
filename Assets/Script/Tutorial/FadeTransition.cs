using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance;

    [Header("UI")]
    [SerializeField] private Image fadeImage;   // 검은 이미지

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutTime = 0.4f;   // 어두워지는 시간
    [SerializeField] private float fadeInTime = 0.4f;    // 밝아지는 시간
    [SerializeField] private float blackHoldTime = 0.05f; // 검은 화면 유지 시간

    private bool isTransitioning = false;

    private void Awake()
    {
        // 싱글톤 설정 및 파괴 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            // 시작할 때는 클릭이 가능해야 하므로 false
            fadeImage.raycastTarget = false;
        }
    }

    // 외부에서 씬 전환을 호출할 때 쓰는 함수
    public void FadeToScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        isTransitioning = true;

        // 페이드 시작할 때 클릭 차단 활성화
        if (fadeImage != null) fadeImage.raycastTarget = true;

        // Fade Out (검게)
        yield return StartCoroutine(Fade(0f, 1f, fadeOutTime));

        if (blackHoldTime > 0f)
            yield return new WaitForSeconds(blackHoldTime);

        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade In (밝게)
        yield return StartCoroutine(Fade(1f, 0f, fadeInTime));

        // 페이드 완전히 끝났으므로 클릭 차단 해제!
        if (fadeImage != null) fadeImage.raycastTarget = false;

        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // 일시정지 영향을 받지 않도록 unscaled 사용
            c.a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}