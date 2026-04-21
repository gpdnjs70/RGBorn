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

    private void Start()
    {
        if (cutsceneScenes != null && cutsceneScenes.Length > 0)
        {
            StartCoroutine(PlayCutscene());
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
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // 다음 씬 이름이 없으면 컷씬 UI만 비활성화
            gameObject.SetActive(false);
        }
    }
}
