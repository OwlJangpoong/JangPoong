using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bgm : MonoBehaviour
{
    [SerializeField] private AudioClip bgmAudioClip;

    // private void Start()
    // {
    //     if (bgmAudioClip == null)
    //     {
    //         Debug.Log("No BGM Audio Clip!!");
    //         return;
    //     }
    //     Managers.Sound.Play(bgmAudioClip, Define.Sound.Bgm);
    // }
    private void Start()
    {
        if (!Managers.IsInitialized)
        {
            Debug.LogWarning("Managers가 아직 초기화되지 않았습니다. 초기화 시도.");
            Managers.Sound.ToString(); // 강제로 접근하여 초기화 유도
        }

        if (bgmAudioClip == null)
        {
            Debug.LogError("No BGM Audio Clip!!");
            return;
        }

        Managers.Sound.Play(bgmAudioClip, Define.Sound.Bgm);
    }

    
}
