using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager
{
    public SettingData settingData;
    // public PlayerData playerData;
    // public ProgressData progressData;
    // public StatisticData statisticData;
    
    public void Init()
    {
        settingData = new SettingData();
        
        //1. 저장된 Setting 파일이 있는지 확인한다.
        //있으면 로드
        if (Managers.Data.HasSaveDataFile(Define.SaveKey.SettingData))
        {
            Managers.Game.settingData = Managers.Data.LoadData<SettingData>(Define.SaveKey.SettingData);
        }
         
        //2. 없으면 초기화 파일 로드
        else
        {
            Managers.Game.settingData = Managers.Data.LoadInitData<SettingData>(Define.SaveKey.SettingData);
            
            //초기화 후 저장 경로에 해당 데이터를 저장한다.
            Managers.Data.SaveData<SettingData>(Define.SaveKey.SettingData, Managers.Game.settingData);
        }
        
        

    }






}
