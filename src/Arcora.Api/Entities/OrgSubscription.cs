// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores subscription details for organizations including plan, status, billing, and provider info.
/// </summary>
[Table("OrgSubscription")]
public class OrgSubscription
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid OrgSubscriptionID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// FK to SubscriptionPlan
    /// </summary>
    [Required]
    public Guid SubscriptionPlanID { get; set; }

    /// <summary>
    /// Subscription status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "TRIALING";

    /// <summary>
    /// Billing frequency
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? BillingFrequency { get; set; } = "MONTHLY";

    /// <summary>
    /// Subscription start date
    /// </summary>
    [Required]
    public DateTime StartedAt { get; set; }
    /// <summary>
    /// Trial end date
    /// </summary>
    public DateTime? TrialEndsAt { get; set; }
    /// <summary>
    /// Current billing period start
    /// </summary>
    public DateTime? CurrentPeriodStart { get; set; }
    /// <summary>
    /// Current billing period end
    /// </summary>
    public DateTime? CurrentPeriodEnd { get; set; }

    /// <summary>
    /// Flag to cancel at period end
    /// </summary>
    [Required]
    public bool CancelAtPeriodEnd { get; set; }
    /// <summary>
    /// Cancellation date
    /// </summary>
    public DateTime? CancelledAt { get; set; }
    /// <summary>
    /// Subscription end date
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Subscription provider name
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider customer identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderCustomerID { get; set; }

    /// <summary>
    /// Provider subscription identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderSubscriptionID { get; set; }
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
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to SubscriptionPlan
    /// </summary>
    [ForeignKey(nameof(SubscriptionPlanID))]
    public SubscriptionPlan? SubscriptionPlan { get; set; }
}