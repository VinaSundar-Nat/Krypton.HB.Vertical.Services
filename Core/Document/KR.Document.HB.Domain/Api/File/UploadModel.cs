namespace KR.Document.HB.Domain;

public class UploadResponse
{
    public string?  Url { get; set; }
    public string?  FileName { get; set; }
    public string?  MetaData { get; set; }
}

public class FileModel
{
  public Byte[]? Data { get; set; }
  public string? FileName { get; set; }
  public string? User { get; set; }
}
