// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Audits actions related to tenant credit reporting consent including consent, withdrawal, updates, and expirations.
/// </summary>
public class CreditReportingConsentAuditDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ConsentAuditID { get; set; }

    /// <summary>
    /// FK to CreditReportingEnrollment
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string? CreditReportingEnrollmentID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public bool TenantID { get; set; }

    /// <summary>
    /// Audit action performed
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Action { get; set; }

    /// <summary>
    /// Version of consent
    /// </summary>
    [MaxLength(50)]
    public string? ConsentVersion { get; set; }

    /// <summary>
    /// Hash of consent text
    /// </summary>
    [MaxLength(255)]
    public string? ConsentTextHash { get; set; }

    /// <summary>
    /// Provider reference ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }

    /// <summary>
    /// Timestamp of action
    /// </summary>
    [Required]
    public DateTime ActionAt { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    [MaxLength(256)]
    public string? Metadata { get; set; }
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
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}