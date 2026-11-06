using System.Globalization;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Email;
using Arcora.Api.Entities;
using Arcora.Api.Enums;
using Arcora.Api.Realtime;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class ListingReviewService : IListingReviewService
{
    public const string PendingReviewStatus = "PENDING_REVIEW";
    private const string ReviewConversationType = "LISTING_REVIEW";

    private readonly ArcoraDbContext dbContext;
    private readonly IListingService listingService;
    private readonly HybridCache cache;
    private readonly IEmailSender emailSender;
    private readonly IHubContext<MessagingHub> hubContext;
    private readonly EmailSettings emailSettings;
    private readonly ILogger<ListingReviewService> logger;

    public ListingReviewService(
        ArcoraDbContext dbContext,
        IListingService listingService,
        HybridCache cache,
        IEmailSender emailSender,
        IHubContext<MessagingHub> hubContext,
        IOptions<EmailSettings> emailSettings,
        ILogger<ListingReviewService> logger)
    {
        this.dbContext = dbContext;
        this.listingService = listingService;
        this.cache = cache;
        this.emailSender = emailSender;
        this.hubContext = hubContext;
        this.emailSettings = emailSettings.Value;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<ListingDto>> GetQueueAsync(CancellationToken cancellationToken = default)
    {
        var listingIDs = await dbContext.Listings
            .AsNoTracking()
            .Where(listing => listing.Status == PendingReviewStatus)
            .OrderBy(listing => listing.ReviewSubmittedAt ?? listing.CapturedDate)
            .Select(listing => listing.ListingID)
            .Take(200)
            .ToListAsync(cancellationToken);

        var results = new List<ListingDto>(listingIDs.Count);
        foreach (var listingID in listingIDs)
        {
            var listing = await listingService.GetID(listingID);
            if (listing != null && string.Equals(listing.Status, PendingReviewStatus, StringComparison.OrdinalIgnoreCase))
                results.Add(SanitizeReviewListing(listing));
        }

        return results;
    }

    public async Task<ListingDto?> GetPendingListingAsync(Guid listingID, CancellationToken cancellationToken = default)
    {
        var isPending = await dbContext.Listings.AsNoTracking()
            .AnyAsync(listing => listing.ListingID == listingID && listing.Status == PendingReviewStatus, cancellationToken);
        if (!isPending)
            return null;

        var listing = await listingService.GetID(listingID);
        return listing == null ? null : SanitizeReviewListing(listing);
    }

    public async Task<ListingDto?> SubmitForReviewAsync(Guid listingID, long actorUserID, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var listing = await dbContext.Listings.FirstOrDefaultAsync(item => item.ListingID == listingID, cancellationToken);
        if (listing == null)
            return null;

        if (!isAdmin)
        {
            var isActiveMember = await dbContext.OrganizationMembers.AnyAsync(member =>
                member.OrganizationID == listing.OrganizationID
                && member.UserID == actorUserID
                && member.Status == "ACTIVE", cancellationToken);
            if (!isActiveMember)
                throw new UnauthorizedAccessException("Only an active member of the listing organization can submit it for review.");
        }

        if (!string.Equals(listing.Status, "DRAFT", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(listing.Status, "REJECTED", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only draft or rejected listings can be submitted for review.");

        var now = DateTime.UtcNow;
        listing.Status = PendingReviewStatus;
        listing.ReviewSubmittedAt = now;
        listing.ReviewedAt = null;
        listing.ReviewedByUserID = null;
        listing.ReviewMessage = null;
        listing.UpdatedDate = now;
        listing.UpdatedBy = actorUserID.ToString(CultureInfo.InvariantCulture);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(Cache.LISTINGS.ToString(), cancellationToken);

        var updated = await listingService.GetID(listingID);
        return updated == null ? null : SanitizeReviewListing(updated);
    }

    public async Task<ListingReviewDecisionResultDto?> DecideAsync(
        Guid listingID,
        ListingReviewDecisionRequest request,
        long reviewerUserID,
        CancellationToken cancellationToken = default)
    {
        var decision = request.Decision.Trim().ToUpperInvariant();
        if (decision is not ("APPROVED" or "REJECTED"))
            throw new ArgumentException("Decision must be APPROVED or REJECTED.");

        var reviewMessage = request.Message?.Trim();
        if (decision == "REJECTED" && string.IsNullOrWhiteSpace(reviewMessage))
            throw new ArgumentException("A rejection message is required.");
        if ((reviewMessage?.Length ?? 0) > 2000)
            throw new ArgumentException("Review message must be 2000 characters or fewer.");

        var now = DateTime.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var updatedCount = await dbContext.Listings
            .Where(item => item.ListingID == listingID && item.Status == PendingReviewStatus)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.Status, decision)
                .SetProperty(item => item.ReviewedAt, now)
                .SetProperty(item => item.ReviewedByUserID, reviewerUserID)
                .SetProperty(item => item.ReviewMessage, decision == "REJECTED" ? reviewMessage : null)
                .SetProperty(item => item.UpdatedDate, now)
                .SetProperty(item => item.UpdatedBy, reviewerUserID.ToString(CultureInfo.InvariantCulture))
                .SetProperty(item => item.PublishedAt, item => decision == "APPROVED" ? now : item.PublishedAt)
                .SetProperty(item => item.UnpublishedAt, item => decision == "APPROVED" ? null : item.UnpublishedAt)
                .SetProperty(item => item.AcceptingApplications, item => decision == "REJECTED" ? false : item.AcceptingApplications),
                cancellationToken);
        if (updatedCount == 0)
        {
            var listingExists = await dbContext.Listings.AsNoTracking()
                .AnyAsync(item => item.ListingID == listingID, cancellationToken);
            await transaction.RollbackAsync(cancellationToken);
            if (!listingExists)
                return null;
            throw new InvalidOperationException("This listing is no longer awaiting review.");
        }

        var listing = await dbContext.Listings.AsNoTracking()
            .FirstAsync(item => item.ListingID == listingID, cancellationToken);
        var hostMember = await dbContext.OrganizationMembers
            .Include(member => member.User)
            .Where(member => member.OrganizationID == listing.OrganizationID && member.Status == "ACTIVE")
            .OrderByDescending(member => member.IsPrimaryOwner)
            .ThenBy(member => member.CapturedDate)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("The listing organization has no active host member to notify.");
        var hostEmail = hostMember.User?.Email;
        if (string.IsNullOrWhiteSpace(hostEmail))
            throw new InvalidOperationException("The host account has no email address to notify.");

        var reviewer = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == reviewerUserID, cancellationToken)
            ?? throw new UnauthorizedAccessException("The reviewer account could not be resolved.");

        var subject = $"Listing review:{listing.ListingID:N}";
        var conversation = await dbContext.Conversations.FirstOrDefaultAsync(item =>
            item.OrganizationID == listing.OrganizationID
            && item.ConversationType == ReviewConversationType
            && item.Subject == subject, cancellationToken);
        if (conversation == null)
        {
            conversation = new Conversation
            {
                ConversationID = Guid.NewGuid(),
                ConversationType = ReviewConversationType,
                OrganizationID = listing.OrganizationID,
                Subject = subject,
                Status = "OPEN",
                CapturedDate = now,
                CapturedBy = reviewer.DisplayName ?? reviewer.Email
            };
            dbContext.Conversations.Add(conversation);
        }

        if (!await dbContext.ConversationParticipants.AnyAsync(participant =>
            participant.ConversationID == conversation.ConversationID
            && participant.OrganizationMemberID == hostMember.OrganizationMemberID, cancellationToken))
        {
            dbContext.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationParticipantID = Guid.NewGuid(),
                ConversationID = conversation.ConversationID,
                OrganizationMemberID = hostMember.OrganizationMemberID,
                ParticipantRole = "HOST",
                JoinedAt = now,
                IsMuted = false,
                CapturedDate = now
            });
        }

        var reviewerName = string.Join(" ", new[] { reviewer.FirstName, reviewer.LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
        if (string.IsNullOrWhiteSpace(reviewerName))
            reviewerName = "Arcora Customer Rep";
        var decisionText = decision == "APPROVED"
            ? $"Your listing \"{listing.Title}\" has been approved and is now visible to renters."
            : $"Your listing \"{listing.Title}\" was not approved.\n\nReview message: {reviewMessage}";
        var chatMessage = new ConversationMessage
        {
            ConversationMessageID = Guid.NewGuid(),
            ConversationID = conversation.ConversationID,
            SenderUserID = reviewerUserID,
            Message = decisionText,
            MessageType = "TEXT",
            SentAt = now,
            CapturedDate = now,
            CapturedBy = reviewerName
        };
        dbContext.ConversationMessages.Add(chatMessage);
        conversation.LastMessageAt = now;
        conversation.UpdatedDate = now;
        conversation.UpdatedBy = reviewerName;

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cache.RemoveAsync(Cache.LISTINGS.ToString(), cancellationToken);

        try
        {
            await hubContext.Clients.Group(MessagingHub.GroupName(conversation.ConversationID)).SendAsync("MessageReceived", new ConversationMessageDto
            {
                ConversationMessageID = chatMessage.ConversationMessageID,
                ConversationID = conversation.ConversationID,
                SenderUserID = reviewerUserID,
                SenderName = reviewerName,
                Message = decisionText,
                MessageType = "TEXT",
                SentAt = now
            }, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Could not push listing review chat message for listing {ListingID}.", listingID);
        }

        var emailSent = false;
        string? warning = null;
        try
        {
            var emailBody = EmailTemplates.BuildListingReviewDecisionEmail(
                hostMember.User?.DisplayName ?? hostMember.User?.FirstName ?? "Host",
                listing.Title ?? "Your listing",
                decision,
                decision == "REJECTED" ? reviewMessage : null,
                string.IsNullOrWhiteSpace(emailSettings.SenderName) ? "Arcora" : emailSettings.SenderName,
                emailSettings.SenderEmail);
            var emailSubject = decision == "APPROVED"
                ? $"Your listing has been approved - {listing.Title}"
                : $"Listing changes requested - {listing.Title}";
            await emailSender.SendEmailAsync(hostEmail, emailSubject, emailBody);
            emailSent = true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Listing review decision saved, but host email failed for listing {ListingID}.", listingID);
            warning = "The decision and host chat were saved, but the email could not be sent.";
        }

        var updatedListing = await listingService.GetID(listingID);
        return new ListingReviewDecisionResultDto
        {
            Listing = updatedListing == null ? new ListingDto() : SanitizeReviewListing(updatedListing),
            ConversationID = conversation.ConversationID,
            EmailSent = emailSent,
            Warning = warning
        };
    }

    private static ListingDto SanitizeReviewListing(ListingDto listing)
    {
        listing.CheckInDoorCode = string.Empty;
        listing.WIFINetwork = string.Empty;
        listing.WIFIPassword = string.Empty;
        listing.Notes = string.Empty;
        listing.ListingAccessInstructions = new List<ListingAccessInstructionDto>();
        return listing;
    }
}