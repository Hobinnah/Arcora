// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores tenant payment methods and provider references.
/// </summary>
public class PaymentMethodDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? PaymentMethodID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Payment method type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? PaymentMethodType { get; set; }

    /// <summary>
    /// Display name
    /// </summary>
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Last 4 digits of account or card
    /// </summary>
    [MaxLength(20)]
    public string? AccountLast4 { get; set; }

    /// <summary>
    /// Card brand
    /// </summary>
    [MaxLength(50)]
    public string? CardBrand { get; set; }

    /// <summary>
    /// Bank name
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }
    /// <summary>
    /// Card expiry month
    /// </summary>
    public short? ExpiryMonth { get; set; }
    /// <summary>
    /// Card expiry year
    /// </summary>
    public short? ExpiryYear { get; set; }

    /// <summary>
    /// Provider name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider customer identifier
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? ProviderCustomerID { get; set; }

    /// <summary>
    /// Provider payment method identifier
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? ProviderPaymentMethodID { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? VerificationStatus { get; set; } = "PENDING";
    /// <summary>
    /// Verification timestamp
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Is default payment method
    /// </summary>
    [Required]
    public bool IsDefault { get; set; }

    /// <summary>
    /// Indicates if the payment method is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Record captured by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }
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
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}