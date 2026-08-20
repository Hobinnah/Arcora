// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Records screening checks for tenants including type, provider info, status, score, and report references.
/// </summary>
public class TenantScreeningCheckDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? TenantScreeningCheckID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? RentalApplicationID { get; set; }

    /// <summary>
    /// Type of screening check
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CheckType { get; set; }

    /// <summary>
    /// Name of screening provider
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Reference ID from provider
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }

    /// <summary>
    /// Screening check status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "REQUESTED";
    /// <summary>
    /// Date consent was captured
    /// </summary>
    public DateTime? ConsentCapturedAt { get; set; }
    /// <summary>
    /// Screening score
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// Summary of screening result
    /// </summary>
    [MaxLength(256)]
    public string? ResultSummary { get; set; }

    /// <summary>
    /// Reference to screening report
    /// </summary>
    [MaxLength(500)]
    public string? ReportReference { get; set; }

    /// <summary>
    /// Date screening was requested
    /// </summary>
    [Required]
    public DateTime RequestedAt { get; set; }
    /// <summary>
    /// Date screening was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Date screening expires
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Reason for screening failure
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }
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
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}