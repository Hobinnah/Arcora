// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents occupants associated with a lease including tenants and other occupant types.
/// </summary>
[Table("LeaseOccupants")]
public class LeaseOccupants
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LeaseOccupantID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }
    /// <summary>
    /// FK to LeaseRenewals
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? UserID { get; set; }

    /// <summary>
    /// First name of occupant
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of occupant
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Type of occupant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? OccupantType { get; set; } = "TENANT";

    /// <summary>
    /// Indicates if occupant is primary tenant
    /// </summary>
    [Required]
    public bool IsPrimaryTenant { get; set; }

    /// <summary>
    /// Indicates if occupant is financially responsible
    /// </summary>
    [Required]
    public bool IsFinanciallyResponsible { get; set; }
    /// <summary>
    /// Date occupant joined
    /// </summary>
    public DateTime? JoinedAt { get; set; }
    /// <summary>
    /// Date occupant was removed
    /// </summary>
    public DateTime? RemovedAt { get; set; }

    /// <summary>
    /// Occupant status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";
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
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to LeaseRenewals
    /// </summary>
    [ForeignKey(nameof(LeaseRenewalID))]
    public LeaseRenewals? LeaseRenewalLeaseRenewals { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}