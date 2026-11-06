using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class InfobipSmsClient : IInfobipSmsClient
{
    private readonly HttpClient _httpClient;
    private readonly InfobipOptions _options;
    private readonly ILogger<InfobipSmsClient> _logger;

    public InfobipSmsClient(HttpClient httpClient, IOptions<InfobipOptions> options, ILogger<InfobipSmsClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value ?? new InfobipOptions();
        _logger = logger;

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<InfobipSmsResult> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured())
            return InfobipSmsResult.Failure("Infobip phone verification is not configured.");

        var endpoint = BuildEndpoint();
        var payload = new
        {
            messages = new[]
            {
                new
                {
                    destinations = new[] { new { to = phoneNumber } },
                    sender = _options.Sender,
                    content = new { text = message }
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("App", _options.ApiKey.Trim());
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = ExtractErrorMessage(responseBody)
                    ?? $"Infobip SMS request failed with status {(int)response.StatusCode}.";
                _logger.LogWarning("Infobip SMS request failed. Status: {StatusCode}. Message: {Message}", (int)response.StatusCode, error);
                return InfobipSmsResult.Failure(error);
            }

            return InfobipSmsResult.Success();
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(exception, "Infobip SMS request failed.");
            return InfobipSmsResult.Failure("Unable to complete phone verification at this time.");
        }
    }

    private bool IsConfigured()
        => !string.IsNullOrWhiteSpace(_options.ApiKey)
           && !string.IsNullOrWhiteSpace(_options.BaseUrl)
           && !string.IsNullOrWhiteSpace(_options.Sender);

    private Uri BuildEndpoint()
    {
        var baseUrl = _options.BaseUrl.Trim();
        if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = $"https://{baseUrl}";
        }

        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";

        return new Uri(new Uri(baseUrl, UriKind.Absolute), "sms/3/messages");
    }

    private static string? ExtractErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty("requestError", out var requestError)
                && requestError.TryGetProperty("serviceException", out var serviceException)
                && serviceException.TryGetProperty("text", out var errorText)
                && errorText.ValueKind == JsonValueKind.String)
            {
                return errorText.GetString();
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
