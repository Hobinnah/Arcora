using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Arcora.Api.DTOs;
using Microsoft.AspNetCore.WebUtilities;

namespace Arcora.Api.Services.Implementations;

public sealed class AzureMapsService(IHttpClientFactory clients, IConfiguration configuration, IHostEnvironment environment)
{
    private readonly TokenCredential credential = CreateCredential(configuration, environment);

    private static TokenCredential CreateCredential(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
            return new ManagedIdentityCredential(configuration["Azure:Maps:ManagedIdentityClientId"]);

        var tenantID = configuration["Azure:Maps:TenantId"];
        var applicationClientID = configuration["Azure:Maps:ApplicationClientId"];
        var clientSecret = configuration["Azure:Maps:ClientSecret"];
        var supplied = new[] { tenantID, applicationClientID, clientSecret }
            .Count(value => !string.IsNullOrWhiteSpace(value));
        if (supplied == 0)
            return new DefaultAzureCredential();
        if (supplied != 3)
            throw new InvalidOperationException(
                "Azure Maps development authentication requires Azure:Maps:TenantId, Azure:Maps:ApplicationClientId and Azure:Maps:ClientSecret together.");

        return new ClientSecretCredential(tenantID, applicationClientID, clientSecret);
    }

    public string? ClientID => configuration["Azure:Maps:ClientId"];
    public bool CanGeocode => !string.IsNullOrWhiteSpace(configuration["Azure:Maps:SubscriptionKey"]) || !string.IsNullOrWhiteSpace(ClientID);

    public ValueTask<AccessToken> GetTokenAsync(CancellationToken ct) =>
        credential.GetTokenAsync(new TokenRequestContext(["https://atlas.microsoft.com/.default"]), ct);

    public async Task<GeocodedAddressLocationDto?> GeocodeAsync(AddressDto address, CancellationToken ct)
    {
        var key = configuration["Azure:Maps:SubscriptionKey"];
        var url = QueryHelpers.AddQueryString("https://atlas.microsoft.com/geocode", new Dictionary<string, string?>
        {
            ["api-version"] = "2025-01-01", ["top"] = "1", ["addressLine"] = address.Line1,
            ["locality"] = address.City, ["adminDistrict"] = address.ProvinceCode,
            ["postalCode"] = address.PostalCode, ["countryRegion"] = address.CountryCode
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.Add("subscription-key", key);
        else
        {
            var token = await GetTokenAsync(ct);
            request.Headers.Authorization = new("Bearer", token.Token);
            request.Headers.Add("x-ms-client-id", ClientID);
        }
        using var response = await clients.CreateClient("AzureMaps").SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Azure Maps geocoding is unavailable.", null, response.StatusCode);
        var collection = await response.Content.ReadFromJsonAsync<FeatureCollection>(cancellationToken: ct);
        if (collection?.Features == null)
            throw new JsonException("Azure Maps returned no feature collection.");
        var match = collection?.Features?.FirstOrDefault();
        if (match == null)
            return null;
        if (match.Properties?.Type != "Address" ||
            (match.Properties.Confidence != "High" && match.Properties.Confidence != "Medium") ||
            match.Properties.MatchCodes?.Contains("Good") != true)
            return null;
        var coordinates = match.Geometry?.Coordinates;
        if (match.Geometry?.Type != "Point" || coordinates?.Length != 2 ||
            Helpers.PublicMapLocation.From(coordinates[1], coordinates[0]) == null)
            throw new JsonException("Azure Maps returned invalid coordinates.");
        return new(coordinates[1], coordinates[0]);
    }

    private sealed class FeatureCollection
    {
        public Feature[]? Features { get; set; }
    }
    private sealed class Feature
    {
        public FeatureProperties? Properties { get; set; }
        public Geometry? Geometry { get; set; }
    }
    private sealed class FeatureProperties
    {
        public string? Type { get; set; }
        public string? Confidence { get; set; }
        public string[]? MatchCodes { get; set; }
    }
    private sealed class Geometry
    {
        public string? Type { get; set; }
        public decimal[]? Coordinates { get; set; }
    }
}
