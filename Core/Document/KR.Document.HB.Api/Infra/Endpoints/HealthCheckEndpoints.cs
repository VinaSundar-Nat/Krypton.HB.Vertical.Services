using KR.Document.HB.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

namespace KR.Document.HB.Api;

public static partial class ApiEndpoints
{
    public static void HealthCheckEndpoints(this WebApplication app){
        app.MapGroup("/api/health/v1");
        
        app.MapGet("/verify",(CancellationToken token = default ) =>{
            app.Logger.LogInformation("Ops started");
            return Results.Ok("alive version");              
        }).WithOpenApi(operation => new(operation)
        {
            Summary = "v1 health check.",
            Description = "verify the health of the application.",
            Security= [ new OpenApiSecurityRequirement{

            }]
        })
        .Produces(StatusCodes.Status200OK);
    }

}
