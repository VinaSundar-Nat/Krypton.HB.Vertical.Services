using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KR.Core.Documents.HB.Tests;

public static class Helper
{
    public static async Task<string> ReadFile(string path)
    {
        return await File.ReadAllTextAsync(path);
    }

    public static T? ToModel<T>(string request, Formatting formatting = Formatting.Indented)
    {
        if (string.IsNullOrEmpty(request))
            return default;

        JsonSerializerSettings settings = new();
        settings.Converters.Add(new StringEnumConverter());
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.Formatting = formatting;

        return JsonConvert.DeserializeObject<T>(request, settings);
    }
}



