﻿using KR.Document.HB.Application;
using KR.Document.HB.Domain;
using KR.Infrastructure;
using KR.Infrastructure.Cache;
using Polly;

namespace KR.Document.HB.Api;

public static class ServiceExtensions
{
    public static void Register(this IServiceCollection services, IConfiguration configuration ){       
        RegisterExceptions(services);
        services.AddScoped<IFileOperation,FileOperation>();
        RegisterPolocies(services);
        // services.RegisterCache(configuration);      
    }

    public static void RegisterPolocies(IServiceCollection services){
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });
    }

    private static void RegisterExceptions(IServiceCollection services){ 
        services.AddProblemDetails(options => {
          options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Add("trace-id",
                         context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.Add("path", 
                        $"{context.HttpContext.Request.Path} : {context.HttpContext.Request.Method}");
            };           
        });      
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
