using System.Security.Claims;
using System.Text.Json;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Signing;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "User, LandLord, Admin")]
    [Route("api/leases/{leaseId:guid}/signing-requests")]
    public class LeaseSigningController : ControllerBase
    {
        private readonly ILeaseRepository _leaseRepository;
        private readonly ILeaseDocumentsService _leaseDocumentsService;
        private readonly ILeaseSignatoriesService _leaseSignatoriesService;
        private readonly ISigningQueue _signingQueue;
        private readonly ArcoraDbContext _dbContext;
        private readonly ISignwellClient _signwellClient;
        private readonly ILogger<LeaseSigningController> _logger;

        public LeaseSigningController(
            ILeaseRepository leaseRepository,
            ILeaseDocumentsService leaseDocumentsService,
            ILeaseSignatoriesService leaseSignatoriesService,
            ISigningQueue signingQueue,
            ArcoraDbContext dbContext,
            ISignwellClient signwellClient,
            ILogger<LeaseSigningController> logger)
        {
            _leaseRepository = leaseRepository;
            _leaseDocumentsService = leaseDocumentsService;
            _leaseSignatoriesService = leaseSignatoriesService;
            _signingQueue = signingQueue;
            _dbContext = dbContext;
            _signwellClient = signwellClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> StartSigning([FromRoute] Guid leaseId, [FromBody] StartSigningRequestDto request)
        {
            if (request == null || request.Signatories == null || !request.Signatories.Any())
                return BadRequest("Signatories are required.");

            var lease = await _leaseRepository.GetByID(leaseId);
            if (lease == null)
                return NotFound();

            if (!TryGetActorUserId(out var actorUserId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            if (!await CanAccessLeaseAsync(lease, actorUserId, isAdmin))
                return Forbid();

            var effectiveSignatories = request.Signatories.ToList();

            var organizationDisplayName = await _dbContext.Organizations
                .AsNoTracking()
                .Where(x => x.OrganizationID == lease.OrganizationID)
                .Select(x => x.DisplayName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(organizationDisplayName))
                return BadRequest("Organization display name is not configured.");

            var primaryOwner = await _dbContext.OrganizationMembers
                .AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.OrganizationID == lease.OrganizationID && x.IsPrimaryOwner && x.DeactivatedAt == null);

            if (primaryOwner == null || primaryOwner.User == null || string.IsNullOrWhiteSpace(primaryOwner.User.Email))
                return BadRequest("Primary owner is not configured for this organization.");

            var landlordSignerName = organizationDisplayName.Trim();

            var landlordSignatory = effectiveSignatories.FirstOrDefault(s =>
                string.Equals(s.Role, "LANDLORD", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(s.Role, "OWNER", StringComparison.OrdinalIgnoreCase) ||
                (s.OrganizationMemberId.HasValue && s.OrganizationMemberId.Value == primaryOwner.OrganizationMemberID) ||
                (!string.IsNullOrWhiteSpace(s.Email) && string.Equals(s.Email, primaryOwner.User.Email, StringComparison.OrdinalIgnoreCase)));

            if (landlordSignatory == null)
            {
                effectiveSignatories.Add(new Arcora.Api.Models.SigningPartyDto
                {
                    OrganizationMemberId = primaryOwner.OrganizationMemberID,
                    UserId = primaryOwner.UserID,
                    Email = primaryOwner.User.Email!,
                    Name = landlordSignerName,
                    Role = "LANDLORD"
                });
            }
            else
            {
                landlordSignatory.OrganizationMemberId = primaryOwner.OrganizationMemberID;
                landlordSignatory.UserId = primaryOwner.UserID;
                landlordSignatory.Email = primaryOwner.User.Email!;
                landlordSignatory.Name = landlordSignerName;
                landlordSignatory.Role = "LANDLORD";
            }

            var signingRequestId = Guid.NewGuid();
            Guid? createdLeaseDocumentId = null;

            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Create or reuse a placeholder LeaseDocuments record that will be updated by the background worker
                var storageRef = $"pending/{Guid.NewGuid():N}.pdf";
                var originalFileName = string.IsNullOrWhiteSpace(lease.LeaseNumber) ? $"lease_{leaseId}.pdf" : $"lease_{lease.LeaseNumber}.pdf";

                var existingDoc = await _dbContext.LeaseDocuments
                    .FirstOrDefaultAsync(x =>
                        x.LeaseID == leaseId &&
                        x.TenantID == lease.TenantID &&
                        x.DocumentType == "LEASE_AGREEMENT" &&
                        (x.DocumentStatus == "QUEUED" || x.DocumentStatus == "PENDING_REVIEW" || x.DocumentStatus == "FAILED"));

                LeaseDocumentsDto createdDoc;
                if (existingDoc != null)
                {
                    createdDoc = new DTOs.LeaseDocumentsDto
                    {
                        LeaseDocumentID = existingDoc.LeaseDocumentID,
                        LeaseID = existingDoc.LeaseID,
                        ListingID = existingDoc.ListingID,
                        TenantID = existingDoc.TenantID,
                        RentalApplicationID = existingDoc.RentalApplicationID,
                        LeaseRenewalID = existingDoc.LeaseRenewalID,
                        DocumentType = existingDoc.DocumentType,
                        DocumentStatus = existingDoc.DocumentStatus,
                        OriginalFilename = existingDoc.OriginalFilename,
                        StorageProvider = existingDoc.StorageProvider,
                        StorageContainer = existingDoc.StorageContainer,
                        StorageReference = existingDoc.StorageReference,
                        Url = existingDoc.Url,
                        FileHash = existingDoc.FileHash,
                        IsPrimary = existingDoc.IsPrimary,
                        GeneratedAt = existingDoc.GeneratedAt,
                        SentForSignatureAt = existingDoc.SentForSignatureAt,
                        FullySignedAt = existingDoc.FullySignedAt,
                        CapturedBy = existingDoc.CapturedBy,
                        CapturedDate = existingDoc.CapturedDate,
                        UpdatedBy = existingDoc.UpdatedBy,
                        UpdatedDate = existingDoc.UpdatedDate
                    };
                }
                else
                {
                    var docDto = new DTOs.LeaseDocumentsDto
                    {
                        LeaseID = leaseId,
                        ListingID = lease.ListingID,
                        TenantID = lease.TenantID,
                        RentalApplicationID = lease.RentalApplicationID,
                        DocumentType = "LEASE_AGREEMENT",
                        DocumentStatus = "QUEUED",
                        OriginalFilename = originalFileName,
                        StorageProvider = "AZURE_BLOB",
                        StorageReference = storageRef,
                        IsPrimary = true,
                        GeneratedAt = DateTime.UtcNow,
                        CapturedBy = request.CapturedBy,
                        CapturedDate = DateTime.UtcNow
                    };

                    createdDoc = await _leaseDocumentsService.CreateLeaseDocuments(docDto);
                }

                createdLeaseDocumentId = createdDoc.LeaseDocumentID;

                var existingSignatories = await _dbContext.LeaseSignatories
                    .Where(x => x.LeaseDocumentID == createdDoc.LeaseDocumentID!.Value)
                    .Select(x => x.Email)
                    .ToListAsync();

                // Create signatory records linked to the placeholder document
                foreach (var s in effectiveSignatories)
                {
                    if (existingSignatories.Any(email => string.Equals(email, s.Email, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var signatoryDto = new DTOs.LeaseSignatoriesDto
                    {
                        LeaseDocumentID = createdDoc.LeaseDocumentID!.Value,
                        UserID = s.UserId,
                        TenantID = s.TenantId,
                        OrganizationMemberID = s.OrganizationMemberId,
                        OrganizationID = lease.OrganizationID,
                        SignatoryRole = s.Role ?? "OTHER",
                        Name = s.Name,
                        Email = s.Email,
                        Status = "PENDING",
                        SignatureOrder = s.SignatureOrder,
                        ProviderSignerID = null,
                        CapturedBy = request.CapturedBy,
                        CapturedDate = DateTime.UtcNow
                    };

                    await _leaseSignatoriesService.CreateLeaseSignatories(signatoryDto);
                }

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to start signing for lease {LeaseId}", leaseId);
                throw;
            }

            var workItem = new SigningWorkItem
            {
                SigningRequestId = signingRequestId,
                LeaseId = leaseId,
                LeaseDocumentId = createdLeaseDocumentId,
                LeaseContractTemplateId = request.LeaseContractTemplateId,
                Signatories = effectiveSignatories,
                Sequential = request.Sequential,
                Subject = request.Subject,
                Message = request.Message,
                ReturnUrl = request.ReturnUrl,
                WebhookCallbackUrl = request.WebhookCallbackUrl,
                CapturedBy = request.CapturedBy
            };

            await _signingQueue.EnqueueAsync(workItem);

            var response = new StartSigningResponseDto
            {
                SigningRequestId = signingRequestId,
                LeaseId = leaseId,
                Status = "Queued"
            };

            return AcceptedAtAction(nameof(GetSigningStatus), new { leaseId = leaseId, signingRequestId = signingRequestId }, response);
        }

        [HttpGet("{signingRequestId:guid}")]
        public async Task<IActionResult> GetSigningStatus([FromRoute] Guid leaseId, [FromRoute] Guid signingRequestId, [FromQuery] bool refreshFromProvider = false, CancellationToken cancellationToken = default)
        {
            var lease = await _leaseRepository.GetByID(leaseId);
            if (lease == null)
                return NotFound();

            if (!TryGetActorUserId(out var actorUserId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            if (!await CanAccessLeaseAsync(lease, actorUserId, isAdmin, cancellationToken))
                return Forbid();

            var leaseDocument = await _dbContext.LeaseDocuments
                .Where(x => x.LeaseID == leaseId)
                .OrderByDescending(x => x.CapturedDate ?? x.GeneratedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (leaseDocument == null)
            {
                return Ok(new SigningRequestStatusDto
                {
                    SigningRequestId = signingRequestId,
                    LeaseId = leaseId,
                    Status = "Queued"
                });
            }

            var signatories = await _dbContext.LeaseSignatories
                .Where(x => x.LeaseDocumentID == leaseDocument.LeaseDocumentID)
                .OrderBy(x => x.SignatureOrder ?? int.MaxValue)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            SignwellDocumentStatusResult? providerStatus = null;
            if (refreshFromProvider)
            {
                var providerDocumentId = signatories
                    .Select(x => x.ProviderDocumentID)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                if (!string.IsNullOrWhiteSpace(providerDocumentId))
                {
                    try
                    {
                        providerStatus = await _signwellClient.GetDocumentStatusAsync(providerDocumentId!, cancellationToken);
                        ApplyProviderStatus(leaseDocument, signatories, providerStatus);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to refresh Signwell status for lease {LeaseId} and document {ProviderDocumentId}", leaseId, providerDocumentId);
                    }
                }
            }

            return Ok(BuildSigningStatusResponse(signingRequestId, leaseId, leaseDocument, signatories, providerStatus, refreshFromProvider));
        }

        private static SigningRequestStatusDto BuildSigningStatusResponse(
            Guid signingRequestId,
            Guid leaseId,
            LeaseDocuments leaseDocument,
            List<LeaseSignatory> signatories,
            SignwellDocumentStatusResult? providerStatus,
            bool refreshedFromProvider)
        {
            var providerDocumentId = signatories.Select(x => x.ProviderDocumentID).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            var providerRequestId = signatories.Select(x => x.ProviderRequestID).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            return new SigningRequestStatusDto
            {
                SigningRequestId = signingRequestId,
                LeaseId = leaseId,
                LeaseDocumentId = leaseDocument.LeaseDocumentID,
                Status = leaseDocument.DocumentStatus ?? "Unknown",
                ProviderRequestId = providerStatus?.ProviderRequestId ?? providerRequestId,
                ProviderDocumentId = providerStatus?.ProviderDocumentId ?? providerDocumentId,
                ProviderStatus = providerStatus?.Status,
                DocumentUrl = leaseDocument.Url,
                SentForSignatureAt = leaseDocument.SentForSignatureAt,
                FullySignedAt = leaseDocument.FullySignedAt,
                RefreshedFromProvider = refreshedFromProvider,
                Signatories = signatories.Select(x => new SigningRequestSignerStatusDto
                {
                    LeaseSignatoryId = x.LeaseSignatoryID,
                    Name = x.Name,
                    Email = x.Email,
                    Role = x.SignatoryRole,
                    Status = x.Status,
                    SignatureOrder = x.SignatureOrder,
                    ProviderSignerId = x.ProviderSignerID,
                    ViewedAt = x.ViewedAt,
                    SignedAt = x.SignedAt,
                    DeclinedAt = x.DeclinedAt
                }).ToList()
            };
        }

        private static void ApplyProviderStatus(LeaseDocuments leaseDocument, List<LeaseSignatory> signatories, SignwellDocumentStatusResult providerStatus)
        {
            var now = DateTime.UtcNow;

            foreach (var local in signatories)
            {
                local.ProviderRequestID ??= providerStatus.ProviderRequestId;
                local.ProviderDocumentID ??= providerStatus.ProviderDocumentId;

                var providerSigner = providerStatus.Signers.FirstOrDefault(x =>
                    (!string.IsNullOrWhiteSpace(local.ProviderSignerID) && string.Equals(x.ProviderSignerId, local.ProviderSignerID, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(local.Email) && string.Equals(x.Email, local.Email, StringComparison.OrdinalIgnoreCase)));

                if (providerSigner == null)
                    continue;

                var normalizedStatus = NormalizeSignatoryStatus(providerSigner.Status);
                if (!string.IsNullOrWhiteSpace(normalizedStatus))
                    local.Status = normalizedStatus;

                local.ViewedAt ??= providerSigner.ViewedAt?.UtcDateTime;
                local.SignedAt ??= providerSigner.SignedAt?.UtcDateTime;
                local.DeclinedAt ??= providerSigner.DeclinedAt?.UtcDateTime;

                if (string.Equals(local.Status, "SIGNED", StringComparison.OrdinalIgnoreCase))
                    local.SignedAt ??= now;
                else if (string.Equals(local.Status, "DECLINED", StringComparison.OrdinalIgnoreCase))
                    local.DeclinedAt ??= now;
                else if (string.Equals(local.Status, "VIEWED", StringComparison.OrdinalIgnoreCase))
                    local.ViewedAt ??= now;
            }

            leaseDocument.UpdatedDate = now;

            var normalizedDocumentStatus = NormalizeDocumentStatus(providerStatus.Status);
            if (!string.IsNullOrWhiteSpace(normalizedDocumentStatus))
                leaseDocument.DocumentStatus = normalizedDocumentStatus;

            if (string.Equals(leaseDocument.DocumentStatus, "FULLY_SIGNED", StringComparison.OrdinalIgnoreCase))
            {
                leaseDocument.FullySignedAt ??= providerStatus.CompletedAt?.UtcDateTime ?? now;
            }
            else if (signatories.Any(x => string.Equals(x.Status, "DECLINED", StringComparison.OrdinalIgnoreCase)))
            {
                leaseDocument.DocumentStatus = "DECLINED";
            }
            else if (signatories.Count > 0 && signatories.All(x => string.Equals(x.Status, "SIGNED", StringComparison.OrdinalIgnoreCase)))
            {
                leaseDocument.DocumentStatus = "FULLY_SIGNED";
                leaseDocument.FullySignedAt ??= providerStatus.CompletedAt?.UtcDateTime ?? now;
            }
            else if (signatories.Any(x => string.Equals(x.Status, "SIGNED", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Status, "VIEWED", StringComparison.OrdinalIgnoreCase)))
            {
                leaseDocument.DocumentStatus = "SENT_FOR_SIGNATURE";
            }
        }

        private static string? NormalizeSignatoryStatus(string? providerStatus)
        {
            if (string.IsNullOrWhiteSpace(providerStatus))
                return null;

            var status = providerStatus.Trim().ToLowerInvariant();
            if (status.Contains("declin") || status.Contains("reject"))
                return "DECLINED";
            if (status.Contains("sign") || status.Contains("complete"))
                return "SIGNED";
            if (status.Contains("view") || status.Contains("open"))
                return "VIEWED";
            if (status.Contains("sent") || status.Contains("pending") || status.Contains("deliver"))
                return "PENDING";

            return providerStatus;
        }

        private static string? NormalizeDocumentStatus(string? providerStatus)
        {
            if (string.IsNullOrWhiteSpace(providerStatus))
                return null;

            var status = providerStatus.Trim().ToLowerInvariant();
            if (status.Contains("declin") || status.Contains("reject") || status.Contains("cancel"))
                return "DECLINED";
            if (status.Contains("complete") || status.Contains("fully_signed"))
                return "FULLY_SIGNED";
            if (status.Contains("sign") || status.Contains("view") || status.Contains("open") || status.Contains("sent") || status.Contains("pending") || status.Contains("deliver"))
                return "SENT_FOR_SIGNATURE";

            return providerStatus;
        }

        private bool TryGetActorUserId(out long actorUserId)
        {
            actorUserId = 0;
            var claim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return long.TryParse(claim, out actorUserId) && actorUserId > 0;
        }

        private async Task<bool> CanAccessLeaseAsync(Lease lease, long actorUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            if (isAdmin)
                return true;

            var isTenantOwner = await _dbContext.Tenants
                .AsNoTracking()
                .AnyAsync(t => t.TenantID == lease.TenantID && t.UserID == actorUserId, cancellationToken);

            if (isTenantOwner)
                return true;

            return await _dbContext.OrganizationMembers
                .AsNoTracking()
                .AnyAsync(m =>
                    m.OrganizationID == lease.OrganizationID &&
                    m.UserID == actorUserId &&
                    !string.Equals(m.Status, "INVITED", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(m.Status, "REJECTED", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(m.Status, "DEACTIVATED", StringComparison.OrdinalIgnoreCase),
                    cancellationToken);
        }
    }
}
