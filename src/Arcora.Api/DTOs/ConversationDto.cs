// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents conversations related to leases, maintenance requests, disputes, etc.
/// </summary>
public class ConversationDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ConversationID { get; set; }

    /// <summary>
    /// Type of conversation
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ConversationType { get; set; } = "DIRECT";
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public Guid? MaintenanceRequestID { get; set; }
    /// <summary>
    /// FK to Dispute
    /// </summary>
    public Guid? DisputeID { get; set; }

    /// <summary>
    /// FK to Tenant (tenant/prospective tenant side of a direct host-to-tenant thread).
    /// </summary>
    public Guid? TenantID { get; set; }

    /// <summary>
    /// FK to Organization (host side of a direct host-to-tenant thread).
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Subject of the conversation
    /// </summary>
    [MaxLength(200)]
    public string? Subject { get; set; }

    /// <summary>
    /// Conversation status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "OPEN";
    /// <summary>
    /// Date and time of last message
    /// </summary>
    public DateTime? LastMessageAt { get; set; }
    /// <summary>
    /// Date and time conversation was closed
    /// </summary>
    public DateTime? ClosedAt { get; set; }
    /// <summary>
    /// Date conversation was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the conversation
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date conversation was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User ID who last updated the conversation
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public MaintenanceRequestDto? MaintenanceRequest { get; set; }
    /// <summary>
    /// FK to Dispute
    /// </summary>
    public DisputeDto? Dispute { get; set; }
}