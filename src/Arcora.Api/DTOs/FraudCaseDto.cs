// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents fraud investigations related to tenants, leases, payments, and chargebacks.
/// </summary>
public class FraudCaseDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? FraudCaseID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to PaymentIntent
    /// </summary>
    public Guid? PaymentIntentID { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public Guid? PaymentID { get; set; }
    /// <summary>
    /// FK to Chargeback
    /// </summary>
    public Guid? ChargebackID { get; set; }

    /// <summary>
    /// Fraud case status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "OPEN";
    /// <summary>
    /// Risk score for fraud case
    /// </summary>
    public decimal? RiskScore { get; set; }

    /// <summary>
    /// Reason for fraud case
    /// </summary>
    [MaxLength(256)]
    public string? Reason { get; set; }

    /// <summary>
    /// Indicates if case is blocking
    /// </summary>
    [Required]
    public bool IsBlocking { get; set; }

    /// <summary>
    /// Case opened timestamp
    /// </summary>
    [Required]
    public DateTime OpenedAt { get; set; }
    /// <summary>
    /// Case reviewed timestamp
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    /// <summary>
    /// Case closed timestamp
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Case resolution details
    /// </summary>
    [MaxLength(100)]
    public string? Resolution { get; set; }
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
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to PaymentIntent
    /// </summary>
    public PaymentIntentDto? PaymentIntent { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
    /// <summary>
    /// FK to Chargeback
    /// </summary>
    public ChargebackDto? Chargeback { get; set; }
}