using System.Net;
using System.Text;
using System.Text.Json;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Arcora.Api.Tests;

public sealed class SignwellTestModeTests
{
    [Theory]
    [InlineData("Completed", true)]
    [InlineData("Sent", false)]
    public async Task OnlyCompletedProviderPdfIsReturnedAsSignedAgreement(string providerStatus, bool completed)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Signwell:ApiKey"] = "test-fixture",
            ["Signwell:BaseUrl"] = "https://signwell.example.test/api/v1/"
        }).Build();
        using var handler = new StatusHandler(providerStatus);
        using var http = new HttpClient(handler);
        var client = new SignwellClient(http, configuration, NullLogger<SignwellClient>.Instance);
        var result = await client.GetDocumentStatusAsync("document-fixture");
        Assert.Equal(completed ? "https://signwell.example.test/signed/actual-completed.pdf" : null, result.DocumentUrl);
        Assert.Equal(completed ? 2 : 1, handler.RequestCount);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("false", false)]
    [InlineData("true", true)]
    public async Task SigningUsesExplicitTestModeWithoutChangingLiveDefault(string? configuredMode, bool expectedMode)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Signwell:ApiKey"] = "test-fixture",
            ["Signwell:BaseUrl"] = "https://signwell.example.test/api/v1/",
            ["Signwell:TestMode"] = configuredMode
        }).Build();
        using var handler = new CaptureHandler(expectedMode);
        using var http = new HttpClient(handler);
        var client = new SignwellClient(http, configuration, NullLogger<SignwellClient>.Instance);
        var result = await client.CreateSigningRequestAsync(Guid.NewGuid(), Encoding.ASCII.GetBytes("test-pdf"),
            "synthetic-lease.pdf", new[] { new SignwellSignerRequest { Name = "Test Tenant", Email = "tenant@example.test" } },
            false);
        Assert.Equal("document-fixture", result.ProviderDocumentId);
        Assert.True(handler.Called);
    }

    private sealed class CaptureHandler(bool expectedMode) : HttpMessageHandler
    {
        public bool Called { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Called = true;
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/v1/documents", request.RequestUri!.AbsolutePath);
            using var payload = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            Assert.Equal(expectedMode, payload.RootElement.GetProperty("test_mode").GetBoolean());
            Assert.Equal("synthetic-lease.pdf", payload.RootElement.GetProperty("name").GetString());
            Assert.Equal("tenant@example.test", payload.RootElement.GetProperty("recipients")[0].GetProperty("email").GetString());
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""{"id":"document-fixture","recipients":[]}""", Encoding.UTF8, "application/json")
            };
        }

    }

    private sealed class StatusHandler(string providerStatus) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            Assert.Equal(HttpMethod.Get, request.Method);
            var completed = request.RequestUri!.AbsolutePath.EndsWith("/completed_pdf");
            if (completed) Assert.Equal("?url_only=true", request.RequestUri.Query);
            var body = completed
                ? """{"file_url":"https://signwell.example.test/signed/actual-completed.pdf"}"""
                : JsonSerializer.Serialize(new { id = "document-fixture", status = providerStatus, url = "https://signwell.example.test/unsigned.pdf" });
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        }
    }
}
