// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Tracks security deposits related to leases including amounts required, received, applied, and returned.
/// </summary>
public class SecurityDepositDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? SecurityDepositID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Amount required for deposit
    /// </summary>
    [Required]
    public decimal RequiredAmount { get; set; }

    /// <summary>
    /// Amount received for deposit
    /// </summary>
    [Required]
    public decimal ReceivedAmount { get; set; } = 0;

    /// <summary>
    /// Amount applied from deposit
    /// </summary>
    [Required]
    public decimal AppliedAmount { get; set; } = 0;

    /// <summary>
    /// Amount returned from deposit
    /// </summary>
    [Required]
    public decimal ReturnedAmount { get; set; } = 0;

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Deposit status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "EXPECTED";
    /// <summary>
    /// Due date for deposit
    /// </summary>
    public DateTime? DueDate { get; set; }
    /// <summary>
    /// Date deposit was fully funded
    /// </summary>
    public DateTime? FullyFundedAt { get; set; }
    /// <summary>
    /// Date deposit was held
    /// </summary>
    public DateTime? HeldAt { get; set; }
    /// <summary>
    /// Date deposit was closed
    /// </summary>
    public DateTime? ClosedAt { get; set; }
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
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
}