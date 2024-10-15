using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace KR.Infrastructure.Cache;

public sealed class CacheConfigurations
{
    public required string Host { get; set; }
    public required string Password { get; set; }
    public bool Ssl { get; set; }
    public int Timeout { get; set; }
    public int ResolvedDns { get; set; }
    public required string InstanceName { get; set; }
    public bool Active { get; set; } = true;
	public string ConnectionString => $"{Host},resolvedns={ResolvedDns},password={Password},ssl={Ssl},connectTimeout={Timeout}"; 
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

