// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents an authorization mandate for automatic debit payments from tenants for leases.
/// </summary>
public class AutopayMandateDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? AutopayMandateID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }
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
    /// FK to PaymentMethod
    /// </summary>
    [Required]
    public Guid PaymentMethodID { get; set; }

    /// <summary>
    /// Mandate status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";

    /// <summary>
    /// Type of mandate
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? MandateType { get; set; } = "VARIABLE";

    /// <summary>
    /// Payment rail used
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? PaymentRail { get; set; }
    /// <summary>
    /// Maximum amount per debit
    /// </summary>
    public decimal? MaximumAmountPerDebit { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Debit frequency
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Frequency { get; set; } = "MONTHLY";

    /// <summary>
    /// Mandate start date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Mandate end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Name of the payment provider
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Mandate ID from provider
    /// </summary>
    [MaxLength(255)]
    public string? ProviderMandateID { get; set; }

    /// <summary>
    /// Version of consent given
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ConsentVersion { get; set; }

    /// <summary>
    /// Hash of consent text
    /// </summary>
    [MaxLength(255)]
    public string? ConsentTextHash { get; set; }

    /// <summary>
    /// IP address where consent was given
    /// </summary>
    [MaxLength(100)]
    public string? ConsentIpAddress { get; set; }
    /// <summary>
    /// Date and time consent was given
    /// </summary>
    public DateTime? ConsentedAt { get; set; }
    /// <summary>
    /// Date and time mandate was activated
    /// </summary>
    public DateTime? ActivatedAt { get; set; }
    /// <summary>
    /// Date and time mandate was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    [MaxLength(256)]
    public string? CancellationReason { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// Date when record was last updated
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
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to PaymentMethod
    /// </summary>
    public PaymentMethodDto? PaymentMethod { get; set; }
}