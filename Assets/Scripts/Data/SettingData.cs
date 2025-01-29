using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class AudioVolume
{
    //프로퍼티로 데이터 변경시 데이터 저장되게 처리
    
    [SerializeField] private float bgm;
    [SerializeField] private float sfx;

    public float Bgm
    {
        get => bgm;
        set
        {
            if (bgm != value)
            {
                bgm = value;
            }
        }
    }

    public float Sfx
    {
        get => sfx;
        set
        {
            if (sfx != value)
            {
                sfx = value;
            }
        }
    }

}


[Serializable]
public class SettingData
{
    public AudioVolume audioVolume = new AudioVolume();
    [JsonProperty("controls")] public Dictionary<string, string> controls = new Dictionary<string, string>(); 
    
}
