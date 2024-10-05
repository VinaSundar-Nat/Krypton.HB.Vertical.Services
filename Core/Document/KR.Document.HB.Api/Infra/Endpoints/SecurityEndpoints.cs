using System;
using Microsoft.AspNetCore.Antiforgery;

namespace KR.Document.HB.Api;

public static partial class ApiEndpoints
{
     public static void SecurityEndpoints(this WebApplication app){
        var securityGroup = app.MapGroup("/api/doc/security/v1");

        securityGroup.MapGet("/csrftoken", (IAntiforgery antiforgerySvc, HttpContext context) =>
        {
            var tokens = antiforgerySvc.GetAndStoreTokens(context);
            var xsrfToken = new{ token= tokens.RequestToken!} ;
            return Results.Ok(xsrfToken);
            //return TypedResults.Content(xsrfToken, "text/plain");
        });
     }
}
