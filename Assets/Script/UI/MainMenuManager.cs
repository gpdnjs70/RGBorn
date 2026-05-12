using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (SoundManager.instance != null)
        {
            bgmSlider.value = SoundManager.instance.GetBGSoundVolume();
            sfxSlider.value = SoundManager.instance.GetSFXVolume();
        }
    }
}
