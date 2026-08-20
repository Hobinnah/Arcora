// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Tracks disputes raised by tenants or organizations related to leases, payments, maintenance, and chargebacks.
/// </summary>
[Table("Dispute")]
public class Dispute
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid DisputeID { get; set; }

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
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public Guid? InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public Guid? PaymentID { get; set; }
    /// <summary>
    /// FK to Chargeback
    /// </summary>
    public Guid? ChargebackID { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public Guid? MaintenanceRequestID { get; set; }

    /// <summary>
    /// Dispute status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "OPEN";

    /// <summary>
    /// Dispute title
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }

    /// <summary>
    /// Dispute description
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Notes on dispute resolution
    /// </summary>
    [MaxLength(1000)]
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Dispute opened timestamp
    /// </summary>
    [Required]
    public DateTime OpenedAt { get; set; }
    /// <summary>
    /// Dispute resolved timestamp
    /// </summary>
    public DateTime? ResolvedAt { get; set; }
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
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [ForeignKey(nameof(InvoiceMasterID))]
    public InvoiceMaster? InvoiceMaster { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }

    /// <summary>
    /// FK to Chargeback
    /// </summary>
    [ForeignKey(nameof(ChargebackID))]
    public Chargeback? Chargeback { get; set; }

    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    [ForeignKey(nameof(MaintenanceRequestID))]
    public MaintenanceRequest? MaintenanceRequest { get; set; }
}