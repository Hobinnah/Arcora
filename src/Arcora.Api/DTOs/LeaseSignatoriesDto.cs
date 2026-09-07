// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores information about signatories of lease documents including their roles and signature status.
/// </summary>
public class LeaseSignatoriesDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? LeaseSignatoryID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public Guid LeaseDocumentID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? UserID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public Guid? OrganizationMemberID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Role of the signatory
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? SignatoryRole { get; set; }

    /// <summary>
    /// Name of the signatory
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Email address of the signatory
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Signature status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";
    /// <summary>
    /// Order of signature
    /// </summary>
    public int? SignatureOrder { get; set; }

    /// <summary>
    /// ID from signature provider
    /// </summary>
    [MaxLength(255)]
    public string? ProviderSignerID { get; set; }
    /// <summary>
    /// Date and time when viewed
    /// </summary>
    public DateTime? ViewedAt { get; set; }
    /// <summary>
    /// Date and time when signed
    /// </summary>
    public DateTime? SignedAt { get; set; }
    /// <summary>
    /// Date and time when declined
    /// </summary>
    public DateTime? DeclinedAt { get; set; }
    /// <summary>
    /// Record creation date/time
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public User? User { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public OrganizationMemberDto? OrganizationMember { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
}