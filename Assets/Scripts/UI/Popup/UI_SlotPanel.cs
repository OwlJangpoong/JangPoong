using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows;

public class UI_SlotPanel : MonoBehaviour
{
    private TMP_Text slotPanelQ;
    private UI_Slot[] slots;
    public bool isNewGame;
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        slotPanelQ = transform.GetChild(0).GetComponent<TMP_Text>();
        // "Slots"라는 이름의 자식 오브젝트에서 모든 UI_Slot 컴포넌트를 가져오기
        Transform slotsContainer = transform.Find("Slots");
        if (slotsContainer != null)
        {
            slots = slotsContainer.GetComponentsInChildren<UI_Slot>(true);
            Debug.Log(slots.Length);
        }
        else
        {
            Debug.LogError("Slots 컨테이너를 찾을 수 없습니다.");
        }
    }
    
    

    #region interface

    public void ActiveSlotPanel(bool isNew)
    {
        this.isNewGame = isNew;
        gameObject.SetActive(true);
        InitSlotPanelQ(isNew);
    }

    public void InitSlotPanelQ(bool isNew)
    {
        slotPanelQ.text = isNew ? "저장할 슬롯을 선택하세요" : "불러올 데이터를 선택하세요";

        for (int i = 0; i < slots.Length; i++)
        {
            
            (bool hasData, string playerName, string lastPlayTime, string currentStage) = Managers.Data.GetSlotInfo(i + 1);
            
            slots[i].InitSlot(hasData,playerName,lastPlayTime,currentStage);
            
        }
        
        Debug.Log("슬롯 초기화 완료!!!");
        
    }

    #endregion
}
