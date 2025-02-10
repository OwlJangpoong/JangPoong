using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager
{
    public bool isSessionStart = false;
    private int cnt = 0;
    

    #region 게임 관련 저장 데이터 클래스 변수 & 프로퍼티
    
    private SettingData _setting;
    private InventoryData _gameInventory;
    private PlayerData _player;
    private StatisticData _statistic;

    private DateTime sessionStartTime;
    private float sessionElapsedTime = 0;
    
    
    
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
    public StatisticData Statistic
    {
        get
        {
            if (_statistic == null)
            {
                LoadStatisticData();
            }
            return _statistic;
        }
    }

    #endregion
    
    
    
    
    public void Init()
    {
        Debug.Log("GameManager Init 호출");
        
        

    }
    


    #region StatisticData Control

    public void LoadStatisticData()
    {
        Debug.Log("load statistic data 호출");
        
        //데이터 로드
        LoadData(Define.SaveKey.StatisticData, out _statistic);
        
        //세션 시간 초기화
        sessionStartTime = DateTime.Now;
        sessionElapsedTime = 0;
        
        //마지막 플레이 시간 업데이트
        UpdateLastPlayTime();
        
        // 데이터 저장 (초기화된 경우 대비)
        SaveStatisticData();
        
        // 세션 시작 플래그 설정 (누적 플레이 시간 업데이트 활성화)
        isSessionStart = true;
        
        // 자동 저장 코루틴 실행 (CoroutineRunner 사용)
        CoroutineRunner.Instance.StartCoroutine(AutoSaveCoroutine());
        
    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(120f); // 2분마다 저장
            SaveStatisticData();
        }
    }
    
    public void SaveStatisticData()
    {
        Debug.Log($"통계 데이터 저장합니다. : {++cnt}회");

        // 현재까지 플레이한 시간 계산 (이전 sessionStartTime 기준)
        float elapsedSinceLastSave = (float)(DateTime.Now - sessionStartTime).TotalSeconds;
    
        // 누적 플레이 시간 업데이트
        _statistic.totalPlayTime += elapsedSinceLastSave;

        // 마지막 플레이 시간 갱신
        UpdateLastPlayTime();

        // 세션 시작 시간 갱신 (새로운 저장 주기 시작)
        sessionStartTime = DateTime.Now;

        // 저장
        Managers.Data.SaveData(Define.SaveKey.StatisticData, _statistic);
    }


    private void UpdateLastPlayTime()
    {
        _statistic.lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd tt hh:mm:ss");
    }
    
    
    #endregion
    
    
    

    #region Load/Save Interface

    public void LoadData<T>(Define.SaveKey dataType, out T dataVariable)
    {
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

    #endregion



    #region session

    public void PauseSession()
    {
        Debug.Log("게임 플레이 일시 중지!");
        isSessionStart = false;
        CoroutineRunner.Instance.StopAllRunningCoroutines();
        SaveStatisticData(); // 현재까지의 플레이 시간 저장
    }
    
    public void ResumeSession()
    {
        Debug.Log("게임 플레이 재개!");
        isSessionStart = true;
        sessionStartTime = DateTime.Now; // 세션 시간 다시 초기화
        CoroutineRunner.Instance.StartCoroutine(AutoSaveCoroutine());
    }
    
    public void EndSession()
    {
        Debug.Log("게임 세션 종료!");
        SaveStatisticData();
        isSessionStart = false;
        
        CoroutineRunner.Instance.StopAllRunningCoroutines();
    }


    #endregion

    
    

}
