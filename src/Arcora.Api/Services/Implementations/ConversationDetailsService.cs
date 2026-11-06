using System.Globalization;
using System.Text.Json;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Enums;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Builds the messaging side pane from the conversation's lease (reservation) or its latest listing enquiry card.
    /// Host-only data (phone number, payout, notes) and tenant access details are filtered per viewer.
    /// </summary>
    public class ConversationDetailsService : IConversationDetailsService
    {
        private const string HostRole = "HOST";
        private const string TenantRole = "TENANT";
        private const string EnquiryCardType = "ENQUIRY_CARD";
        private const int ShortTermStayNights = 35;

        private static readonly HashSet<string> InactiveLeaseStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "DRAFT", "CANCELLED", "CANCELED", "DECLINED", "VOID", "VOIDED", "REJECTED", "WITHDRAWN"
        };

        // A lease only becomes ACTIVE once it is fully signed and the first payment has cleared; later states keep that history.
        private static readonly HashSet<string> ConfirmedLeaseStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "ACTIVE", "EXPIRED", "ENDED", "COMPLETED", "TERMINATED", "RENEWED"
        };

        private static readonly string[] PaidStatuses = { "PAID", "SUCCEEDED", "SETTLED" };

        // Mirrors the listing editor, which treats an unset policy as these defaults.
        private const string DefaultShortTermCancellationPolicy = "Flexible";
        private const string DefaultLongTermCancellationPolicy = "Firm Long Term";

        // Rule types rendered elsewhere in the pane (times) or not house rules at all (safety items).
        private static readonly HashSet<string> NonHouseRuleTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "CHECK_IN", "CHECK_IN_END", "CHECK_OUT", "QUIET_HOURS_START", "QUIET_HOURS_END",
            "NUMBER_OF_GUESTS", "COMMERCIAL_PHOTOGRAPHY", "ADDITIONAL"
        };

        private static readonly JsonSerializerOptions webJson = new(JsonSerializerDefaults.Web);

        private readonly ArcoraDbContext context;
        private readonly IConfiguration configuration;
        private readonly IPreferenceService preferenceService;
        private readonly IFileStorageService fileStorageService;
        private readonly ILogger<ConversationDetailsService> logger;

        public ConversationDetailsService(
            ArcoraDbContext context,
            IConfiguration configuration,
            IPreferenceService preferenceService,
            IFileStorageService fileStorageService,
            ILogger<ConversationDetailsService> logger)
        {
            this.context = context;
            this.configuration = configuration;
            this.preferenceService = preferenceService;
            this.fileStorageService = fileStorageService;
            this.logger = logger;
        }

        public async Task<ConversationDetailsDto> GetDetailsAsync(Guid conversationId, ConversationActor actor, CancellationToken cancellationToken = default)
        {
            var conversation = await LoadConversationAsync(conversationId, cancellationToken);
            var viewer = await ResolveViewerAsync(conversation, actor, cancellationToken);
            var isHost = viewer.Role == HostRole;
            var now = DateTime.UtcNow;

            var card = await LoadLatestEnquiryCardAsync(conversation.ConversationID, cancellationToken);
            var lease = await FindReservationAsync(conversation, card?.Card.ListingID, cancellationToken);
            var listingId = lease?.ListingID ?? card?.Card.ListingID;
            var listing = listingId.HasValue ? await LoadListingAsync(listingId.Value, cancellationToken) : null;
            var confirmed = lease != null && await IsConfirmedAsync(lease, cancellationToken);

            var details = new ConversationDetailsDto
            {
                ConversationID = conversation.ConversationID,
                Kind = lease != null ? ConversationDetailsDto.KindReservation : card != null ? ConversationDetailsDto.KindEnquiry : ConversationDetailsDto.KindDirect,
                ViewerRole = viewer.Role,
                Status = conversation.Status,
                StartedAt = conversation.CapturedDate,
                LastMessageAt = conversation.LastMessageAt,
            };

            if (card != null)
            {
                details.Enquiry = new ConversationDetailsEnquiryDto
                {
                    ProposedStartDate = card.Card.ProposedStartDate,
                    ProposedEndDate = card.Card.ProposedEndDate,
                    EnquiredAt = card.SentAt,
                };
            }

            if (lease != null)
                details.Reservation = MapReservation(lease, confirmed);

            if (listing != null)
            {
                var rules = ActiveRules(listing, now);
                var revealLocation = isHost || (lease != null &&
                    await new PropertyLocationAccess(context).IsActiveAndPaidAsync(lease, cancellationToken));
                details.Listing = MapListing(listing, card?.Card, lease, revealLocation, rules);
                details.HouseRules = BuildHouseRules(listing, rules);
            }
            else if (card != null)
            {
                // The listing may have been removed; the card snapshot is still what the renter enquired about.
                details.Listing = new ConversationDetailsListingDto
                {
                    ListingID = card.Card.ListingID,
                    Title = card.Card.Title,
                    MonthlyRent = card.Card.MonthlyRent,
                    SecurityDeposit = card.Card.SecurityDeposit,
                    Currency = card.Card.Currency,
                };
            }

            details.Renter = await MapRenterAsync(conversation.Tenant, includePhone: isHost && lease != null, cancellationToken);
            details.Host = await MapHostAsync(conversation.Organization, cancellationToken);

            // Hosts always know their own access details; renters only get them once the reservation is confirmed.
            var accessConfirmed = lease != null && await new PropertyLocationAccess(context).IsActiveAndPaidAsync(lease, cancellationToken);
            if (listing != null && (isHost || accessConfirmed))
                details.Access = await BuildAccessAsync(listing, lease, accessConfirmed, isHost, now, cancellationToken);

            if (lease != null)
            {
                details.Review = await FindReviewAsync(lease, conversation, isHost, now, cancellationToken);
                if (isHost)
                    details.Payout = await FindPayoutAsync(lease, cancellationToken);
            }

            if (isHost)
                details.Note = await LoadNoteAsync(conversation.ConversationID, cancellationToken);

            details.Support = await LoadSupportAsync();
            return details;
        }

        public async Task<ConversationNoteDto?> SaveNoteAsync(Guid conversationId, ConversationActor actor, string? note, CancellationToken cancellationToken = default)
        {
            var conversation = await LoadConversationAsync(conversationId, cancellationToken);
            var viewer = await ResolveViewerAsync(conversation, actor, cancellationToken);
            if (viewer.Role != HostRole || viewer.Member == null)
                throw new UnauthorizedAccessException("Only the host team can keep notes on a conversation.");

            var text = note?.Trim() ?? string.Empty;
            if (text.Length > ConversationNote.MaxLength)
                throw new ArgumentException($"Notes can be at most {ConversationNote.MaxLength} characters.");

            var existing = await context.ConversationNotes.FirstOrDefaultAsync(x => x.ConversationID == conversation.ConversationID, cancellationToken);
            if (text.Length == 0)
            {
                if (existing != null)
                {
                    context.ConversationNotes.Remove(existing);
                    await context.SaveChangesAsync(cancellationToken);
                }
                return null;
            }

            var now = DateTime.UtcNow;
            if (existing == null)
            {
                existing = new ConversationNote { ConversationNoteID = Guid.NewGuid(), ConversationID = conversation.ConversationID, CreatedAt = now };
                context.ConversationNotes.Add(existing);
            }

            existing.Note = text;
            existing.UpdatedAt = now;
            existing.UpdatedByOrganizationMemberID = viewer.Member.OrganizationMemberID;
            await context.SaveChangesAsync(cancellationToken);

            return new ConversationNoteDto { Note = existing.Note, UpdatedAt = existing.UpdatedAt, UpdatedByName = FirstName(viewer.Member.User) };
        }

        private async Task<Conversation> LoadConversationAsync(Guid conversationId, CancellationToken cancellationToken)
        {
            return await context.Conversations
                .Include(x => x.Tenant).ThenInclude(x => x!.User)
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.ConversationID == conversationId, cancellationToken)
                ?? throw new KeyNotFoundException("Conversation not found.");
        }

        private async Task<(string Role, OrganizationMember? Member)> ResolveViewerAsync(Conversation conversation, ConversationActor actor, CancellationToken cancellationToken)
        {
            if (actor.UserID is not long userId)
                throw new UnauthorizedAccessException("You are not a participant in this conversation.");

            if (actor.IsHost)
            {
                var member = await context.OrganizationMembers
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.OrganizationMemberID == actor.OrganizationMemberID, cancellationToken);
                if (member != null
                    && member.UserID == userId
                    && conversation.OrganizationID.HasValue
                    && member.OrganizationID == conversation.OrganizationID
                    && string.Equals(member.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                    return (HostRole, member);
            }
            else if (actor.IsTenant
                && conversation.TenantID.HasValue
                && actor.TenantID == conversation.TenantID
                && conversation.Tenant?.UserID == userId)
            {
                return (TenantRole, null);
            }

            throw new UnauthorizedAccessException("You are not a participant in this conversation.");
        }

        private sealed record EnquiryCardSnapshot(ListingEnquiryCard Card, DateTime SentAt);

        private async Task<EnquiryCardSnapshot?> LoadLatestEnquiryCardAsync(Guid conversationId, CancellationToken cancellationToken)
        {
            var messages = await context.ConversationMessages
                .Where(x => x.ConversationID == conversationId && x.MessageType == EnquiryCardType && x.DeletedAt == null)
                .OrderByDescending(x => x.SentAt)
                .Select(x => new { x.Message, x.SentAt })
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                if (string.IsNullOrWhiteSpace(message.Message))
                    continue;
                try
                {
                    var card = JsonSerializer.Deserialize<ListingEnquiryCard>(message.Message, webJson);
                    if (card != null && card.ListingID != Guid.Empty)
                        return new EnquiryCardSnapshot(card, message.SentAt);
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "Skipping unreadable enquiry card in conversation {ConversationID}.", conversationId);
                }
            }

            return null;
        }

        private async Task<Lease?> FindReservationAsync(Conversation conversation, Guid? enquiryListingId, CancellationToken cancellationToken)
        {
            if (conversation.LeaseID.HasValue)
            {
                var linked = await context.Leases.FirstOrDefaultAsync(x => x.LeaseID == conversation.LeaseID.Value, cancellationToken);
                if (linked != null)
                    return linked;
            }

            if (!conversation.TenantID.HasValue || !conversation.OrganizationID.HasValue)
                return null;

            var query = context.Leases.Where(x => x.TenantID == conversation.TenantID.Value && x.OrganizationID == conversation.OrganizationID.Value);
            if (enquiryListingId.HasValue)
                query = query.Where(x => x.ListingID == enquiryListingId.Value);

            var leases = await query.OrderByDescending(x => x.StartDate).ToListAsync(cancellationToken);
            return leases.FirstOrDefault(x => !InactiveLeaseStatuses.Contains(x.Status ?? string.Empty));
        }

        private async Task<bool> IsConfirmedAsync(Lease lease, CancellationToken cancellationToken)
        {
            if (!ConfirmedLeaseStatuses.Contains(lease.Status ?? string.Empty))
                return false;

            return await context.PaymentIntents
                .Where(x => x.LeaseID == lease.LeaseID)
                .AnyAsync(intent => PaidStatuses.Contains(intent.Status!)
                    || context.Payments.Any(payment => payment.PaymentIntentID == intent.PaymentIntentID && PaidStatuses.Contains(payment.Status!)),
                    cancellationToken);
        }

        private async Task<Listing?> LoadListingAsync(Guid listingId, CancellationToken cancellationToken)
        {
            return await context.Listings
                .Include(x => x.RentalUnit).ThenInclude(x => x!.Property).ThenInclude(x => x!.Address)
                .Include(x => x.ListingRules)
                .Include(x => x.ListingPolicies)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.ListingID == listingId, cancellationToken);
        }

        private static List<ListingRule> ActiveRules(Listing listing, DateTime now) =>
            (listing.ListingRules ?? new List<ListingRule>())
                .Where(x => (x.EffectiveFrom == null || x.EffectiveFrom <= now) && (x.EffectiveTo == null || x.EffectiveTo > now))
                .ToList();

        private static ConversationDetailsListingDto MapListing(Listing listing, ListingEnquiryCard? card, Lease? lease, bool confirmed, List<ListingRule> rules)
        {
            var address = listing.RentalUnit?.Property?.Address;
            var checkIn = RuleTime(rules, "CHECK_IN");
            var checkInEnd = RuleTime(rules, "CHECK_IN_END");
            var shortStay = IsShortStay(card, lease);

            return new ConversationDetailsListingDto
            {
                ListingID = listing.ListingID,
                Title = listing.Title ?? card?.Title,
                MonthlyRent = listing.BaseMonthlyRentAmount,
                SecurityDeposit = listing.SecurityDepositAmount,
                Currency = listing.Currency ?? card?.Currency,
                MinimumLeaseMonths = listing.MinimumLeaseMonths > 0 ? listing.MinimumLeaseMonths : null,
                CancellationPolicy = shortStay
                    ? Clean(listing.ShortTermCancellationPolicy) ?? DefaultShortTermCancellationPolicy
                    : Clean(listing.LongTermCancellationPolicy) ?? DefaultLongTermCancellationPolicy,
                CheckInTime = checkIn != null && checkInEnd != null ? $"{checkIn} – {checkInEnd}" : checkIn,
                CheckOutTime = RuleTime(rules, "CHECK_OUT"),
                Address = address == null ? null : confirmed
                    ? new ConversationDetailsAddressDto
                    {
                        Line1 = address.Line1,
                        Line2 = address.Line2,
                        City = address.City,
                        ProvinceCode = address.ProvinceCode,
                        PostalCode = address.PostalCode,
                        CountryCode = address.CountryCode,
                        Latitude = address.Latitude,
                        Longitude = address.Longitude,
                    }
                    // Exact location is only shared once the lease is approved and paid.
                    : new ConversationDetailsAddressDto { City = address.City, ProvinceCode = address.ProvinceCode, CountryCode = address.CountryCode },
            };
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static bool IsShortStay(ListingEnquiryCard? card, Lease? lease)
        {
            if (lease != null)
                return lease.EndDate.HasValue && (lease.EndDate.Value.Date - lease.StartDate.Date).TotalDays < ShortTermStayNights;
            if (card?.ProposedStartDate is DateOnly start && card.ProposedEndDate is DateOnly end)
                return end.DayNumber - start.DayNumber < ShortTermStayNights;
            return false;
        }

        private static string? RuleTime(List<ListingRule> rules, string ruleType)
        {
            var value = rules.FirstOrDefault(x => string.Equals(x.RuleType, ruleType, StringComparison.OrdinalIgnoreCase))?.RuleTitle?.Trim();
            if (string.IsNullOrEmpty(value))
                return null;
            return TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time)
                ? time.ToString("h:mm tt", CultureInfo.InvariantCulture)
                : null;
        }

        private static List<string> BuildHouseRules(Listing listing, List<ListingRule> rules)
        {
            var lines = new List<string>();
            var policy = listing.ListingPolicies?.FirstOrDefault();

            var guests = rules.FirstOrDefault(x => string.Equals(x.RuleType, "NUMBER_OF_GUESTS", StringComparison.OrdinalIgnoreCase));
            var maxGuests = int.TryParse(guests?.RuleTitle, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) && parsed > 0
                ? parsed
                : policy?.MaximumOccupants ?? 0;
            if (maxGuests > 0)
                lines.Add($"{maxGuests} {(maxGuests == 1 ? "guest" : "guests")} maximum");

            if (policy?.AllowsPets is bool pets)
                lines.Add(pets ? "Pets allowed" : "No pets");
            if (policy?.AllowsSmoking is bool smoking)
                lines.Add(smoking ? "Smoking allowed" : "No smoking");
            if (policy?.AllowsChildren == false)
                lines.Add("Not suitable for children");

            var quietStart = rules.FirstOrDefault(x => string.Equals(x.RuleType, "QUIET_HOURS_START", StringComparison.OrdinalIgnoreCase));
            var quietFrom = RuleTime(rules, "QUIET_HOURS_START");
            var quietTo = RuleTime(rules, "QUIET_HOURS_END");
            if (quietStart?.IsAllowed != false && quietFrom != null && quietTo != null)
                lines.Add($"Quiet hours {quietFrom} – {quietTo}");

            var photography = rules.FirstOrDefault(x => string.Equals(x.RuleType, "COMMERCIAL_PHOTOGRAPHY", StringComparison.OrdinalIgnoreCase));
            if (photography?.IsAllowed is bool filming)
                lines.Add(filming ? "Commercial photography and filming allowed" : "No commercial photography or filming");

            // Free-form rules only count when the host wrote something beyond the seeded placeholder title.
            foreach (var rule in rules.Where(x => !NonHouseRuleTypes.Contains(x.RuleType ?? string.Empty)
                && !(x.RuleType ?? string.Empty).StartsWith("SAFETY_", StringComparison.OrdinalIgnoreCase)))
            {
                var description = rule.RuleDescription?.Trim();
                if (!string.IsNullOrEmpty(description) && !string.Equals(description, rule.RuleTitle?.Trim(), StringComparison.OrdinalIgnoreCase))
                    lines.Add(description);
            }

            var additional = rules.FirstOrDefault(x => string.Equals(x.RuleType, "ADDITIONAL", StringComparison.OrdinalIgnoreCase))?.RuleTitle?.Trim();
            if (!string.IsNullOrEmpty(additional) && !string.Equals(additional, "Additional Rules", StringComparison.OrdinalIgnoreCase))
                lines.Add(additional);

            return lines.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static ConversationDetailsReservationDto MapReservation(Lease lease, bool confirmed)
        {
            int? termMonths = lease.LeaseTermMonths is short months && months > 0 ? months : null;
            if (termMonths == null && lease.EndDate.HasValue && lease.EndDate.Value > lease.StartDate)
            {
                var end = lease.EndDate.Value;
                var whole = (end.Year - lease.StartDate.Year) * 12 + end.Month - lease.StartDate.Month;
                if (end.Day < lease.StartDate.Day)
                    whole--;
                termMonths = whole > 0 ? whole : null;
            }

            return new ConversationDetailsReservationDto
            {
                LeaseID = lease.LeaseID,
                Status = lease.Status,
                StartDate = lease.StartDate,
                EndDate = lease.EndDate,
                TermMonths = termMonths,
                MonthlyRent = lease.BaseRentAmount,
                Total = termMonths.HasValue ? lease.BaseRentAmount * termMonths.Value : null,
                Currency = lease.Currency,
                IsConfirmed = confirmed,
                ConfirmationCode = !confirmed ? null : string.IsNullOrWhiteSpace(lease.LeaseCode) ? lease.LeaseNumber : lease.LeaseCode,
                BookedAt = lease.SignedAt ?? lease.CapturedDate,
                RentalApplicationID = lease.RentalApplicationID,
            };
        }

        private async Task<ConversationDetailsPersonDto?> MapRenterAsync(Tenant? tenant, bool includePhone, CancellationToken cancellationToken)
        {
            if (tenant == null)
                return null;
            return new ConversationDetailsPersonDto
            {
                Name = FullName(tenant.User),
                FirstName = FirstName(tenant.User),
                PhotoUrl = await ResolveImageUrlAsync(tenant.PhotoUrl, cancellationToken),
                PhoneNumber = includePhone ? tenant.PhoneNumber : null,
            };
        }

        private async Task<ConversationDetailsPersonDto?> MapHostAsync(Organization? organization, CancellationToken cancellationToken)
        {
            if (organization == null)
                return null;

            var owner = await context.OrganizationMembers
                .Include(x => x.User)
                .Where(x => x.OrganizationID == organization.OrganizationID && x.IsPrimaryOwner)
                .FirstOrDefaultAsync(cancellationToken);

            return new ConversationDetailsPersonDto
            {
                Name = organization.DisplayName ?? organization.LegalName,
                FirstName = FirstName(owner?.User),
                PhotoUrl = await ResolveImageUrlAsync(string.IsNullOrWhiteSpace(organization.BrandLogoUrl) ? owner?.ProfilePhotoUrl : organization.BrandLogoUrl, cancellationToken),
                Subtitle = organization.CapturedDate.HasValue ? $"Hosting since {organization.CapturedDate.Value.Year}" : null,
            };
        }

        /// <summary>Uploaded images are stored as blob references; the browser needs a readable (SAS) URL.</summary>
        private async Task<string?> ResolveImageUrlAsync(string? value, CancellationToken cancellationToken)
        {
            var reference = Clean(value);
            if (reference == null)
                return null;
            if (Uri.TryCreate(reference, UriKind.Absolute, out _) || reference.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return reference;

            try
            {
                return await fileStorageService.GetReadSasUrlAsync(StorageCategory.Image, reference.TrimStart('/'), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not resolve image reference {Reference} for the conversation details pane.", reference);
                return null;
            }
        }

        private async Task<ConversationDetailsReviewDto?> FindReviewAsync(Lease lease, Conversation conversation, bool isHost, DateTime now, CancellationToken cancellationToken)
        {
            // Hosts see what the renter wrote about them; renters see what the host wrote about them.
            var subjectType = isHost ? "HOST" : "TENANT";
            var subjectId = isHost ? conversation.OrganizationID?.ToString() : conversation.Tenant?.UserID.ToString(CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(subjectId))
                return null;

            var candidates = await context.Ratings
                .Include(x => x.ReviewerUser)
                .Where(x => x.LeaseID == lease.LeaseID && x.IsPublic && x.SubjectType == subjectType)
                .ToListAsync(cancellationToken);

            var rating = candidates
                .Where(x => string.Equals(x.SubjectReferenceID, subjectId, StringComparison.OrdinalIgnoreCase) && (x.PublishedAt == null || x.PublishedAt <= now))
                .OrderByDescending(x => x.PublishedAt ?? x.CapturedDate)
                .FirstOrDefault();

            return rating == null ? null : new ConversationDetailsReviewDto
            {
                RatingID = rating.RatingID,
                ReviewerFirstName = FirstName(rating.ReviewerUser),
                OverallRating = rating.OverallRating,
                PublishedAt = rating.PublishedAt,
            };
        }

        private async Task<ConversationDetailsAccessDto?> BuildAccessAsync(Listing listing, Lease? lease, bool confirmed, bool isHost, DateTime now, CancellationToken cancellationToken)
        {
            var leaseId = lease?.LeaseID;
            var instructions = await context.ListingAccessInstructions
                .Where(x => x.ListingID == listing.ListingID && x.IsActive && (x.LeaseID == null || x.LeaseID == leaseId))
                .ToListAsync(cancellationToken);

            var instruction = instructions
                .Where(x => (x.AvailableFrom == null || x.AvailableFrom <= now) && (x.AvailableUntil == null || x.AvailableUntil >= now))
                .OrderByDescending(x => x.LeaseID.HasValue)
                .ThenByDescending(x => x.UpdatedDate ?? x.CapturedDate)
                .FirstOrDefault();

            var doorCode = string.IsNullOrWhiteSpace(listing.CheckInDoorCode) ? null : listing.CheckInDoorCode.Trim();
            var text = string.IsNullOrWhiteSpace(instruction?.Instructions) ? null : instruction!.Instructions!.Trim();
            if (doorCode == null && text == null)
                return null;

            if (isHost)
                return new ConversationDetailsAccessDto { DoorCode = doorCode, Instructions = text };
            if (lease == null || !confirmed || !string.Equals(lease.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                return null;

            // Mirrors the check-in details email: renters get access details a few days before an active lease starts.
            var leadDays = Math.Max(1, configuration.GetValue<int?>("LeaseCheckIn:DetailsLeadDays") ?? 3);
            var availableFrom = lease.StartDate.Date.AddDays(-leadDays);
            if (now >= availableFrom)
                return new ConversationDetailsAccessDto { DoorCode = doorCode, Instructions = text };

            return new ConversationDetailsAccessDto { AvailableFrom = availableFrom };
        }

        private async Task<ConversationDetailsPayoutDto?> FindPayoutAsync(Lease lease, CancellationToken cancellationToken)
        {
            var items = await (
                from item in context.PayoutItems
                join payment in context.Payments on item.PaymentID equals payment.PaymentID
                join intent in context.PaymentIntents on payment.PaymentIntentID equals intent.PaymentIntentID
                join payout in context.Payouts on item.PayoutID equals payout.PayoutID
                where intent.LeaseID == lease.LeaseID && payout.OrganizationID == lease.OrganizationID
                select new { item.NetPayoutAmount, payout.PayoutID, payout.Status, payout.PaidAt, payout.ScheduledAt, payout.RequestedAt, payout.Currency })
                .ToListAsync(cancellationToken);

            var latest = items
                .GroupBy(x => x.PayoutID)
                .Select(group => new { Payout = group.First(), Amount = group.Sum(x => x.NetPayoutAmount) })
                .OrderByDescending(x => x.Payout.PaidAt ?? x.Payout.ScheduledAt ?? x.Payout.RequestedAt)
                .FirstOrDefault();

            return latest == null ? null : new ConversationDetailsPayoutDto
            {
                Amount = latest.Amount,
                Currency = latest.Payout.Currency ?? lease.Currency,
                Status = latest.Payout.Status,
                PaidAt = latest.Payout.PaidAt,
                ScheduledAt = latest.Payout.ScheduledAt,
            };
        }

        private async Task<ConversationNoteDto?> LoadNoteAsync(Guid conversationId, CancellationToken cancellationToken)
        {
            var note = await context.ConversationNotes
                .Include(x => x.UpdatedByOrganizationMember).ThenInclude(x => x!.User)
                .FirstOrDefaultAsync(x => x.ConversationID == conversationId, cancellationToken);
            return note == null ? null : new ConversationNoteDto
            {
                Note = note.Note,
                UpdatedAt = note.UpdatedAt,
                UpdatedByName = FirstName(note.UpdatedByOrganizationMember?.User),
            };
        }

        private async Task<ConversationDetailsSupportDto?> LoadSupportAsync()
        {
            try
            {
                var preference = await preferenceService.GetPreference();
                if (preference != null && (!string.IsNullOrWhiteSpace(preference.CompanyEmail) || !string.IsNullOrWhiteSpace(preference.CompanyName)))
                    return new ConversationDetailsSupportDto { CompanyName = preference.CompanyName, Email = preference.CompanyEmail };
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Support contact unavailable for conversation details.");
            }
            return null;
        }

        private static string? FirstName(User? user) => string.IsNullOrWhiteSpace(user?.FirstName) ? null : user!.FirstName.Trim();

        private static string? FullName(User? user)
        {
            if (user == null)
                return null;
            var name = $"{user.FirstName} {user.LastName}".Trim();
            return name.Length > 0 ? name : user.DisplayName;
        }
    }
}
