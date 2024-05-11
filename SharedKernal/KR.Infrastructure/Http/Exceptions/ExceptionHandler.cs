using System;
using System.Net;
using System.Reflection;
using KR.Common.Exceptions;
using KR.Common.Gaurd;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace KR.Infrastructure.Http.Exceptions;

public interface IExceptionHandler
{
    void Handle(ExceptionContext context);
}

internal sealed class ValidationExceptionHandler: IExceptionHandler
{
    public  void Handle(ExceptionContext context)
    {
        var problemDetails = new ValidationProblemDetails()
        {
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status400BadRequest,
            Detail = "Please refer to the errors property for additional details."
        };

        var exception = context.Exception as DomainValidationException;

        problemDetails.Errors.Add("DomainValidations",
                    exception?.Errors.Select(a => $"{a.PropertyName},{a.ErrorMessage}").ToArray() ??
                            new string[] {});

        context.Result = new BadRequestObjectResult(problemDetails);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
}

internal sealed class InternalServerExceptionHandler : IExceptionHandler
{
    public void Handle(ExceptionContext context)
    {
        var exception = context.Exception as DomainServerException;
        var problemDetails = new ProblemDetails()
        {
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception?.CustomMessage ?? "Internal server Error."
        };

        context.Result = new ObjectResult(problemDetails);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    }
}

internal sealed class UnauthorizedExceptionHandler : IExceptionHandler
{
    public void Handle(ExceptionContext context)
    {
        var exception = context.Exception as DomainUnauthorizedException;
        var problemDetails = new ProblemDetails()
        {
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status401Unauthorized,
            Detail = exception?.CustomMessage ?? "Unauthorized Exception."
        };

        context.Result = new ObjectResult(problemDetails);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}

public static class ExceptionFactory
{
    private const string exNamespace = "Account.Infrastructure.Http.Exceptions";
    public static void OnException(ExceptionContext context, ILogger logger)
    {
        IExceptionHandler? handler = null;
        try
        {
            var exceptionType = context.Exception.GetType();

            ImplementorAttribute? attribute = (ImplementorAttribute)Attribute.GetCustomAttribute(exceptionType, typeof(ImplementorAttribute))
                                                .Gaurd("Implementor type not defined.");

            Type handlerType = Type.GetType($"{exNamespace}.{attribute.nameof}Handler")
                                    .Gaurd("Implementor type not defined.");

            handler = (IExceptionHandler) Activator.CreateInstance(handlerType)
                                            .Gaurd("Handle invocation error.") ;

            handler.Handle(context);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error handler exception.{ex.ToString()}");
            var serverException = new InternalServerExceptionHandler();
            serverException.Handle(context);
        }
        finally
        {
            handler = null;
        }
    }
}



