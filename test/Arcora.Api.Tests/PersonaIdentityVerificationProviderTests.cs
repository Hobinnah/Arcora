using System.Net;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class PersonaIdentityVerificationProviderTests
{
    [Fact]
    public async Task StartVerificationUsesPersonaInquiryAttributesAndHostedRedirect()
    {
        string? requestBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://persona.test/api/v1/inquiries"), request.RequestUri);
            Assert.Equal("Bearer test-key", request.Headers.Authorization?.ToString());
            requestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""{"data":{"id":"inq_123","attributes":{"status":"created"}}}""")
            };
        });
        using var httpClient = new HttpClient(handler);
        var provider = new PersonaIdentityVerificationProvider(
            httpClient,
            Options.Create(new PersonaOptions
            {
                BaseUrl = "https://persona.test/api/v1",
                ApiKey = "test-key",
                TemplateId = "itmpl_123",
                HostedVerificationUrlTemplate = "https://inquiry.withpersona.com/verify?inquiry-id={inquiryId}&language=en"
            }),
            NullLogger<PersonaIdentityVerificationProvider>.Instance);

        var result = await provider.StartVerificationAsync(new IdentityVerificationProviderRequest(
            42,
            "IDENTITY",
            "42",
            "user@example.test",
            "First",
            "Last",
            "https://arcora.test/verify-identity"));

        using var requestDocument = JsonDocument.Parse(requestBody!);
        var data = requestDocument.RootElement.GetProperty("data");
        var attributes = data.GetProperty("attributes");
        Assert.Equal("inquiry", data.GetProperty("type").GetString());
        Assert.Equal("itmpl_123", attributes.GetProperty("inquiry-template-id").GetString());
        Assert.Equal("42", attributes.GetProperty("reference-id").GetString());
        Assert.False(data.TryGetProperty("relationships", out _));
        Assert.Equal("inq_123", result.ProviderReferenceId);
        Assert.Equal("created", result.Status);
        Assert.Contains("inquiry-id=inq_123", result.VerificationUrl);
        Assert.Contains("language=en", result.VerificationUrl);
        Assert.Contains("redirect-uri=https%3A%2F%2Farcora.test%2Fverify-identity", result.VerificationUrl);
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => sendAsync(request, cancellationToken);
    }
}
