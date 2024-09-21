using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KR.Infrastructure;

public class GlobalExceptionHandler: IExceptionHandler
{
      private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GlobalExceptionHandler>();
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogServerException(httpContext.Request.Path, httpContext.Request.Method, exception);
  
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "Request has encountered a failure. Please contact service support.",
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails);

        return true;
    }
}
