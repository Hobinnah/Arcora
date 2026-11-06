using Arcora.Api.DTOs;

namespace Arcora.Api.Services.Interfaces;

public interface IListingReviewService
{
    Task<IReadOnlyList<ListingDto>> GetQueueAsync(CancellationToken cancellationToken = default);
    Task<ListingDto?> GetPendingListingAsync(Guid listingID, CancellationToken cancellationToken = default);
    Task<ListingDto?> SubmitForReviewAsync(Guid listingID, long actorUserID, bool isAdmin, CancellationToken cancellationToken = default);
    Task<ListingReviewDecisionResultDto?> DecideAsync(Guid listingID, ListingReviewDecisionRequest request, long reviewerUserID, CancellationToken cancellationToken = default);
}