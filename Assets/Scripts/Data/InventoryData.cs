using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class InventoryData
{
    [JsonConverter(typeof(DictionaryEnumKeyConverter<Define.Item, int>))]
    public Dictionary<Define.Item, int> items;
    
}
