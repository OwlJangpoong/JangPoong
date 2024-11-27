using UnityEngine;

public class DataManager
{
    public void Init()
    {
        GetData();
        GetInventoryData();
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
        if (Managers.PlayerData == null)
        {
            return;
        }

        PlayerPrefs.SetFloat(Define.SaveKey.playerHp.ToString(), Managers.PlayerData.Hp);
        PlayerPrefs.SetInt(Define.SaveKey.playerMana.ToString(), Managers.PlayerData.Mana);
        PlayerPrefs.SetInt(Define.SaveKey.levelToken.ToString(), Managers.PlayerData.LevelUpToken);

        SaveInventoryData();
    }

    // 인벤토리 데이터 저장하기
    public void SaveInventoryData()
    {
        if (Managers.Inventory == null) return;

        PlayerPrefs.SetInt(Define.SaveKey.hpPotionSmallCnt.ToString(), Managers.Inventory.hpSmallCnt);
        PlayerPrefs.SetInt(Define.SaveKey.hpPotionLargeCnt.ToString(), Managers.Inventory.hpLargeCnt);
        PlayerPrefs.SetInt(Define.SaveKey.mpPotionSmallCnt.ToString(), Managers.Inventory.mpSmallCnt);
        PlayerPrefs.SetInt(Define.SaveKey.mpPotionLargeCnt.ToString(), Managers.Inventory.mpLargeCnt);
        PlayerPrefs.SetInt(Define.SaveKey.invisibilityPotionCnt.ToString(), Managers.Inventory.invinsibilityCnt);
    }


    // 인벤토리 초기화
    public void ResetInventory()
    {
        Managers.Inventory.hpSmallCnt = 0;
        Managers.Inventory.hpLargeCnt = 0;
        Managers.Inventory.mpSmallCnt = 0;
        Managers.Inventory.mpLargeCnt = 0;
        Managers.Inventory.invinsibilityCnt = 0;

        SaveInventoryData();
    }
}
