// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores pricing details for different lease terms of a listing.
/// </summary>
[Table("ListingTermPrice")]
public class ListingTermPrice
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingTermPriceID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// Lease term in months
    /// </summary>
    [Required]
    public short LeaseTermMonths { get; set; }

    /// <summary>
    /// Monthly rent amount for the lease term
    /// </summary>
    [Required]
    public decimal MonthlyRentAmount { get; set; }
    /// <summary>
    /// Security deposit amount for the lease term
    /// </summary>
    public decimal? SecurityDepositAmount { get; set; }
    /// <summary>
    /// Effective start date of this price
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }
    /// <summary>
    /// Effective end date of this price
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Indicates if this price is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }
}