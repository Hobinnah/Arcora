// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Logs audit records of tenant consents and actions related to autopay mandates.
/// </summary>
public class AutopayConsentAuditDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? AutopayConsentAuditID { get; set; }

    /// <summary>
    /// FK to AutopayMandate
    /// </summary>
    [Required]
    public Guid AutopayMandateID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

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
    /// IP address of action
    /// </summary>
    [MaxLength(100)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Reference ID from provider
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }

    /// <summary>
    /// Date and time of action
    /// </summary>
    [Required]
    public DateTime ActionAt { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    [MaxLength(256)]
    public string? Metadata { get; set; }

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
    /// FK to AutopayMandate
    /// </summary>
    public AutopayMandateDto? AutopayMandate { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}