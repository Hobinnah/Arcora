using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Enums;
using Arcora.Api.Exceptions;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace Arcora.Api.Services.Implementations;

public class LeaseCheckInService : ILeaseCheckInService
{
    private const string MoveInInspectionType = "MOVE_IN";
    private const string DraftStatus = "DRAFT";
    private const string SubmittedStatus = "SUBMITTED";
    private const string CompletedStatus = "COMPLETED";
    private const string InspectionEntityType = "Inspection";
    private const string InspectionItemEntityType = "InspectionItem";
    private const string MoveInAttachmentType = "MOVE_IN_CHECK_IN";
    private static readonly string[] AllowedReadinessValues = ["READY", "NEEDS_ATTENTION", "NOT_SURE"];
    private static readonly string[] AllowedImageFormats = ["JPEG", "PNG", "WEBP"];
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ArcoraDbContext context;
    private readonly IFileStorageService fileStorageService;
    private readonly IMessagingService messagingService;
    private readonly BlobStorageConfiguration blobOptions;
    private readonly ILogger<LeaseCheckInService> logger;

    public LeaseCheckInService(
        ArcoraDbContext context,
        IFileStorageService fileStorageService,
        IMessagingService messagingService,
        IOptions<BlobStorageConfiguration> blobOptions,
        ILogger<LeaseCheckInService> logger)
    {
        this.context = context;
        this.fileStorageService = fileStorageService;
        this.messagingService = messagingService;
        this.blobOptions = blobOptions.Value;
        this.logger = logger;
    }

    public async Task<LeaseCheckInResponseDto> GetAsync(Guid leaseId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var lease = await GetAuthorizedLeaseAsync(leaseId, user, cancellationToken);
        var inspection = await GetMoveInInspectionAsync(leaseId, true, cancellationToken);

        return new LeaseCheckInResponseDto
        {
            LeaseID = lease.LeaseID,
            Eligibility = BuildEligibility(lease, inspection),
            Inspection = inspection == null ? null : await BuildInspectionDtoAsync(inspection, cancellationToken)
        };
    }

    public async Task<(LeaseCheckInDraftResponseDto Draft, bool Created)> CreateOrGetDraftAsync(Guid leaseId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var lease = await GetAuthorizedLeaseAsync(leaseId, user, cancellationToken);
        var existing = await GetMoveInInspectionAsync(leaseId, false, cancellationToken);
        if (existing != null)
        {
            if (IsSubmitted(existing))
            {
                throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Check-in already submitted", "The move-in check-in has already been submitted.", "ALREADY_SUBMITTED");
            }

            return (new LeaseCheckInDraftResponseDto
            {
                InspectionID = existing.InspectionID,
                Version = existing.Version ?? string.Empty
            }, false);
        }

        var eligibility = BuildEligibility(lease, null);
        if (!eligibility.CanSubmit)
        {
            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Lease is not eligible for move-in check-in", eligibility.Message ?? "The lease is not eligible.", eligibility.ReasonCode);
        }

        var userId = GetRequiredUserId(user);
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        existing = await context.Inspections.SingleOrDefaultAsync(
            x => x.LeaseID == leaseId && x.InspectionType == MoveInInspectionType,
            cancellationToken);

        if (existing != null)
        {
            await transaction.CommitAsync(cancellationToken);
            if (IsSubmitted(existing))
            {
                throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Check-in already submitted", "The move-in check-in has already been submitted.", "ALREADY_SUBMITTED");
            }

            return (new LeaseCheckInDraftResponseDto
            {
                InspectionID = existing.InspectionID,
                Version = existing.Version ?? string.Empty
            }, false);
        }

        var inspection = new Inspection
        {
            InspectionID = Guid.NewGuid(),
            LeaseID = lease.LeaseID,
            RentalUnitID = lease.RentalUnitID,
            PropertyID = lease.RentalUnit?.PropertyID,
            InspectionType = MoveInInspectionType,
            Status = DraftStatus,
            Version = Guid.NewGuid().ToString("N"),
            CapturedDate = now,
            CapturedBy = userId.ToString(CultureInfo.InvariantCulture),
            UpdatedDate = now,
            UpdatedBy = userId.ToString(CultureInfo.InvariantCulture)
        };

        context.Inspections.Add(inspection);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return (new LeaseCheckInDraftResponseDto
        {
            InspectionID = inspection.InspectionID,
            Version = inspection.Version ?? string.Empty
        }, true);
    }

