using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reviews")]
public sealed class ReviewWorkflowController : ControllerBase
{
    private readonly ArcoraDbContext _db;
    private readonly UserManager<User> _userManager;

    public ReviewWorkflowController(ArcoraDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("eligible")]
    public async Task<ActionResult<IReadOnlyList<EligibleLeaseReviewDto>>> GetEligibleReviews(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var tenantIDs = await _db.Tenants
            .Where(tenant => tenant.UserID == user.Id)
            .Select(tenant => tenant.TenantID)
            .ToListAsync(cancellationToken);

        var organizationIDs = await _db.OrganizationMembers
            .Where(member => member.UserID == user.Id &&
                member.Status != null &&
                member.Status.ToUpper() != "INVITED" &&
                member.Status.ToUpper() != "REJECTED" &&
                member.Status.ToUpper() != "DEACTIVATED")
            .Select(member => member.OrganizationID)
            .ToListAsync(cancellationToken);

        var leases = await _db.Leases
            .AsNoTracking()
            .Include(lease => lease.Tenant)
                .ThenInclude(tenant => tenant!.User)
            .Include(lease => lease.Organization)
            .Include(lease => lease.Listing)
            .Where(lease => tenantIDs.Contains(lease.TenantID) || organizationIDs.Contains(lease.OrganizationID))
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var result = new List<EligibleLeaseReviewDto>();
        foreach (var lease in leases)
        {
            var endedAt = lease.ActualMoveOutAt ?? lease.TerminatedAt ?? lease.EndDate;
            if (endedAt is null || endedAt.Value > now || endedAt.Value < now.AddDays(-14))
                continue;

            var isTenant = tenantIDs.Contains(lease.TenantID);
            var subjectType = isTenant ? "HOST" : "TENANT";
            var hasSubmitted = await _db.Ratings.AnyAsync(
                rating => rating.LeaseID == lease.LeaseID && rating.SubjectType == subjectType,
                cancellationToken);

            result.Add(new EligibleLeaseReviewDto
            {
                LeaseID = lease.LeaseID,
                ListingTitle = lease.Listing?.Title ?? "Rental home",
                LeaseEndedAt = DateTime.SpecifyKind(endedAt.Value, DateTimeKind.Utc),
                ReviewMode = isTenant ? "stay" : "tenant",
                SubjectName = isTenant
                    ? lease.Organization?.DisplayName ?? "Your host"
                    : lease.Tenant?.User is { } tenantUser
                        ? $"{tenantUser.FirstName} {tenantUser.LastName}".Trim()
                        : "Your tenant",
                HasSubmitted = hasSubmitted
            });
        }

        return Ok(result.OrderByDescending(item => item.LeaseEndedAt));
    }

    [HttpPost]
    public async Task<ActionResult<ReviewWorkflowResultDto>> SubmitReview(
        [FromBody] SubmitLeaseReviewDto request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var lease = await _db.Leases
            .Include(item => item.Tenant)
            .FirstOrDefaultAsync(item => item.LeaseID == request.LeaseID, cancellationToken);
        if (lease is null)
            return NotFound(new { message = "Lease not found." });

        var tenant = await _db.Tenants
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.TenantID == lease.TenantID, cancellationToken);
        var isTenant = tenant?.UserID == user.Id;
        var isOrganizationMember = await _db.OrganizationMembers.AnyAsync(
            member => member.OrganizationID == lease.OrganizationID &&
                member.UserID == user.Id &&
                member.Status != null &&
                member.Status.ToUpper() != "INVITED" &&
                member.Status.ToUpper() != "REJECTED" &&
                member.Status.ToUpper() != "DEACTIVATED",
            cancellationToken);

        if (!isTenant && !isOrganizationMember)
            return Forbid();

        var endedAt = lease.ActualMoveOutAt ?? lease.TerminatedAt ?? lease.EndDate;
        var now = DateTime.UtcNow;
        if (endedAt is null || endedAt.Value > now || endedAt.Value < now.AddDays(-14))
            return Conflict(new { message = "This lease is outside its 14-day review window." });

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var reviewSubjectType = isTenant ? "HOST" : "TENANT";
        if (await _db.Ratings.AnyAsync(
                rating => rating.LeaseID == lease.LeaseID && rating.SubjectType == reviewSubjectType,
                cancellationToken))
            return Conflict(new { message = "You have already submitted a review for this lease." });

        var rating = new Rating
        {
            RatingID = Guid.NewGuid(),
            LeaseID = lease.LeaseID,
            ReviewerUserID = user.Id,
            SubjectType = reviewSubjectType,
            SubjectReferenceID = isTenant ? lease.OrganizationID.ToString() : tenant!.UserID.ToString(),
            OverallRating = request.OverallRating,
            PaymentRating = request.PaymentRating,
            CommunicationRating = request.CommunicationRating,
            PropertyCareRating = request.PropertyCareRating,
            ResponsivenessRating = request.ResponsivenessRating,
            AccuracyRating = request.AccuracyRating,
            CleanlinessRating = request.CleanlinessRating,
            ReviewBody = request.ReviewBody.Trim(),
            IsPublic = false,
            CapturedDate = now,
            CapturedBy = user.UserName,
            UpdatedDate = now
        };

