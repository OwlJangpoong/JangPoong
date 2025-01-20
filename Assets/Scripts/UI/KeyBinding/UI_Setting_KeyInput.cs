using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Setting_KeyInput : MonoBehaviour
{
    
    [SerializeField] private Define.ControlKey keyBindingType;
    private TMP_InputField inputField;
    
    
    // Start is called before the first frame update
    void Start()
    {
      
        
    }
    

    //호출 순서 때문에 여기에서 초기화 하도록..?
    private void Awake()
    {
        Debug.Log("Awake 호출!!!!!!!!!!!!!!!1");
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(SetKey);
       

    }

    private void OnEnable()
    {
        if (Managers.Game != null)
        {
            // 이벤트 구독
            Managers.Game.OnDataLoaded += Init;

            // GameManager가 이미 초기화된 상태라면 바로 초기화
            if (Managers.Game.IsInit)
            {
                Init();
            }
        }
    }

    private void Init()
    {
        Debug.Log("제발 좀 호출되라고");
        //초기화
        inputField.text = GetKey(keyBindingType);
    }

    private void SetKey(string input)
    {
        string originKey = inputField.text;
        
        if (!string.IsNullOrEmpty(input))
        {
            //키 변경에 성공한 경우 text 반영, 실패한 경우 원래 키로 text 반영
            if (Managers.KeyBind.SetKeyBinding(keyBindingType, input))
            {
                inputField.text = input.Trim();
            }
            else
            {
                Debug.Log("키 변경 실패! 원래 키 값으로 복구");
                inputField.text = originKey;
            }
        }
    }

    private string GetKey(Define.ControlKey controlKey)
    {
        return Managers.KeyBind.GetKeyCode(controlKey).ToString();
    }


}
