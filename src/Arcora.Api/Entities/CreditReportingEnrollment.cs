// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Manages tenant enrollments for credit reporting related to leases and payments.
/// </summary>
[Table("CreditReportingEnrollment")]
public class CreditReportingEnrollment
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid CreditReportingEnrollmentID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Enrollment status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING_CONSENT";
    /// <summary>
    /// Consent given timestamp
    /// </summary>
    public DateTime? ConsentedAt { get; set; }
    /// <summary>
    /// Cancellation timestamp
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Credit reporting provider name
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider reference ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }
}