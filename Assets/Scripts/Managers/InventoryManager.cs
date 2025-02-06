using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    private bool _isInitialized = false;

    private InventoryData currentInventory;
    //public event Action OnInventoryLoaded;
    public event Action OnInventoryUpdated; // 인벤토리 데이터가 변경될 때 호출되는 이벤트(추가/ 삭제 등)
    
    
    //초기화(로컬 내 저장된 파일이 있다면 그걸 가져오기. GameManager에서 파일에서 읽은 데이터를 가지고 있음. 그러므로 GameManager에서 들고 있는 데이터에 접근해서 가져와야함.)

    public void Init()
    {
        if (_isInitialized) return;
        //초기화
        Debug.Log("InventoryManager 초기화!");

        LoadInventoryState();
        
        
        _isInitialized = true;

    }

    #region Inventory Interface
    
    //인벤토리 아이템 추가/제거
    public void InventoryItem(Define.Item itemType, int num)
    {
        if (!currentInventory.items.ContainsKey(itemType))
        {
            Debug.LogWarning($"Unknown item type: {itemType.ToString()}"); //장풍 레벨업 토큰이면 log warning 떠도 무시해도 됩니다. 게임 진행에 영향을 미치지 않습니다.
            return;
        }

        if (num == 0)
        {
            Debug.Log("아이템 수가 변하지 않았습니다. 'num' 값이 0입니다.");
            return;
        }

        currentInventory.items[itemType] += num;
        if (currentInventory.items[itemType] < 0)
        {
            currentInventory.items[itemType] = 0;
        }
        OnInventoryUpdated?.Invoke();
        
        Debug.Log($"현재 {itemType.ToString()} 개수 : {currentInventory.items[itemType]}");
    }
    
    
    /// <summary>
    /// 지정된 itemType에 해당하는 아이템의 현재 보유 개수를 반환한다.
    /// </summary>
    /// <param name="itemType">아이템 유형(Define.Item)</param>
    /// <returns>보유 중인 아이템 개수 (없으면 0 반환)</returns>
    public int GetItemCount(Define.Item itemType)
    {
        return currentInventory.items.ContainsKey(itemType)?currentInventory.items[itemType] : 0;
    }

    /// <summary>
    /// 인벤토리에 있는 모든 아이템들의 개수를 반환한다. 순서대로 hpSmall, hpLarge, mpSmall, mpLarge, invisibilityPotion
    /// </summary>
    /// <returns></returns>
    public int[] GetAllItemCount()
    {
        //Dictionary의 값들을 배열로 변환하여 반환
        return currentInventory.items.Values.ToArray();
    }
    

    #endregion

    #region Inventory Data Control

    public void CommitInventoryState()
    {
        Debug.Log("InventoryManager: 최종 Inventory 상태 저장을 요청합니다");
        Managers.Game.GameInventory = currentInventory.DeepCopy();


    }

    public void LoadInventoryState()
    {
        currentInventory = Managers.Game.GameInventory.DeepCopy();
    }

    public void RestoreInventoryState()
    {
        Debug.Log("InventoryManager : Inventory 상태를 복구합니다. 이전 데이터를 가져와 덮어씁니다.");

        LoadInventoryState();
    }

    #endregion
    
    
    
    
    
    
    

}
