namespace KR.Document.HB.Domain;

public interface IBlobUploadService
{
    Task<UploadResponse> UploadDataAsync(FileModel fileModel, CancellationToken token);
}
