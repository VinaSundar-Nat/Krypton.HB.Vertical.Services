using System.Net;
using System.Security.Authentication;
using KR.Infrastructure.Logging;
using KR.Infrastructure.Server.Model;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KR.Document.HB.Api;

public static class AppBuilder
{
    public static void Setup(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        Configuration(builder);
        Host(builder);   
        builder.RegisterLogger();
    }

    private static void Configuration(WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile("./Configuration/serilog.json", optional: false);
        builder.Configuration.AddJsonFile("./Configuration/services.json", optional: false);
        builder.Configuration.AddJsonFile($"./Configuration/services.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    private static void Host(WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrel((WebHostBuilderContext context, KestrelServerOptions options) =>
        {
            var _hostConfig = new KerstalConfiguration();
            context.Configuration.GetSection(KerstalConfiguration.HostingOptions).Bind(_hostConfig);

            if (!_hostConfig.UseKerstal)
                return;

            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });

            options.Listen(IPAddress.Any, _hostConfig.Port, listenOptions =>
            {
                try
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps(_hostConfig.CertPath, _hostConfig.CertPassword);
                }
                catch (Exception ex)
                {
                    var loggerFactory = builder.Services.BuildServiceProvider()
                                            .GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("Application");
                    logger.LogApplicationException(ex);
                }
            });

            options.Listen(IPAddress.Any, 5002, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

            options.Listen(IPAddress.Any, 8085, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

            options.Limits.MaxRequestBodySize = 9703180;           
            options.Configure();
        });
    }


}
