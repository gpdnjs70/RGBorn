using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    public static bool IsPaused { get; private set; } = false;

    [Header("Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                if (optionGroup.alpha > 0)
                {
                    CloseSoundSetting();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    // 게임으로 돌아가기
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 시간 정상화
        IsPaused = false;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PauseAllSFX(false);
        }
    }

    // 게임 일시 정지
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        CanvasGroupOn(mainGroup);
        CanvasGroupOff(optionGroup);

        Time.timeScale = 0f;
        IsPaused = true;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PauseAllSFX(true);
        }
    }

    public void SoundSetting()
    {
        if (SoundManager.instance != null)
        {
            bgmSlider.value = SoundManager.instance.GetBGSoundVolume();
            sfxSlider.value = SoundManager.instance.GetSFXVolume();
        }

        CanvasGroupOn(optionGroup);
        CanvasGroupOff(mainGroup);
    }

    public void CloseSoundSetting()
    {
        CanvasGroupOff(optionGroup);
        CanvasGroupOn(mainGroup);
    }

    // 메인 메뉴로 이동 버튼용 메서드
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 중요: 씬을 옮기기 전에 시간을 다시 흐르게 해야 함

        if (SaveManager.Instance != null)
        {
            // 1. 기존 데이터를 불러옵니다.
            SaveData currentData = SaveManager.Instance.LoadGame();

            if (currentData != null)
            {
                // 2. 씬 이름은 그대로 두고(이미 저장된 안전구역), 리스폰 여부만 true로 바꿉니다.
                currentData.isRespawn = true;

                // 3. 이 수정된 데이터를 통째로 넘겨서 저장합니다.
                // targetSceneName에 currentData.lastSceneName을 넣어주면 씬 이름이 갱신되지 않습니다.
                SaveManager.Instance.SaveGame(true, currentData.lastSceneName, currentData);
            }
            else
            {
                // 데이터가 아예 없다면 기본 저장
                SaveManager.Instance.SaveGame(true);
            }
        }

        if (FadeTransition.Instance != null)
        {
            FadeTransition.Instance.FadeToScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
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
}
