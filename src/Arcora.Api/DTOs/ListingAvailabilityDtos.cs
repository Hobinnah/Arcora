using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class ListingLeaseTermOptionDto
{
    public short LeaseTermMonths { get; set; }
    public decimal MonthlyRentAmount { get; set; }
    public decimal SecurityDepositAmount { get; set; }
    public string Currency { get; set; } = "CAD";
}

public class AvailableListingForTermDto
{
    public Guid ListingID { get; set; }
    public Guid OrganizationID { get; set; }
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public short RequestedLeaseTermMonths { get; set; }
    public decimal RequestedMonthlyRentAmount { get; set; }
    public decimal RequestedSecurityDepositAmount { get; set; }
    public string Currency { get; set; } = "CAD";
    public List<ListingLeaseTermOptionDto> AvailableLeaseTerms { get; set; } = new();
}

public class ListingAvailabilityConflictDto
{
    public string SourceType { get; set; } = string.Empty;
    public Guid? SourceID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
}

public class ListingAvailabilityResponseDto
{
    public Guid ListingID { get; set; }
    public Guid OrganizationID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public short LeaseTermMonths { get; set; }
    public bool IsAvailable { get; set; }
    public string DateBoundaryRule { get; set; } = "Start date is occupied; end date is exclusive (first non-occupied day).";
    public List<ListingAvailabilityConflictDto> Conflicts { get; set; } = new();
}

public class CreateTenantInvitationRequestDto
{
    [Required]
    public Guid OrganizationID { get; set; }

    [Required]
    public Guid ListingID { get; set; }

    public Guid? LeaseID { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Range(1, 120)]
    public short LeaseTermMonths { get; set; }

    [Required]
    [MaxLength(50)]
    public string? InvitationPurpose { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? TokenHash { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public Guid? RentalApplicationID { get; set; }
    public Guid? TenantID { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }
}

public class CreateTenantInvitationResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ListingAvailabilityResponseDto? Availability { get; set; }
    public TenantInvitationDto? TenantInvitation { get; set; }
    public ReservationHoldDto? ReservationHold { get; set; }
}

public class TenantInvitationTokenResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public TenantInvitationDto? TenantInvitation { get; set; }
    public Guid? TenantInvitationID { get; set; }
    public string? Status { get; set; }
}

public class CreateLeaseFromTenantInvitationRequestDto
{
    [Required]
    public Guid TenantID { get; set; }
}

public class CreateLeaseFromTenantInvitationResultDto
{
    public Guid LeaseID { get; set; }
    public string Status { get; set; } = "PENDING_TENANT_SIGNATURE";
    public bool AlreadyExists { get; set; }
}
