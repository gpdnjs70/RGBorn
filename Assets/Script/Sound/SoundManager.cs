using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bglist;
    private AudioSource sfxPlayer;

    public static SoundManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;

            SetupSFXPlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        // 현재 씬의 빌드 인덱스(Build Settings에 등록된 순서)를 가져옵니다.
        int sceneIndex = arg0.buildIndex;

        // 만약 빌드 인덱스에 맞는 음악이 bglist에 있다면 바로 재생
        if (sceneIndex < bglist.Length && bglist[sceneIndex] != null)
        {
            BgSoundPlay(bglist[sceneIndex]);
        }
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }

    private void SetupSFXPlayer()
    {
        if (sfxPlayer == null)
        {
            sfxPlayer = gameObject.AddComponent<AudioSource>();
            // 믹서 그룹 재확인
            if (mixer != null)
            {
                AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
                if (groups.Length > 0) sfxPlayer.outputAudioMixerGroup = groups[0];
            }
            sfxPlayer.playOnAwake = false;
        }
    }

    public void PlayVariableSFX(AudioClip clip)
    {
        if (clip == null || sfxPlayer == null) return;
        if (sfxPlayer.isPlaying)
        {
            if (sfxPlayer.clip == clip) return;

            // 바뀌었다면 기존 소리 중지
            sfxPlayer.Stop();
        }

        sfxPlayer.clip = clip;
        sfxPlayer.Play();
    }

    public void StopVariableSFX()
    {
        if (sfxPlayer != null)
        {
            sfxPlayer.Stop(); // 현재 재생 중인 타이핑 소리를 모두 즉시 중단
        }
    }

    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();
    }

    public void SetBGSoundVolume(float val)
    {
        if (val <= 0.0001f)
        {
            mixer.SetFloat("BGSoundVolume", -80f);
        }
        else
        {
            // Mathf.Log10(val) * 20은 0dB 기준입니다. 
            // 여기에 20을 더하면 슬라이더 1일 때 20dB, 슬라이더 0.1일 때 0dB가 됩니다.
            float volume = Mathf.Log10(val) * 20 + 20;

            // 믹서의 최댓값인 20을 넘지 않도록 제한
            mixer.SetFloat("BGSoundVolume", Mathf.Clamp(volume, -80f, 20f));
        }
    }

    public void SetSFXVolume(float val)
    {
        if (val <= 0.0001f)
        {
            mixer.SetFloat("SFXVolume", -80f);
        }
        else
        {
            float volume = Mathf.Log10(val) * 20 + 20;
            mixer.SetFloat("SFXVolume", Mathf.Clamp(volume, -80f, 20f));
        }
    }
}
