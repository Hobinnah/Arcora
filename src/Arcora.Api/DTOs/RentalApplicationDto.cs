// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores rental application details submitted by tenants for listings.
/// </summary>
public class RentalApplicationDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? RentalApplicationID { get; set; }

    /// <summary>
    /// Unique application code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ApplicationCode { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Desired move-in date
    /// </summary>
    [Required]
    public DateTime DesiredMoveInDate { get; set; }
    /// <summary>
    /// Desired move-out date
    /// </summary>
    public DateTime? DesiredMoveOutDate { get; set; }

    /// <summary>
    /// Requested lease term in months
    /// </summary>
    [Required]
    public short RequestedLeaseTermMonths { get; set; }

    /// <summary>
    /// Number of adult occupants
    /// </summary>
    [Required]
    public int AdultOccupantCount { get; set; } = 1;

    /// <summary>
    /// Number of child occupants
    /// </summary>
    [Required]
    public int ChildOccupantCount { get; set; } = 0;

    /// <summary>
    /// Number of pets
    /// </summary>
    [Required]
    public int PetCount { get; set; } = 0;
    /// <summary>
    /// Proposed monthly rent amount
    /// </summary>
    public decimal? ProposedMonthlyRentAmount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Application status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";

    /// <summary>
    /// Screening status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ScreeningStatus { get; set; } = "NOT_STARTED";

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates whether the lease contract has been reviewed
    /// </summary>
    public bool? LeaseContractReviewed { get; set; } = false;

    /// <summary>
    /// Indicates whether the applicant authorized verification
    /// </summary>
    public bool? VerificationAuthorization { get; set; } = false;
    /// <summary>
    /// Date and time application was submitted
    /// </summary>
    public DateTime? SubmittedAt { get; set; }
    /// <summary>
    /// Date and time application was reviewed
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    /// <summary>
    /// FK to OrganizationMember who reviewed
    /// </summary>
    public Guid? ReviewedByOrganizationMemberID { get; set; }
    /// <summary>
    /// Date and time application was approved
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    /// <summary>
    /// Date and time application was declined
    /// </summary>
    public DateTime? DeclinedAt { get; set; }

    /// <summary>
    /// Reason for decline
    /// </summary>
    [MaxLength(256)]
    public string? DeclineReason { get; set; }
    /// <summary>
    /// Expiration date of application
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    public ListingDto? Listing { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
    /// <summary>
    /// FK to OrganizationMember who reviewed
    /// </summary>
    public OrganizationMemberDto? ReviewedByOrganizationMember { get; set; }
}