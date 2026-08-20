// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores notifications sent to users, tenants, or organization members via various channels.
/// </summary>
[Table("Notification")]
public class Notification
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid NotificationID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? RecipientUserID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public Guid? OrganizationMemberID { get; set; }

    /// <summary>
    /// Recipient email address
    /// </summary>
    [MaxLength(255)]
    public string? RecipientEmail { get; set; }

    /// <summary>
    /// Recipient phone number
    /// </summary>
    [MaxLength(50)]
    public string? RecipientPhoneNumber { get; set; }

    /// <summary>
    /// Notification channel
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Channel { get; set; }

    /// <summary>
    /// Notification template code
    /// </summary>
    [MaxLength(100)]
    public string? TemplateCode { get; set; }

    /// <summary>
    /// Notification subject
    /// </summary>
    [MaxLength(255)]
    public string? Subject { get; set; }

    /// <summary>
    /// Notification body content
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string? Body { get; set; }

    /// <summary>
    /// Notification status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "QUEUED";

    /// <summary>
    /// Scheduled send time
    /// </summary>
    [Required]
    public DateTime ScheduledAt { get; set; }
    /// <summary>
    /// Actual send time
    /// </summary>
    public DateTime? SentAt { get; set; }
    /// <summary>
    /// Delivery time
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
    /// <summary>
    /// Read time
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Failure reason if any
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Provider message identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderMessageID { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    [MaxLength(256)]
    public string? Metadata { get; set; }
    /// <summary>
    /// Record capture date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(RecipientUserID))]
    public User? RecipientUser { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    [ForeignKey(nameof(OrganizationMemberID))]
    public OrganizationMember? OrganizationMember { get; set; }
}