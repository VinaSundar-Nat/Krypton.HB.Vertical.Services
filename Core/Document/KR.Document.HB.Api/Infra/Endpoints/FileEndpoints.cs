using KR.Document.HB.Domain;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace KR.Document.HB.Api;

public static partial class ApiEndpoints
{
    public static void FileEndpoints(this WebApplication app)
    {
        var fileGroup = app.MapGroup("/api/doc/file/v1");
        app.UseAntiforgery();
        fileGroup.MapPost("/upload", async ([FromForm] IFormFile file,
                IFileOperation fileOperation, HttpContext context,
                IAntiforgery antiforgery,
                [FromHeader(Name = "X-User")] string user,
                CancellationToken token = default) =>
        {

            await antiforgery.ValidateRequestAsync(context);

            var model = new FileModel
            {
                FileName = file.FileName,
                User = user
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, token);
                model.Data = memoryStream.ToArray();
            };

            UploadResponse? fileResult = await fileOperation.Upload(model, token);

            return Results.Created(fileResult?.Url, fileResult);
        }).WithOpenApi(operation => new(operation)
        {
            Summary = "v1 file upload.",
            Description = "upload a document to azure blob storage.",
            Security = [ new OpenApiSecurityRequirement{
            }]
        })
        .Produces<UploadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

}
