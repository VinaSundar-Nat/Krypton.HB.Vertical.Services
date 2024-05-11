using System.Net.Http.Headers;

namespace KR.Infrastructure.Http;

public static class HttpExtensions
{
    public static void Addheaders(this HttpClient client, Dictionary<string, string>? headers,
        HttpRequestMessage request, Func<string> contextVersion = null)
    {
        client.DefaultRequestHeaders.Clear();

        if (contextVersion != null)
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contextVersion.Invoke()));

        client.DefaultRequestHeaders.UserAgent.ParseAdd(HttpConstants.UserAgent);

        foreach (var header in headers ?? new Dictionary<string, string>())
        {
            request.Headers.Add(header.Key, header.Value);
        }
    }

    public static int Page(this string source, int defaultVal) =>
        string.IsNullOrEmpty(source) ? defaultVal : int.TryParse(source, out int page) ? page : defaultVal;
}




