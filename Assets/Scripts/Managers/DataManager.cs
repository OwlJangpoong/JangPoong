using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;



public class DataManager
{
    //File/Folder Path for Save, Load
    public string folderPath;
    public string filePath;



    public void Init()
    {
        // GetData();
        // GetInventoryData();
        folderPath = Application.persistentDataPath;

    }

    public void GetData()
    {
        if (Managers.PlayerData == null) return;
        // 플레이어 데이터 가져오기
        if (PlayerPrefs.HasKey(Define.SaveKey.playerHp.ToString()))
        {
            float value = PlayerPrefs.GetFloat(Define.SaveKey.playerHp.ToString());
            Debug.Log($"PlayerPrefs.GetKey(playerHP) = {value}");
            Managers.PlayerData.Hp = value;
        }
        else
        {
            Debug.Log("playerHp playerprefs 키 없음.");
            // Managers.PlayerData.Hp = 10;
        }

        if (PlayerPrefs.HasKey(Define.SaveKey.playerMana.ToString()))
        {
            Managers.PlayerData.Mana = PlayerPrefs.GetInt(Define.SaveKey.playerMana.ToString());
        }
        else
        {
            Debug.Log("playerMana playerprefs 키 없음.");
            // Managers.PlayerData.Mana = 100;
        }

        if (PlayerPrefs.HasKey(Define.SaveKey.levelToken.ToString()))
        {
            Managers.PlayerData.LevelUpToken = PlayerPrefs.GetInt(Define.SaveKey.levelToken.ToString());
        }

        if (PlayerPrefs.HasKey(Define.SaveKey.monsterPoint.ToString()))
        {
            Managers.PlayerData.MonsterPoint = PlayerPrefs.GetInt(Define.SaveKey.monsterPoint.ToString());
        }
    }

    public void GetInventoryData()
    {
        if (Managers.Inventory == null) return;

        Managers.Inventory.hpSmallCnt = PlayerPrefs.GetInt(Define.SaveKey.hpPotionSmallCnt.ToString(), 0);
        Managers.Inventory.hpLargeCnt = PlayerPrefs.GetInt(Define.SaveKey.hpPotionLargeCnt.ToString(), 0);
        Managers.Inventory.mpSmallCnt = PlayerPrefs.GetInt(Define.SaveKey.mpPotionSmallCnt.ToString(), 0);
        Managers.Inventory.mpLargeCnt = PlayerPrefs.GetInt(Define.SaveKey.mpPotionLargeCnt.ToString(), 0);
        Managers.Inventory.invinsibilityCnt = PlayerPrefs.GetInt(Define.SaveKey.invisibilityPotionCnt.ToString(), 0);
    }

    // 데이터 저장하기
    public void SaveData()
    {
        //직렬화
        // string toJsonData = JsonConvert.SerializeObject(GameManager.Instance.playerData);
        //
        // filePath = Path.Combine(Application.persistentDataPath, playerDataFile);
        //
        // File.WriteAllText(filePath, toJsonData);
        // Debug.Log($"로컬에 playerData 저장 완료");

    }


    public void LoadData()
    {
        // filePath = Path.Combine(Application.persistentDataPath, playerDataFile);
        // Debug.Log(filePath);
        // if (File.Exists(filePath))
        // {
        //     string jsonString = File.ReadAllText(filePath);
        //     Debug.Log("플레이어 데이터 불러오기");
        //
        //     GameManager.Instance.playerData = JsonConvert.DeserializeObject<PlayerData>(jsonString);
        // }
        // else
        // {
        //     Debug.Log($"로컬에 저장된 데이터 없음. 초기화 메소드 호출");
        //     InitializePlayerData();
        // }
    }

    public void InitializeData()
    {
        // string initDataFolder = "InitData";
        // string path = Path.Combine(initDataFolder, Define.InitFileNames["afaf"]);
        // FileName enum에 있는 값들을 반복
        foreach (Define.FileName fileName in Enum.GetValues(typeof(Define.FileName)))
        {
            //1. init 파일 경로 지정
                // 각 enum 값을 문자열로 변환해서 출력 & Resouces 기준 상대 경로로 작성
                //초기화 데이터 파일 이름 : [fileName]_init.json
            string initFilePath = "Data/" + fileName.ToString() + "_init.json";
            
            
            //2. 파일 읽어오기
            TextAsset jsonFile = Resources.Load<TextAsset>(initFilePath);
            
            
            //3. deserialize
            if (jsonFile != null)
            {
                // Managers.Game.
                // GameManager.Instance.playerData = JsonConvert.DeserializeObject<PlayerData>(jsonFile.text);
                // SavePlayerData();
            }
            else
            {
                Debug.Log($"초기화 파일을 찾을 수 없습니다 : Resources/{initFilePath}");
            }
            
        }

    }

    public void Reset()
    {
        
        Util.DeleteAllFilesInFolder(folderPath);
        // InitializeData();
    }



    //
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
    // // 데이터 저장하기
    // public void SaveData()
    // {
    //     if (Managers.PlayerData == null)
    //     {
    //         return;
    //     }
    //
    //     PlayerPrefs.SetFloat(Define.SaveKey.playerHp.ToString(), Managers.PlayerData.Hp);
    //     PlayerPrefs.SetInt(Define.SaveKey.playerMana.ToString(), Managers.PlayerData.Mana);
    //     PlayerPrefs.SetInt(Define.SaveKey.levelToken.ToString(), Managers.PlayerData.LevelUpToken);
    //
    //     SaveInventoryData();
    // }
    //
    // // 인벤토리 데이터 저장하기
    // public void SaveInventoryData()
    // {
    //     if (Managers.Inventory == null) return;
    //
    //     PlayerPrefs.SetInt(Define.SaveKey.hpPotionSmallCnt.ToString(), Managers.Inventory.hpSmallCnt);
    //     PlayerPrefs.SetInt(Define.SaveKey.hpPotionLargeCnt.ToString(), Managers.Inventory.hpLargeCnt);
    //     PlayerPrefs.SetInt(Define.SaveKey.mpPotionSmallCnt.ToString(), Managers.Inventory.mpSmallCnt);
    //     PlayerPrefs.SetInt(Define.SaveKey.mpPotionLargeCnt.ToString(), Managers.Inventory.mpLargeCnt);
    //     PlayerPrefs.SetInt(Define.SaveKey.invisibilityPotionCnt.ToString(), Managers.Inventory.invinsibilityCnt);
    // }
    //
    //
    // // 인벤토리 초기화
    // public void ResetInventory()
    // {
    //     Managers.Inventory.hpSmallCnt = 0;
    //     Managers.Inventory.hpLargeCnt = 0;
    //     Managers.Inventory.mpSmallCnt = 0;
    //     Managers.Inventory.mpLargeCnt = 0;
    //     Managers.Inventory.invinsibilityCnt = 0;
    //
    //     SaveInventoryData();
    // }
}
