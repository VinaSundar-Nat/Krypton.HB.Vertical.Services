namespace KR.Document.HB.Domain;

public interface IBlobDownloadService
{
    Task<Uri> GenerateSas(string blob, CancellationToken token);

    Task<byte[]?> DownloadWithSas(string url, CancellationToken token);
}
