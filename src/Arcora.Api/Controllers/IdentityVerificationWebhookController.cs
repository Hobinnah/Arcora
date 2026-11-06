using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/webhooks/identity-verification")]
public sealed class IdentityVerificationWebhookController : ControllerBase
{
    private static readonly TimeSpan PersonaSignatureMaxAge = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ReplayProtectionWindow = TimeSpan.FromMinutes(15);

    private readonly IIdentityVerificationService _identityVerificationService;
    private readonly PersonaOptions _personaOptions;
    private readonly OnfidoOptions _onfidoOptions;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IdentityVerificationWebhookController> _logger;

    public IdentityVerificationWebhookController(
        IIdentityVerificationService identityVerificationService,
        IOptions<PersonaOptions> personaOptions,
        IOptions<OnfidoOptions> onfidoOptions,
        IMemoryCache cache,
        ILogger<IdentityVerificationWebhookController> logger)
    {
        _identityVerificationService = identityVerificationService;
        _personaOptions = personaOptions.Value ?? new PersonaOptions();
        _onfidoOptions = onfidoOptions.Value ?? new OnfidoOptions();
        _cache = cache;
        _logger = logger;
    }

    [HttpPost("persona")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Persona(CancellationToken cancellationToken)
    {
        var correlationId = GetOrSetCorrelationId();
        var payload = await ReadPayloadAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
            return BadRequest(new { message = "Empty payload." });

        if (!ValidatePersonaSignature(payload, out var signatureReason))
        {
            _logger.LogWarning(
                "Rejected Persona webhook due to invalid signature. CorrelationId: {CorrelationId}, Reason: {Reason}",
                correlationId,
                signatureReason);
            return Unauthorized(new { message = "Invalid webhook signature." });
        }

        var replayKey = BuildReplayKey("PERSONA", payload);
        if (IsReplay(replayKey))
            return Ok(new { received = true, duplicate = true });

        var (providerReferenceId, status, failureReason) = ExtractPersonaEvent(payload);
        if (string.IsNullOrWhiteSpace(providerReferenceId))
            return Ok(new { received = true, ignored = true, reason = "missing_provider_reference_id" });

        var updated = await _identityVerificationService.UpdateProviderVerificationStatusAsync(
            "PERSONA",
            providerReferenceId,
            status,
            failureReason,
            "PERSONA_WEBHOOK",
            cancellationToken);

        if (updated == null)
            return Ok(new { received = true, ignored = true, reason = "verification_not_found" });

        MarkReplay(replayKey);

        _logger.LogInformation(
            "Processed Persona verification webhook. CorrelationId: {CorrelationId}, ProviderReferenceId: {ProviderReferenceId}, Status: {Status}",
            correlationId,
            providerReferenceId,
            status);

        return Ok(new { received = true });
    }

    [HttpPost("onfido")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Onfido(CancellationToken cancellationToken)
    {
        var correlationId = GetOrSetCorrelationId();
        var payload = await ReadPayloadAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
            return BadRequest(new { message = "Empty payload." });

        if (!ValidateOnfidoSignature(payload))
        {
            _logger.LogWarning("Rejected Onfido webhook due to invalid signature. CorrelationId: {CorrelationId}", correlationId);
            return Unauthorized(new { message = "Invalid webhook signature." });
        }

        var replayKey = BuildReplayKey("ONFIDO", payload);
        if (IsReplay(replayKey))
            return Ok(new { received = true, duplicate = true });

        var (providerReferenceId, status, failureReason) = ExtractOnfidoEvent(payload);
        if (string.IsNullOrWhiteSpace(providerReferenceId))
            return Ok(new { received = true, ignored = true, reason = "missing_provider_reference_id" });

        var updated = await _identityVerificationService.UpdateProviderVerificationStatusAsync(
            "ONFIDO",
            providerReferenceId,
            status,
            failureReason,
            "ONFIDO_WEBHOOK",
            cancellationToken);

        if (updated == null)
            return Ok(new { received = true, ignored = true, reason = "verification_not_found" });

        MarkReplay(replayKey);

        _logger.LogInformation(
            "Processed Onfido verification webhook. CorrelationId: {CorrelationId}, ProviderReferenceId: {ProviderReferenceId}, Status: {Status}",
            correlationId,
            providerReferenceId,
            status);

        return Ok(new { received = true });
    }

    private string GetOrSetCorrelationId()
    {
        var correlationId = Request.Headers["X-Correlation-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = HttpContext.TraceIdentifier;

        Response.Headers["X-Correlation-ID"] = correlationId;
        return correlationId;
    }

    private async Task<string> ReadPayloadAsync(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private bool ValidatePersonaSignature(string payload, out string reason)
    {
        reason = "invalid_signature";

        if (string.IsNullOrWhiteSpace(_personaOptions.WebhookSecret))
        {
            reason = "missing_webhook_secret";
            return false;
        }

        var signatureHeader = Request.Headers["Persona-Signature"].FirstOrDefault()
            ?? Request.Headers["X-Persona-Signature"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            reason = "missing_signature_header";
            return false;
        }

        if (!TryParseVersionedSignature(signatureHeader, out var timestampSeconds, out var signature))
        {
            reason = "invalid_signature_format";
            return false;
        }

        var timestamp = DateTimeOffset.FromUnixTimeSeconds(timestampSeconds);
        var age = DateTimeOffset.UtcNow - timestamp;
        if (age.Duration() > PersonaSignatureMaxAge)
        {
            reason = "signature_expired";
            return false;
        }

        var expected = ComputeHexHmac($"{timestampSeconds}.{payload}", _personaOptions.WebhookSecret);
        if (!FixedTimeEqualsHex(expected, signature))
        {
            reason = "signature_mismatch";
            return false;
        }

        return true;
    }

    private bool ValidateOnfidoSignature(string payload)
    {
        if (string.IsNullOrWhiteSpace(_onfidoOptions.WebhookSecret))
            return false;

        var signatureHeader = Request.Headers["X-SHA2-Signature"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(signatureHeader))
            return false;

        var expected = ComputeHexHmac(payload, _onfidoOptions.WebhookSecret);
        return FixedTimeEqualsHex(expected, signatureHeader);
    }

    private static (string? ProviderReferenceId, string Status, string? FailureReason) ExtractPersonaEvent(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        var providerReferenceId = TryGetString(root, "data", "attributes", "payload", "data", "id")
            ?? TryGetString(root, "inquiry_id")
            ?? TryGetString(root, "inquiry-id")
            ?? TryGetString(root, "data", "id");

        var eventName = TryGetString(root, "data", "attributes", "name")
            ?? TryGetString(root, "name")
            ?? TryGetString(root, "event_name");

        var status = TryGetString(root, "data", "attributes", "payload", "data", "attributes", "status")
            ?? TryGetString(root, "data", "attributes", "status")
            ?? MapPersonaEventNameToStatus(eventName)
            ?? "PENDING";

        var failureReason = TryGetString(root, "data", "attributes", "payload", "data", "attributes", "fields", "reason")
            ?? TryGetString(root, "error", "message")
            ?? TryGetString(root, "message");

        return (providerReferenceId, status, failureReason);
    }

    private static string? MapPersonaEventNameToStatus(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return null;

        var normalized = eventName.Trim().ToLowerInvariant();
        if (normalized.Contains("approved"))
            return "APPROVED";
        if (normalized.Contains("marked-for-review") || normalized.Contains("review"))
            return "UNDER_REVIEW";
        if (normalized.Contains("declined") || normalized.Contains("failed"))
            return "FAILED";
        if (normalized.Contains("expired"))
            return "EXPIRED";
        if (normalized.Contains("canceled") || normalized.Contains("cancelled"))
            return "CANCELED";
        if (normalized.Contains("completed"))
            return "SUBMITTED";
        if (normalized.Contains("started") || normalized.Contains("created"))
            return "PENDING";

        return null;
    }

    private static (string? ProviderReferenceId, string Status, string? FailureReason) ExtractOnfidoEvent(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        var providerReferenceId = TryGetString(root, "payload", "resource", "applicant_id")
            ?? TryGetString(root, "payload", "object", "applicant_id")
            ?? TryGetString(root, "resource", "applicant_id")
            ?? TryGetString(root, "applicant_id");

        var eventName = TryGetString(root, "action")
            ?? TryGetString(root, "payload", "action")
            ?? TryGetString(root, "payload", "resource_type")
            ?? TryGetString(root, "event");

        var status = TryGetString(root, "payload", "resource", "status")
            ?? TryGetString(root, "payload", "object", "status")
            ?? TryGetString(root, "resource", "status")
            ?? TryGetString(root, "status")
            ?? MapEventNameToStatus(eventName)
            ?? "PENDING";

        var failureReason = TryGetString(root, "payload", "resource", "result")
            ?? TryGetString(root, "payload", "object", "result")
            ?? TryGetString(root, "error", "message")
            ?? TryGetString(root, "message");

        return (providerReferenceId, status, failureReason);
    }

    private static string? MapEventNameToStatus(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return null;

        var normalized = eventName.Trim().ToLowerInvariant();
        if (normalized.Contains("approve") || normalized.Contains("verified") || normalized.Contains("pass") || normalized.Contains("complete"))
            return "APPROVED";
        if (normalized.Contains("declin") || normalized.Contains("reject") || normalized.Contains("fail") || normalized.Contains("error"))
            return "FAILED";
        if (normalized.Contains("review"))
            return "UNDER_REVIEW";
        if (normalized.Contains("expire"))
            return "EXPIRED";
        if (normalized.Contains("cancel"))
            return "CANCELED";

        return "PENDING";
    }

    private static string ComputeHexHmac(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static bool TryParseVersionedSignature(string signatureHeader, out long timestampSeconds, out string signature)
    {
        timestampSeconds = 0;
        signature = string.Empty;

        var parts = signatureHeader.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (part.StartsWith("t=", StringComparison.OrdinalIgnoreCase)
                && long.TryParse(part[2..], out var parsed))
                timestampSeconds = parsed;
            else if (part.StartsWith("v1=", StringComparison.OrdinalIgnoreCase))
                signature = part[3..];
        }

        return timestampSeconds > 0 && !string.IsNullOrWhiteSpace(signature);
    }

    private static bool FixedTimeEqualsHex(string expectedHex, string providedHex)
    {
        var expected = expectedHex.Trim().ToLowerInvariant();
        var provided = providedHex.Trim().ToLowerInvariant();
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    private static string? TryGetString(JsonElement element, params string[] path)
    {
        var current = element;
        foreach (var segment in path)
        {
            if (!current.TryGetProperty(segment, out current))
                return null;
        }

        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
    }

    private static string BuildReplayKey(string providerName, string payload)
        => $"identity-verification-webhook:{providerName}:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)))}";

    private bool IsReplay(string replayKey)
        => _cache.TryGetValue(replayKey, out _);

    private void MarkReplay(string replayKey)
        => _cache.Set(replayKey, true, ReplayProtectionWindow);
}
