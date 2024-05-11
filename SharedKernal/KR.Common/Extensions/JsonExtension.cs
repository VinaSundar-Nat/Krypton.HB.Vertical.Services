using System;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace KR.Common.Extensions;

public static class JsonExtension
{
    public const string StreamCreateError = "Stream create content not supported.";
    public const string StreamReadError = "Stream read content not supported.";

    private static int bufferSize { get; } = 1024;

    private static DefaultContractResolver contractResolver => new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };

    public static void SerializeToJsonStream<T>(this Stream stream, T request,
                bool leaveOpen = true, bool reset = true) =>
                SerializeToJsonStream<T>(stream, request, new UTF8Encoding(), bufferSize, leaveOpen, reset);

    public static void SerializeToJsonStream<T>(
       this Stream stream,
       T request,
       Encoding encoding,
       int bufferSize,
       bool leaveOpen,
       bool reset)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanWrite)
            throw new NotSupportedException(StreamCreateError);

        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        using (var streamWriter = new StreamWriter(stream, encoding, bufferSize, leaveOpen))
        {
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                jsonSerializer.Serialize(jsonTextWriter, request);
                jsonTextWriter.Flush();
            }
        }

        if (reset && stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    public static T? ReadAndDeserializeFromJson<T>(this Stream stream) =>
          ReadAndDeserializeFromJson<T>(stream, new UTF8Encoding(), true, bufferSize, false,
              new Newtonsoft.Json.JsonSerializer());

    public static T? ReadAndDeserializeFromJson<T>(this Stream stream, Newtonsoft.Json.JsonSerializer jsonSerializer) =>
          ReadAndDeserializeFromJson<T>(stream, new UTF8Encoding(), true, bufferSize, false, jsonSerializer);

    public static T? ReadAndDeserializeFromJson<T>(
        this Stream stream,
        Encoding encoding,
        bool detectEncodingFactor,
        int bufferSize,
        bool leaveOpen,
        Newtonsoft.Json.JsonSerializer jsonSerializer)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new NotSupportedException(StreamReadError);

        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        using (var streamReader = new StreamReader(stream, encoding,
            detectEncodingFactor, bufferSize, leaveOpen))
        {
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {              
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }

    public static string ToJson(this object model, bool camelCase = true, Formatting formatting = Formatting.Indented)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            Formatting = formatting,
            ContractResolver = camelCase ? contractResolver : default
        };

        return JsonConvert.SerializeObject(model, serializerSettings);
    }

    public static T? ToModel<T>(this string source, bool camelCase = true, Formatting formatting = Formatting.None)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            Formatting = formatting,
            ContractResolver = camelCase ? contractResolver : default
        };

        return JsonConvert.DeserializeObject<T>(source, serializerSettings);
    }

    public static T? GetValue<T>(this string source, string path) where T : IComparable
    {
        var jObject = JObject.Parse(source);
        var token = jObject.SelectToken(path);

        if (token == null)
            return default(T);

        return (T)Convert.ChangeType(token, typeof(T));
    }
}


