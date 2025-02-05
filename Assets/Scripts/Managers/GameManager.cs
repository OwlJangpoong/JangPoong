using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager
{
    // private bool isInit = false;
    // public bool IsInit
    // {
    //     get => isInit;
    // }

    #region 게임 관련 저장 데이터 클래스 변수 & 프로퍼티
    
    private SettingData _setting;
    private InventoryData _gameInventory;
    private PlayerData _player;
    
    public SettingData Setting
    {
        get
        {
            if (_setting == null)
            {
                LoadData(Define.SaveKey.SettingData, out _setting);
            }

            return _setting;
        }
    }
    
    public InventoryData GameInventory
    {
        get
        {
            if (_gameInventory == null)
            {
                LoadData(Define.SaveKey.InventoryData, out _gameInventory);
            }

            return _gameInventory;
        }

        set
        {
            Debug.Log("GameManager : Inventory 상태 업데이트 후 로컬 저장 요청");
            _gameInventory = value;
            Managers.Data.SaveData(Define.SaveKey.InventoryData, _gameInventory);
        }
    }

    public PlayerData Player
    {
        get
        {
            if (_player == null)
            {
                LoadData(Define.SaveKey.PlayerData,out _player);
            }

            return _player;
        }
        set
        {
            Debug.Log("GameManager : Player 데이터 업데이트 후 로컬 저장 요청");
            _player = value;
            Managers.Data.SaveData(Define.SaveKey.PlayerData, _player);
        }
    }
    
    // public ProgressData progressData;
    // public StatisticData statisticData;

    #endregion
    
    

    

    
    public void Init()
    {
        Debug.Log("GameManager Init 호출");

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
    
    
    //초기 Init() 
    // public void Init()
    // {
    //     
    //     Debug.Log("GameManager Init 호출");
    //     
    //     isInit = true;
    //     
    //     Debug.Log($"GameManager.Init called. OnInitialized is {(OnDataLoaded == null ? "null" : "not null")}");
    //     //데이터 읽어와서 초기화 완료됐음을 알림
    //     OnDataLoaded?.Invoke();
    //     Debug.Log("GameManager: Data loaded and OnDataLoaded invoked.");
    //     
    //
    // }

    
    
    

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
