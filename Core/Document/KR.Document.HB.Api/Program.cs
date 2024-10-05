using System.Text.Json;
using System.Text.Json.Serialization;
using KR.Document.HB.Adapter;
using KR.Document.HB.Api;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Setup();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    sg => sg.SwaggerDoc("v1", new OpenApiInfo { Title = "KR.Document.Api", Version = "v 1.0" })
);
builder.Services.AddAntiforgery(
    options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";                               
    }
);

builder.Services.AddControllers((options)=>{}).
AddJsonOptions((options) => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.Register(builder.Configuration);
builder.Services.RegisterAdapters(builder.Configuration, builder.Environment.IsDev());


var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() 
        || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.FileEndpoints();
app.SecurityEndpoints();
app.HealthCheckEndpoints();

app.Run();
