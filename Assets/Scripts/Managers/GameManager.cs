using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager
{
    private bool isInit = false;
    public bool IsInit
    {
        get => isInit;
    }
    public SettingData settingData;
    // public PlayerData playerData;
    // public ProgressData progressData;
    // public StatisticData statisticData;
    
    
    //초기화 완료 이벤트
    public event Action OnDataLoaded;
    
    public void Init()
    {
        
        Debug.Log("GameManager Init 호출");
        settingData = new SettingData();
        
        //1. 저장된 Setting 파일이 있는지 확인한다.
        //있으면 로드
        if (Managers.Data.HasSaveDataFile(Define.SaveKey.SettingData))
        {
            Debug.Log("<color=red>로컬에 저장된 데이터를 가져옵니다.</color>");
            Managers.Game.settingData = Managers.Data.LoadData<SettingData>(Define.SaveKey.SettingData);
        }
         
        //2. 없으면 초기화 파일 로드
        else
        {
            Debug.Log("<color=red>로컬에 저장된 데이터가 없습니다. 파일을 초기화합니다.</color>");
            Managers.Game.settingData = Managers.Data.LoadInitData<SettingData>(Define.SaveKey.SettingData);
            
            //초기화 후 저장 경로에 해당 데이터를 저장한다.
            Managers.Data.SaveData<SettingData>(Define.SaveKey.SettingData, Managers.Game.settingData);
        }

        isInit = true;
        
        Debug.Log($"GameManager.Init called. OnInitialized is {(OnDataLoaded == null ? "null" : "not null")}");
        //데이터 읽어와서 초기화 완료됐음을 알림
        OnDataLoaded?.Invoke();
        Debug.Log("GameManager: Data loaded and OnDataLoaded invoked.");
        
        
    }






}
