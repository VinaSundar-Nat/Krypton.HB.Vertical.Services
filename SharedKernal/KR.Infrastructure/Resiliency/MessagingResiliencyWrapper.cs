using System;
using KR.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace KR.Infrastructure.Resiliency;

public static class MessagingResiliencyWrapper
{
    internal static AsyncRetryPolicy GetRetryPolicy<T>(ILogger<T> log, int retryCount,
        int retryIn, long correlation)
        => Policy
        .Handle<Common.Exceptions.MessageException>()
        .WaitAndRetryAsync(retryCount, retryAttempt =>
        {
            log.LogInformation($"Attempting retry: {retryAttempt} for failure.");
            return TimeSpan.FromSeconds(Math.Pow(retryAttempt, retryIn));
        },
        (exception, timeSpan, retryCount, context) =>
        {
            log.LogError($"Attempting retry: Correlation-ID - {correlation}. error : {exception}");
        });
}

