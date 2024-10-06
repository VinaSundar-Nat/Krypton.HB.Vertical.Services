using KR.Document.HB.Application;
using KR.Document.HB.Domain;
using KR.Infrastructure;

namespace KR.Document.HB.Api;

public static class ServiceExtensions
{
    public static void Register(this IServiceCollection services, IConfiguration configuration ){       
        RegisterExceptions(services);
        services.AddScoped<IFileOperation,FileOperation>();
        RegisterPolocies(services);      
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
        services.AddProblemDetails();      
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
