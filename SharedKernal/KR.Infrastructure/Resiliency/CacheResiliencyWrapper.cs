using System;
using KR.Common.Exceptions;
using KR.Infrastructure.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace KR.Infrastructure.Resiliency;

public static class CacheResiliencyWrapper
{
    internal static RetryPolicy GetRetryPolicy(ILogger<RedisStore> logger)       
      => Policy
        .Handle<CacheConnectionException>()
        .Retry(3, (exception, retryCount, context) =>
        {
            logger.LogCritical($"Cache connection exception.Retrying {retryCount}", exception);
        });
}

