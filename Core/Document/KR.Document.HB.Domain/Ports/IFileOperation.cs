namespace KR.Document.HB.Domain;

public interface IFileOperation
{
    Task<UploadResponse> Upload(FileModel file, CancellationToken token);
}
