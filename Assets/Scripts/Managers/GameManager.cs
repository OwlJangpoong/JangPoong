using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager
{
    private bool isInit = false;
    public bool IsInit
    {
        get => isInit;
    }
    public SettingData _settingData;
    private InventoryData gameInventory;
    
    public InventoryData GameInventory
    {
        get
        {
            if (gameInventory == null)
            {
                LoadData(Define.SaveKey.InventoryData, out gameInventory);
            }

            return gameInventory;
        }

        set
        {
            Debug.Log("GameManager : Inventory 상태 업데이트 후 로컬 저장 요청");
            gameInventory = value;
            Managers.Data.SaveData(Define.SaveKey.InventoryData, gameInventory);
        }
    }
    
    
    // public PlayerData playerData;
    // public ProgressData progressData;
    // public StatisticData statisticData;
    
    
    //초기화 완료 이벤트
    public event Action OnDataLoaded;
    
    
    //Setting Data 불러오기
    //
    public void Init()
    {
        
        Debug.Log("GameManager Init 호출");
        LoadData(Define.SaveKey.SettingData, out _settingData);
        
        isInit = true;
        
        Debug.Log($"GameManager.Init called. OnInitialized is {(OnDataLoaded == null ? "null" : "not null")}");
        //데이터 읽어와서 초기화 완료됐음을 알림
        OnDataLoaded?.Invoke();
        Debug.Log("GameManager: Data loaded and OnDataLoaded invoked.");
        

    }


    public void LoadData<T>(Define.SaveKey dataType, out T dataVariable)
    {
        Debug.Log($"<color=green>SetData({dataType.ToString()}, {typeof(T)})호출 </color>");
        //1. 저장된 Setting 파일이 있는지 확인한다.
        //있으면 로드
        if (Managers.Data.HasSaveDataFile(dataType))
        {
            Debug.Log($"<color=red>로컬에 저장된 {dataType.ToString()} 데이터를 가져옵니다.</color>");
             dataVariable = Managers.Data.LoadData<T>(dataType);
        }
         
        //2. 없으면 초기화 파일 로드
        else
        {
            Debug.Log($"<color=red>로컬에 저장된 {dataType.ToString()} 데이터가 없습니다. 파일을 초기화합니다.</color>");
            dataVariable = Managers.Data.LoadInitData<T>(dataType);
            
            //초기화 후 저장 경로에 해당 데이터를 저장한다.
            Managers.Data.SaveData<T>(dataType, dataVariable);
        }
    }
    


    #region Comment

    // public void SetSettingData()
    // {
    //     //1. 저장된 Setting 파일이 있는지 확인한다.
    //     //있으면 로드
    //     if (Managers.Data.HasSaveDataFile(Define.SaveKey.SettingData))
    //     {
    //         Debug.Log("<color=red>로컬에 저장된 데이터를 가져옵니다.</color>");
    //         settingData = Managers.Data.LoadData<SettingData>(Define.SaveKey.SettingData);
    //     }
    //      
    //     //2. 없으면 초기화 파일 로드
    //     else
    //     {
    //         Debug.Log("<color=red>로컬에 저장된 데이터가 없습니다. 파일을 초기화합니다.</color>");
    //         settingData = Managers.Data.LoadInitData<SettingData>(Define.SaveKey.SettingData);
    //         
    //         //초기화 후 저장 경로에 해당 데이터를 저장한다.
    //         Managers.Data.SaveData<SettingData>(Define.SaveKey.SettingData, Managers.Game.settingData);
    //     }
    // }
    

    #endregion

    
    

}
