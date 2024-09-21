using System.Text.Json;
using System.Text.Json.Serialization;
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

builder.Services.AddControllers((options)=>{}).AddJsonOptions((options) => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.Register(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.FileEndpoints();

app.Run();