        var counterpartReviewExists = await _db.Ratings.AnyAsync(
            item => item.LeaseID == lease.LeaseID &&
                item.ReviewerUserID != user.Id &&
                item.SubjectType == (isTenant ? "TENANT" : "HOST") &&
                item.SubjectReferenceID == (isTenant ? tenant!.UserID.ToString() : lease.OrganizationID.ToString()) &&
                (isTenant
                    ? _db.OrganizationMembers.Any(member => member.OrganizationID == lease.OrganizationID && member.UserID == item.ReviewerUserID)
                    : _db.Tenants.Any(otherTenant => otherTenant.TenantID == lease.TenantID && otherTenant.UserID == item.ReviewerUserID)),
            cancellationToken);
        if (counterpartReviewExists)
        {
            var unreleasedReviews = await _db.Ratings
                .Where(item => item.LeaseID == lease.LeaseID && !item.IsPublic)
                .ToListAsync(cancellationToken);
            foreach (var item in unreleasedReviews)
            {
                item.IsPublic = true;
                item.PublishedAt = now;
                item.UpdatedDate = now;
            }
            rating.IsPublic = true;
            rating.PublishedAt = now;
        }

        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ReviewWorkflowResultDto
        {
            RatingID = rating.RatingID,
            LeaseID = rating.LeaseID,
            IsPublic = rating.IsPublic,
            PublishedAt = rating.PublishedAt,
            ReleaseAt = rating.IsPublic ? rating.PublishedAt : now.AddDays(14)
        });
    }

    [HttpGet("received/{id:guid}")]
    public async Task<ActionResult<object>> GetReceivedReview(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        await PublishExpiredReviews(cancellationToken);
        var rating = await _db.Ratings
            .AsNoTracking()
            .Include(item => item.ReviewerUser)
            .Include(item => item.Lease)
                .ThenInclude(lease => lease!.Listing)
            .SingleOrDefaultAsync(item => item.RatingID == id, cancellationToken);
        if (rating is null || !rating.IsPublic)
            return NotFound();

        var isTenantSubject = string.Equals(rating.SubjectType, "TENANT", StringComparison.OrdinalIgnoreCase);
        var ownsSubject = isTenantSubject
            ? long.TryParse(rating.SubjectReferenceID, out var subjectUserID) && subjectUserID == user.Id
            : Guid.TryParse(rating.SubjectReferenceID, out var subjectOrganizationID) &&
                await _db.OrganizationMembers.AnyAsync(
                    member => member.OrganizationID == subjectOrganizationID &&
                        member.UserID == user.Id &&
                        member.Status != null &&
                        member.Status.ToUpper() != "INVITED" &&
                        member.Status.ToUpper() != "REJECTED" &&
                        member.Status.ToUpper() != "DEACTIVATED",
                    cancellationToken);

        if (!ownsSubject)
            return Forbid();

        return Ok(ToReviewDetails(rating));
    }

    [HttpGet("received")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetReceivedReviews(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        await PublishExpiredReviews(cancellationToken);
        var organizationIDs = (await _db.OrganizationMembers
            .Where(member => member.UserID == user.Id &&
                member.Status != null &&
                member.Status.ToUpper() != "INVITED" &&
                member.Status.ToUpper() != "REJECTED" &&
                member.Status.ToUpper() != "DEACTIVATED")
            .Select(member => member.OrganizationID)
            .ToListAsync(cancellationToken))
            .Select(id => id.ToString())
            .ToList();
        var tenantUserID = user.Id.ToString();

        var reviews = await _db.Ratings
            .AsNoTracking()
            .Where(rating => rating.IsPublic &&
                ((rating.SubjectType == "TENANT" && rating.SubjectReferenceID == tenantUserID) ||
                 (rating.SubjectType == "HOST" && organizationIDs.Contains(rating.SubjectReferenceID!))))
            .Include(rating => rating.ReviewerUser)
            .Include(rating => rating.Lease)
                .ThenInclude(lease => lease!.Listing)
            .OrderByDescending(rating => rating.PublishedAt)
            .ToListAsync(cancellationToken);

        return Ok(reviews.Select(ToReviewDetails).ToList());
    }

    private async Task PublishExpiredReviews(CancellationToken cancellationToken)
    {
        var releaseBefore = DateTime.UtcNow.AddDays(-14);
        var expiredReviews = await _db.Ratings
            .Where(rating => !rating.IsPublic && rating.CapturedDate <= releaseBefore)
            .ToListAsync(cancellationToken);
        if (expiredReviews.Count == 0)
            return;

        var publishedAt = DateTime.UtcNow;
        foreach (var rating in expiredReviews)
        {
            rating.IsPublic = true;
            rating.PublishedAt = publishedAt;
            rating.UpdatedDate = publishedAt;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static object ToReviewDetails(Rating rating) => new
    {
        rating.RatingID,
        rating.LeaseID,
        rating.SubjectType,
        rating.SubjectReferenceID,
        rating.OverallRating,
        rating.PaymentRating,
        rating.CommunicationRating,
        rating.PropertyCareRating,
        rating.ResponsivenessRating,
        rating.AccuracyRating,
        rating.CleanlinessRating,
        rating.ReviewBody,
        rating.PublishedAt,
        ListingTitle = rating.Lease?.Listing?.Title ?? "Rental home",
        ReviewerName = $"{rating.ReviewerUser?.FirstName} {rating.ReviewerUser?.LastName}".Trim()
    };
}
