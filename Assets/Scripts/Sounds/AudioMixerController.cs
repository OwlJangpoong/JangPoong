using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        audioMixer = Managers.Sound.audioMixer;

        bgmSlider.value = Managers.Game.settingData.audioVolume.Bgm;
        sfxSlider.value = Managers.Game.settingData.audioVolume.Sfx;
            
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void SetBgmVolume (float volume)
    {
       Managers.Sound.SetBgmVolume(volume);
    }

    private void SetSfxVolume(float volume)
    {
        Managers.Sound.SetSfxVolume(volume);
    }
}
