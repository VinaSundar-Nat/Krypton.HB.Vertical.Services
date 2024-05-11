using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Moq.Protected;
using TechTalk.SpecFlow;

namespace KR.Core.Documents.HB.Tests;

public class BaseStepDefinition : IClassFixture<TestWebApplicationFactory<Program>>
{
    protected readonly string? _BasePath;
    protected TestWebApplicationFactory<Program> _Factory;
    protected readonly Mock<HttpMessageHandler> _MockMsgHandler;
    protected ScenarioContext _ScenarioContext;
    protected HttpClient? _Client;

    public BaseStepDefinition(TestWebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _BasePath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
        _Factory = factory;
        _MockMsgHandler = new();
        _ScenarioContext = scenarioContext;
    }

    protected async Task<string> GetContent(string file, string folder = "Response")
    {
        if (string.IsNullOrEmpty(file))
            throw new ArgumentNullException("Test source file path not defined.");

        return await Helper.ReadFile(Path.Combine(_BasePath, "Files", folder, file));
    }

    private async Task<(HttpStatusCode code, string response)> setup(ScenarioContext scenarioContext)
    {
        var title = _ScenarioContext.ScenarioInfo.Title;

        return (title switch
        {
            "doc1" => new(HttpStatusCode.OK, await GetContent("doc_200_base_res.json", "Responses")),
            _ => throw new ArgumentNullException("Mock data not setup.")
        });
    }

    protected void addHeaders(HttpRequestMessage request)
    {
        _Client?.DefaultRequestHeaders.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.ms.kr.1.0+json"));
        _Client?.DefaultRequestHeaders.UserAgent.ParseAdd("");
        request.Headers.Add("X-User", "Test_User");
        request.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());
    }

    [BeforeScenario]
    public async Task Initilize(ScenarioContext scenarioContext)
    {
        var definitions = await setup(scenarioContext);

        var apiResponse = new HttpResponseMessage
        {
            Content = new StringContent(definitions.response),
            StatusCode = definitions.code
        };

        _MockMsgHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>()).ReturnsAsync(apiResponse);

        _Client = _Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                builder.UseSetting("urls", "http://localhost");                 
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [AfterScenario]
    public void Destroy(ScenarioContext scenarioContext)
    {
        _Client?.Dispose();
        _Client = null;

    }

}
