using System.Text.Json;
using KR.Common.Exceptions;
using KR.Infrastructure.Cache.Interface;
using KR.Infrastructure.Resiliency;
using KR.Infrastructure.ResiliencyWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Core;
using StackExchange.Redis;

namespace KR.Infrastructure.Cache;

internal sealed class RedisStore : IRedisStore
{   
    private ConnectionMultiplexer? _Store { get; set; }
    private readonly ILogger<RedisStore> _logger;

    public RedisStore(IOptions<CacheConfigurations> options, ILogger<RedisStore> logger)
	{
        _logger = logger;

        if (_Store != null && _Store.IsConnected)
            return;

        _Store = Connect(options.Value.ConnectionString);
        _Store.InternalError += RedisInternalExceptionHandler;
        _Store.ConnectionFailed += RedisConnectionFailedHandler;
    }

    private IDatabase Database()
    {
        if (_Store == null || !_Store.IsConnected)
            throw new CacheException("Redis Database is not connected.");

        return  _Store.GetDatabase();
    }

    private ConnectionMultiplexer Connect(string redisCS)
    {
        var configuration = ConfigurationOptions.Parse(redisCS);
       
        var policy = CacheResiliencyWrapper.GetRetryPolicy(_logger);

        ConnectionMultiplexer? plex = null;

        policy.Execute(() =>
        {
             plex = ConnectionMultiplexer.Connect(configuration);

            if (!plex.IsConnected)
                throw new CacheConnectionException(CacheConstants.CacheConnectionError);
        });

        if(!plex?.IsConnected ?? false)
            throw new ApplicationException(CacheConstants.CacheConnectionError);


        return plex;
    }

    private static void RedisConnectionFailedHandler(object sender, ConnectionFailedEventArgs e)
    {
        throw new CacheException("Redis connection exception.", e.Exception);
    }


    private static void RedisInternalExceptionHandler(object sender, InternalErrorEventArgs e)
    {
        throw new CacheException("Redis exception.", e.Exception);
    }

    public async Task<bool> HasKey(string key)
    {
       return await Database().KeyExistsAsync(key, CommandFlags.PreferReplica).ConfigureAwait(false);
    }

    public async Task<bool> CreateCache(string key, DateTime expiry, object data)
    {

        if (expiry < DateTime.UtcNow)
            throw new CacheException($"Cannot create Cache for key {key}.Cache expiry invalid .Please try again later");
 
        if (!await Database().StringSetAsync(key, JsonSerializer.Serialize(data)).ConfigureAwait(false) )
            throw new CacheException($"Cannot create Cache for key {key}");

        Database().KeyExpire(key, expiry);

        return true;
    }

    public async Task<RedisValue?> GetCacheAsString(string key)
    {
        if (!await Database().KeyExistsAsync(key))
            return null;

        return await Database().StringGetAsync(key).ConfigureAwait(false);
    }

    public async Task<bool> DeleteCache(string key)
    {
        if (!await Database().KeyExistsAsync(key))
            return true;

        await Database().KeyDeleteAsync(key);

        return !Database().KeyExists(key);
    }

}


