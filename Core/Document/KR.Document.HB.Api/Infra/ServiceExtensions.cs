﻿using KR.Document.HB.Application;
using KR.Document.HB.Domain;
using KR.Infrastructure;

namespace KR.Document.HB.Api;

public static class ServiceExtensions
{
    public static void Register(this IServiceCollection services, IConfiguration configuration ){
        RegisterExceptions(services);
        
        services.AddScoped<IFileOperation,FileOperation>();
    }

    private static void RegisterExceptions(IServiceCollection services){
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
