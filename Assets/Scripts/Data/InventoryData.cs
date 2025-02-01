using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class InventoryData
{
    [JsonConverter(typeof(DictionaryEnumKeyConverter<Define.Item, int>))]
    public Dictionary<Define.Item, int> items = new Dictionary<Define.Item, int>();
    
    
    /* '=' 으로 할당시 새로운 인스턴스로 복사하는 것이 아니라 기존 객체를 참조 : 얕은 복사
     * 따라서 의도한'복사'를 구현하기 위해선 DeepCopy(깊은 복사)를 사용해야함.
     * 기본적으로 new Dictiaonary<>()를 생성해서 복사해야하며, 코드의 재사용성이 높기 때문에 InventoryData에 직접 깊은 복사 기능을 추가한다.*/
    public InventoryData DeepCopy()
    {
        return new InventoryData
        {
            items = new Dictionary<Define.Item, int>(this.items)
        };


    }
    
}
