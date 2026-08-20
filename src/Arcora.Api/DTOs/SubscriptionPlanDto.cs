// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Defines subscription plans with pricing, features, and limits for landlords and property managers.
/// </summary>
public class SubscriptionPlanDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? SubscriptionPlanID { get; set; }

    /// <summary>
    /// Plan code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Code { get; set; }

    /// <summary>
    /// Plan name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Plan description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Monthly price
    /// </summary>
    [Required]
    public decimal MonthlyPrice { get; set; }
    /// <summary>
    /// Annual price
    /// </summary>
    public decimal? AnnualPrice { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";
    /// <summary>
    /// Maximum properties allowed
    /// </summary>
    public int? MaxProperties { get; set; }
    /// <summary>
    /// Maximum rental units allowed
    /// </summary>
    public int? MaxRentalUnits { get; set; }
    /// <summary>
    /// Maximum active listings allowed
    /// </summary>
    public int? MaxActiveListings { get; set; }
    /// <summary>
    /// Maximum organization members allowed
    /// </summary>
    public int? MaxOrganizationMembers { get; set; }

    /// <summary>
    /// Plan features
    /// </summary>
    [MaxLength(256)]
    public string? Features { get; set; }

    /// <summary>
    /// Provider product identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderProductID { get; set; }

    /// <summary>
    /// Provider monthly price ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderMonthlyPriceID { get; set; }

    /// <summary>
    /// Provider annual price ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderAnnualPriceID { get; set; }

    /// <summary>
    /// Active status
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
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
    /// Record update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}