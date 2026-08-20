// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Records audit trail of actions performed by users or system actors on entities.
/// </summary>
[Table("AuditLog")]
public class AuditLog
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid AuditLogID { get; set; }
    /// <summary>
    /// User who performed the action
    /// </summary>
    public long? ActorUserID { get; set; }
    /// <summary>
    /// Tenant identifier
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// Organization member identifier
    /// </summary>
    public Guid? OrganizationMemberID { get; set; }
    /// <summary>
    /// Organization identifier
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Type of actor performing the action
    /// </summary>
    [MaxLength(50)]
    public string? ActorType { get; set; }

    /// <summary>
    /// Action performed
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? Action { get; set; }

    /// <summary>
    /// Type of entity affected
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? EntityType { get; set; }

    /// <summary>
    /// ID of the entity affected
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? EntityID { get; set; }

    /// <summary>
    /// Old values before action
    /// </summary>
    [MaxLength(256)]
    public string? OldValues { get; set; }

    /// <summary>
    /// New values after action
    /// </summary>
    [MaxLength(256)]
    public string? NewValues { get; set; }

    /// <summary>
    /// IP address of actor
    /// </summary>
    [MaxLength(100)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Correlation identifier for tracing
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationID { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(256)]
    public string? Note { get; set; }
    /// <summary>
    /// Record capture date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Navigation property for User entity.
    /// </summary>
    [ForeignKey(nameof(ActorUserID))]
    public User? ActorUser { get; set; }

    /// <summary>
    /// Navigation property for Tenant entity.
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// Navigation property for OrganizationMember entity.
    /// </summary>
    [ForeignKey(nameof(OrganizationMemberID))]
    public OrganizationMember? OrganizationMember { get; set; }

    /// <summary>
    /// Navigation property for Organization entity.
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}