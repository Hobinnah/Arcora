// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents renewal offers and agreements for leases including terms and status.
/// </summary>
[Table("LeaseRenewals")]
public class LeaseRenewals
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LeaseRenewalID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }

    /// <summary>
    /// Renewal status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";
    /// <summary>
    /// Date renewal was offered
    /// </summary>
    public DateTime? OfferedAt { get; set; }
    /// <summary>
    /// Date offer expires
    /// </summary>
    public DateTime? OfferExpiresAt { get; set; }
    /// <summary>
    /// Date offer was accepted
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
    /// <summary>
    /// Date offer was declined
    /// </summary>
    public DateTime? DeclinedAt { get; set; }

    /// <summary>
    /// Lease renewal start date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Lease renewal end date
    /// </summary>
    public DateTime? EndDate { get; set; }
    /// <summary>
    /// Lease term in months
    /// </summary>
    public short? LeaseTermMonths { get; set; }

    /// <summary>
    /// Rent amount
    /// </summary>
    [Required]
    public decimal RentAmount { get; set; }
    /// <summary>
    /// Security deposit adjustment amount
    /// </summary>
    public decimal? SecurityDepositAdjustmentAmount { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
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
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }
}