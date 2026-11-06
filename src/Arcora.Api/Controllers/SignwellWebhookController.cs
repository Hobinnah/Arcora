using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Signing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/webhooks/signwell")]
    public class SignwellWebhookController : ControllerBase
    {
        private readonly ArcoraDbContext _dbContext;
        private readonly ILogger<SignwellWebhookController> _logger;
        private readonly ILeaseFirstPaymentService _firstPaymentService;
        private readonly string? _webhookSecret;

        public SignwellWebhookController(
            ArcoraDbContext dbContext,
            IConfiguration configuration,
            ILeaseFirstPaymentService firstPaymentService,
            ILogger<SignwellWebhookController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _firstPaymentService = firstPaymentService;
            _webhookSecret = configuration["Signwell:WebhookSecret"];
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(payload))
                return Ok(new { received = true, ignored = true, reason = "empty_payload" });

            if (string.IsNullOrWhiteSpace(_webhookSecret))
            {
                _logger.LogError("Signwell webhook secret is not configured.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Webhook signing secret not configured." });
            }

            var signature = Request.Headers["X-Signwell-Signature"].FirstOrDefault()
                ?? Request.Headers["X-SW-Signature"].FirstOrDefault();

            if (!IsValidWebhookSignature(payload, signature, _webhookSecret))
            {
                _logger.LogWarning("Rejected Signwell webhook due to invalid signature.");
                return Unauthorized(new { message = "Invalid webhook signature." });
            }

            try
            {
                using var json = JsonDocument.Parse(payload);
                var root = json.RootElement;

                var eventType = ExtractString(root, "event", "event_type", "type")?.ToLowerInvariant() ?? string.Empty;
                var documentId = ExtractDocumentId(root);
                var providerSignerId = ExtractSignerId(root);
                var signerEmail = ExtractSignerEmail(root);

                if (string.IsNullOrWhiteSpace(documentId))
                {
                    _logger.LogWarning("Signwell webhook ignored due to missing document id. EventType: {EventType}", eventType);
                    return Ok(new { received = true, ignored = true, reason = "missing_document_id" });
                }

                var allSignatories = await _dbContext.LeaseSignatories
                    .Where(x => x.ProviderDocumentID == documentId)
                    .ToListAsync(cancellationToken);

                if (allSignatories.Count == 0)
                {
                    _logger.LogWarning("Signwell webhook document id {DocumentId} not found in local signatories.", documentId);
                    return Ok(new { received = true, ignored = true, reason = "document_not_found" });
                }

                var matchedSignatories = allSignatories.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(providerSignerId))
                    matchedSignatories = matchedSignatories.Where(x => string.Equals(x.ProviderSignerID, providerSignerId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(signerEmail))
                    matchedSignatories = matchedSignatories.Where(x => string.Equals(x.Email, signerEmail, StringComparison.OrdinalIgnoreCase));

                var targeted = matchedSignatories.ToList();
                if (targeted.Count == 0)
                    targeted = allSignatories;

                var now = DateTime.UtcNow;
                Guid? leaseToInitiateFirstCharge = null;
                var isCompletedEvent = eventType.Contains("completed", StringComparison.Ordinal);
                var isSignedEvent = eventType.Contains("signed", StringComparison.Ordinal);
                var isViewedEvent = eventType.Contains("viewed", StringComparison.Ordinal) || eventType.Contains("opened", StringComparison.Ordinal);
                var isDeclinedEvent = eventType.Contains("declined", StringComparison.Ordinal) || eventType.Contains("rejected", StringComparison.Ordinal);

                if (isDeclinedEvent)
                {
                    foreach (var signatory in targeted)
                    {
                        signatory.Status = "DECLINED";
                        signatory.DeclinedAt ??= now;
                    }
                }
                else if (isCompletedEvent)
                {
                    foreach (var signatory in allSignatories)
                    {
                        if (!string.Equals(signatory.Status, "DECLINED", StringComparison.OrdinalIgnoreCase))
                        {
                            signatory.Status = "SIGNED";
                            signatory.SignedAt ??= now;
                        }
                    }
                }
                else if (isSignedEvent)
                {
                    foreach (var signatory in targeted)
                    {
                        signatory.Status = "SIGNED";
                        signatory.SignedAt ??= now;
                    }
                }
                else if (isViewedEvent)
                {
                    foreach (var signatory in targeted)
                    {
                        if (!string.Equals(signatory.Status, "SIGNED", StringComparison.OrdinalIgnoreCase))
                            signatory.Status = "VIEWED";

                        signatory.ViewedAt ??= now;
                    }
                }

                var leaseDocumentId = allSignatories.Select(x => x.LeaseDocumentID).FirstOrDefault();
                if (leaseDocumentId != Guid.Empty)
                {
                    var leaseDocument = await _dbContext.LeaseDocuments
                        .FirstOrDefaultAsync(x => x.LeaseDocumentID == leaseDocumentId, cancellationToken);

                    if (leaseDocument != null)
                    {
                        if (isDeclinedEvent)
                        {
                            leaseDocument.DocumentStatus = "DECLINED";
                        }
                        else if (isCompletedEvent)
                        {
                            leaseDocument.DocumentStatus = "FULLY_SIGNED";
                            leaseDocument.FullySignedAt ??= now;

                            if (leaseDocument.LeaseID.HasValue)
                            {
                                var lease = await _dbContext.Leases.FirstOrDefaultAsync(x => x.LeaseID == leaseDocument.LeaseID.Value, cancellationToken);
                                if (lease != null)
                                {
                                    if (await LeaseSigningCompletion.ReconcileAsync(_dbContext, lease,
                                        leaseDocument.FullySignedAt.Value, "SIGNWELL_WEBHOOK", cancellationToken))
                                    {
                                        leaseToInitiateFirstCharge = lease.LeaseID;
                                    }
                                 }
                             }
                         }

                         leaseDocument.UpdatedDate = now;
                     }
                 }

                 await _dbContext.SaveChangesAsync(cancellationToken);

                if (leaseToInitiateFirstCharge.HasValue)
                {
                    var lease = await _dbContext.Leases
                        .FirstOrDefaultAsync(x => x.LeaseID == leaseToInitiateFirstCharge.Value, cancellationToken);

                    if (lease != null)
                    {
                        await _firstPaymentService.CollectAsync(lease, cancellationToken: cancellationToken);
                    }
                }

                 _logger.LogInformation("Processed Signwell webhook. EventType: {EventType}, DocumentId: {DocumentId}, MatchedSignatories: {Count}",
                     eventType, documentId, targeted.Count);

                return Ok(new { received = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Signwell webhook payload.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { received = true, retryable = true });
            }
        }

        private static string? ExtractDocumentId(JsonElement root)
        {
            var direct = ExtractString(root, "document_id", "documentId");
            if (!string.IsNullOrWhiteSpace(direct))
                return direct;

            if (TryGetProperty(root, out var doc, "document") && doc.ValueKind == JsonValueKind.Object)
            {
                var nested = ExtractString(doc, "id", "document_id", "documentId");
                if (!string.IsNullOrWhiteSpace(nested))
                    return nested;
            }

            if (TryGetProperty(root, out var data, "data") && data.ValueKind == JsonValueKind.Object)
            {
                var nested = ExtractString(data, "document_id", "documentId", "id");
                if (!string.IsNullOrWhiteSpace(nested))
                    return nested;

                if (TryGetProperty(data, out var dataDoc, "document") && dataDoc.ValueKind == JsonValueKind.Object)
                    return ExtractString(dataDoc, "id", "document_id", "documentId");
            }

            return ExtractString(root, "id");
        }

        private static string? ExtractSignerId(JsonElement root)
        {
            var direct = ExtractString(root, "recipient_id", "signer_id", "signerId");
            if (!string.IsNullOrWhiteSpace(direct))
                return direct;

            if (TryGetProperty(root, out var recipient, "recipient", "signer") && recipient.ValueKind == JsonValueKind.Object)
                return ExtractString(recipient, "id", "recipient_id", "signer_id", "signerId");

            if (TryGetProperty(root, out var data, "data") && data.ValueKind == JsonValueKind.Object)
            {
                var nested = ExtractString(data, "recipient_id", "signer_id", "signerId");
                if (!string.IsNullOrWhiteSpace(nested))
                    return nested;

                if (TryGetProperty(data, out var recipientData, "recipient", "signer") && recipientData.ValueKind == JsonValueKind.Object)
                    return ExtractString(recipientData, "id", "recipient_id", "signer_id", "signerId");
            }

            return null;
        }

        private static string? ExtractSignerEmail(JsonElement root)
        {
            var direct = ExtractString(root, "recipient_email", "signer_email", "email");
            if (!string.IsNullOrWhiteSpace(direct))
                return direct;

            if (TryGetProperty(root, out var recipient, "recipient", "signer") && recipient.ValueKind == JsonValueKind.Object)
                return ExtractString(recipient, "email", "recipient_email", "signer_email");

            if (TryGetProperty(root, out var data, "data") && data.ValueKind == JsonValueKind.Object)
            {
                var nested = ExtractString(data, "recipient_email", "signer_email", "email");
                if (!string.IsNullOrWhiteSpace(nested))
                    return nested;

                if (TryGetProperty(data, out var recipientData, "recipient", "signer") && recipientData.ValueKind == JsonValueKind.Object)
                    return ExtractString(recipientData, "email", "recipient_email", "signer_email");
            }

            return null;
        }

        private static string? ExtractString(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (!element.TryGetProperty(name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.String)
                    return value.GetString();

                if (value.ValueKind == JsonValueKind.Number || value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
                    return value.ToString();
            }

            return null;
        }

        private static bool TryGetProperty(JsonElement element, out JsonElement value, params string[] names)
        {
            foreach (var name in names)
            {
                if (element.TryGetProperty(name, out value))
                    return true;
            }

            value = default;
            return false;
        }

        private static bool IsValidWebhookSignature(string payload, string? signatureHeader, string secret)
        {
            if (string.IsNullOrWhiteSpace(signatureHeader) || string.IsNullOrWhiteSpace(secret))
                return false;

            var providedSignature = signatureHeader.Trim();
            if (providedSignature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
                providedSignature = providedSignature[7..];

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var expected = Convert.ToHexString(hash).ToLowerInvariant();

            var provided = providedSignature.Replace("-", string.Empty).ToLowerInvariant();
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var providedBytes = Encoding.UTF8.GetBytes(provided);

            return expectedBytes.Length == providedBytes.Length && CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
        }
    }
}
