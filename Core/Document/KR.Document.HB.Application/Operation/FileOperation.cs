using KR.Document.HB.Domain;

namespace KR.Document.HB.Application;

public class FileOperation(
         IBlobUploadService BlobUploadService,
         IBlobDownloadService BlobDownloadService) : IFileOperation
{
    private void Validate(FileModel file)
    {
        if (file == null) { }

    }

    public async Task<UploadResponse> Upload(FileModel file, CancellationToken token = default)
    {
        return await BlobUploadService.UploadDataAsync(file, token);
    }

    public async Task<string?> GenerateSas(string file,  CancellationToken token = default)
    {
        var uri = await BlobDownloadService.GenerateSas(file,  token)
                    .ConfigureAwait(false);

        return uri?.AbsoluteUri;
    }
  
    public async Task<byte[]?> Download(string url, CancellationToken token = default){
         return await BlobDownloadService.DownloadWithSas(url, token).ConfigureAwait(false);
    }
}
