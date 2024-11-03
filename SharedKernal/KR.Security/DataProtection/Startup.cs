using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace KR.Security.DataProtection;

public static class Startup
{    
    public static void DataProtectionFileStore(this IServiceCollection services,
    string application,string path) => 
        services.AddDataProtection()
            .SetApplicationName(application)
            .PersistKeysToFileSystem(new DirectoryInfo(path))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

    public static void DataProtectionCacheStore(this IServiceCollection services,
    string application,ConnectionMultiplexer multiplexer) {
         var redis = ConnectionMultiplexer.Connect("localhost:6379");
         services.AddDataProtection()        
            .PersistKeysToStackExchangeRedis(multiplexer,$"STOREKEY-{application}")
            .SetApplicationName(application)
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });
    }
        



}
