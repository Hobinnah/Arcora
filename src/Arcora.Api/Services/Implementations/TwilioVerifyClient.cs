using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class TwilioVerifyClient : ITwilioVerifyClient
{
    private readonly HttpClient _httpClient;
    private readonly TwilioVerifyOptions _options;
    private readonly ILogger<TwilioVerifyClient> _logger;

    public TwilioVerifyClient(HttpClient httpClient, IOptions<TwilioVerifyOptions> options, ILogger<TwilioVerifyClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value ?? new TwilioVerifyOptions();
        _logger = logger;

        _httpClient.BaseAddress ??= new Uri("https://verify.twilio.com/v2/");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Task<TwilioVerifyResult> StartVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => PostAsync($"Services/{_options.ServiceSid}/Verifications", new Dictionary<string, string>
        {
            ["To"] = phoneNumber,
            ["Channel"] = string.IsNullOrWhiteSpace(_options.Channel) ? "sms" : _options.Channel.Trim().ToLowerInvariant()
        }, cancellationToken);

    public Task<TwilioVerifyResult> CheckVerificationAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
        => PostAsync($"Services/{_options.ServiceSid}/VerificationCheck", new Dictionary<string, string>
        {
            ["To"] = phoneNumber,
            ["Code"] = code.Trim()
        }, cancellationToken);

    private async Task<TwilioVerifyResult> PostAsync(string requestPath, Dictionary<string, string> payload, CancellationToken cancellationToken)
    {
        if (!IsConfigured())
            return TwilioVerifyResult.Failure("Phone verification is not configured.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestPath)
            {
                Content = new FormUrlEncodedContent(payload)
            };

            request.Headers.Authorization = BuildAuthorizationHeader();
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(responseBody)
                    ?? $"Twilio verification request failed with status {(int)response.StatusCode}.";
                _logger.LogWarning("Twilio Verify request failed. Status: {StatusCode}. Message: {Message}", (int)response.StatusCode, errorMessage);
                return TwilioVerifyResult.Failure(errorMessage);
            }

            return ParseResponse(responseBody);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(exception, "Twilio Verify request failed.");
            return TwilioVerifyResult.Failure("Unable to complete phone verification at this time.");
        }
    }

    private bool IsConfigured()
        => !string.IsNullOrWhiteSpace(_options.AccountSid)
           && !string.IsNullOrWhiteSpace(_options.AuthToken)
           && !string.IsNullOrWhiteSpace(_options.ServiceSid);

    private AuthenticationHeaderValue BuildAuthorizationHeader()
    {
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.AccountSid}:{_options.AuthToken}"));
        return new AuthenticationHeaderValue("Basic", credentials);
    }

    private static TwilioVerifyResult ParseResponse(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        var status = root.TryGetProperty("status", out var statusProperty) && statusProperty.ValueKind == JsonValueKind.String
            ? statusProperty.GetString() ?? string.Empty
            : string.Empty;

        var sid = root.TryGetProperty("sid", out var sidProperty) && sidProperty.ValueKind == JsonValueKind.String
            ? sidProperty.GetString()
            : null;

        var isApproved = string.Equals(status, "approved", StringComparison.OrdinalIgnoreCase);
        if (root.TryGetProperty("valid", out var validProperty) && validProperty.ValueKind is JsonValueKind.True or JsonValueKind.False)
            isApproved |= validProperty.GetBoolean();

        return TwilioVerifyResult.Success(string.IsNullOrWhiteSpace(status) ? "pending" : status, sid, isApproved);
    }

    private static string? ExtractErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            if (root.TryGetProperty("message", out var messageProperty) && messageProperty.ValueKind == JsonValueKind.String)
                return messageProperty.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
