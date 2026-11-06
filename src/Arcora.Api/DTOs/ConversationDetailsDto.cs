namespace Arcora.Api.DTOs;

/// <summary>
/// Everything the messaging side pane shows about a conversation, resolved for the viewing participant.
/// Sensitive sections (phone number, payout, notes, access details) are only populated for viewers allowed to see them.
/// </summary>
public sealed class ConversationDetailsDto
{
    public const string KindReservation = "RESERVATION";
    public const string KindEnquiry = "ENQUIRY";
    public const string KindDirect = "DIRECT";

    public Guid ConversationID { get; set; }

    /// <summary>RESERVATION when the thread is tied to a lease, ENQUIRY when it carries a listing enquiry, otherwise DIRECT.</summary>
    public string Kind { get; set; } = KindDirect;

    /// <summary>HOST or TENANT.</summary>
    public string ViewerRole { get; set; } = string.Empty;

    public string? Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ConversationDetailsListingDto? Listing { get; set; }
    public ConversationDetailsEnquiryDto? Enquiry { get; set; }
    public ConversationDetailsReservationDto? Reservation { get; set; }
    public ConversationDetailsPersonDto? Renter { get; set; }
    public ConversationDetailsPersonDto? Host { get; set; }
    public ConversationDetailsReviewDto? Review { get; set; }
    public ConversationDetailsAccessDto? Access { get; set; }
    public List<string> HouseRules { get; set; } = new();
    public ConversationDetailsPayoutDto? Payout { get; set; }
    public ConversationNoteDto? Note { get; set; }
    public ConversationDetailsSupportDto? Support { get; set; }
}

public sealed class ConversationDetailsListingDto
{
    public Guid ListingID { get; set; }
    public string? Title { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public string? Currency { get; set; }
    public short? MinimumLeaseMonths { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public ConversationDetailsAddressDto? Address { get; set; }
}

/// <summary>Street-level fields are only populated for confirmed (approved and paid) reservations; otherwise the city and region.</summary>
public sealed class ConversationDetailsAddressDto
{
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? ProvinceCode { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public sealed class ConversationDetailsEnquiryDto
{
    public DateOnly? ProposedStartDate { get; set; }
    public DateOnly? ProposedEndDate { get; set; }
    public DateTime EnquiredAt { get; set; }
}

public sealed class ConversationDetailsReservationDto
{
    public Guid LeaseID { get; set; }
    public string? Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? TermMonths { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal? Total { get; set; }
    public string? Currency { get; set; }

    /// <summary>True once the lease is approved (active) and a payment has cleared. Confirmation code and exact address depend on it.</summary>
    public bool IsConfirmed { get; set; }

    /// <summary>Only populated once the reservation is confirmed.</summary>
    public string? ConfirmationCode { get; set; }
    public DateTime? BookedAt { get; set; }
    public Guid? RentalApplicationID { get; set; }
}

public sealed class ConversationDetailsPersonDto
{
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Subtitle { get; set; }

    /// <summary>Only populated for the host side of a reservation.</summary>
    public string? PhoneNumber { get; set; }
}

public sealed class ConversationDetailsReviewDto
{
    public Guid RatingID { get; set; }
    public string? ReviewerFirstName { get; set; }
    public short OverallRating { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public sealed class ConversationDetailsAccessDto
{
    public string? DoorCode { get; set; }
    public string? Instructions { get; set; }

    /// <summary>When the tenant will be able to see access details, if they are still hidden from them.</summary>
    public DateTime? AvailableFrom { get; set; }
}

public sealed class ConversationDetailsPayoutDto
{
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

public sealed class ConversationNoteDto
{
    public string Note { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedByName { get; set; }
}

public sealed class ConversationDetailsSupportDto
{
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
}
