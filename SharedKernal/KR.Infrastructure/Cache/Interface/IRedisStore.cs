using StackExchange.Redis;

namespace KR.Infrastructure.Cache.Interface
{
	public interface IRedisStore
	{
        Task<RedisValue?> GetCacheAsString(string key);
        Task<bool> DeleteCache(string key);
        Task<bool> CreateCache(string key, DateTime expiry, object data);
        Task<bool> HasKey(string key);
    }
}

