using KR.Document.HB.Domain;

namespace KR.Document.HB.Application;

public class FileOperation : IFileOperation
{
    public Task<UploadResponse> Upload(FileModel file, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
