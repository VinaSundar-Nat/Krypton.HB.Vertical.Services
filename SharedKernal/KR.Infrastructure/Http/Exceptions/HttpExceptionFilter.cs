using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace KR.Infrastructure.Http.Exceptions;

public partial class HttpExceptionFilter : IExceptionFilter
{
    private readonly ILogger<HttpExceptionFilter> _logger;
    private readonly IHttpContextAccessor _contextAccessor;

    private string? correlationId => this._contextAccessor?.HttpContext?.Request?.Headers["X-Correlation-ID"];

    public HttpExceptionFilter(ILogger<HttpExceptionFilter> logger,
        IHttpContextAccessor httpContext)
    {
        this._logger = logger;
        this._contextAccessor = httpContext;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
           $"{context.Exception.ToString()},correlationId :{correlationId ?? Guid.NewGuid().ToString()}" );

        ExceptionFactory.OnException(context, _logger);
    }   
}


