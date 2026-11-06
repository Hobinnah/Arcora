using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class PersonaIdentityVerificationProvider : IIdentityVerificationProvider
{
    private readonly HttpClient _httpClient;
    private readonly PersonaOptions _options;
    private readonly ILogger<PersonaIdentityVerificationProvider> _logger;

    public PersonaIdentityVerificationProvider(HttpClient httpClient, IOptions<PersonaOptions> options, ILogger<PersonaIdentityVerificationProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value ?? new PersonaOptions();
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = EnsureTrailingSlash(_options.BaseUrl);

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public string ProviderName => "PERSONA";

    public async Task<IdentityVerificationProviderResult> StartVerificationAsync(IdentityVerificationProviderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey)
            || string.IsNullOrWhiteSpace(_options.TemplateId)
            || string.IsNullOrWhiteSpace(_options.HostedVerificationUrlTemplate)
            || !_options.HostedVerificationUrlTemplate.Contains("{inquiryId}", StringComparison.OrdinalIgnoreCase))
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, "Persona provider is not configured with a hosted verification URL template.");

        var payload = new
        {
            data = new
            {
                type = "inquiry",
                attributes = new Dictionary<string, string>
                {
                    ["inquiry-template-id"] = _options.TemplateId,
                    ["reference-id"] = request.ReferenceId
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "inquiries")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(responseBody) ?? $"Persona request failed with status {(int)response.StatusCode}.";
                _logger.LogWarning("Persona verification start failed. Status: {StatusCode}. Message: {Message}", (int)response.StatusCode, errorMessage);
                return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, errorMessage);
            }

            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            var data = root.GetProperty("data");
            var providerReferenceId = data.GetProperty("id").GetString();
            var status = TryGetString(data, "attributes", "status") ?? "PENDING";
            var expiresAt = TryGetDateTimeOffset(data, "attributes", "expired-at")?.UtcDateTime;
            var verificationUrl = BuildHostedUrl(providerReferenceId, request.RedirectUrl);

            return new IdentityVerificationProviderResult(ProviderName, status, providerReferenceId, verificationUrl, null, expiresAt, null);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(exception, "Persona verification start failed.");
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, "Unable to start Persona identity verification.");
        }
    }

    public async Task<IdentityVerificationProviderResult> GetVerificationStatusAsync(string providerReferenceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerReferenceId))
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, "Persona provider reference ID is required.");

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", providerReferenceId, null, null, null, "Persona provider is not configured.");

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"inquiries/{Uri.EscapeDataString(providerReferenceId)}");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(responseBody) ?? $"Persona status request failed with status {(int)response.StatusCode}.";
                _logger.LogWarning("Persona verification status check failed. Status: {StatusCode}. Message: {Message}", (int)response.StatusCode, errorMessage);
                return new IdentityVerificationProviderResult(ProviderName, "FAILED", providerReferenceId, null, null, null, errorMessage);
            }

            using var document = JsonDocument.Parse(responseBody);
            var data = document.RootElement.GetProperty("data");
            var status = TryGetString(data, "attributes", "status") ?? "PENDING";
            var expiresAt = TryGetDateTimeOffset(data, "attributes", "expired-at")?.UtcDateTime;
            var failureReason = TryGetString(data, "attributes", "fields", "reason");

            return new IdentityVerificationProviderResult(ProviderName, status, providerReferenceId, null, null, expiresAt, failureReason);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(exception, "Persona verification status check failed.");
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", providerReferenceId, null, null, null, "Unable to check Persona identity verification status.");
        }
    }

    private string? BuildHostedUrl(string? inquiryId, string? redirectUrl)
    {
        if (string.IsNullOrWhiteSpace(_options.HostedVerificationUrlTemplate) || string.IsNullOrWhiteSpace(inquiryId))
            return null;

        var builder = new UriBuilder(_options.HostedVerificationUrlTemplate.Replace(
            "{inquiryId}",
            Uri.EscapeDataString(inquiryId),
            StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(redirectUrl))
        {
            var queryParameters = builder.Query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Where(parameter => !string.Equals(
                    Uri.UnescapeDataString(parameter.Split('=', 2)[0]),
                    "redirect-uri",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            queryParameters.Add($"redirect-uri={Uri.EscapeDataString(redirectUrl)}");
            builder.Query = string.Join("&", queryParameters);
        }

        return builder.Uri.ToString();
    }

    private static Uri EnsureTrailingSlash(string baseUrl)
    {
        var normalized = baseUrl.EndsWith('/') ? baseUrl : $"{baseUrl}/";
        return new Uri(normalized, UriKind.Absolute);
    }

    private static string? ExtractErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            if (root.TryGetProperty("errors", out var errors)
                && errors.ValueKind == JsonValueKind.Array
                && errors.GetArrayLength() > 0)
            {
                var first = errors[0];
                if (first.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                    return detail.GetString();
                if (first.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    return title.GetString();
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }

    private static string? TryGetString(JsonElement element, params string[] path)
    {
        if (!TryNavigate(element, out var current, path) || current.ValueKind != JsonValueKind.String)
            return null;

        return current.GetString();
    }

    private static DateTimeOffset? TryGetDateTimeOffset(JsonElement element, params string[] path)
    {
        var value = TryGetString(element, path);
        return DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
    }

    private static bool TryNavigate(JsonElement element, out JsonElement current, params string[] path)
    {
        current = element;
        foreach (var segment in path)
        {
            if (!current.TryGetProperty(segment, out current))
                return false;
        }

        return true;
    }
}
