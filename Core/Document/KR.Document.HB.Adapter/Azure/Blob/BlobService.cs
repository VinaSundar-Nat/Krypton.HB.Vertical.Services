using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using KR.Common.Extensions;
using KR.Document.HB.Domain;
using KR.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KR.Document.HB.Adapter;

public class BlobService(ILogger<BlobService> Logger,
     BlobServiceClient BlobServiceClient, IOptions<BlobConfiguration> options) : IBlobService
{
    private BlobConfiguration Configuration => options.Value;
    public async Task<UploadResponse> UploadDataAsync(FileModel fileModel, CancellationToken token)
    {
        var containerClient =
            BlobServiceClient.GetBlobContainerClient(Configuration.Container);

        var blobClient = containerClient.GetBlobClient(fileModel.FileName);

        var metadata = new Dictionary<string, string>
            {
                { "provider", Constants.ServiceName },
                { "created", BlobHelpers.UtcStamp() },
                { "tag", Constants.Version }
            };

        var blobOpts = new BlobUploadOptions
        {
            Metadata = metadata,
            TransferOptions = new StorageTransferOptions()
            {
                MaximumConcurrency = 3
            }
        };

        await CloudResiliencyWrapper.GetRetryPolicy<BlobService>(Logger)
        .ExecuteAsync(async () =>
        {
            Logger.LogBlobUploadStart(fileModel.FileName!, Configuration.Container!);
            using var stream = new MemoryStream(fileModel.Data!, false);
            var uploadOps = await blobClient.UploadAsync(stream, blobOpts, token);
            Logger.LogBlobUploadComplete(fileModel.FileName!, Configuration.Container!, uploadOps.Value.VersionId);
        });

        return new UploadResponse
        {
            MetaData = metadata.ToJson(),
            FileName = fileModel.FileName,
            Url = BlobHelpers.ConstructBlobUrl(Configuration.BaseUrl!, Configuration.Container!, fileModel.FileName!),
        };

    }
}
