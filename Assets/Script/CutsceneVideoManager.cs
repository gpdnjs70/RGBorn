using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "TutorialScene_HouseInside";

    private void Awake()
    {
        // 영상 끝났을 때 호출될 함수 등록
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDestroy()
    {
        // 씬 전환 시 메모리 정리 (권장)
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}