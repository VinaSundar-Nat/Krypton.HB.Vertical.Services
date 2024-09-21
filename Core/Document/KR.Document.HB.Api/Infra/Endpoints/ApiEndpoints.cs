using KR.Document.HB.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

namespace KR.Document.HB.Api;

public static partial class ApiEndpoints
{
    public static void FileEndpoints(this WebApplication app){
        app.MapGroup("/api/file/v1");
        
        app.MapPost("/upload",async(IFormFile file,
            IFileOperation fileOperation, CancellationToken token = default ) =>{
            
            var model = new FileModel{
                FileName = file.Name,
                Path = file.FileName
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream,token);
                model.Data = memoryStream.ToArray();
            };

            var fileResult = await fileOperation.Upload(model, token);

            return Results.Created(fileResult?.Url , fileResult);              
        }).WithOpenApi(operation => new(operation)
        {
            Summary = "v1 file upload.",
            Description = "upload a document to azure blob storage.",
            Security= [ new OpenApiSecurityRequirement{

            }]
        })
        .Produces<UploadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

}
