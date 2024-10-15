using System;
using KR.Document.HB.Api.Infra.Helpers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.OpenApi.Models;

namespace KR.Document.HB.Api;

public static partial class ApiEndpoints
{
    public static void SecurityEndpoints(this WebApplication app)
    {
        var securityGroup = app.MapGroup("/api/doc/security/v1");

        securityGroup.MapGet("/csrftoken", (IAntiforgery antiforgerySvc, HttpContext context) =>
        {
            var tokens = antiforgerySvc.GetAndStoreTokens(context);
            var xsrfToken = new { token = tokens.RequestToken! };
            return Results.Ok(xsrfToken);
        }).WithOpenApi(operation =>
            operation.GenerateOpenApiDoc(
                "v1 security csrf token.",
                "request csrf token used to validate form requests.",
                "Security",
                "Backing security to support enterprise operations.", false
            )
       )
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