    public async Task<LeaseCheckInPhotoUploadResponseDto> UploadPhotoAsync(Guid leaseId, LeaseCheckInPhotoUploadRequestDto request, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lease = await GetAuthorizedLeaseAsync(leaseId, user, cancellationToken);
        var userId = GetRequiredUserId(user);

        if (request.File == null || request.File.Length == 0)
        {
            throw ValidationError("file", "A photo file is required.");
        }

        if (request.File.Length > 10 * 1024 * 1024)
        {
            throw new LeaseCheckInException(StatusCodes.Status413PayloadTooLarge, "Photo too large", "Each photo must be 10 MiB or smaller.", errors: new Dictionary<string, string[]> { ["file"] = ["Each photo must be 10 MiB or smaller."] });
        }

        if (string.IsNullOrWhiteSpace(request.ClientPhotoID))
        {
            throw ValidationError("clientPhotoID", "A clientPhotoID is required.");
        }

        if (request.ClientPhotoID.Length > 100)
        {
            throw ValidationError("clientPhotoID", "The clientPhotoID must be 100 characters or fewer.");
        }

        if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 1000)
        {
            throw ValidationError("description", "The description must be 1000 characters or fewer.");
        }

        if ((request.File.FileName?.Length ?? 0) > 100)
        {
            throw ValidationError("file", "The file name must be 100 characters or fewer.");
        }

        var (fileBytes, contentType, checksum) = await ReadAndValidateImageAsync(request.File, cancellationToken);

        Attachment? existing;
        Attachment pendingAttachment;

