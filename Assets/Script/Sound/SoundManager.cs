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
        for (int i = 0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]);
            }
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
        if (sfxPlayer.isPlaying) return;

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
}
