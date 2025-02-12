using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class DataManager
{
    //File/Folder Path for Save, Load
    private int slotNum = 1;
    public int SlotNum { get => slotNum; set => slotNum = value; }
    private int totalSlots = 3;
    private string saveFolderPath=""; //  "SaveData" 폴더까지의 경로

    private string slotFolderPath=""; // "SaveData/(슬롯이름)" 폴더 아래에 데이터 파일 저장됨
    
    //초기화
    public void Init ()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, "SaveData");
        // SetSlotNum(1);
        
        //"SaveData" 폴더가 있는지 확인.
        //1. 없으면 폴더 생성
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.Log("SaveData 폴더가 없습니다. 폴더를 생성합니다.");
            Directory.CreateDirectory(saveFolderPath);
            
            // Debug.Log("SettingData 파일을 초기화합니다.");
            // //초기화 파일을 가져와서 GameManager에 넣어준다.
            // Managers.Game.SettingData = LoadInitData<SettingData>(Define.SaveKey.SettingData);
            //
            // //초기화 후 저장 경로에 해당 데이터를 저장한다.
            // SaveData<SettingData>(Define.SaveKey.SettingData, Managers.Game.SettingData);
        }
        // 2. 있으면 setting 데이터 파일이 존재할 것. setting 데이터 파일을 로드한다.
        
    }

  

    /// <summary>
    /// Loads data from JSON file and converts it to the specified class type.
    /// </summary>
    /// <param name="saveKey">The key of dictionary that have JSON file's names as value, which is to load.</param>
    /// <typeparam name="T">The class type to which the data will be converted</typeparam>
    /// <returns>The data converted to the specified class type, or default(T) if the file is not found.</returns>
    public T LoadData<T>(Define.SaveKey saveKey)
    {
        
        //1. 파일 경로 설정
        string path;
        string fileName = Define.FileNames[saveKey.ToString()];
        
        //setting 파일만 경로가 다르기 때문에 saveKey가 SettingData인지 확인.
        //setting 파일 -> saveFolderPath / 나머지 파일 -> slotFolderPath
        if (saveKey == Define.SaveKey.SettingData)
        {
            
            path = Path.Combine(saveFolderPath, fileName);
        }
        else
        {
            if (!string.IsNullOrEmpty(slotFolderPath))
            {
                path = Path.Combine(slotFolderPath, fileName);
            }
            else
            {
                    Debug.LogWarning($"Can't Find Desired Slot Folder");
                    return default;
            }
        }

        //2. 파일이 있는지 확인
        if (File.Exists(path))
        {
            //3. 파일 있으면 json 파일 읽기
            string json = File.ReadAllText(path);
            //4. 해당하는 클래스 형식으로 전환하기 & 반환
            return JsonConvert.DeserializeObject<T>(json);
        }

        //5. 파일 없으면 오류!
        Debug.LogWarning($"File not found : {path}");
        return default;
        
    }

    /// <summary>
    /// Saves the specified data to a JSON file. If the file is already exist, it will be overwritten.
    /// </summary>
    /// <param name="fileName">The name of the JSON file to save.</param>
    /// <param name="data">The data to save as JSON</param>
    /// <typeparam name="T">The class type of the data to save.</typeparam>
    public void SaveData<T>(Define.SaveKey saveKey, T data)
    {
        Debug.Log("!!!!!!!데이터 저장 호출");
        //1. 저장 경로 설정
        string fileName = Define.FileNames[saveKey.ToString()];

        string path;
        if (saveKey == Define.SaveKey.SettingData)
        {
            path = Path.Combine(saveFolderPath, fileName);
        }
        else
        {
            if (!string.IsNullOrEmpty(slotFolderPath))
            {
                path = Path.Combine(slotFolderPath, fileName);
            }
            else
            {
                Debug.LogWarning($"Can't Find Desired Slot Folder");
                return;
            }
        }
        
        
        //2. 클래스를 Json 형식으로 전환
        string json = JsonConvert.SerializeObject(data, Formatting.Indented); //들여쓰기, 줄바꿈 적용된 형태로 저장(유지보수 위해)

        //3. 이미 파일이 존재하고 있다면 덮어쓰기, 없다면 새로쓰기
        File.WriteAllText(path,json);
        
        //4. 저장됐는지 확인(생략가능)
        Debug.Log($"{fileName} file saved successfully at {path}");
    }
    

    public T LoadInitData<T>(Define.SaveKey saveKey)
    {
        //1. 파일 경로를 설정한다.
        string fileName = Define.FileNames[saveKey.ToString()];
        string path = Path.Combine("Data", "InitData", fileName);
        
        //2. 파일을 가져온다.
        //초기화 파일은 Resource/Data/InitData에 있으므로 Managers.Resource(리소스 매니져)를 사용해서 리소스를 로드한다.
        TextAsset initJsonFile = Managers.Resource.Load<TextAsset>(path);

        //3. 파일이 정상적으로 로드되었는지 체크한다.
        if (initJsonFile == null)
        {
            Debug.LogWarning($"Failed to load JSON file from Resources : {path}");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
           
        }

        //4. 로드한 파일을 읽어서 클래스에 맞게 전환해서 반환한다.
        string json = initJsonFile.text;
        return JsonConvert.DeserializeObject<T>(json);

    }

    public bool HasSaveDataFile(Define.SaveKey saveKey)
    {
        string fileName = Define.FileNames[saveKey.ToString()];
        string path;
        if (saveKey == Define.SaveKey.SettingData)
        {
            path = Path.Combine(saveFolderPath, fileName);
        }
        else
        {
            path = Path.Combine(slotFolderPath, fileName);
        }

        return File.Exists(path);
    }





    #region Slot
    
    public void SetSlotNum(int slotNum)
    {
        this.slotNum = slotNum;
        string slot = "Slot" + slotNum;
        
        Debug.Log($"Current Slot Num : {this.slotNum}");
        
        slotFolderPath = Path.Combine(Application.persistentDataPath, "SaveData", slot);
        if (!Directory.Exists(slotFolderPath))
        {
            Debug.Log($"{slot} 폴더가 없습니다. 폴더를 생성합니다.");
            Directory.CreateDirectory(slotFolderPath);
        }

    }
    
    
    

    public void DeleteAllFilesInSlot()
    {
        Debug.Log(Util.DeleteAllFilesInFolder(slotFolderPath)?"슬롯 초기화 성공":"슬롯 초기화 실패");
    }
    
    public (bool hasInfo, string playerName, string lastPlayTime, string currentStag) GetSlotInfo(int slotNumber)
    {
        SetSlotNum(slotNumber);
        if (!Directory.Exists(slotFolderPath) || !(HasSaveDataFile(Define.SaveKey.PlayerData)&&HasSaveDataFile(Define.SaveKey.StatisticData)))
        {
            return (false, "", "", ""); // 슬롯이 비어 있는 경우
        }
        
        string playerName = "Unknown";
        string currentStage = "Stage1";
        string lastPlayTime = "N/A";
        
        PlayerData playerData = LoadData<PlayerData>(Define.SaveKey.PlayerData);
        StatisticData statisticData = LoadData<StatisticData>(Define.SaveKey.StatisticData);
        
        playerName = playerData.playerName;
        currentStage = playerData.currentStage;
        lastPlayTime = statisticData.lastPlayTime;
        
        // //플레이어 이름
        // if (HasSaveDataFile(Define.SaveKey.PlayerData))
        // {
        //     
        //
        // }
        //
        // if (HasSaveDataFile(Define.SaveKey.StatisticData))
        // {
        //     
        // }

        return (true, playerName, lastPlayTime, currentStage);


    }

    #endregion


    #region comment

    //
    // public void Init()
    // {
    //     // GetData();
    //     // GetInventoryData();
    //     folderPath = Application.persistentDataPath;
    //
    // }

    // public void GetData()
    // {
    //     if (Managers.PlayerData == null) return;
    //     // 플레이어 데이터 가져오기
    //     if (PlayerPrefs.HasKey(Define.SaveKey.playerHp.ToString()))
    //     {
    //         float value = PlayerPrefs.GetFloat(Define.SaveKey.playerHp.ToString());
    //         Debug.Log($"PlayerPrefs.GetKey(playerHP) = {value}");
    //         Managers.PlayerData.Hp = value;
    //     }
    //     else
    //     {
    //         Debug.Log("playerHp playerprefs 키 없음.");
    //         // Managers.PlayerData.Hp = 10;
    //     }
    //
    //     if (PlayerPrefs.HasKey(Define.SaveKey.playerMana.ToString()))
    //     {
    //         Managers.PlayerData.Mana = PlayerPrefs.GetInt(Define.SaveKey.playerMana.ToString());
    //     }
    //     else
    //     {
    //         Debug.Log("playerMana playerprefs 키 없음.");
    //         // Managers.PlayerData.Mana = 100;
    //     }
    //
    //     if (PlayerPrefs.HasKey(Define.SaveKey.levelToken.ToString()))
    //     {
    //         Managers.PlayerData.LevelUpToken = PlayerPrefs.GetInt(Define.SaveKey.levelToken.ToString());
    //     }
    //
    //     if (PlayerPrefs.HasKey(Define.SaveKey.monsterPoint.ToString()))
    //     {
    //         Managers.PlayerData.MonsterPoint = PlayerPrefs.GetInt(Define.SaveKey.monsterPoint.ToString());
    //     }
    // }
    //
    // public void GetInventoryData()
    // {
    //     if (Managers.Inventory == null) return;
    //
    //     Managers.Inventory.hpSmallCnt = PlayerPrefs.GetInt(Define.SaveKey.hpPotionSmallCnt.ToString(), 0);
    //     Managers.Inventory.hpLargeCnt = PlayerPrefs.GetInt(Define.SaveKey.hpPotionLargeCnt.ToString(), 0);
    //     Managers.Inventory.mpSmallCnt = PlayerPrefs.GetInt(Define.SaveKey.mpPotionSmallCnt.ToString(), 0);
    //     Managers.Inventory.mpLargeCnt = PlayerPrefs.GetInt(Define.SaveKey.mpPotionLargeCnt.ToString(), 0);
    //     Managers.Inventory.invinsibilityCnt = PlayerPrefs.GetInt(Define.SaveKey.invisibilityPotionCnt.ToString(), 0);
    // }



    //
    // public void InitializeData()
    // {
    //     // string initDataFolder = "InitData";
    //     // string path = Path.Combine(initDataFolder, Define.InitFileNames["afaf"]);
    //     // FileName enum에 있는 값들을 반복
    //     foreach (Define.FileName fileName in Enum.GetValues(typeof(Define.FileName)))
    //     {
    //         //1. init 파일 경로 지정
    //             // 각 enum 값을 문자열로 변환해서 출력 & Resouces 기준 상대 경로로 작성
    //             //초기화 데이터 파일 이름 : [fileName]_init.json
    //         string initFilePath = "Data/" + fileName.ToString() + "_init.json";
    //         
    //         
    //         //2. 파일 읽어오기
    //         TextAsset jsonFile = Resources.Load<TextAsset>(initFilePath);
    //         
    //         
    //         //3. deserialize
    //         if (jsonFile != null)
    //         {
    //             // Managers.Game.
    //             // GameManager.Instance.playerData = JsonConvert.DeserializeObject<PlayerData>(jsonFile.text);
    //             // SavePlayerData();
    //         }
    //         else
    //         {
    //             Debug.Log($"초기화 파일을 찾을 수 없습니다 : Resources/{initFilePath}");
    //         }
    //         
    //     }
    //
    // }

    // public void Reset()
    // {
    //     
    //     Util.DeleteAllFilesInFolder(saveFolderPath);
    //     // InitializeData();
    // }

    #endregion
    
    
}
