using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Arcora.Api.Configurations;
using Arcora.Api.Email;
using Arcora.Api.Signing;
using Arcora.Api.Realtime;
using Arcora.Api.Services.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace Arcora.Api.Operations;

public sealed class OperationsProbes(
    IServiceScopeFactory scopes, OperationsRegistry registry, IConfiguration config,
    IOptions<OperationsOptions> options, IHttpClientFactory clients, BlobServiceClient blobs,
    IEmailQueue emails, ISigningQueue signing, ILogger<OperationsProbes> logger,
    OperationsRequestMetrics? requests = null, KeyVaultConnectivity? keyVault = null)
{
    private DateTime nextPdfCheck;
    private sealed record Result(string Status, string Description, int? Backlog = null);
    private static Result Healthy(string message) => new("Healthy", message);
    private static Result Missing(string message) => new("NotConfigured", message);

    public async Task RunAsync(CancellationToken ct)
    {
        var probes = new List<(string Id, string Name, string Category, Func<CancellationToken, Task<Result>> Run)>
        {
            ("dependency.database", "SQL database and migrations", "Infrastructure", Database),
            ("dependency.storage", "Blob storage containers (read access)", "Infrastructure", Storage),
            ("dependency.smtp", "SMTP connectivity and TLS", "Infrastructure", Smtp),
            ("dependency.stripe", "Stripe API authentication", "Provider", Stripe),
            ("dependency.signwell", "SignWell connectivity / authenticated read", "Provider", Signwell),
            ("dependency.realtime", "Realtime hub registration / transport", "Infrastructure", Realtime),
            ("dependency.identity", "Configured identity verification provider", "Provider", Identity),
            ("dependency.phone", "Configured phone verification provider", "Provider", Phone),
            ("dependency.web", "Web application availability", "Infrastructure",
                token => PublicHttp(options.Value.WebUrl ?? config["FrontendUrl"], token)),
            ("dependency.google", "Google login discovery", "Provider", token =>
                string.IsNullOrWhiteSpace(config["Authentication:Google:ClientId"]) ||
                config["Authentication:Google:ClientId"]!.Contains("your-", StringComparison.OrdinalIgnoreCase)
                    ? Task.FromResult(Missing("Google login is not configured."))
                    : PublicHttp("https://accounts.google.com/.well-known/openid-configuration", token)),
            ("dependency.oidc", "OIDC login discovery", "Provider", token =>
                string.IsNullOrWhiteSpace(config["Authentication:OIDC:Authority"]) ||
                config["Authentication:OIDC:Authority"]!.Contains("your-", StringComparison.OrdinalIgnoreCase)
                    ? Task.FromResult(Missing("OIDC login is not configured."))
                    : PublicHttp(config["Authentication:OIDC:Authority"]!.TrimEnd('/') + "/.well-known/openid-configuration", token)),
            ("dependency.address", "Canada Post HTTPS connectivity", "Provider", token =>
                Tls(config["CanadaPostAddress:FindUrl"] ?? "https://ws1.postescanada-canadapost.ca",
                    config["CanadaPostAddress:Token"], token)),
            ("workflow.payments", "Payment processing and uncertain outcomes", "Workflow", Payments),
            ("workflow.accounting", "Paid rent accounting reconciliation", "Workflow", Accounting),
            ("workflow.rent-due", "Missed recurring rent collections", "Workflow", RentDue),
            ("workflow.payouts", "Host transfer and payout exceptions", "Workflow", Payouts),
            ("workflow.signing", "Stuck and failed signing preparations", "Workflow", Signing),
            ("workflow.verification", "Stuck identity verifications", "Workflow", Verification),
            ("workflow.webhooks", "Payment webhook processing", "Workflow", Webhooks),
            ("workflow.invitations", "Invitation email retries", "Workflow", Invitations),
            ("workflow.email-queue", "General email queue", "Workflow",
                _ => Task.FromResult(Queue(emails.Reader.CanCount ? emails.Reader.Count : null,
                    emails.Reader.TryPeek(out var item) ? item.EnqueuedAt : null))),
            ("workflow.signing-queue", "Signing queue", "Workflow",
                _ => Task.FromResult(Queue(signing.Reader.CanCount ? signing.Reader.Count : null,
                    signing.Reader.TryPeek(out var item) ? item.EnqueuedAt : null)))
        };
        if (registry.Now >= nextPdfCheck)
        {
            probes.Add(("dependency.pdf", "PDF renderer", "Infrastructure", Pdf));
        }
        if (keyVault != null)
            probes.Add(("dependency.key-vault", "Key Vault startup secrets / transport", "Infrastructure", async token =>
            {
                var check = await keyVault.CheckAsync(token);
                return new(check.Status, check.Description);
            }));
        await Parallel.ForEachAsync(probes, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
            async (probe, token) =>
            {
                var watch = Stopwatch.StartNew();
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token);
                deadline.CancelAfter(TimeSpan.FromSeconds(probe.Id == "dependency.pdf"
                    ? options.Value.PdfTimeoutSeconds : options.Value.TimeoutSeconds));
                try
                {
                    var result = await probe.Run(deadline.Token);
                    registry.Report(probe.Id, probe.Name, probe.Category, result.Status, result.Description,
                        watch.Elapsed.TotalMilliseconds, result.Backlog);
                    if (probe.Id == "dependency.pdf")
                        nextPdfCheck = registry.Now.AddMinutes(15);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
                catch (Exception ex)
                {
                    // Provider bodies, URLs and exception messages may contain credentials or personal data.
                    logger.LogError("Operations probe {CheckId} failed ({ExceptionType}).", probe.Id, ex.GetType().Name);
                    registry.Report(probe.Id, probe.Name, probe.Category, "Failed",
                        FailureDescription(ex),
                        watch.Elapsed.TotalMilliseconds);
                    if (probe.Id == "dependency.pdf")
                        nextPdfCheck = registry.Now.AddSeconds(options.Value.IntervalSeconds);
                }
            });
        registry.Report("dependency.api", "API process", "Infrastructure", "Healthy", "API process is running.");
        if (requests != null)
        {
            var traffic = requests.Snapshot();
            registry.Report("dependency.api-traffic", "API error rate and response time", "Infrastructure",
                traffic.Status, traffic.Description);
        }
    }

    private async Task<Result> Database(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
        await db.Database.ExecuteSqlRawAsync("SELECT 1", ct);
        var pending = (await db.Database.GetPendingMigrationsAsync(ct)).Count();
        return pending > 0 ? new("Failed", $"{pending} database migrations are pending.") : Healthy("Database query succeeded; all known migrations are applied.");
    }

    private async Task<Result> Storage(CancellationToken ct)
    {
        var settings = config.GetSection("Azure:BlobStorage").Get<BlobStorageConfiguration>() ?? new();
        var uninitialized = 0;
        foreach (var name in new[] { settings.ImagesContainer, settings.DocumentsContainer, settings.FilesContainer }.Distinct())
        {
            try { await blobs.GetBlobContainerClient(name).GetPropertiesAsync(cancellationToken: ct); }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                uninitialized++;
            }
        }
        return uninitialized > 0
            ? new("Degraded", $"Storage is reachable, but {uninitialized} configured containers do not exist. The app creates containers on first upload; review whether initialization is expected. This probe does not create containers.")
            : Healthy("Configured containers are readable. Write permission is not exercised by this read-only probe.");
    }

    private static string FailureDescription(Exception ex) => ex switch
    {
        OperationCanceledException => "Probe timed out.",
        RequestFailedException azure => $"Storage request failed with HTTP {azure.Status}. Review storage credentials, permissions and endpoint.",
        SocketException socket => $"Network connection failed ({socket.SocketErrorCode}).",
        System.Security.Authentication.AuthenticationException => "TLS certificate validation or handshake failed.",
        HttpRequestException http when http.StatusCode.HasValue => $"Provider request failed with HTTP {(int)http.StatusCode.Value}.",
        _ => $"Probe failed ({ex.GetType().Name}). Check service configuration and application logs."
    };

    private async Task<Result> Realtime(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        _ = scope.ServiceProvider.GetRequiredService<IHubContext<MessagingHub>>();
        var connection = config["Azure:SignalR:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connection))
            return Healthy("Local SignalR hub services resolve. End-to-end authenticated websocket delivery requires an external synthetic monitor.");
        var endpoint = connection.Split(';').FirstOrDefault(x => x.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase));
        if (endpoint == null) return new("Failed", "Azure SignalR connection string has no endpoint.");
        var result = await Tls(endpoint["Endpoint=".Length..], "configured", ct);
        return result with { Description = result.Description + " Azure SignalR message delivery is not exercised." };
    }

    private async Task<Result> Stripe(CancellationToken ct)
    {
        var key = config["Stripe:SecretKey"];
        if (string.IsNullOrWhiteSpace(key)) return Missing("Stripe secret is not configured.");
        return await Http("https://api.stripe.com/v1/balance", new("Bearer", key), ct,
            $"Read-only Stripe balance request succeeded ({(key.StartsWith("sk_test_", StringComparison.Ordinal) ? "test" : "live")} mode).");
    }

    private async Task<Result> Identity(CancellationToken ct)
    {
        var provider = config["IdentityVerification:DefaultProvider"]?.ToUpperInvariant();
        if (provider == "PERSONA")
        {
            var settings = config.GetSection("Persona").Get<PersonaOptions>() ?? new();
            if (string.IsNullOrWhiteSpace(settings.ApiKey)) return Missing("Selected Persona provider has no API key.");
            return await Http(settings.BaseUrl.TrimEnd('/') + "/inquiries?page[size]=1",
                new("Bearer", settings.ApiKey), ct, "Persona authenticated read succeeded; no verification was created.");
        }

        if (provider == "ONFIDO")
        {
            var settings = config.GetSection("Onfido").Get<OnfidoOptions>() ?? new();
            if (string.IsNullOrWhiteSpace(settings.ApiKey)) return Missing("Selected Onfido provider has no API key.");
            return await Http(settings.BaseUrl.TrimEnd('/') + "/applicants?per_page=1",
                new("Token", $"token={settings.ApiKey}"), ct, "Onfido authenticated read succeeded; no applicant was created.");
        }
        return Missing("No supported identity provider is selected.");
    }

    private async Task<Result> Signwell(CancellationToken ct)
    {
        var connectivity = await Tls(config["Signwell:BaseUrl"], config["Signwell:ApiKey"], ct);
        if (connectivity.Status != "Healthy") return connectivity;
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
        var id = await db.LeaseSignatories.AsNoTracking().Where(x => x.ProviderDocumentID != null)
            .OrderByDescending(x => x.CapturedDate).Select(x => x.ProviderDocumentID).FirstOrDefaultAsync(ct);
        if (id == null) return connectivity with { Description = connectivity.Description + " No existing document is available for an authenticated read." };
        await scope.ServiceProvider.GetRequiredService<ISignwellClient>().GetDocumentStatusAsync(id, ct);
        return Healthy("Authenticated status read of an existing document succeeded; no signing request was created.");
    }

    private async Task<Result> Phone(CancellationToken ct)
    {
        var provider = config["PhoneVerification:Provider"]?.ToUpperInvariant() ?? "AUTO";
        var twilio = config.GetSection("Twilio:Verify").Get<TwilioVerifyOptions>() ?? new();
        var infobip = config.GetSection("Infobip").Get<InfobipOptions>() ?? new();
        if (provider == "LOCAL") return new("Disabled", "Local development verification is selected; no external OTP service is used.");
        if (provider == "INFOBIP" || provider == "AUTO" && !string.IsNullOrWhiteSpace(infobip.ApiKey) &&
            !string.IsNullOrWhiteSpace(infobip.BaseUrl) && !string.IsNullOrWhiteSpace(infobip.Sender))
        {
            if (string.IsNullOrWhiteSpace(infobip.ApiKey) || string.IsNullOrWhiteSpace(infobip.BaseUrl))
                return Missing("Infobip credentials are incomplete.");
            return await Http(infobip.BaseUrl.TrimEnd('/') + "/account/1/balance", new("App", infobip.ApiKey), ct,
                "Infobip authenticated read succeeded; no SMS was sent.");
        }
        if (string.IsNullOrWhiteSpace(twilio.AccountSid) || string.IsNullOrWhiteSpace(twilio.AuthToken) ||
            string.IsNullOrWhiteSpace(twilio.ServiceSid)) return Missing("Twilio Verify credentials are incomplete.");
        return await Http($"https://verify.twilio.com/v2/Services/{Uri.EscapeDataString(twilio.ServiceSid)}",
            new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{twilio.AccountSid}:{twilio.AuthToken}"))), ct,
            "Twilio Verify authenticated read succeeded; no OTP was sent.");
    }

    private async Task<Result> PublicHttp(string? url, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(url)) return Missing("Endpoint is not configured.");
        return await Http(url, null, ct, "Read-only HTTP request succeeded; interactive user flows are not exercised.");
    }

    private async Task<Result> Http(string url, AuthenticationHeaderValue? auth, CancellationToken ct, string success)
    {
        using var client = clients.CreateClient("Operations");
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = auth;
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        return response.IsSuccessStatusCode ? Healthy(success) :
            new(response.StatusCode == HttpStatusCode.TooManyRequests ? "Degraded" : "Failed",
                $"Read-only endpoint returned HTTP {(int)response.StatusCode}. No response body is exposed.");
    }

    private static async Task<Result> Tls(string? url, string? credential, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(credential))
            return Missing("Endpoint or API key is not configured.");
        var uri = new Uri(url);
        if (uri.Scheme != "https") return new("Failed", "Provider endpoint must use HTTPS.");
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(uri.Host, uri.Port, ct);
        await using var ssl = new SslStream(tcp.GetStream());
        await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions { TargetHost = uri.Host }, ct);
        return Healthy("DNS, TCP and certificate-validated TLS succeeded. Provider authentication and operations are not probed.");
    }

    private async Task<Result> Smtp(CancellationToken ct)
    {
        var settings = config.GetSection("EmailSettings").Get<EmailSettings>() ?? new();
        if (string.IsNullOrWhiteSpace(settings.SmtpHost)) return Missing("SMTP host is not configured.");
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(settings.SmtpHost, settings.SmtpPort, ct);
        using var stream = tcp.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true) { NewLine = "\r\n", AutoFlush = true };
        await ExpectSmtp(reader, "220", ct);
        if (settings.EnableSsl)
        {
            await writer.WriteLineAsync("EHLO arcora-health".AsMemory(), ct);
            await ExpectSmtp(reader, "250", ct);
            await writer.WriteLineAsync("STARTTLS".AsMemory(), ct);
            await ExpectSmtp(reader, "220", ct);
            await using var tls = new SslStream(stream, leaveInnerStreamOpen: true);
            await tls.AuthenticateAsClientAsync(new SslClientAuthenticationOptions { TargetHost = settings.SmtpHost }, ct);
        }
        return Healthy($"SMTP connection succeeded ({(settings.EnableSsl ? "STARTTLS verified" : "TLS disabled by configuration")}). No authentication or email send was attempted; delivery failures are monitored separately.");
    }

    private static async Task ExpectSmtp(StreamReader reader, string code, CancellationToken ct)
    {
        for (var lines = 0; lines < 100; lines++)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line == null || !line.StartsWith(code, StringComparison.Ordinal))
                throw new IOException("SMTP handshake failed.");
            if (line.Length < 4 || line[3] != '-') return;
        }
        throw new IOException("SMTP handshake exceeded response limit.");
    }

    private async Task<Result> Pdf(CancellationToken ct)
    {
        using var playwright = await Playwright.CreateAsync();
        ct.ThrowIfCancellationRequested();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true, Timeout = options.Value.PdfTimeoutSeconds * 1000, Args = ["--no-sandbox", "--disable-dev-shm-usage"]
        });
        var page = await browser.NewPageAsync().WaitAsync(ct);
        await page.SetContentAsync("<html><body>Arcora health probe</body></html>", new() { Timeout = 5000 }).WaitAsync(ct);
        var bytes = await page.PdfAsync().WaitAsync(ct);
        ct.ThrowIfCancellationRequested();
        return bytes.Length > 0 ? Healthy("Chromium launched and generated a small in-memory PDF. Checked every 15 minutes.") :
            new("Failed", "PDF renderer returned an empty document.");
    }

    private async Task<Result> Count(CancellationToken ct, Func<ArcoraDbContext, Task<int>> query, string description)
    {
        using var scope = scopes.CreateScope();
        ct.ThrowIfCancellationRequested();
        var count = await query(scope.ServiceProvider.GetRequiredService<ArcoraDbContext>());
        return new(count > 0 ? "Degraded" : "Healthy", $"{count} {description}", count);
    }

    private Task<Result> Payments(CancellationToken ct)
    {
        var cutoff = registry.Now.AddHours(-options.Value.PaymentPendingHours);
        return Count(ct, db => db.PaymentIntents.CountAsync(x =>
            x.Status == "CARD_FAILED" || x.Status == "OVERDUE" || x.Status == "PAD_FAILED" ||
            (x.Status == "PROCESSING" && x.StartedAt < cutoff), ct),
            "failed/overdue payments or processing payments beyond the configured settlement threshold.");
    }

    private Task<Result> Accounting(CancellationToken ct) => Count(ct, db =>
        db.Payments.CountAsync(payment => payment.Status == "PAID" && payment.PaymentIntent != null &&
            payment.PaymentIntent.LeaseID != null &&
            (payment.RefundedAmount > 0 || payment.Currency == null ||
             !db.RentPaymentAccountings.Any(allocation => allocation.PaymentID == payment.PaymentID &&
                 allocation.SettledAt != null && allocation.Currency == payment.Currency &&
                 allocation.RentAmount + allocation.DepositAmount + allocation.TenantFeeAmount == payment.GrossAmount &&
                 allocation.HostNetAmount == allocation.RentAmount - allocation.HostPlacementFee - allocation.HostMonthlyFee)), ct),
        "paid rent collections require accounting reconciliation or refund review. No balances are repaired by this check.");

    private Task<Result> Payouts(CancellationToken ct)
    {
        var cutoff = registry.Now.AddHours(-options.Value.PayoutPendingHours);
        return Count(ct, db => db.RentDisbursements.CountAsync(x =>
            x.Status == "FAILED" || x.Status == "REVERSED" || x.Status == "REVIEW_REQUIRED" ||
            x.Status == "PROVIDER_UNCERTAIN" || x.Status == "CANCELED" ||
            (x.Status != "PAID" && x.CapturedDate < cutoff), ct),
            "host disbursements need review. Ordinary settlement waits below the configured threshold are excluded.");
    }

    private Task<Result> Signing(CancellationToken ct)
    {
        var cutoff = registry.Now.AddMinutes(-options.Value.StuckMinutes);
        return Count(ct, db => db.LeaseDocuments.CountAsync(x => x.DocumentStatus == "FAILED" ||
            (x.DocumentStatus == "QUEUED" && x.CapturedDate < cutoff), ct),
            "failed or overdue queued signing preparations. Waiting for a human signature is not a worker outage.");
    }

    private Task<Result> Verification(CancellationToken ct)
    {
        var cutoff = registry.Now.AddHours(-options.Value.VerificationPendingHours);
        return Count(ct, db => db.IdentityVerifications.CountAsync(x =>
            (x.Status == "PENDING" || x.Status == "SUBMITTED" || x.Status == "UNDER_REVIEW") &&
            x.RequestedAt < cutoff, ct), "identity verifications exceed the configured completion threshold; investigate before treating as provider outages.");
    }

    private Task<Result> Webhooks(CancellationToken ct)
    {
        var cutoff = registry.Now.AddMinutes(-options.Value.StuckMinutes);
        return Count(ct, db => db.PaymentProviderEvents.CountAsync(x => x.ProcessingStatus == "FAILED" ||
            (x.ProcessingStatus == "RECEIVED" && x.ReceivedAt < cutoff), ct),
            "payment webhook events are failed or overdue. Lack of incoming events alone is not considered a failure.");
    }

    private Task<Result> Invitations(CancellationToken ct)
    {
        var cutoff = registry.Now.AddMinutes(-options.Value.StuckMinutes);
        return Count(ct, db => db.TenantInvitationEmails.CountAsync(x => x.Status == "FAILED" ||
            (x.Status == "PENDING" && x.NextAttemptAt < cutoff), ct), "invitation emails are retrying or overdue.");
    }

    private async Task<Result> RentDue(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
        var now = registry.Now;
        var month = new DateTime(now.Year, now.Month, 1);
        var next = month.AddMonths(1);
        var leases = await db.Leases.AsNoTracking().Where(x => x.Status == "ACTIVE" && x.StartDate < month &&
            (x.EndDate == null || x.EndDate >= month))
            .Select(x => new { x.LeaseID, x.StartDate, x.EndDate }).ToListAsync(ct);
        var keys = await db.PaymentIntents.AsNoTracking().Where(x => x.LeaseID != null && x.ScheduledChargeAt >= month &&
            x.ScheduledChargeAt < next).Select(x => x.IdempotencyKey).ToListAsync(ct);
        var existing = keys.ToHashSet(StringComparer.Ordinal);
        var hour = config.GetValue<int?>("RentCollection:ChargeHourUtc") ?? 9;
        var count = leases.Count(x =>
        {
            var due = month.AddDays(Math.Min(x.StartDate.Day, DateTime.DaysInMonth(now.Year, now.Month)) - 1).AddHours(hour);
            return now > due.AddHours(2) && (!x.EndDate.HasValue || due.Date <= x.EndDate.Value.Date) &&
                !existing.Contains($"pi_{x.LeaseID:N}_{now:yyyyMM}");
        });
        return new(count > 0 ? "Degraded" : "Healthy",
            $"{count} current-month recurring collections are overdue without a matching intent (two-hour grace). This check never initiates a debit.", count);
    }

    private Result Queue(int? count, DateTime? oldest) => count.HasValue
        ? new(count > 100 || oldest < registry.Now.AddMinutes(-options.Value.StuckMinutes) ? "Degraded" : "Healthy",
            $"{count} queued items. Oldest enqueued at: {(oldest.HasValue ? oldest.Value.ToString("O") : "none")}.", count)
        : new("Unknown", "This queue implementation does not expose a count.");
}
