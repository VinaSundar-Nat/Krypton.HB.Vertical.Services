namespace KR.Document.HB.Adapter;

public static class BlobHelpers
{
    public static string UtcStamp() => DateTime.UtcNow.ToString("s");

    public static string ConstructBlobUrl(string url, string container, string file) => 
    $"{url}{container}/{file}";
}
