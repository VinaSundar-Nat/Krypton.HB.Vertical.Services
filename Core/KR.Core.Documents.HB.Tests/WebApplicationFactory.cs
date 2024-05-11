using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace KR.Core.Documents.HB.Tests;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((hostContext, configApp) =>
        {
            var env = hostContext.HostingEnvironment;
            configApp.AddJsonFile($"appsettings.json", optional: false);
            configApp.AddJsonFile($"appsettings.Logs.json", optional: true);
            configApp.AddEnvironmentVariables();
        });      
    }
}

