using System;
using Newtonsoft.Json;

namespace KR.Common.Converters;

public class UIntConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(uint) || t == typeof(uint?);

    public override object? ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
    
        if (UInt32.TryParse(value, out uint output))      
            return output;
        
        return null;
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (uint)untypedValue;
        serializer.Serialize(writer, value.ToString());
        return;
    }

    public static readonly UIntConverter Singleton = new UIntConverter();
}

