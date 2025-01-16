using System;
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
                Debug.Log("bgm 사운드 데이터 변경");
                OnDataChanged?.Invoke(Define.SaveKey.SettingData, Managers.Game.settingData);
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
                Debug.Log("sfx 사운드 데이터 변경");
                OnDataChanged?.Invoke(Define.SaveKey.SettingData, Managers.Game.settingData);
            }
        }
    }

    public event Action<Define.SaveKey,SettingData> OnDataChanged;
}

[Serializable]
public class Controls
{
    
}


public class SettingData
{
    public AudioVolume audioVolume = new AudioVolume();
    public Controls constrols = new Controls();

    // public event Action OnDataChanged;
    
    //생성자
    public SettingData()
    {
        audioVolume.OnDataChanged += (saveKey, data) =>
        {
            Debug.Log("OnDataChanged event triggered");
            Managers.Data.SaveData(saveKey, data);
        };
    }
    
}
