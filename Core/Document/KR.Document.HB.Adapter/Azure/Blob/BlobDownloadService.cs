using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using KR.Document.HB.Domain;
using KR.Infrastructure;
using KR.Infrastructure.Cache.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KR.Document.HB.Adapter;

public sealed class BlobDownloadService(ILogger<BlobDownloadService> Logger,
     BlobServiceClient BlobServiceClient, IOptions<BlobConfiguration> options
      ) : IBlobDownloadService
{
    private BlobConfiguration Configuration => options.Value;

    public async Task<Uri> GenerateSas(string blob, CancellationToken token)
    {
        var blobClient = BlobServiceClient
                        .GetBlobContainerClient(Configuration.Container)
                        .GetBlobClient(blob);

        BlobSasBuilder sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = Constants.BlobResource,
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(Configuration.SasValidity!.Value)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        //var key = CacheWrapper.GenerateCacheKey(Constants.BlobCacheKey,blob);

        var userDelegationKey = await RequestUserDelegationKey(token);

        BlobUriBuilder uriBuilder = new BlobUriBuilder(blobClient.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(userDelegationKey,
            blobClient.GetParentBlobContainerClient()
                    .GetParentBlobServiceClient().AccountName)
        };

        return uriBuilder.ToUri();
    }

    public async Task<byte[]?> DownloadWithSas(string url, CancellationToken token)
    {

        var blobClient = new BlobClient(new Uri(url));

        try
        {
            byte[]? data = null;
            Logger.LogBlogDownloadStart(blobClient!.Name, Configuration.Container!);

            await CloudResiliencyWrapper.GetRetryPolicy(Logger, maxRetry: 1)
           .ExecuteAsync(async () =>
           {
               using MemoryStream stream = new();
               await blobClient.DownloadToAsync(stream, token);
               data = stream.ToArray();
           });

            Logger.LogBlogDownloadComplete(blobClient!.Name, Configuration.Container!);
            return data;
        }
        finally
        {
            blobClient = null;
        }
    }

    private UserDelegationKey? key = null;
    private async ValueTask<UserDelegationKey> RequestUserDelegationKey( CancellationToken token)
    {
        if (key != null && key.SignedExpiresOn > DateTimeOffset.UtcNow.AddHours(1))
            return key;

        key = await BlobServiceClient.GetUserDelegationKeyAsync(
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(1), token);
        return key;
    }

}
