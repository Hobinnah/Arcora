using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcora.Api.Signing
{
    public class SigningQueueBackgroundService : BackgroundService
    {
        private readonly ISigningQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SigningQueueBackgroundService> _logger;

        public SigningQueueBackgroundService(ISigningQueue queue, IServiceProvider serviceProvider, ILogger<SigningQueueBackgroundService> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Signing queue background service started.");

            await foreach (var item in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
                    var templateService = scope.ServiceProvider.GetRequiredService<ILeaseContractTemplateService>();
                    var htmlToPdf = scope.ServiceProvider.GetRequiredService<IHtmlToPdfService>();
                    var signwell = scope.ServiceProvider.GetRequiredService<ISignwellClient>();
                    var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    _logger.LogInformation("Processing signing request {SigningRequestId} for lease {LeaseId}", item.SigningRequestId, item.LeaseId);

                    var renderRequest = new DTOs.LeaseContractRenderRequestDto { LeaseID = item.LeaseId, LeaseContractTemplateID = item.LeaseContractTemplateId };
                    var renderResult = await templateService.RenderContract(renderRequest);
                    if (renderResult == null)
                    {
                        _logger.LogWarning("RenderContract returned null for lease {LeaseId}", item.LeaseId);
                        continue;
                    }

                    var html = renderResult.RenderedHtml ?? renderResult.RawHtml ?? string.Empty;
                    var pdfRenderResult = await htmlToPdf.ConvertHtmlToPdfWithAnchorsAsync(html, ".sig-line", stoppingToken);
                    var pdfBytes = pdfRenderResult.PdfBytes;

                    var leaseFileInfo = await dbContext.Leases
                        .AsNoTracking()
                        .Where(l => l.LeaseID == item.LeaseId)
                        .Select(l => new
                        {
                            UnitName = l.RentalUnit != null
                                ? string.IsNullOrWhiteSpace(l.RentalUnit.UnitNumber)
                                    ? l.RentalUnit.Name
                                    : $"{l.RentalUnit.Name} {l.RentalUnit.UnitNumber}"
                                : null,
                            TenantName = l.Tenant != null
                                ? (!string.IsNullOrWhiteSpace(l.Tenant.User.DisplayName)
                                    ? l.Tenant.User.DisplayName
                                    : ($"{l.Tenant.User.FirstName} {l.Tenant.User.LastName}").Trim())
                                : null
                        })
                        .FirstOrDefaultAsync(stoppingToken);

                    var fileName = BuildLeaseAgreementFileName(leaseFileInfo?.UnitName, leaseFileInfo?.TenantName);
                    var uploadResult = await fileStorage.UploadAsync(Enums.StorageCategory.Document, fileName, new MemoryStream(pdfBytes), "application/pdf", stoppingToken);
                    var viewablePdfUrl = await fileStorage.GetReadSasUrlAsync(Enums.StorageCategory.Document, uploadResult.BlobName, stoppingToken)
                        ?? uploadResult.Uri;

                    byte[] providerPdfBytes = pdfBytes;
                    var storedPdf = await fileStorage.DownloadAsync(Enums.StorageCategory.Document, uploadResult.BlobName, stoppingToken);
                    if (storedPdf.HasValue)
                    {
                        await using var storedStream = storedPdf.Value.Content;
                        using var storedMemory = new MemoryStream();
                        await storedStream.CopyToAsync(storedMemory, stoppingToken);
                        providerPdfBytes = storedMemory.ToArray();
                    }

                    Console.WriteLine($"[LEASE_SIGNING_PDF_URL_UNMASKED_BEFORE_SIGNWELL] {viewablePdfUrl}");
                    _logger.LogInformation("Viewable lease PDF URL for signing request {SigningRequestId}: {Url}", item.SigningRequestId, viewablePdfUrl);

                    var autoSendToSignwell = configuration.GetValue<bool?>("Signwell:AutoSendEnabled") ?? true;
                    var placeholder = item.LeaseDocumentId.HasValue
                        ? await dbContext.LeaseDocuments.FirstOrDefaultAsync(d => d.LeaseDocumentID == item.LeaseDocumentId.Value, stoppingToken)
                        : null;

                    if (!autoSendToSignwell)
                    {
                        _logger.LogInformation("Signwell auto-send is disabled. Signing request {SigningRequestId} is held for manual PDF confirmation.", item.SigningRequestId);

                        if (placeholder != null)
                        {
                            placeholder.StorageReference = uploadResult.BlobName;
                            placeholder.StorageContainer = uploadResult.Container;
                            placeholder.Url = viewablePdfUrl;
                            placeholder.DocumentStatus = "PENDING_REVIEW";
                            placeholder.OriginalFilename = fileName;
                            placeholder.UpdatedDate = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }

                        continue;
                    }

                    // Prefer signature placement based on rendered .sig-line anchors and fall back to defaults.
                    var anchors = pdfRenderResult.SignatureAnchors ?? new List<PdfSignatureAnchor>();
                    var landlordAnchor = anchors.FirstOrDefault();
                    var nonLandlordAnchors = anchors.Skip(1).ToList();
                    var nonLandlordAnchorIndex = 0;

                    var signers = item.Signatories.Select((s, index) =>
                    {
                        var isLandlord = string.Equals(s.Role, "LANDLORD", StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(s.Role, "OWNER", StringComparison.OrdinalIgnoreCase);

                        PdfSignatureAnchor? anchor = null;
                        if (isLandlord && landlordAnchor != null)
                        {
                            anchor = landlordAnchor;
                        }
                        else if (nonLandlordAnchors.Count > 0)
                        {
                            anchor = nonLandlordAnchors[Math.Min(nonLandlordAnchorIndex, nonLandlordAnchors.Count - 1)];
                            nonLandlordAnchorIndex++;
                        }
                        else if (landlordAnchor != null)
                        {
                            anchor = landlordAnchor;
                        }

                        var defaultX = isLandlord ? 330m : 60m;
                        var defaultY = 700m - (index * 70m);

                        return new SignwellSignerRequest
                        {
                            Name = s.Name,
                            Email = s.Email,
                            Order = s.SignatureOrder,
                            Page = s.SignaturePage ?? anchor?.Page ?? 1,
                            X = s.SignatureX ?? anchor?.X ?? defaultX,
                            Y = s.SignatureY ?? anchor?.Y ?? defaultY,
                            Width = s.SignatureWidth ?? anchor?.Width ?? 180m,
                            Height = s.SignatureHeight ?? anchor?.Height ?? 45m
                        };
                    }).ToList();

                    SignwellCreateResult signwellResult;
                    try
                    {
                        signwellResult = await signwell.CreateSigningRequestAsync(item.LeaseId, providerPdfBytes, fileName, signers, item.Sequential, item.Subject, item.Message, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create provider signing request for {SigningRequestId}", item.SigningRequestId);
                        if (placeholder != null)
                        {
                            placeholder.StorageReference = uploadResult.BlobName;
                            placeholder.StorageContainer = uploadResult.Container;
                            placeholder.Url = viewablePdfUrl;
                            placeholder.OriginalFilename = fileName;
                            placeholder.DocumentStatus = "FAILED";
                            placeholder.UpdatedDate = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                        continue;
                    }

                    await using var tx = await dbContext.Database.BeginTransactionAsync(stoppingToken);
                    try
                    {
                        if (placeholder != null)
                        {
                            placeholder.StorageReference = uploadResult.BlobName;
                            placeholder.StorageContainer = uploadResult.Container;
                            placeholder.Url = viewablePdfUrl;
                            placeholder.DocumentStatus = "SENT_FOR_SIGNATURE";
                            placeholder.SentForSignatureAt = DateTime.UtcNow;
                            placeholder.OriginalFilename = fileName;
                        }

                        if (item.LeaseDocumentId.HasValue && signwellResult != null)
                        {
                            var existingSignatories = await dbContext.LeaseSignatories
                                .Where(x => x.LeaseDocumentID == item.LeaseDocumentId.Value)
                                .ToListAsync(stoppingToken);

                            foreach (var s in item.Signatories)
                            {
                                var match = existingSignatories.FirstOrDefault(x =>
                                    string.Equals(x.Email, s.Email, StringComparison.OrdinalIgnoreCase));

                                if (match == null)
                                    continue;

                                match.ProviderRequestID = signwellResult.ProviderRequestId;
                                match.ProviderDocumentID = signwellResult.ProviderDocumentId;
                                if (signwellResult.SignerIdsByEmail != null && signwellResult.SignerIdsByEmail.TryGetValue(s.Email, out var providerSignerId))
                                    match.ProviderSignerID = providerSignerId;
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                        await tx.CommitAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        await tx.RollbackAsync(stoppingToken);
                        _logger.LogError(ex, "Failed transactional updates for signing request {SigningRequestId}", item.SigningRequestId);
                        throw;
                    }

                    _logger.LogInformation("Signing request {SigningRequestId} for lease {LeaseId} processed and sent to provider.", item.SigningRequestId, item.LeaseId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error while processing signing work item.");
                }
            }
        }

        private static string BuildLeaseAgreementFileName(string? unitName, string? tenantName)
        {
            var cleanedUnitName = SanitizeFileNamePart(unitName);
            var cleanedTenantName = SanitizeFileNamePart(tenantName);

            if (string.IsNullOrWhiteSpace(cleanedUnitName))
                cleanedUnitName = "Unit";
            if (string.IsNullOrWhiteSpace(cleanedTenantName))
                cleanedTenantName = "Tenant";

            return $"Lease Agreement For {cleanedUnitName} - {cleanedTenantName}.pdf";
        }

        private static string SanitizeFileNamePart(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(value.Trim().Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            return string.Join(' ', cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
