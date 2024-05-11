using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace KR.Infrastructure.Cache;

public sealed class CacheConfigurations
{
	public string ConnectionString { get; set; }
	public string InstanceName { get; set; }

    public bool Active { get; set; } = true;
    public bool IsValid => !string.IsNullOrEmpty(ConnectionString);
}

public sealed class CacheFilter : IStartupFilter
{
    private readonly IOptions<CacheConfigurations> options;
    public CacheFilter(IOptions<CacheConfigurations> _options)
    {
        options = _options;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        if (!options.Value.IsValid)
            throw new ArgumentNullException("Cache API Configuration not defined.");

        return next;
    }
}

