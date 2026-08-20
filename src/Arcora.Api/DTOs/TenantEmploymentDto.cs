// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores employment details of tenants including employer info, income, and verification status.
/// </summary>
public class TenantEmploymentDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? TenantEmploymentID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Name of employer
    /// </summary>
    [MaxLength(100)]
    public string? EmployerName { get; set; }

    /// <summary>
    /// Job title
    /// </summary>
    [MaxLength(150)]
    public string? JobTitle { get; set; }

    /// <summary>
    /// Type of employment
    /// </summary>
    [MaxLength(50)]
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Employer email address
    /// </summary>
    [MaxLength(255)]
    public string? EmployerEmail { get; set; }

    /// <summary>
    /// Employer phone number
    /// </summary>
    [MaxLength(50)]
    public string? EmployerPhoneNumber { get; set; }
    /// <summary>
    /// Annual income
    /// </summary>
    public decimal? AnnualIncome { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";
    /// <summary>
    /// Employment start date
    /// </summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>
    /// Employment end date
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Indicates if employment is current
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Employment verification status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? VerificationStatus { get; set; } = "NOT_VERIFIED";
    /// <summary>
    /// Date record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date record was last updated
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
    public TenantDto? Tenant { get; set; }
}