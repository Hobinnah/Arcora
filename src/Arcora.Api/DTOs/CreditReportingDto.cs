// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Records credit reporting details for tenants including amounts reported, payment timeliness, and provider responses.
/// </summary>
public class CreditReportingDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? CreditReportingID { get; set; }

    /// <summary>
    /// FK to CreditReportingEnrollment
    /// </summary>
    [Required]
    public Guid CreditReportingEnrollmentID { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public Guid? InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public Guid? PaymentID { get; set; }

    /// <summary>
    /// Amount reported to credit agency
    /// </summary>
    [Required]
    public decimal ReportedAmount { get; set; }
    /// <summary>
    /// Indicates if payment was on time
    /// </summary>
    public bool? WasPaidOnTime { get; set; }
    /// <summary>
    /// Start date of reporting period
    /// </summary>
    public DateTime? ReportingPeriodStart { get; set; }
    /// <summary>
    /// End date of reporting period
    /// </summary>
    public DateTime? ReportingPeriodEnd { get; set; }

    /// <summary>
    /// Status from credit reporting provider
    /// </summary>
    [MaxLength(100)]
    public string? ProviderStatus { get; set; }

    /// <summary>
    /// Provider reference ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }

    /// <summary>
    /// Response from credit reporting provider
    /// </summary>
    [MaxLength(256)]
    public string? ProviderResponse { get; set; }

    /// <summary>
    /// Timestamp when reported
    /// </summary>
    [Required]
    public DateTime ReportedAt { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to CreditReportingEnrollment
    /// </summary>
    public CreditReportingEnrollmentDto? CreditReportingEnrollment { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public InvoiceMasterDto? InvoiceMaster { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
}