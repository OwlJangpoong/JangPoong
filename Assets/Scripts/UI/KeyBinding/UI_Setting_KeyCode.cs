using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.KeyBinding;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UI_Setting_KeyCode : MonoBehaviour
{
    [SerializeField] private Define.ControlKey keyBindingType;
    [SerializeField] private TextMeshProUGUI displayText; //현재 키를 보여주는 텍스트
    [SerializeField] private UI_Setting_KeyInputOverlay keyInputOverlay; // 키 입력 대기 UI Overlay

    private Button button;
    private string originKey;
    private bool isListeningForKey = false; //키 입력 대기 상태 플래그

    //호출 순서 때문에 여기에서 초기화 하도록..?
    private void Awake()
    {
        button = GetComponent<Button>();
        
        if(displayText==null)
            displayText = GetComponentInChildren<TextMeshProUGUI>(true);
        
        if(keyInputOverlay.gameObject.activeInHierarchy)
            keyInputOverlay.gameObject.SetActive(false); //처음엔 비활성화
        
        
        //버튼에 StartKeyListening 연결하기
        button.onClick.RemoveListener(StartListeningForKey);
        button.onClick.AddListener(StartListeningForKey);
        //displayText.onEndEdit.AddListener(SetKey);
       

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
        //초기화
        originKey = GetKey(keyBindingType);
        UpdateDisplayText(originKey);
    }
    
    

    #region 키 코드 변경, 불러오기

    private bool SetKey(KeyCode newKeyCode)
    {
        KeyBindingResult result = Managers.KeyBind.SetKeyBinding(keyBindingType, newKeyCode);

        if (result.IsSuccess)
        {
            Debug.Log($"키 설정 완료: {newKeyCode}");
            keyInputOverlay.gameObject.SetActive(false);
            isListeningForKey = false;
            UpdateDisplayText(newKeyCode.ToString());
            originKey = newKeyCode.ToString();

            return true;
        }

        else
        {
            switch (result.ErrorType)
            {
                case 0:
                    //유효하지 않은 키
                    Debug.Log("<color=red>키 설정 실패: 잘못된 키입니다.</color>");
                    keyInputOverlay.ShowMessage(keyInputOverlay.errorMessage);
                    break;
                case 1:
                    Debug.Log("<color=red>키 설정 실패: 이미 설정된 키입니다.</color>");
                    keyInputOverlay.ShowMessage(keyInputOverlay.duplicateMessage);
                    break;
                default:
                    break;
            }
            Debug.Log("키 변경 실패! 원래 키 값으로 복구");
            UpdateDisplayText(originKey);
            
            //다시 입력 대기
            isListeningForKey = true;
            return false;
        }
        
    }

    private string GetKey(Define.ControlKey controlKey)
    {
        return Managers.KeyBind.GetKeyCode(controlKey).ToString();
    }


    private IEnumerator WaitForKeyPress()
    {
        Debug.Log("키입력 대기 시작!");
        //yield return null; //한프레임 대기
        while (isListeningForKey)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                // ESC 키 입력 처리
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("<color=green>키 입력 취소</color>");
                    keyInputOverlay.gameObject.SetActive(false);
                    isListeningForKey = false;
                    yield break; // 대기 종료
                }

                // 유효한 키 입력 처리
                if (Input.GetKeyDown(keyCode))
                {
                    if(SetKey(keyCode))
                        yield break; // 키 입력 시 대기 종료
                }
            }

            // 아무 키 입력이 없으면 & 키 변경 실패시 한 프레임 대기
            yield return null;
        }
    }
    
    
    public void StartListeningForKey()
    {
        if (!isListeningForKey)
        {
            Debug.Log("리스닝 시작!");
            keyInputOverlay.gameObject.SetActive(true);
            isListeningForKey = true;
            //입력 대기 UI 활성화
            StartCoroutine(WaitForKeyPress());
        }
    }
    
    #endregion


    #region UI

    public void UpdateDisplayText(string key)
    {
        displayText.text = key;
    }
    

    #endregion
   
    
    
    
    

}
