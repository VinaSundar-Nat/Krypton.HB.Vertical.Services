using System.Collections.ObjectModel;
using System.Text.Json;
using KR.Common.Exceptions;
using KR.Infrastructure.Cache.Interface;
using KR.Security;

namespace KR.Infrastructure.Cache
{
	public sealed class CacheWrapper: ICacheWrapper
    {
        private readonly IRedisStore _redisStore;
        private readonly IHashing _hashing;

        public CacheWrapper(IRedisStore redisStore,IHashing hashing)
		{
            _redisStore = redisStore;
            _hashing = hashing;
        }

        public string GenerateCacheKey(string code, string text)
            => _hashing.Hash(text, code);

        public async Task<bool> HasKey(string key) => await _redisStore.HasKey(key);
    
        public async Task<bool> CreateAsync<T>(string key, DateTime expiry, T data)
            => await _redisStore.CreateCache(key, expiry, data).ConfigureAwait(false);

        public async Task<bool> CreateCollectionAsync<T>(string key, DateTime expiry, IEnumerable<T> data)
            => await _redisStore.CreateCache(key, expiry, data).ConfigureAwait(false);
    
        public async Task<bool> RemoveAsync(string key)       
            => await _redisStore.DeleteCache(key).ConfigureAwait(false);
        
        public async Task UpdateAsync<T>(string key, DateTime expiry, T data)
        {
            if (!await _redisStore.DeleteCache(key).ConfigureAwait(false))
                throw new CacheException($"Cannot delete Cache for key {key} before update.");

            if (!await _redisStore.CreateCache(key, expiry, data).ConfigureAwait(false))
                throw new CacheException($"Cannot update Cache for key {key}");
        }

        public async Task UpdateCollectionAsync<T>(string key, T data, DateTime expiry)
        {
            var cache = await this.GetCollectionAsync<T>(key) ?? new Collection<T>();
            cache.Add(data);

            if (!await _redisStore.DeleteCache(key).ConfigureAwait(false))
                throw new CacheException($"Cannot delete Cache for key {key} before update.");

            if(!await _redisStore.CreateCache(key, expiry, data).ConfigureAwait(false))
                throw new CacheException($"Cannot update Cache collection for key {key}");
        }

        public async Task UpsertAsync<T>(string key, DateTime expiry, T data)
        {
            if (await HasKey(key))
            {
                await UpdateAsync<T>(key, expiry, data);
                return;
            }

            await CreateAsync<T>(key, expiry, data);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _redisStore.GetCacheAsString(key).ConfigureAwait(false);
            return data.HasValue ?  JsonSerializer.Deserialize<T>(data! ) : default;
        }

        public async Task<ICollection<T>?> GetCollectionAsync<T>(string key)
        {
            var data = await _redisStore.GetCacheAsString(key).ConfigureAwait(false);
            return data.HasValue ? JsonSerializer.Deserialize<ICollection<T>>(data! ) : default;
        }
    }
}

