using KR.Document.HB.Domain;

namespace KR.Document.HB.Application;

public class FileOperation(IBlobService BlobService) : IFileOperation
{
    public async Task<UploadResponse> Upload(FileModel file, CancellationToken token = default)
    {
       return await BlobService.UploadDataAsync(file,token);    
    }
}
