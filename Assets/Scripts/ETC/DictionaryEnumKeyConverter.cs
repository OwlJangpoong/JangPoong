using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


//GPT가 만들어줬어요... 저도 모르겠네요....
//JSON과 C# 클래스 간의 직렬화, 역직렬화 과정에서 
//C#의 Dictionary<Enum, int>를 JSON으로 변환할 때, Enum이 string으로 저장이 되나,
//역직렬화할 때 JSON의 string 키를 Enum으로 변환하는 과정이 필요한데, 기본 Json.NET(Newtonsoft.Json)에서는 자동 변환이 안 된다.
//따라서 Dictionary<Define.Item, int>를 JSON에서 자동 변환되도록 JsonConverter를 만들어야한다.

public class DictionaryEnumKeyConverter<TEnum, TValue> : JsonConverter<Dictionary<TEnum, TValue>>
    where TEnum : struct, Enum
{
    // // 📌 JsonConverter가 이 타입을 변환할 수 있는지 확인하는 메서드
    // public override bool CanConvert(Type objectType)
    // {
    //     return objectType == typeof(Dictionary<TEnum, TValue>);
    // }

    // 📌 JSON → C# 객체 변환 (역직렬화)
    public override Dictionary<TEnum, TValue> ReadJson(JsonReader reader, Type objectType, Dictionary<TEnum, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dictionary = new Dictionary<TEnum, TValue>();
        var jsonObject = JObject.Load(reader);

        foreach (var property in jsonObject.Properties())  // JSON의 모든 속성 반복
        {
            if (Enum.TryParse(property.Name, out TEnum key))  // JSON의 string 키를 Enum으로 변환
            {
                TValue value = property.Value.ToObject<TValue>(); // JSON 값 변환
                dictionary[key] = value;
            }
            else
            {
                throw new JsonSerializationException($"Invalid enum key: {property.Name}");
            }
        }

        return dictionary;
    }

    // 📌 C# 객체 → JSON 변환 (직렬화)
    public override void WriteJson(JsonWriter writer, Dictionary<TEnum, TValue> value, JsonSerializer serializer)
    {
        var jsonObject = new JObject();

        foreach (var kvp in value)
        {
            jsonObject.Add(kvp.Key.ToString(), JToken.FromObject(kvp.Value));  // Enum 키를 string으로 변환
        }

        jsonObject.WriteTo(writer);
    }
}