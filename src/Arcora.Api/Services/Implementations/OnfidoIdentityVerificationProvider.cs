using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class OnfidoIdentityVerificationProvider : IIdentityVerificationProvider
{
    private readonly HttpClient _httpClient;
    private readonly OnfidoOptions _options;
    private readonly ILogger<OnfidoIdentityVerificationProvider> _logger;

    public OnfidoIdentityVerificationProvider(HttpClient httpClient, IOptions<OnfidoOptions> options, ILogger<OnfidoIdentityVerificationProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value ?? new OnfidoOptions();
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = EnsureTrailingSlash(_options.BaseUrl);

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public string ProviderName => "ONFIDO";

    public async Task<IdentityVerificationProviderResult> StartVerificationAsync(IdentityVerificationProviderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            return new IdentityVerificationProviderResult(ProviderName, "CONFIGURATION_ERROR", null, null, null, null, "Onfido provider is not configured.");

        try
        {
            var applicantId = await CreateApplicantAsync(request, cancellationToken);
            if (string.IsNullOrWhiteSpace(applicantId))
                return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, "Onfido applicant could not be created.");

            var clientToken = await CreateSdkTokenAsync(applicantId, cancellationToken);
            var verificationUrl = BuildHostedUrl(applicantId, clientToken);

            return new IdentityVerificationProviderResult(ProviderName, "PENDING", applicantId, verificationUrl, clientToken, null, null);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(exception, "Onfido verification start failed.");
            return new IdentityVerificationProviderResult(ProviderName, "FAILED", null, null, null, null, "Unable to start Onfido identity verification.");
        }
    }

    public Task<IdentityVerificationProviderResult> GetVerificationStatusAsync(string providerReferenceId, CancellationToken cancellationToken = default)
        => Task.FromResult(new IdentityVerificationProviderResult(
            ProviderName,
            "PENDING",
            providerReferenceId,
            null,
            null,
            null,
            "Onfido status polling is not implemented. Use webhooks for updates."));

    private async Task<string?> CreateApplicantAsync(IdentityVerificationProviderRequest request, CancellationToken cancellationToken)
    {
        var payload = new
        {
            first_name = request.FirstName ?? "Unknown",
            last_name = request.LastName ?? "User",
            email = request.Email,
            external_id = request.ReferenceId
        };

        var responseBody = await SendJsonAsync("applicants", payload, cancellationToken);
        using var document = JsonDocument.Parse(responseBody);
        return document.RootElement.TryGetProperty("id", out var idProperty) && idProperty.ValueKind == JsonValueKind.String
            ? idProperty.GetString()
            : null;
    }

    private async Task<string?> CreateSdkTokenAsync(string applicantId, CancellationToken cancellationToken)
    {
        var payload = new Dictionary<string, object?>
        {
            ["applicant_id"] = applicantId
        };

        if (!string.IsNullOrWhiteSpace(_options.WorkflowRunId))
            payload["workflow_run_id"] = _options.WorkflowRunId;

        var responseBody = await SendJsonAsync("sdk_token", payload, cancellationToken);
        using var document = JsonDocument.Parse(responseBody);
        return document.RootElement.TryGetProperty("token", out var tokenProperty) && tokenProperty.ValueKind == JsonValueKind.String
            ? tokenProperty.GetString()
            : null;
    }

    private async Task<string> SendJsonAsync(string requestPath, object payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestPath)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", $"token={_options.ApiKey}");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = ExtractErrorMessage(responseBody) ?? $"Onfido request failed with status {(int)response.StatusCode}.";
            _logger.LogWarning("Onfido request to {RequestPath} failed. Status: {StatusCode}. Message: {Message}", requestPath, (int)response.StatusCode, errorMessage);
            throw new HttpRequestException(errorMessage);
        }

        return responseBody;
    }

    private string? BuildHostedUrl(string applicantId, string? clientToken)
    {
        if (string.IsNullOrWhiteSpace(_options.HostedVerificationUrlTemplate))
            return null;

        return _options.HostedVerificationUrlTemplate
            .Replace("{applicantId}", Uri.EscapeDataString(applicantId), StringComparison.OrdinalIgnoreCase)
            .Replace("{sdkToken}", Uri.EscapeDataString(clientToken ?? string.Empty), StringComparison.OrdinalIgnoreCase);
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
            if (root.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.String)
                return error.GetString();
            if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                return message.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
