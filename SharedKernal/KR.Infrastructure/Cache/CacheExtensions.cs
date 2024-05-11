using System;
using KR.Infrastructure.Cache.Interface;
using KR.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KR.Infrastructure.Cache
{
	public static class CacheExtensions
	{
		public static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
		{
            services.Configure<CacheConfigurations>(configuration.GetSection("Cache"));
            services.AddSingleton<IStartupFilter, CacheFilter>();          
            services.AddSingleton<IHashing, Crypto>();
            services.AddSingleton<IRedisStore, RedisStore>();
            services.AddScoped<ICacheWrapper, CacheWrapper>();
        }
	}
}

