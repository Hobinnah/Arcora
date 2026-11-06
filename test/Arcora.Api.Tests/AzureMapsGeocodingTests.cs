using System.Net;
using System.Text;
using System.Text.Json;
using System.Reflection;
using Azure.Identity;
using Arcora.Api.DTOs;
using Arcora.Api.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Arcora.Api.Tests;

public sealed class AzureMapsGeocodingTests
{
    [Theory]
    [InlineData("Development", 0, typeof(DefaultAzureCredential))]
    [InlineData("Development", 7, typeof(ClientSecretCredential))]
    [InlineData("Production", 7, typeof(ManagedIdentityCredential))]
    [InlineData("Production", 1, typeof(ManagedIdentityCredential))]
    public void CredentialSelectionRespectsDevelopmentBoundary(string environment, int fields, Type expected)
    {
        var maps = ConfiguredMaps(environment, fields);
        var credential = typeof(AzureMapsService).GetField("credential", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(maps);
        Assert.IsType(expected, credential);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void PartialDevelopmentCredentialsFailExplicitlyWithoutLeakingValues(int fields)
    {
        var error = Assert.Throws<InvalidOperationException>(() => ConfiguredMaps("Development", fields));
        Assert.Contains("requires Azure:Maps:TenantId", error.Message);
        Assert.DoesNotContain("synthetic-secret-value", error.Message);
    }

    private static AzureMapsService ConfiguredMaps(string environment, int fields)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Azure:Maps:TenantId"] = (fields & 1) != 0 ? "11111111-1111-1111-1111-111111111111" : null,
            ["Azure:Maps:ApplicationClientId"] = (fields & 2) != 0 ? "22222222-2222-2222-2222-222222222222" : null,
            ["Azure:Maps:ClientSecret"] = (fields & 4) != 0 ? "synthetic-secret-value" : null
        }).Build();
        return new AzureMapsService(new Clients(new Handler(_ =>
            throw new InvalidOperationException("Credential selection must not make HTTP requests."))),
            configuration, new Environment { EnvironmentName = environment });
    }

    [Fact]
    public void ConfigurationReadsMapsNestedUnderAzure()
    {
        using var json = new MemoryStream(Encoding.UTF8.GetBytes(
            """{"Azure":{"Maps":{"ClientId":"synthetic-maps-client"}}}"""));
        var maps = new AzureMapsService(new Clients(new Handler(_ =>
            throw new InvalidOperationException("Configuration must not make HTTP requests."))),
            new ConfigurationBuilder().AddJsonStream(json).Build(), new Environment());
        Assert.Equal("synthetic-maps-client", maps.ClientID);
        Assert.True(maps.CanGeocode);
    }

    [Fact]
    public async Task GeocodingUsesStructuredStreetAddressAndKeepsTheKeyOutOfTheUrl()
    {
        var handler = new Handler(request =>
        {
            Assert.Equal("atlas.microsoft.com", request.RequestUri!.Host);
            Assert.Contains("api-version=2025-01-01", request.RequestUri.Query);
            Assert.Contains("addressLine=1%20Synthetic%20Street", request.RequestUri.Query);
            Assert.DoesNotContain("SyntheticUnit", request.RequestUri.Query);
            Assert.DoesNotContain("private-test-key", request.RequestUri.ToString());
            Assert.Equal("private-test-key", request.Headers.GetValues("subscription-key").Single());
            return Reply(Response());
        });
        var result = await Maps(handler).GeocodeAsync(Address(), default);
        Assert.Equal(43.6501m, result!.Latitude);
        Assert.Equal(-79.3833m, result.Longitude);
    }

    [Theory]
    [InlineData("Low", "Address", "Good")]
    [InlineData("High", "PopulatedPlace", "Good")]
    [InlineData("High", "Address", "Ambiguous")]
    public async Task UnreliableMatchesNeverPretendToBeAnExactHome(string confidence, string type, string code)
    {
        var maps = Maps(new Handler(_ => Reply(Response(confidence, type, code))));
        Assert.Null(await maps.GeocodeAsync(Address(), default));
    }

    [Fact]
    public async Task MissingMatchesRemainUnlocated()
    {
        Assert.Null(await Maps(new Handler(_ => Reply("""{"type":"FeatureCollection","features":[]}""")))
            .GeocodeAsync(Address(), default));
    }

    [Fact]
    public async Task MalformedProviderCoordinatesAreAnExplicitFailure()
    {
        var maps = Maps(new Handler(_ => Reply(Response().Replace("[ -79.3833, 43.6501 ]", "[ -79.3833, 999 ]"))));
        await Assert.ThrowsAsync<JsonException>(() => maps.GeocodeAsync(Address(), default));
    }

    [Fact]
    public async Task ProviderOutageIsNotReportedAsNoMatchingAddress()
    {
        var maps = Maps(new Handler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)));
        var error = await Assert.ThrowsAsync<HttpRequestException>(() => maps.GeocodeAsync(Address(), default));
        Assert.Equal(HttpStatusCode.TooManyRequests, error.StatusCode);
    }

    private static AddressDto Address() => new() { Line1 = "1 Synthetic Street", Line2 = "SyntheticUnit", City = "Toronto",
        ProvinceCode = "ON", PostalCode = "M5H 1A1", CountryCode = "CA" };
    private static HttpResponseMessage Reply(string json) => new(HttpStatusCode.OK)
    { Content = new StringContent(json, Encoding.UTF8, "application/geo+json") };
    private static string Response(string confidence = "High", string type = "Address", string code = "Good") => $$$"""
        {"type":"FeatureCollection","features":[{"properties":{"type":"{{{type}}}","confidence":"{{{confidence}}}","matchCodes":["{{{code}}}"]},
        "geometry":{"type":"Point","coordinates":[ -79.3833, 43.6501 ]}}]}
        """;
    private static AzureMapsService Maps(HttpMessageHandler handler) => new(new Clients(handler),
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["Azure:Maps:SubscriptionKey"] = "private-test-key" }).Build(), new Environment());
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(respond(request));
    }
    private sealed class Clients(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }
    private sealed class Environment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
