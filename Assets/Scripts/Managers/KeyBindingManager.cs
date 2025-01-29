using UnityEngine;
using System;

public class KeyBindingManager
{
    
    public KeyBindingResult SetKeyBinding(Define.ControlKey controlKey, KeyCode newKeyCode)
    {
        
        //<키 검사>
        if (newKeyCode == KeyCode.None)
        {
            Debug.Log("잘못된 키 입니다.");
            return new KeyBindingResult(false, 0);
        }
        
        
        //이미 다른 키에 설정되어 있는지 확인한다.
        foreach (var key in Managers.Game._settingData.controls.Values)
        {
            if (Enum.TryParse<KeyCode>(key, out KeyCode existingKey) && existingKey == newKeyCode)
            {
                Debug.Log("중복된 키입니다.");
                return new KeyBindingResult(false, 1);
                //리턴할 때 어떤 오륜지 넘기기
            }
        }
        
        //기존과 동일한 키인지 확인
        if (GetKeyCode(controlKey) == newKeyCode)
        {
            Debug.Log("이미 설정된 키입니다. 바꿀 필요 없음");
            return new KeyBindingResult(true);
        }
        
        //저장
        string keyString = controlKey.ToString();
        Managers.Game._settingData.controls[keyString] = newKeyCode.ToString();
        
        SaveKeyBinding();
        return new KeyBindingResult(true);
        
    }
    
    
    
    /// <summary>
    /// Save GameManager's Setting Data into Setting.json
    /// </summary>
    public void SaveKeyBinding()
    {
        Managers.Data.SaveData(Define.SaveKey.SettingData, Managers.Game._settingData);
        Debug.Log($"Save SettingData");
    }

    

    /// <summary>
    /// Define.ControlKey의 enum의 문자열 값을 키로하여 GameManager에 저장된 KeyCode를 반환하는 메소드.SettingData.contorls 딕셔너리에 string 형태로 저장된 KeyCode 데이터를 KeyCode 형태로 전환하여 반환한다.
    /// </summary>
    /// <param name="controlKey"></param>
    /// <returns></returns>
    public KeyCode GetKeyCode(Define.ControlKey controlKey)
    {
        string keyString = controlKey.ToString();
        // 저장된 KeyCode 문자열을 가져오기
        if (Managers.Game._settingData.controls.TryGetValue(keyString, out string keyName))
        {
            if (Enum.TryParse<KeyCode>(keyName, out KeyCode keyCode))
            {
                return keyCode;
            }
        }

        // KeyCode가 없을 경우 기본값 반환 (필요 시 수정)
        Debug.LogWarning($"Fail to load {keyString} KeyCode");
        return KeyCode.None;

    }


    #region Comment
    // private KeyCode GetKeyCodeFromName(string keyName)
    // {
    //     KeyCode keyCode;
    //     
    //     // 알파벳 키라면 대문자로 변환 (알파멧 키만 대문자로 변환해야함! 다른 키는 변환하면 parsing 실패함)
    //     if (keyName.Length == 1 && char.IsLetter(keyName[0]))
    //     {
    //         keyName = keyName.ToUpper();
    //     }
    //     if (Enum.TryParse(keyName, out keyCode))
    //     {
    //         if (Enum.IsDefined(typeof(KeyCode), keyCode))
    //         {
    //             return keyCode;
    //         }
    //             Debug.LogWarning("Enum.isDefined 오류");
    //             return KeyCode.None;
    //         
    //     }
    //     Debug.LogWarning($"Enum Parse 실패. Invalid key name: {keyName}. Defaulting to KeyCode.None.");
    //     return KeyCode.None;
    //     
    // }
    
    
    

    #endregion
}
