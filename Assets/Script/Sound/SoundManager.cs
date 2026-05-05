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
        instance = this;

        SetupSFXPlayer();
    }

    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
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

    public float GetBGSoundVolume()
    {
        float value;
        if (mixer.GetFloat("BGSoundVolume", out value))
        {
            // 공식: 10^(dB / 20)
            return Mathf.Pow(10, value / 20f);
        }
        return 0.5f;
    }

    public void SetBGSoundVolume(float val)
    {
        if (val <= 0.0001f)
        {
            mixer.SetFloat("BGSoundVolume", -80f);
        }
        else
        {
            float volume = Mathf.Log10(val) * 20;
            mixer.SetFloat("BGSoundVolume", Mathf.Clamp(volume, -80f, 0f));
        }
    }

    public float GetSFXVolume()
    {
        float value;
        if (mixer.GetFloat("SFXVolume", out value))
        {
            return Mathf.Pow(10, value / 20f);
        }
        return 0.5f;
    }

    public void SetSFXVolume(float val)
    {
        if (val <= 0.0001f)
        {
            mixer.SetFloat("SFXVolume", -80f);
        }
        else
        {
            float volume = Mathf.Log10(val) * 20;
            mixer.SetFloat("SFXVolume", Mathf.Clamp(volume, -80f, 0f));
        }
    }
}
