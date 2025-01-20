using UnityEngine;
using System;

public class KeyBindingManager
{
    private KeyCode GetKeyCodeFromName(string keyName)
    {
        KeyCode keyCode;
        string keyNameUpper = keyName.ToUpper();
        if (Enum.TryParse(keyNameUpper, out keyCode))
        {
            if (Enum.IsDefined(typeof(KeyCode), keyCode))
            {
                return keyCode;
            }
                
     
                Debug.LogWarning("Enum.isDefined오류오류");
                return KeyCode.None;
            
        }
        Debug.LogWarning("Enum Parse 실패!!!!!!!!1");
        Debug.LogWarning($"Invalid key name: {keyName}. Defaulting to KeyCode.None.");
        return KeyCode.None;
        
    }
    
    
    
    
    public bool SetKeyBinding(Define.ControlKey controlKey, string keyCodeName)
    {
        //키 검사
        //1.string을 KeyCode로 변환한다. 유효한 키코드인지 확인한다.
        KeyCode newKeyCode = GetKeyCodeFromName(keyCodeName);
        
        //2. 새로운 키 코드가 KeyCode.None이 아니고, 저장된 키 코드와 비교해서 같은 키면 그냥 return, 다른 키면 변경 & 저장해준다.
        if (newKeyCode != KeyCode.None && newKeyCode!=GetKeyCode(controlKey))
        {
            string keyString = controlKey.ToString();
            Managers.Game.settingData.controls[keyString] = keyCodeName;
        
            //SettingData 저장
            SaveKeyBinding();

            return true;

        }

        else
        {
            Debug.Log("동일한 키이거나 잘못된 키 입니다.");
            return false;
        }
        
    }
    
    
    
    /// <summary>
    /// Save GameManager's Setting Data into Setting.json
    /// </summary>
    public void SaveKeyBinding()
    {
        Managers.Data.SaveData(Define.SaveKey.SettingData, Managers.Game.settingData);
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
        if (Managers.Game.settingData.controls.TryGetValue(keyString, out string keyCodeString))
        {
            Debug.Log(keyCodeString);
            // 문자열을 KeyCode로 변환
            return GetKeyCodeFromName(keyCodeString);
            // if (Enum.TryParse(typeof(KeyCode), keyCodeString, out object keyCode))
            // {
            //     return (KeyCode)keyCode;
            // }
        }

        // KeyCode가 없을 경우 기본값 반환 (필요 시 수정)
        Debug.LogWarning($"Fail to load {keyString} KeyCode");
        return KeyCode.None;

    }
}
