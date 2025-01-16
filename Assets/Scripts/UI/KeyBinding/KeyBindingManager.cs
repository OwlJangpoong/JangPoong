using UnityEngine;
using System;

public class KeyBindingManager
{

    public KeyCode leftKeyCode
    {
        get => LoadKeyBinding(Define.ControlKey.leftKey);
        set
        {
            if (value != LoadKeyBinding(Define.ControlKey.leftKey))
            {
                SaveKeyBinding(Define.ControlKey.leftKey, value);
                Managers.Data.SaveData(Define.SaveKey.SettingData,Managers.Game.settingData);
            }
        }
    }
    public KeyCode rightKeyCode
    {
        get => LoadKeyBinding(Define.ControlKey.rightKey);
        set
        {
            if (value != LoadKeyBinding(Define.ControlKey.rightKey))
            {
                SaveKeyBinding(Define.ControlKey.rightKey, value);
                Managers.Data.SaveData(Define.SaveKey.SettingData,Managers.Game.settingData);
            }
        }
    }
    
    
    public KeyCode jumpKeyCode
    {
        get => LoadKeyBinding(Define.ControlKey.jumpKey);
        set
        {
            if (value != LoadKeyBinding(Define.ControlKey.jumpKey))
            {
                SaveKeyBinding(Define.ControlKey.jumpKey, value);
                Managers.Data.SaveData(Define.SaveKey.SettingData,Managers.Game.settingData);
            }
        }
    }
    public KeyCode slideKeyCode
    {
        get => LoadKeyBinding(Define.ControlKey.slideKey);
        set
        {
            if (value != LoadKeyBinding(Define.ControlKey.slideKey))
            {
                SaveKeyBinding(Define.ControlKey.slideKey, value);
                Managers.Data.SaveData(Define.SaveKey.SettingData,Managers.Game.settingData);
            }
        }
    }
    public KeyCode runKeyCode
    {
        get => LoadKeyBinding(Define.ControlKey.runKey);
        set
        {
            if (value != LoadKeyBinding(Define.ControlKey.runKey))
            {
                SaveKeyBinding(Define.ControlKey.runKey, value);
                Managers.Data.SaveData(Define.SaveKey.SettingData,Managers.Game.settingData);
            }
        }
    }
    
    
    public void SaveKeyBinding(Define.ControlKey controlKey, KeyCode newKeyCode)
    {
        string keyString = controlKey.ToString();
        string newKey = newKeyCode.ToString();
        Managers.Game.settingData.controls[keyString] = newKey;

    }

    /// <summary>
    /// Define.ControlKey의 enum의 문자열 값을 키로하여 GameManager에 저장된 KeyCode를 반환하는 메소드.SettingData.contorls 딕셔너리에 string 형태로 저장된 KeyCode 데이터를 KeyCode 형태로 전환하여 반환한다.
    /// </summary>
    /// <param name="controlKey"></param>
    /// <returns></returns>
    public KeyCode LoadKeyBinding(Define.ControlKey controlKey)
    {
        string keyString = controlKey.ToString();
        // 저장된 KeyCode 문자열을 가져오기
        if (Managers.Game.settingData.controls.TryGetValue(keyString, out string keyCodeString))
        {
            // 문자열을 KeyCode로 변환
            if (Enum.TryParse(typeof(KeyCode), keyCodeString, out object keyCode))
            {
                return (KeyCode)keyCode;
            }
        }

        // KeyCode가 없을 경우 기본값 반환 (필요 시 수정)
        Debug.LogWarning($"Fail to load {keyString} KeyCode");
        return KeyCode.None;

    }
}
