// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores receipt information issued to tenants for payments including voiding details.
/// </summary>
public class ReceiptMasterDto
{
    /// <summary>
    /// Key
    /// </summary>
    public long ReceiptMasterID { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [Required]
    public Guid PaymentID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Receipt number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ReceiptNumber { get; set; }

    /// <summary>
    /// Receipt amount
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string? Currency { get; set; }

    /// <summary>
    /// Receipt issued date and time
    /// </summary>
    [Required]
    public DateTime IssuedAt { get; set; }
    /// <summary>
    /// Receipt voided date and time
    /// </summary>
    public DateTime? VoidedAt { get; set; }

    /// <summary>
    /// Reason for voiding receipt
    /// </summary>
    [MaxLength(256)]
    public string? VoidReason { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}