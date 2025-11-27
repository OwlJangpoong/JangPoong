using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour
{
    private Button slot;
    private UI_SlotPanel slotPanel;
    private Color emptyColor;   //D4D3D3
    private Color fullColor;   //65CFAC

    [Header("Slot info & variables")] 
    public int slotNum;
    public TMP_Text emptyTxt;
    public GameObject slotText;
    public TMP_Text playerName;
    public TMP_Text lastPlayTime;
    public TMP_Text stage;
    public bool hasData;

    [Header("PopUp Dialog Panels")] 
    public GameObject NewGamePanelDialogue;
    public GameObject LoadGamePanelDialogue;
    public GameObject NoSavePanelDialogue;
    public GameObject SlotWarningDiaglogue;
    private void Awake()
    {
        slot = GetComponent<Button>();
        slotPanel = GetComponentInParent<UI_SlotPanel>();
        emptyColor = Util.HexToColor("#D4D3D3");
        fullColor = Util.HexToColor("#65CFAC");
        
        slot.onClick.RemoveListener(SetActivePopUpPanel);
        slot.onClick.AddListener(SetActivePopUpPanel);
    }

    public void InitSlot(bool hasData, string playerName=null, string lastPlayTime=null, string stage=null)
    {
        this.hasData = hasData;
        
        //색깔 변경
        ColorBlock cb = slot.colors;
        cb.normalColor = hasData ? fullColor : emptyColor;
        slot.colors = cb;
        
        //텍스트 변경
        //1.비어있음 텍스트 활/비활처리
        emptyTxt.gameObject.SetActive(!hasData);
        //2. 슬롯 정보 활/비활처리
        slotText.SetActive(hasData);
    
        //3. 비어있지 않은 경우 슬롯 정보 업데이트
        if (hasData)
        {
            this.playerName.text = "플레이어 이름 : " + playerName;
            this.lastPlayTime.text = "마지막 플레이 시간 : " + lastPlayTime;
            this.stage.text = "진행 스테이지 : " + stage;
            
            
        }
        
        // //이어하기의 경우 비어있는 슬롯은 버튼 비활성화 처리하기
        // if (!slotPanel.isNewGame && !this.hasData) slot.interactable = false;
        // else slot.interactable = true;
    }

    public void SetActivePopUpPanel()
    {
        Managers.Data.SetSlotNum(slotNum);
        //새로하기
        if (slotPanel.isNewGame)
        {
            if (!hasData)
            {
                NewGamePanelDialogue.SetActive(true);
                
            }
            else
            {
                SlotWarningDiaglogue.SetActive(true);
            }
            
        }
        //이어하기
        else
        {
            if (!hasData)
            {
                NoSavePanelDialogue.SetActive(true);
                
            }
            else
            {
                LoadGamePanelDialogue.SetActive(true);
            }
        }
        
        slotPanel.gameObject.SetActive(false);
    }
    
    
}