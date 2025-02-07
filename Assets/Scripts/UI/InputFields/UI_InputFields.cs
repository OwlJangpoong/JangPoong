using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_InputFields : MonoBehaviour
{
    private TMP_InputField inputField;
    public GameObject warningText;

    public Image fadePanel; //페이드용 Panel UI 할당필요

    // Start is called before the first frame update
    void Start()
    {
        //변수 - UI 오브젝트 할당
        inputField = GetComponent<TMP_InputField>();
        
        //리스너 연결 전 리스너 초기화
        inputField.onValueChanged.RemoveListener(HandleInputChange);
        inputField.onEndEdit.RemoveListener(HandleInputEndEdit);


        //리스너 연결
        inputField.onValueChanged.AddListener(HandleInputChange);
        inputField.onEndEdit.AddListener(HandleInputEndEdit);

        
        warningText.SetActive(false);
        
    }


    
    /// inputfield의 텍스트 입력 여부에 따른 버튼 활성화 여부 결정
    private void HandleInputChange(string newValue)
    {
        //이름 입력 경고 문구 표시
        warningText.SetActive(string.IsNullOrEmpty(newValue));
        
        //효과음 적용 : 입력된 텍스트 데이터에 변화가 있을 때마다 사운드 출력
        Managers.Sound.Play("DM-CGS-01");
        
    }
    
    
    private void HandleInputEndEdit(string text)
    {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //임시로 playerprefs로 저장합니다. -> json으로 변경
            //데이터 저장 구현 완료되면 로직 변경할 예정입니다.
            string playerName = "전설" + text; //영웅재중, 유노유노같은 느낌.. 전설00000
            // PlayerPrefs.SetString("PlayerName", playerName);
            // Debug.Log(PlayerPrefs.GetString("PlayerName"));
            Managers.Player.SetName(playerName); //플레이어 매니져쪽을 변경할지 GameManager쪽을 변경할지 추후 결정.. 어차피 이름은 초반에 한 번만 설정하고 절대 변경안하나까....????
            
            //페이드용 패널 활성화
            fadePanel.gameObject.SetActive(true); 
            
            //페이드 코루틴 호출
            StartCoroutine(FadeEffect.Fade(fadePanel, 0f, 1f,
                action: new UnityAction(() => Managers.Scene.LoadScene("0 Intro"))));
            
            
        }
    
    }
    
}
