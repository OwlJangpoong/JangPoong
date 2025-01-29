using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    private Dictionary<Define.Item, int> inventory;
    //public event Action OnInventoryLoaded;
    public event Action OnInventoryUpdated; // 인벤토리 데이터가 변경될 때 호출되는 이벤트(추가/ 삭제 등)
    
    
    //초기화(로컬 내 저장된 파일이 있다면 그걸 가져오기. GameManager에서 파일에서 읽은 데이터를 가지고 있음. 그러므로 GameManager에서 들고 있는 데이터에 접근해서 가져와야함.)

    public void Init()
    {
        inventory = new Dictionary<Define.Item, int>();
        // 초기화
        foreach (Define.Item itemType in System.Enum.GetValues(typeof(Define.Item)))
        {
            inventory[itemType] = 0; // 모든 아이템 개수를 0으로 초기화
        }
    }
    
    
    
    public int hpPotion_Small_cnt;
    public int hpPotion_Large_cnt;
    public int mpPotion_Small_cnt;
    public int mpPotion_Large_cnt;
    public int invisibilityPotion_cnt;
    
    //인벤토리 아이템 추가/제거
    public void InventoryItem(Define.Item itemType, int num)
    {
        if (!inventory.ContainsKey(itemType))
        {
            Debug.LogWarning($"Unknown item type: {itemType.ToString()}");
            return;
        }

        if (num == 0)
        {
            Debug.Log("아이템 수가 변하지 않았습니다. 'num' 값이 0입니다.");
            return;
        }

        inventory[itemType] += num;
        if (inventory[itemType] < 0)
        {
            inventory[itemType] = 0;
        }
        OnInventoryUpdated?.Invoke();
        
        Debug.Log($"현재 {itemType.ToString()} 개수 : {inventory[itemType]}");
    }
    
    
    /// <summary>
    /// 지정된 itemType에 해당하는 아이템의 현재 보유 개수를 반환한다.
    /// </summary>
    /// <param name="itemType">아이템 유형(Define.Item)</param>
    /// <returns>보유 중인 아이템 개수 (없으면 0 반환)</returns>
    public int GetItemCount(Define.Item itemType)
    {
        return inventory.ContainsKey(itemType)?inventory[itemType] : 0;
    }

    /// <summary>
    /// 인벤토리에 있는 모든 아이템들의 개수를 반환한다. 순서대로 hpSmall, hpLarge, mpSmall, mpLarge, invisibilityPotion
    /// </summary>
    /// <returns></returns>
    public int[] GetAllItemCount()
    {
        //Dictionary의 값들을 배열로 변환하여 반환
        return inventory.Values.ToArray();
    }
    

}
