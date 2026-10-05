using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Arcora.Api.Services.Implementations
{
    public class SignwellClient : ISignwellClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SignwellClient> _logger;
        private readonly string _createDocumentPath;
        private readonly string _getDocumentPathTemplate;
        private readonly string? _apiKey;
        private readonly string _preferredAuthScheme;

        public SignwellClient(HttpClient httpClient, IConfiguration configuration, ILogger<SignwellClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var baseUrl = configuration["Signwell:BaseUrl"];
            _apiKey = configuration["Signwell:ApiKey"];
            var authScheme = configuration["Signwell:AuthScheme"];
            var configuredPath = configuration["Signwell:CreateDocumentPath"] ?? "/api/v1/documents";
            var configuredGetPath = configuration["Signwell:GetDocumentPathTemplate"] ?? "/api/v1/documents/{documentId}";

            if (!string.IsNullOrWhiteSpace(baseUrl))
                _httpClient.BaseAddress = EnsureTrailingSlash(baseUrl);

            _preferredAuthScheme = string.IsNullOrWhiteSpace(authScheme)
                ? "basic"
                : authScheme.Trim().ToLowerInvariant();

            _createDocumentPath = NormalizeRequestPath(_httpClient.BaseAddress, configuredPath);
            _getDocumentPathTemplate = NormalizeRequestPath(_httpClient.BaseAddress, configuredGetPath);

            if (string.IsNullOrWhiteSpace(_apiKey))
                _logger.LogWarning("Signwell API key is empty. Requests to Signwell will return 401.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger.LogInformation("Signwell client configured. BaseAddress: {BaseAddress}, Path: {Path}, PreferredAuthScheme: {AuthScheme}",
                _httpClient.BaseAddress?.ToString(), _createDocumentPath, _preferredAuthScheme);
        }

        public async Task<SignwellCreateResult> CreateSigningRequestAsync(
            Guid leaseId,
            byte[] pdfBytes,
            string fileName,
            IEnumerable<SignwellSignerRequest> signers,
            bool sequential,
            string? subject = null,
            string? message = null,
            CancellationToken cancellationToken = default)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("PDF payload is required.", nameof(pdfBytes));

            var signerList = signers?
                .Where(s => !string.IsNullOrWhiteSpace(s.Email))
                .Select((s, index) => new
                {
                    id = (index + 1).ToString(),
                    name = s.Name,
                    email = s.Email,
                    order = s.Order ?? (index + 1)
                })
                .ToList();

            if (signerList == null || signerList.Count == 0)
                throw new ArgumentException("At least one signer is required.", nameof(signers));

            var base64Pdf = Convert.ToBase64String(pdfBytes);

            var payload = new
            {
                name = fileName,
                subject,
                message,
                external_id = $"lease-{leaseId}",
                sequential_signing = sequential,
                recipients = signerList,
                files = new[]
                {
                    new
                    {
                        name = fileName,
                        file_base64 = base64Pdf
                    }
                },
                with_signature_page = true,
                reminders = true
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var jsonBytes = Encoding.UTF8.GetBytes(json);

            async Task<HttpResponseMessage> SendJsonRequestAsync(string authScheme, CancellationToken ct)
            {
                var content = new ByteArrayContent(jsonBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var req = new HttpRequestMessage(HttpMethod.Post, _createDocumentPath)
                {
                    Content = content
                };

                var resolvedUri = req.RequestUri != null && req.RequestUri.IsAbsoluteUri
                    ? req.RequestUri
                    : (_httpClient.BaseAddress != null && req.RequestUri != null ? new Uri(_httpClient.BaseAddress, req.RequestUri) : null);
                _logger.LogInformation("Signwell request URI: {Uri}", resolvedUri?.ToString() ?? _createDocumentPath);

                req.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                req.Headers.Accept.Clear();
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ApplyAuthHeaders(req, authScheme);

                return await _httpClient.SendAsync(req, ct);
            }

            _logger.LogInformation("Creating Signwell request for lease {LeaseId} with {SignerCount} signer(s).", leaseId, signerList.Count);

            var authAttempts = BuildAuthAttempts(_preferredAuthScheme);
            HttpResponseMessage? response = null;
            string responseBody = string.Empty;

            foreach (var auth in authAttempts)
            {
                response?.Dispose();
                response = await SendJsonRequestAsync(auth, cancellationToken);
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                    break;

                if ((int)response.StatusCode != 401)
                    break;

                _logger.LogWarning("Signwell returned 401 using auth scheme '{AuthScheme}'. Trying next scheme if available.", auth);
            }

            using (response)
            {
                if (response == null)
                    throw new HttpRequestException("Signwell request was not executed.");

                if (!response.IsSuccessStatusCode &&
                    responseBody.Contains("missing_content_type_error", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Signwell reported missing content type. Retrying request with strict JSON headers for lease {LeaseId}.", leaseId);
                    using var retryResponse = await SendJsonRequestAsync(_preferredAuthScheme, cancellationToken);
                    responseBody = await retryResponse.Content.ReadAsStringAsync(cancellationToken);

                    if (!retryResponse.IsSuccessStatusCode)
                    {
                        _logger.LogError("Signwell create request failed. Status: {StatusCode}. Body: {Body}", (int)retryResponse.StatusCode, responseBody);
                        throw new HttpRequestException($"Signwell create request failed with status {(int)retryResponse.StatusCode}.");
                    }

                    return ParseCreateResult(responseBody);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Signwell create request failed. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, responseBody);
                    throw new HttpRequestException($"Signwell create request failed with status {(int)response.StatusCode}.");
                }

                return ParseCreateResult(responseBody);
            }
        }

        public async Task<SignwellDocumentStatusResult> GetDocumentStatusAsync(string providerDocumentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerDocumentId))
                throw new ArgumentException("Provider document id is required.", nameof(providerDocumentId));

            var requestPath = BuildDocumentStatusPath(providerDocumentId);
            var authAttempts = BuildAuthAttempts(_preferredAuthScheme);
            HttpResponseMessage? response = null;
            string responseBody = string.Empty;

            foreach (var auth in authAttempts)
            {
                response?.Dispose();
                using var request = new HttpRequestMessage(HttpMethod.Get, requestPath);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ApplyAuthHeaders(request, auth);

                var resolvedUri = request.RequestUri != null && request.RequestUri.IsAbsoluteUri
                    ? request.RequestUri
                    : (_httpClient.BaseAddress != null && request.RequestUri != null ? new Uri(_httpClient.BaseAddress, request.RequestUri) : null);
                _logger.LogInformation("Signwell status request URI: {Uri}", resolvedUri?.ToString() ?? requestPath);

                response = await _httpClient.SendAsync(request, cancellationToken);
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                    break;

                if ((int)response.StatusCode != 401)
                    break;

                _logger.LogWarning("Signwell status request returned 401 using auth scheme '{AuthScheme}'. Trying next scheme if available.", auth);
            }

            using (response)
            {
                if (response == null)
                    throw new HttpRequestException("Signwell status request was not executed.");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Signwell status request failed. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, responseBody);
                    throw new HttpRequestException($"Signwell status request failed with status {(int)response.StatusCode}.");
                }

                return ParseDocumentStatusResult(responseBody, providerDocumentId);
            }
        }

        private static SignwellCreateResult ParseCreateResult(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return new SignwellCreateResult();

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            string? requestId = TryGetString(root, "request_id", "requestId", "id");
            string? documentId = TryGetString(root, "document_id", "documentId", "id");

            if (TryGetProperty(root, out var documentNode, "document") && documentNode.ValueKind == JsonValueKind.Object)
            {
                requestId ??= TryGetString(documentNode, "request_id", "requestId", "id");
                documentId ??= TryGetString(documentNode, "document_id", "documentId", "id");
            }

            var signerIdsByEmail = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (TryGetProperty(root, out var signersNode, "signers", "recipients") && signersNode.ValueKind == JsonValueKind.Array)
            {
                foreach (var signer in signersNode.EnumerateArray())
                {
                    var email = TryGetString(signer, "email");
                    var signerId = TryGetString(signer, "id", "signer_id", "signerId", "recipient_id", "recipientId");

                    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(signerId))
                        signerIdsByEmail[email] = signerId;
                }
            }

            return new SignwellCreateResult
            {
                ProviderRequestId = requestId,
                ProviderDocumentId = documentId,
                SignerIdsByEmail = signerIdsByEmail
            };
        }

        private static SignwellDocumentStatusResult ParseDocumentStatusResult(string responseBody, string providerDocumentId)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new SignwellDocumentStatusResult
                {
                    ProviderDocumentId = providerDocumentId
                };
            }

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            var documentNode = FindDocumentNode(root);

            var result = new SignwellDocumentStatusResult
            {
                ProviderRequestId = TryGetString(documentNode, "request_id", "requestId", "external_id") ?? TryGetString(root, "request_id", "requestId", "external_id"),
                ProviderDocumentId = TryGetString(documentNode, "document_id", "documentId", "id") ?? TryGetString(root, "document_id", "documentId", "id") ?? providerDocumentId,
                Status = TryGetString(documentNode, "status", "document_status") ?? TryGetString(root, "status", "document_status"),
                DocumentUrl = TryGetString(documentNode, "url", "document_url", "embedded_signing_url") ?? TryGetString(root, "url", "document_url", "embedded_signing_url"),
                CompletedAt = TryGetDateTimeOffset(documentNode, "completed_at", "completedAt", "signed_at", "signedAt") ?? TryGetDateTimeOffset(root, "completed_at", "completedAt", "signed_at", "signedAt")
            };

            if (TryGetProperty(documentNode, out var signersNode, "signers", "recipients", "parties") ||
                TryGetProperty(root, out signersNode, "signers", "recipients", "parties"))
            {
                if (signersNode.ValueKind == JsonValueKind.Array)
                {
                    foreach (var signer in signersNode.EnumerateArray())
                    {
                        result.Signers.Add(new SignwellSignerStatusResult
                        {
                            ProviderSignerId = TryGetString(signer, "id", "recipient_id", "recipientId", "signer_id", "signerId"),
                            Name = TryGetString(signer, "name", "full_name"),
                            Email = TryGetString(signer, "email", "recipient_email", "signer_email"),
                            Status = TryGetString(signer, "status", "recipient_status", "signer_status"),
                            ViewedAt = TryGetDateTimeOffset(signer, "viewed_at", "viewedAt", "opened_at", "openedAt"),
                            SignedAt = TryGetDateTimeOffset(signer, "signed_at", "signedAt", "completed_at", "completedAt"),
                            DeclinedAt = TryGetDateTimeOffset(signer, "declined_at", "declinedAt", "rejected_at", "rejectedAt")
                        });
                    }
                }
            }

            return result;
        }

        private static JsonElement FindDocumentNode(JsonElement root)
        {
            if (TryGetProperty(root, out var documentNode, "document") && documentNode.ValueKind == JsonValueKind.Object)
                return documentNode;

            if (TryGetProperty(root, out var dataNode, "data") && dataNode.ValueKind == JsonValueKind.Object)
            {
                if (TryGetProperty(dataNode, out documentNode, "document") && documentNode.ValueKind == JsonValueKind.Object)
                    return documentNode;

                return dataNode;
            }

            return root;
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

        private static string? TryGetString(JsonElement element, params string[] names)
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

        private static DateTimeOffset? TryGetDateTimeOffset(JsonElement element, params string[] names)
        {
            var value = TryGetString(element, names);
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
        }

        private void ApplyAuthHeaders(HttpRequestMessage req, string scheme)
        {
            req.Headers.Authorization = null;
            req.Headers.Remove("X-Api-Key");

            if (string.IsNullOrWhiteSpace(_apiKey))
                return;

            var normalized = (scheme ?? string.Empty).Trim().ToLowerInvariant();
            if (normalized == "bearer")
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }
            else if (normalized == "x-api-key")
            {
                req.Headers.TryAddWithoutValidation("X-Api-Key", _apiKey);
            }
            else
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Basic", _apiKey);
            }
        }

        private string BuildDocumentStatusPath(string providerDocumentId)
        {
            return _getDocumentPathTemplate
                .Replace("{documentId}", Uri.EscapeDataString(providerDocumentId), StringComparison.OrdinalIgnoreCase)
                .Replace("{id}", Uri.EscapeDataString(providerDocumentId), StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> BuildAuthAttempts(string preferred)
        {
            var attempts = new List<string>();
            void Add(string value)
            {
                if (!attempts.Contains(value, StringComparer.OrdinalIgnoreCase))
                    attempts.Add(value);
            }

            Add(preferred);
            Add("basic");
            Add("bearer");
            Add("x-api-key");
            return attempts;
        }

        private static string NormalizeRequestPath(Uri? baseAddress, string path)
        {
            var configured = string.IsNullOrWhiteSpace(path) ? "/api/v1/documents" : path.Trim();

            if (Uri.TryCreate(configured, UriKind.Absolute, out _))
                return configured;

            var normalizedPath = configured.Replace("\\", "/").Trim();
            if (!normalizedPath.StartsWith('/'))
                normalizedPath = "/" + normalizedPath;

            if (baseAddress == null)
                return normalizedPath;

            var basePath = (baseAddress.AbsolutePath ?? string.Empty).TrimEnd('/');
            if (!string.IsNullOrEmpty(basePath) && !string.Equals(basePath, "/", StringComparison.Ordinal))
            {
                if (normalizedPath.StartsWith(basePath + "/", StringComparison.OrdinalIgnoreCase))
                    normalizedPath = normalizedPath[basePath.Length..];
                else if (string.Equals(normalizedPath, basePath, StringComparison.OrdinalIgnoreCase))
                    normalizedPath = "/";
            }

            return normalizedPath.TrimStart('/');
        }

        private static Uri EnsureTrailingSlash(string baseUrl)
        {
            var value = baseUrl.Trim();
            if (!value.EndsWith("/", StringComparison.Ordinal))
                value += "/";

            return new Uri(value, UriKind.Absolute);
        }
    }
}
