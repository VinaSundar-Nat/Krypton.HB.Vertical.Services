using Azure;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace KR.Infrastructure;

public static class CloudResiliencyWrapper
{
    private static readonly Action<ILogger, string, Exception>
    _exceptionCaughtOnErrorRetry = LoggerMessage.Define<string>(
            LogLevel.Warning,
            100,
            "Unhandled exception retrying component: {Message}");

    public static AsyncRetryPolicy GetRetryPolicy<T>(ILogger<T> logger,int sleepFor= 2,int maxRetry = 3)
      => Policy
        .Handle<RequestFailedException>()
        .WaitAndRetryAsync(
            retryCount: maxRetry,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(sleepFor, attempt)), // Exponential backoff
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _exceptionCaughtOnErrorRetry(logger, $"Retry due to process exception .Retrying {retryCount}. Waiting {timeSpan.TotalSeconds} seconds", exception);
                });
}
