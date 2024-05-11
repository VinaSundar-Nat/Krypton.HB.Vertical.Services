using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace KR.Infrastructure.ResiliencyWrapper;

public static class HttpResiliencyWrapper
{
    public static void GetRetryPolicy(this IHttpClientBuilder builder, int retryCount,
        int retryIn,
        Action<string> log)
    {
        builder.AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(retryCount, (retryAttempt) => {
                log($"Attempting retry: {retryAttempt} for failure.");
                return TimeSpan.FromSeconds(Math.Pow(retryIn, retryAttempt));
            } ));
    }


    public static void GetOnlyRetryPolicy(this IHttpClientBuilder builder, int retryCount, int retryIn,
       Action<string> log)
    {
        builder.AddPolicyHandler((provider, request) =>
        {
            if (request.Method != HttpMethod.Get)
                return Policy.NoOpAsync<HttpResponseMessage>();
          
            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                {
                    log($"Attempting retry: {retryAttempt} for failure.");
                    return TimeSpan.FromSeconds(Math.Pow(retryAttempt, retryIn));
                },
                (exception, timeSpan, retryCount, context) =>
                {             
                    var cId = request.Headers.FirstOrDefault(a => a.Key == "X-Correlation-ID");
                    log($"Attempting retry: Correlation-ID - {cId}. error : {exception.Exception}");
                });          
        });
      
    }
}


