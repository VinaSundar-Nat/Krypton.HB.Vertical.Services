using KR.Document.HB.Api.Infra.Helpers;
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
        }).WithOpenApi(operation =>
            operation.GenerateOpenApiDoc(
                "v1 file upload.",
                "upload a document to azure blob storage.",
                "File opertaions",
                "File opertaions to support enterprise operations."
        ))
        .Produces<UploadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status400BadRequest);

        fileGroup.MapGet("/sas", async ([FromQuery(Name = "file")] string file,
        IFileOperation fileOperation, CancellationToken token = default) =>
        {
            return Results.Ok(new { Url = await fileOperation.GenerateSas(file, token) });
        }).WithOpenApi(operation =>
         operation.GenerateOpenApiDoc(
            "v1 generate sas.",
            "generate sas uri for a document in azure blob storage.",
            "File opertaions",
            "File opertaions to support enterprise operations."
        ))
        .Produces<UploadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status404NotFound);

        fileGroup.MapPost("/download", ([FromBody] DownloadRequest model,
        IFileOperation fileOperation, CancellationToken token = default) =>
        {
            return fileOperation.Download(model.Url, token);
        }).WithOpenApi(operation =>
        operation.GenerateOpenApiDoc(
            "v1 file download.",
            "download a document to azure blob storage for request sas.",
            "File opertaions",
            "File opertaions to support enterprise operations."
        ))
        .Produces<UploadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status404NotFound);
    }

}
