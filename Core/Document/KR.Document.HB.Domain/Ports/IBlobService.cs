namespace KR.Document.HB.Domain;

public interface IBlobService
{
    Task<UploadResponse> UploadDataAsync(FileModel fileModel, CancellationToken token);
}
