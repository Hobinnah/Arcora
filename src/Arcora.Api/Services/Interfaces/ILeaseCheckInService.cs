using System.Security.Claims;
using Arcora.Api.DTOs;

namespace Arcora.Api.Services.Interfaces;

public interface ILeaseCheckInService
{
    Task<LeaseCheckInResponseDto> GetAsync(Guid leaseId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<(LeaseCheckInDraftResponseDto Draft, bool Created)> CreateOrGetDraftAsync(Guid leaseId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<LeaseCheckInPhotoUploadResponseDto> UploadPhotoAsync(Guid leaseId, LeaseCheckInPhotoUploadRequestDto request, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task DeletePhotoAsync(Guid leaseId, Guid attachmentId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<LeaseCheckInInspectionDto> SubmitAsync(Guid leaseId, SubmitLeaseCheckInRequestDto request, string idempotencyKey, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<InspectionReviewRecordDto> CreateReviewAsync(Guid inspectionId, InspectionReviewRequestDto request, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<bool> CanAccessMoveInInspectionAsync(Guid inspectionId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<bool> CanAccessMoveInAttachmentAsync(Guid attachmentId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<int> CleanupExpiredDraftEvidenceAsync(CancellationToken cancellationToken = default);
}
