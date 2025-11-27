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

    
    float epsilon = 0.0001f;
    
    
    private void Start()
    {
        audioMixer = Managers.Sound.audioMixer;

        bgmSlider.value = Managers.Game.Setting.audioVolume.Bgm;
        sfxSlider.value = Managers.Game.Setting.audioVolume.Sfx;
            
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void SetBgmVolume (float volume)
    { 
        Debug.Log("슬라이드 값 변경에 따른 이벤트 호출");
        if (Mathf.Abs(volume - Managers.Game.Setting.audioVolume.Bgm) > epsilon)
        {
            Managers.Sound.SetBgmVolume(volume);
            Managers.Data.SaveData(Define.SaveKey.SettingData, Managers.Game.Setting);
        }
            
    }

    private void SetSfxVolume(float volume)
    {
        
        if (Mathf.Abs(volume - Managers.Game.Setting.audioVolume.Sfx) > epsilon)
        {
            Managers.Sound.SetSfxVolume(volume);
            Managers.Data.SaveData(Define.SaveKey.SettingData, Managers.Game.Setting);
        }
            
    }
}