        await using (var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
        {
            var inspection = await context.Inspections.SingleOrDefaultAsync(
                x => x.InspectionID == request.InspectionID &&
                     x.LeaseID == lease.LeaseID &&
                     x.InspectionType == MoveInInspectionType,
                cancellationToken);

            if (inspection == null)
            {
                throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Draft not found", "The move-in draft was not found.");
            }

            if (!string.Equals(inspection.Status, DraftStatus, StringComparison.OrdinalIgnoreCase))
            {
                throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Draft is no longer editable", "Photos cannot be uploaded after submission.", "ALREADY_SUBMITTED");
            }

            existing = await context.Attachments.SingleOrDefaultAsync(
                x => x.EntityType == InspectionEntityType &&
                     x.EntityID == inspection.InspectionID.ToString() &&
                     x.ClientUploadID == request.ClientPhotoID,
                cancellationToken);

            if (existing != null)
            {
                if (!string.Equals(existing.ChecksumSha256, checksum, StringComparison.OrdinalIgnoreCase))
                {
                    throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Duplicate clientPhotoID", "The same clientPhotoID cannot be reused for different file content.");
                }

                if (string.Equals(existing.UploadStatus, "READY", StringComparison.OrdinalIgnoreCase))
                {
                    await transaction.CommitAsync(cancellationToken);
                    return await BuildUploadResponseAsync(existing, cancellationToken);
                }

                throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Upload already in progress", "The photo is already being processed for this clientPhotoID.");
            }

            var activePhotoCount = await context.Attachments.CountAsync(
                x => x.EntityType == InspectionEntityType &&
                     x.EntityID == inspection.InspectionID.ToString() &&
                     x.AttachmentType == MoveInAttachmentType &&
                     x.UploadStatus != "FAILED" &&
                     x.UploadStatus != "ABANDONED",
                cancellationToken);

            if (activePhotoCount >= 10)
            {
                throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Photo limit reached", "A move-in check-in can include at most 10 active photos.", errors: new Dictionary<string, string[]> { ["file"] = ["A move-in check-in can include at most 10 active photos."] });
            }

            pendingAttachment = new Attachment
            {
                AttachmentID = Guid.NewGuid(),
                EntityType = InspectionEntityType,
                EntityID = inspection.InspectionID.ToString(),
                FileName = request.File.FileName,
                MimeType = contentType,
                FileSizeBytes = fileBytes.LongLength,
                StorageProvider = "AZURE_BLOB",
                StorageReference = $"pending/{Guid.NewGuid():N}",
                AttachmentType = MoveInAttachmentType,
                Description = request.Description,
                ClientUploadID = request.ClientPhotoID,
                ChecksumSha256 = checksum,
                UploadStatus = "PENDING",
                UploadExpiresAt = DateTime.UtcNow.AddHours(24),
                CapturedDate = DateTime.UtcNow,
                CapturedBy = userId.ToString(CultureInfo.InvariantCulture)
            };

            context.Attachments.Add(pendingAttachment);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        BlobUploadResult uploadResult;
        try
        {
            await using var stream = new MemoryStream(fileBytes);
            uploadResult = await fileStorageService.UploadAsync(StorageCategory.Image, request.File.FileName, stream, contentType, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Move-in evidence upload failed for draft {InspectionID}. Timestamp: {Timestamp}", request.InspectionID, DateTime.UtcNow);
            await MarkAttachmentFailedAsync(pendingAttachment.AttachmentID, ex.Message, cancellationToken);
            throw new LeaseCheckInException(StatusCodes.Status503ServiceUnavailable, "Photo upload failed", "The photo upload did not complete successfully.");
        }

        try
        {
            var attachment = await context.Attachments.SingleAsync(x => x.AttachmentID == pendingAttachment.AttachmentID, cancellationToken);
            attachment.StorageReference = uploadResult.BlobName;
            attachment.StorageProvider = uploadResult.Container;
            attachment.MimeType = contentType;
            attachment.FileSizeBytes = fileBytes.LongLength;
            attachment.UploadStatus = "READY";
            attachment.UploadFailureReason = null;
            attachment.CapturedDate ??= DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            return await BuildUploadResponseAsync(attachment, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Move-in evidence database finalization failed for attachment {AttachmentID}. Timestamp: {Timestamp}", pendingAttachment.AttachmentID, DateTime.UtcNow);
            throw new LeaseCheckInException(StatusCodes.Status503ServiceUnavailable, "Photo upload failed", "The photo metadata could not be finalized.");
        }
    }

    public async Task DeletePhotoAsync(Guid leaseId, Guid attachmentId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var lease = await GetAuthorizedLeaseAsync(leaseId, user, cancellationToken);

        var attachment = await context.Attachments.SingleOrDefaultAsync(x => x.AttachmentID == attachmentId, cancellationToken);
        if (attachment == null)
        {
            return;
        }

        if (!string.Equals(attachment.AttachmentType, MoveInAttachmentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Photo not found", "The requested photo was not found.");
        }

        var inspection = await ResolveInspectionForAttachmentAsync(attachment, cancellationToken);
        if (inspection == null || inspection.LeaseID != lease.LeaseID || !string.Equals(inspection.InspectionType, MoveInInspectionType, StringComparison.OrdinalIgnoreCase))
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Photo not found", "The requested photo was not found.");
        }

        if (!string.Equals(inspection.Status, DraftStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Draft is no longer editable", "Submitted evidence cannot be deleted by the tenant.");
        }

        if (!string.IsNullOrWhiteSpace(attachment.StorageReference) && !attachment.StorageReference.StartsWith("pending/", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await fileStorageService.DeleteAsync(StorageCategory.Image, attachment.StorageReference, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Move-in evidence blob deletion failed for attachment {AttachmentID}. Timestamp: {Timestamp}", attachmentId, DateTime.UtcNow);
                throw new LeaseCheckInException(StatusCodes.Status503ServiceUnavailable, "Photo deletion failed", "The photo could not be deleted from storage.");
            }
        }

        context.Attachments.Remove(attachment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<LeaseCheckInInspectionDto> SubmitAsync(Guid leaseId, SubmitLeaseCheckInRequestDto request, string idempotencyKey, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new LeaseCheckInException(StatusCodes.Status400BadRequest, "Missing idempotency key", "The Idempotency-Key header is required.");
        }

        ValidateSubmitRequest(request);
        var requestHash = ComputeRequestHash(request);
        var lease = await GetAuthorizedLeaseAsync(leaseId, user, cancellationToken);
        var eligibility = BuildEligibility(lease, null);
        var userId = GetRequiredUserId(user);
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var inspection = await context.Inspections.SingleOrDefaultAsync(
            x => x.InspectionID == request.InspectionID &&
                 x.LeaseID == lease.LeaseID &&
                 x.InspectionType == MoveInInspectionType,
            cancellationToken);

        if (inspection == null)
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Draft not found", "The move-in draft was not found.");
        }

        if (IsSubmitted(inspection))
        {
            await transaction.CommitAsync(cancellationToken);
            if (string.Equals(inspection.SubmissionIdempotencyKey, idempotencyKey, StringComparison.Ordinal) &&
                string.Equals(inspection.SubmissionRequestHash, requestHash, StringComparison.Ordinal))
            {
                return await BuildInspectionDtoAsync(inspection, cancellationToken);
            }

            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Check-in already submitted", "The move-in check-in has already been submitted.", "ALREADY_SUBMITTED");
        }

        if (!eligibility.CanSubmit)
        {
            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Lease is not eligible for move-in check-in", eligibility.Message ?? "The lease is not eligible.", eligibility.ReasonCode);
        }

        if (!string.Equals(inspection.Version, request.Version, StringComparison.Ordinal))
        {
            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Concurrency conflict", "The check-in draft has changed. Refresh and try again.");
        }

        var readyAttachments = await context.Attachments
            .Where(x => x.EntityType == InspectionEntityType &&
                        x.EntityID == inspection.InspectionID.ToString() &&
                        x.AttachmentType == MoveInAttachmentType &&
                        x.UploadStatus == "READY")
            .ToListAsync(cancellationToken);

        var readyAttachmentLookup = readyAttachments.ToDictionary(x => x.AttachmentID);
        var photoLookup = request.Photos.ToDictionary(x => x.AttachmentID);

        foreach (var photo in request.Photos)
        {
            if (!readyAttachmentLookup.ContainsKey(photo.AttachmentID))
            {
                throw ValidationError("photos", "Every submitted photo must belong to the editable draft and be fully uploaded.");
            }
        }

        var duplicateAttachmentUsage = request.Items
            .SelectMany(x => x.AttachmentIDs)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);
        if (duplicateAttachmentUsage != null)
        {
            throw ValidationError("items", "A photo can be assigned to only one issue.");
        }

        foreach (var item in request.Items)
        {
            foreach (var attachmentId in item.AttachmentIDs)
            {
                if (!photoLookup.ContainsKey(attachmentId))
                {
                    throw ValidationError("items", "Each issue attachment must also appear in the top-level photos collection.");
                }
            }
        }

        var itemEntities = new List<InspectionItem>();
        foreach (var item in request.Items)
        {
            var entity = new InspectionItem
            {
                InspectionItemID = Guid.NewGuid(),
                InspectionID = inspection.InspectionID,
                Area = item.Area.Trim(),
                ItemName = item.ItemName.Trim(),
                Condition = item.Condition?.Trim(),
                Notes = item.Notes?.Trim(),
                RequiresRepair = null,
                EstimatedRepairCost = null,
                CapturedDate = now,
                CapturedBy = userId.ToString(CultureInfo.InvariantCulture)
            };

            itemEntities.Add(entity);
        }

        if (itemEntities.Count != 0)
        {
            context.InspectionItems.AddRange(itemEntities);
        }

        var itemByClientId = itemEntities.Zip(request.Items, (entity, dto) => new { entity, dto.ClientItemID })
            .ToDictionary(x => x.ClientItemID, x => x.entity);

        foreach (var attachment in readyAttachments)
        {
            if (!photoLookup.TryGetValue(attachment.AttachmentID, out var photoDto))
            {
                attachment.UploadStatus = "ABANDONED";
                attachment.UploadExpiresAt = now.AddHours(24);
                continue;
            }

            attachment.Description = photoDto.Description?.Trim();
            attachment.UploadExpiresAt = null;
            attachment.UploadFailureReason = null;
            attachment.EntityType = InspectionEntityType;
            attachment.EntityID = inspection.InspectionID.ToString();
        }

        foreach (var item in request.Items)
        {
            var itemEntity = itemByClientId[item.ClientItemID];
            foreach (var attachmentId in item.AttachmentIDs)
            {
                var attachment = readyAttachmentLookup[attachmentId];
                attachment.EntityType = InspectionItemEntityType;
                attachment.EntityID = itemEntity.InspectionItemID.ToString();
            }
        }

        inspection.Status = SubmittedStatus;
        inspection.OccupancyReadiness = request.OccupancyReadiness.Trim().ToUpperInvariant();
        inspection.ArrivalConfirmedAt = now;
        inspection.SubmittedAt = now;
        inspection.SubmittedByUserID = userId;
        inspection.Notes = request.Notes?.Trim();
        inspection.SubmissionIdempotencyKey = idempotencyKey;
        inspection.SubmissionRequestHash = requestHash;
        inspection.UpdatedDate = now;
        inspection.UpdatedBy = userId.ToString(CultureInfo.InvariantCulture);
        inspection.Version = Guid.NewGuid().ToString("N");

        if (!lease.ActualMoveInAt.HasValue)
        {
            lease.ActualMoveInAt = now;
            lease.UpdatedDate = now;
            lease.UpdatedBy = userId.ToString(CultureInfo.InvariantCulture);
        }

        await CreateNotificationsAsync(lease, inspection, now, cancellationToken);
        await SendLandlordChatMessageAsync(lease, inspection, request, now, userId, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await BuildInspectionDtoAsync(inspection, cancellationToken);
    }

    public async Task<InspectionReviewRecordDto> CreateReviewAsync(Guid inspectionId, InspectionReviewRequestDto request, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var inspection = await context.Inspections.SingleOrDefaultAsync(
            x => x.InspectionID == inspectionId && x.InspectionType == MoveInInspectionType,
            cancellationToken);

        if (inspection == null)
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Inspection not found", "The requested move-in check-in was not found.");
        }

        var lease = await GetAuthorizedLeaseAsync(inspection.LeaseID ?? Guid.Empty, user, cancellationToken, requireLandlordAccess: true);
        if (!IsSubmitted(inspection))
        {
            throw new LeaseCheckInException(StatusCodes.Status409Conflict, "Inspection not submitted", "Only submitted move-in check-ins can be reviewed.");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw ValidationError("status", "A review status is required.");
        }

        if (!string.IsNullOrWhiteSpace(request.Comments) && request.Comments.Length > 1000)
        {
            throw ValidationError("comments", "The comments must be 1000 characters or fewer.");
        }

        var userId = GetRequiredUserId(user);
        var now = DateTime.UtcNow;
        var review = new InspectionReview
        {
            InspectionReviewID = Guid.NewGuid(),
            InspectionID = inspection.InspectionID,
            Status = request.Status.Trim().ToUpperInvariant(),
            Comments = request.Comments?.Trim(),
            ReviewedByUserID = userId,
            ReviewedAt = now,
            CapturedDate = now,
            CapturedBy = userId.ToString(CultureInfo.InvariantCulture)
        };

        context.InspectionReviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        _ = lease;
        return new InspectionReviewRecordDto
        {
            InspectionReviewID = review.InspectionReviewID,
            Status = review.Status ?? string.Empty,
            Comments = review.Comments,
            ReviewedByUserID = review.ReviewedByUserID,
            ReviewedAt = review.ReviewedAt
        };
    }

    public async Task<bool> CanAccessMoveInInspectionAsync(Guid inspectionId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var inspection = await context.Inspections.AsNoTracking().SingleOrDefaultAsync(x => x.InspectionID == inspectionId, cancellationToken);
        if (inspection == null || !string.Equals(inspection.InspectionType, MoveInInspectionType, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!inspection.LeaseID.HasValue)
        {
            return false;
        }

        try
        {
            await GetAuthorizedLeaseAsync(inspection.LeaseID.Value, user, cancellationToken);
            return true;
        }
        catch (LeaseCheckInException)
        {
            return false;
        }
    }

    public async Task<bool> CanAccessMoveInAttachmentAsync(Guid attachmentId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var attachment = await context.Attachments.AsNoTracking().SingleOrDefaultAsync(x => x.AttachmentID == attachmentId, cancellationToken);
        if (attachment == null || !string.Equals(attachment.AttachmentType, MoveInAttachmentType, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var inspection = await ResolveInspectionForAttachmentAsync(attachment, cancellationToken);
        if (inspection?.LeaseID == null)
        {
            return false;
        }

        try
        {
            await GetAuthorizedLeaseAsync(inspection.LeaseID.Value, user, cancellationToken);
            return true;
        }
        catch (LeaseCheckInException)
        {
            return false;
        }
    }

    public async Task<int> CleanupExpiredDraftEvidenceAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var attachments = await context.Attachments
            .Where(x => x.AttachmentType == MoveInAttachmentType && x.UploadExpiresAt != null && x.UploadExpiresAt < now)
            .ToListAsync(cancellationToken);

        var deletedCount = 0;
        foreach (var attachment in attachments)
        {
            if (!string.IsNullOrWhiteSpace(attachment.StorageReference) && !attachment.StorageReference.StartsWith("pending/", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await fileStorageService.DeleteAsync(StorageCategory.Image, attachment.StorageReference, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to clean up expired move-in evidence attachment {AttachmentID}. Timestamp: {Timestamp}", attachment.AttachmentID, DateTime.UtcNow);
                    continue;
                }
            }

            context.Attachments.Remove(attachment);
            deletedCount++;
        }

        if (deletedCount > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return deletedCount;
    }

    private async Task<Lease> GetAuthorizedLeaseAsync(Guid leaseId, ClaimsPrincipal user, CancellationToken cancellationToken, bool requireLandlordAccess = false)
    {
        var lease = await context.Leases
            .Include(x => x.Tenant)
            .Include(x => x.RentalUnit)
                .ThenInclude(x => x!.Property)
            .SingleOrDefaultAsync(x => x.LeaseID == leaseId, cancellationToken);

        if (lease == null)
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Lease not found", "The requested lease was not found.");
        }

        var userId = GetRequiredUserId(user);
        var isAdmin = user.IsInRole("Admin");
        var isTenant = lease.Tenant?.UserID == userId;
        var isOrgMember = isAdmin || await context.OrganizationMembers.AnyAsync(
            x => x.OrganizationID == lease.OrganizationID &&
                 x.UserID == userId &&
                 x.DeactivatedAt == null &&
                 (x.AcceptedAt != null || x.Status == "ACTIVE"),
            cancellationToken);

        if (requireLandlordAccess)
        {
            if (!isOrgMember)
            {
                throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Lease not found", "The requested lease was not found.");
            }
        }
        else if (!isTenant && !isOrgMember)
        {
            throw new LeaseCheckInException(StatusCodes.Status404NotFound, "Lease not found", "The requested lease was not found.");
        }

        return lease;
    }

    private async Task<Inspection?> GetMoveInInspectionAsync(Guid leaseId, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = context.Inspections.Where(x => x.LeaseID == leaseId && x.InspectionType == MoveInInspectionType);
        return asNoTracking
            ? await query.AsNoTracking().SingleOrDefaultAsync(cancellationToken)
            : await query.SingleOrDefaultAsync(cancellationToken);
    }

    private LeaseCheckInEligibilityDto BuildEligibility(Lease lease, Inspection? inspection)
    {
        var timeZoneId = lease.RentalUnit?.Property?.TimeZone ?? "UTC";
        var timeZone = ResolveTimeZone(timeZoneId);
        var localDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
        var moveInDate = DateOnly.FromDateTime(lease.StartDate.Date);

        string? reasonCode = null;
        string? message = null;
        var canSubmit = true;

        if (!string.Equals(lease.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            canSubmit = false;
            reasonCode = "LEASE_NOT_ACTIVE";
            message = "The lease must be active before move-in check-in can be submitted.";
        }
        else if (lease.TerminatedAt.HasValue)
        {
            canSubmit = false;
            reasonCode = "LEASE_TERMINATED";
            message = "The lease is terminated and is not eligible for move-in check-in.";
        }
        else if (localDate < lease.StartDate.Date)
        {
            canSubmit = false;
            reasonCode = "MOVE_IN_NOT_STARTED";
            message = "Move-in check-in opens on the property's local start date.";
        }
        else if (inspection != null && IsSubmitted(inspection))
        {
            canSubmit = false;
            reasonCode = "ALREADY_SUBMITTED";
            message = "A move-in check-in has already been submitted for this lease.";
        }

        return new LeaseCheckInEligibilityDto
        {
            CanSubmit = canSubmit,
            ReasonCode = reasonCode,
            Message = message,
            MoveInDate = moveInDate,
            TimeZone = timeZoneId
        };
    }

    private async Task<LeaseCheckInInspectionDto> BuildInspectionDtoAsync(Inspection inspection, CancellationToken cancellationToken)
    {
        var items = await context.InspectionItems
            .AsNoTracking()
            .Where(x => x.InspectionID == inspection.InspectionID)
            .OrderBy(x => x.CapturedDate)
            .ToListAsync(cancellationToken);

        var itemIds = items.Select(x => x.InspectionItemID.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var attachments = await context.Attachments
            .AsNoTracking()
            .Where(x => x.AttachmentType == MoveInAttachmentType && x.UploadStatus == "READY" &&
                        ((x.EntityType == InspectionEntityType && x.EntityID == inspection.InspectionID.ToString()) ||
                         (x.EntityType == InspectionItemEntityType && itemIds.Contains(x.EntityID))))
            .OrderBy(x => x.CapturedDate)
            .ToListAsync(cancellationToken);

        var reviews = await context.InspectionReviews
            .AsNoTracking()
            .Where(x => x.InspectionID == inspection.InspectionID)
            .OrderBy(x => x.ReviewedAt)
            .ToListAsync(cancellationToken);

        var attachmentDtos = new List<LeaseCheckInAttachmentDto>();
        foreach (var attachment in attachments)
        {
            attachmentDtos.Add(await MapAttachmentAsync(attachment, cancellationToken));
        }

        var itemDtos = items.Select(item => new LeaseCheckInItemDto
        {
            InspectionItemID = item.InspectionItemID,
            Area = item.Area ?? string.Empty,
            ItemName = item.ItemName ?? string.Empty,
            Condition = item.Condition,
            Notes = item.Notes,
            RequiresRepair = item.RequiresRepair,
            Attachments = attachmentDtos.Where(x => attachments.Any(a => a.AttachmentID == x.AttachmentID && a.EntityType == InspectionItemEntityType && a.EntityID == item.InspectionItemID.ToString())).ToList()
        }).ToList();

        return new LeaseCheckInInspectionDto
        {
            InspectionID = inspection.InspectionID,
            InspectionType = inspection.InspectionType ?? string.Empty,
            Status = inspection.Status ?? string.Empty,
            OccupancyReadiness = inspection.OccupancyReadiness,
            ArrivalConfirmedAt = inspection.ArrivalConfirmedAt,
            SubmittedAt = inspection.SubmittedAt,
            Notes = inspection.Notes,
            Version = inspection.Version ?? string.Empty,
            RequiresFollowUp = string.Equals(inspection.OccupancyReadiness, "NEEDS_ATTENTION", StringComparison.OrdinalIgnoreCase),
            Attachments = attachmentDtos,
            Items = itemDtos,
            Reviews = reviews.Select(x => new InspectionReviewRecordDto
            {
                InspectionReviewID = x.InspectionReviewID,
                Status = x.Status ?? string.Empty,
                Comments = x.Comments,
                ReviewedByUserID = x.ReviewedByUserID,
                ReviewedAt = x.ReviewedAt
            }).ToList()
        };
    }

    private async Task<LeaseCheckInAttachmentDto> MapAttachmentAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(blobOptions.SasExpiryMinutes <= 0 ? 60 : blobOptions.SasExpiryMinutes);
        string? downloadUrl = null;

        if (!string.IsNullOrWhiteSpace(attachment.StorageReference) && !attachment.StorageReference.StartsWith("pending/", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                downloadUrl = await fileStorageService.GetReadSasUrlAsync(StorageCategory.Image, attachment.StorageReference, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate move-in attachment read URL for attachment {AttachmentID}. Timestamp: {Timestamp}", attachment.AttachmentID, DateTime.UtcNow);
            }
        }

        return new LeaseCheckInAttachmentDto
        {
            AttachmentID = attachment.AttachmentID,
            FileName = attachment.FileName ?? string.Empty,
            MimeType = attachment.MimeType,
            FileSizeBytes = attachment.FileSizeBytes,
            Description = attachment.Description,
            DownloadUrl = downloadUrl,
            ExpiresAt = downloadUrl == null ? null : expiresAt
        };
    }

    private async Task<LeaseCheckInPhotoUploadResponseDto> BuildUploadResponseAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        var dto = await MapAttachmentAsync(attachment, cancellationToken);
        return new LeaseCheckInPhotoUploadResponseDto
        {
            AttachmentID = dto.AttachmentID,
            FileName = dto.FileName,
            MimeType = dto.MimeType,
            FileSizeBytes = dto.FileSizeBytes,
            Description = dto.Description,
            DownloadUrl = dto.DownloadUrl,
            ExpiresAt = dto.ExpiresAt
        };
    }

    private async Task<Inspection?> ResolveInspectionForAttachmentAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        if (string.Equals(attachment.EntityType, InspectionEntityType, StringComparison.OrdinalIgnoreCase) && Guid.TryParse(attachment.EntityID, out var inspectionId))
        {
            return await context.Inspections.AsNoTracking().SingleOrDefaultAsync(x => x.InspectionID == inspectionId, cancellationToken);
        }

        if (string.Equals(attachment.EntityType, InspectionItemEntityType, StringComparison.OrdinalIgnoreCase) && Guid.TryParse(attachment.EntityID, out var itemId))
        {
            var item = await context.InspectionItems.AsNoTracking().SingleOrDefaultAsync(x => x.InspectionItemID == itemId, cancellationToken);
            if (item == null)
            {
                return null;
            }

            return await context.Inspections.AsNoTracking().SingleOrDefaultAsync(x => x.InspectionID == item.InspectionID, cancellationToken);
        }

        return null;
    }

    private async Task MarkAttachmentFailedAsync(Guid attachmentId, string reason, CancellationToken cancellationToken)
    {
        var attachment = await context.Attachments.SingleOrDefaultAsync(x => x.AttachmentID == attachmentId, cancellationToken);
        if (attachment == null)
        {
            return;
        }

        attachment.UploadStatus = "FAILED";
        attachment.UploadFailureReason = Truncate(reason, 256);
        attachment.UploadExpiresAt = DateTime.UtcNow.AddHours(24);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<(byte[] Bytes, string ContentType, string Checksum)> ReadAndValidateImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var input = file.OpenReadStream();
        await using var buffer = new MemoryStream();
        await input.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        try
        {
            await using var imageStream = new MemoryStream(bytes);
            var imageInfo = await Image.IdentifyAsync(imageStream, cancellationToken);
            if (imageInfo == null || imageInfo.Metadata.DecodedImageFormat == null)
            {
                throw new LeaseCheckInException(StatusCodes.Status415UnsupportedMediaType, "Unsupported image format", "Only JPEG, PNG, and WebP images are supported.");
            }

            var formatName = imageInfo.Metadata.DecodedImageFormat.Name.ToUpperInvariant();
            if (!Array.Exists(AllowedImageFormats, x => string.Equals(x, formatName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new LeaseCheckInException(StatusCodes.Status415UnsupportedMediaType, "Unsupported image format", "Only JPEG, PNG, and WebP images are supported.");
            }

            if (imageInfo.Width <= 0 || imageInfo.Height <= 0 || (long)imageInfo.Width * imageInfo.Height > 40_000_000)
            {
                throw new LeaseCheckInException(StatusCodes.Status415UnsupportedMediaType, "Unsupported image dimensions", "The image dimensions are not supported.");
            }

            string? contentType = formatName switch
            {
                "JPEG" => "image/jpeg",
                "PNG" => "image/png",
                "WEBP" => "image/webp",
                _ => file.ContentType
            };

            var checksum = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            return (bytes, contentType ?? "application/octet-stream", checksum);
        }
        catch (LeaseCheckInException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new LeaseCheckInException(StatusCodes.Status415UnsupportedMediaType, "Unsupported image format", "Only JPEG, PNG, and WebP images are supported.");
        }
    }

    private async Task CreateNotificationsAsync(Lease lease, Inspection inspection, DateTime now, CancellationToken cancellationToken)
    {
        var metadata = $"MOVE_IN_CHECKIN_SUBMITTED:{inspection.InspectionID}";
        var alreadyExists = await context.Notifications.AnyAsync(x => x.Metadata == metadata, cancellationToken);
        if (alreadyExists)
        {
            return;
        }

        var recipients = await context.OrganizationMembers
            .Where(x => x.OrganizationID == lease.OrganizationID && x.DeactivatedAt == null && (x.AcceptedAt != null || x.Status == "ACTIVE"))
            .ToListAsync(cancellationToken);

        foreach (var recipient in recipients)
        {
            context.Notifications.Add(new Notification
            {
                NotificationID = Guid.NewGuid(),
                RecipientUserID = recipient.UserID,
                OrganizationID = lease.OrganizationID,
                OrganizationMemberID = recipient.OrganizationMemberID,
                Channel = "IN_APP",
                TemplateCode = "MOVE_IN_CHECKIN_SUBMITTED",
                Subject = "Move-in check-in submitted",
                Body = inspection.OccupancyReadiness == "NEEDS_ATTENTION"
                    ? "A tenant submitted a move-in check-in that requires follow-up."
                    : "A tenant submitted a move-in check-in.",
                Status = "QUEUED",
                ScheduledAt = now,
                Metadata = metadata,
                CapturedDate = now,
                CapturedBy = inspection.SubmittedByUserID?.ToString(CultureInfo.InvariantCulture)
            });
        }
    }

    private async Task SendLandlordChatMessageAsync(Lease lease, Inspection inspection, SubmitLeaseCheckInRequestDto request, DateTime now, long userId, CancellationToken cancellationToken)
    {
        try
        {
            var subject = $"Move-in check-in submitted{(string.IsNullOrWhiteSpace(lease.LeaseNumber) ? string.Empty : $" - {lease.LeaseNumber}")}";
            var thread = await messagingService.GetOrCreateDirectThreadAsync(new StartThreadRequest
            {
                TenantID = lease.TenantID,
                OrganizationID = lease.OrganizationID,
                Subject = subject
            });

            var message = $"Move-in check-in submitted. Readiness: {inspection.OccupancyReadiness}." +
                          (string.IsNullOrWhiteSpace(request.Notes) ? string.Empty : $" Notes: {request.Notes.Trim()}");

            await messagingService.SendMessageAsync(new SendMessageRequest
            {
                ConversationID = thread.ConversationID ?? Guid.Empty,
                Message = message,
                SenderTenantID = lease.TenantID,
                SenderUserID = userId
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send landlord chat message for move-in check-in {InspectionID}. Timestamp: {Timestamp}", inspection.InspectionID, DateTime.UtcNow);
        }
    }

    private static void ValidateSubmitRequest(SubmitLeaseCheckInRequestDto request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.InspectionID == Guid.Empty)
        {
            errors["inspectionID"] = ["An inspectionID is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Version))
        {
            errors["version"] = ["A version is required."];
        }

        if (!request.ArrivalConfirmed)
        {
            errors["arrivalConfirmed"] = ["Arrival must be confirmed before submission."];
        }

        if (string.IsNullOrWhiteSpace(request.OccupancyReadiness) || !AllowedReadinessValues.Contains(request.OccupancyReadiness.Trim().ToUpperInvariant(), StringComparer.Ordinal))
        {
            errors["occupancyReadiness"] = ["Occupancy readiness must be READY, NEEDS_ATTENTION, or NOT_SURE."];
        }

        if (!string.IsNullOrWhiteSpace(request.Notes) && request.Notes.Length > 1000)
        {
            errors["notes"] = ["The notes must be 1000 characters or fewer."];
        }

        if (request.Photos.Count > 10)
        {
            errors["photos"] = ["A move-in check-in can include at most 10 photos."];
        }

        if (request.Photos.GroupBy(x => x.AttachmentID).Any(x => x.Count() > 1))
        {
            errors["photos"] = ["Each attachment can be listed only once in the photos collection."];
        }

        foreach (var photo in request.Photos)
        {
            if (photo.AttachmentID == Guid.Empty)
            {
                errors["photos"] = ["Each photo requires an attachmentID."];
                break;
            }

            if (!string.IsNullOrWhiteSpace(photo.Description) && photo.Description.Length > 1000)
            {
                errors["photos"] = ["Photo descriptions must be 1000 characters or fewer."];
                break;
            }
        }

        if (request.Items.GroupBy(x => x.ClientItemID, StringComparer.Ordinal).Any(x => x.Count() > 1))
        {
            errors["items"] = ["Each item requires a unique clientItemID."];
        }

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ClientItemID) || item.ClientItemID.Length > 100)
            {
                errors["items"] = ["Each item requires a clientItemID of 100 characters or fewer."];
                break;
            }

            if (string.IsNullOrWhiteSpace(item.Area) || item.Area.Length > 100)
            {
                errors["items"] = ["Each item requires an area of 100 characters or fewer."];
                break;
            }

            if (string.IsNullOrWhiteSpace(item.ItemName) || item.ItemName.Length > 100)
            {
                errors["items"] = ["Each item requires a itemName of 100 characters or fewer."];
                break;
            }

            if (!string.IsNullOrWhiteSpace(item.Condition) && item.Condition.Length > 50)
            {
                errors["items"] = ["Item condition must be 50 characters or fewer."];
                break;
            }

            if (!string.IsNullOrWhiteSpace(item.Notes) && item.Notes.Length > 1000)
            {
                errors["items"] = ["Item notes must be 1000 characters or fewer."];
                break;
            }
        }

        if (errors.Count > 0)
        {
            throw new LeaseCheckInException(StatusCodes.Status400BadRequest, "Validation failed", "One or more validation errors occurred.", errors: errors);
        }
    }

    private static string ComputeRequestHash(SubmitLeaseCheckInRequestDto request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    }

    private static bool IsSubmitted(Inspection inspection)
    {
        return string.Equals(inspection.Status, SubmittedStatus, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(inspection.Status, CompletedStatus, StringComparison.OrdinalIgnoreCase) ||
               inspection.SubmittedAt.HasValue;
    }

    private static long GetRequiredUserId(ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue("UserId") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(raw, out var userId))
        {
            throw new LeaseCheckInException(StatusCodes.Status401Unauthorized, "Unauthorized", "A valid authenticated user is required.");
        }

        return userId;
    }

    private static LeaseCheckInException ValidationError(string field, string message)
    {
        return new LeaseCheckInException(StatusCodes.Status400BadRequest, "Validation failed", "One or more validation errors occurred.", errors: new Dictionary<string, string[]> { [field] = [message] });
    }

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value ?? string.Empty;
        }

        return value[..maxLength];
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch
        {
            return TimeZoneInfo.Utc;
        }
    }
}
