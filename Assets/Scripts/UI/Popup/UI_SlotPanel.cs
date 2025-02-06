using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_SlotPanel : MonoBehaviour
{
    private TMP_Text slotPanelQ;
    private UI_Slot[] slots;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Init()
    {
        // "Slots"라는 이름의 자식 오브젝트에서 모든 UI_Slot 컴포넌트를 가져오기
        Transform slotsContainer = transform.Find("Slots");
        if (slotsContainer != null)
        {
            slots = slotsContainer.GetComponentsInChildren<UI_Slot>();
        }
        else
        {
            Debug.LogError("Slots 컨테이너를 찾을 수 없습니다.");
        }
    }
    
    

    #region interface

    public void InitSlotPanelQ(bool isNew)
    {
        slotPanelQ.text = isNew ? "저장할 슬롯을 선택하세요" : "불러올 데이터를 선택하세요";

        foreach (var slot in slots)
        {
            
            // slot
        }
        
    }

    #endregion
}
