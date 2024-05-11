using System;
namespace KR.Infrastructure.Cache.Interface;

public interface ICacheWrapper
{
    string GenerateCacheKey(string code, string text);
    Task<bool> HasKey(string key);
    Task<bool> CreateAsync<T>(string key, DateTime expiry, T data);
    Task<bool> CreateCollectionAsync<T>(string key, DateTime expiry, IEnumerable<T> data);
    Task<ICollection<T>?> GetCollectionAsync<T>(string key);
    Task<T?> GetAsync<T>(string key);
    Task UpdateCollectionAsync<T>(string key, T request, DateTime expiry);
    Task UpdateAsync<T>(string key, DateTime expiry, T data);
    Task UpsertAsync<T>(string key, DateTime expiry, T data);
    Task<bool> RemoveAsync(string key);
}


