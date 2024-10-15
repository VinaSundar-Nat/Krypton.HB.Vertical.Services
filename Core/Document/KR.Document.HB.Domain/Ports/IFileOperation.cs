namespace KR.Document.HB.Domain;

public interface IFileOperation
{
    Task<UploadResponse> Upload(FileModel file, CancellationToken token);
    Task<string?> GenerateSas(string file,  CancellationToken token);
    Task<byte[]?> Download(string url, CancellationToken token);
}
