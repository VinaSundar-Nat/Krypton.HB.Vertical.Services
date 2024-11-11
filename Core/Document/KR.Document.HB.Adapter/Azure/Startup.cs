

using Azure.Core;
using Azure.Identity;
using KR.Document.HB.Domain;
using KR.Infrastructure.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KR.Document.HB.Adapter;

public static class Startup
{
    public static void RegisterAdapters(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddAzureClients(clientBuilder =>
        {
            var blobConfig = new BlobConfiguration();
            configuration.GetSection("Cloud:Blob").Bind(blobConfig);

            if (!blobConfig.IsValid)
                throw new InvalidOperationException("Blob Configuration not defined");

            clientBuilder.AddBlobServiceClient(new Uri(blobConfig.BaseUrl!));
            var options = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = blobConfig.ExcludeEnvironmentCredential,  // Don't exclude Environment Credential
                ExcludeManagedIdentityCredential = blobConfig.ExcludeManagedIdentityCredential,  // Use Managed Identity if available
                ExcludeAzureCliCredential = blobConfig.ExcludeAzureCliCredential,  // Use Azure CLI credentials if available
                ExcludeSharedTokenCacheCredential = !env.IsDev(),  // Exclude Shared Token Cache
                ExcludeInteractiveBrowserCredential = true,
                ExcludeVisualStudioCredential = true,
                ExcludeVisualStudioCodeCredential = !env.IsDevelopment(),
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                Diagnostics = { IsLoggingContentEnabled = env.IsDev() }
            };

            if (!env.IsDev())
                options.ManagedIdentityClientId = blobConfig.ManagedIdentityId;

            clientBuilder.UseCredential(new DefaultAzureCredential(options));
        });

        RegisterOptions(services, configuration);
        RegisterServices(services);
    }

    private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobConfiguration>(configuration.GetSection("Cloud:Blob"));
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IBlobUploadService, BlobUploadService>();
        services.AddSingleton<IBlobDownloadService, BlobDownloadService>();
    }
}