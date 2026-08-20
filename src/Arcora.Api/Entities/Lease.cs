// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents lease agreements between organizations and tenants for listings.
/// </summary>
[Table("Lease")]
public class Lease
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LeaseID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    [Required]
    public Guid RentalUnitID { get; set; }

    /// <summary>
    /// FK to TenancyType
    /// </summary>
    [Required]
    public int TenancyTypeID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public Guid? RentalApplicationID { get; set; }

    /// <summary>
    /// Lease code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? LeaseCode { get; set; }

    /// <summary>
    /// Lease number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? LeaseNumber { get; set; }

    /// <summary>
    /// Lease status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";

    /// <summary>
    /// Lease start date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Lease end date
    /// </summary>
    public DateTime? EndDate { get; set; }
    /// <summary>
    /// Lease term in months
    /// </summary>
    public short? LeaseTermMonths { get; set; }

    /// <summary>
    /// Base rent amount
    /// </summary>
    [Required]
    public decimal BaseRentAmount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Grace period days
    /// </summary>
    [Required]
    public short GracePeriodDays { get; set; }

    /// <summary>
    /// Late fee fixed amount
    /// </summary>
    [Required]
    public decimal LateFeeFixedAmount { get; set; } = 0;

    /// <summary>
    /// Late fee percentage
    /// </summary>
    [Required]
    public decimal LateFeePercentage { get; set; } = 0;

    /// <summary>
    /// Indicates if lease auto-renews
    /// </summary>
    [Required]
    public bool AutoRenew { get; set; }
    /// <summary>
    /// Renewal notice days
    /// </summary>
    public int? RenewalNoticeDays { get; set; } = 90;
    /// <summary>
    /// Lease signed at
    /// </summary>
    public DateTime? SignedAt { get; set; }
    /// <summary>
    /// Lease activated at
    /// </summary>
    public DateTime? ActivatedAt { get; set; }
    /// <summary>
    /// Actual move-in time
    /// </summary>
    public DateTime? ActualMoveInAt { get; set; }
    /// <summary>
    /// Actual move-out time
    /// </summary>
    public DateTime? ActualMoveOutAt { get; set; }
    /// <summary>
    /// Lease terminated at
    /// </summary>
    public DateTime? TerminatedAt { get; set; }

    /// <summary>
    /// Lease termination reason
    /// </summary>
    [MaxLength(256)]
    public string? TerminationReason { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record captured by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }

    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    [ForeignKey(nameof(RentalUnitID))]
    public RentalUnit? RentalUnit { get; set; }

    /// <summary>
    /// FK to TenancyType
    /// </summary>
    [ForeignKey(nameof(TenancyTypeID))]
    public TenancyType? TenancyType { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    [ForeignKey(nameof(RentalApplicationID))]
    public RentalApplication? RentalApplication { get; set; }
}