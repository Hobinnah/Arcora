// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Policies for listings including pet, smoking, children allowances, occupancy, furnishing, parking, utilities, credit score, background check, and application instructions.
/// </summary>
public class ListingPolicyDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ListingPolicyID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }
    /// <summary>
    /// Indicates if pets are allowed
    /// </summary>
    public bool? AllowsPets { get; set; }
    /// <summary>
    /// Indicates if smoking is allowed
    /// </summary>
    public bool? AllowsSmoking { get; set; }
    /// <summary>
    /// Indicates if children are allowed
    /// </summary>
    public bool? AllowsChildren { get; set; }

    /// <summary>
    /// Maximum number of occupants allowed
    /// </summary>
    [Required]
    public int MaximumOccupants { get; set; }
    /// <summary>
    /// Indicates if the listing is furnished
    /// </summary>
    public bool? Furnished { get; set; }
    /// <summary>
    /// Indicates if parking is included
    /// </summary>
    public bool? ParkingIncluded { get; set; }
    /// <summary>
    /// Indicates if utilities are included
    /// </summary>
    public bool? UtilitiesIncluded { get; set; }
    /// <summary>
    /// Minimum credit score required
    /// </summary>
    public int? MinimumCreditScore { get; set; }
    /// <summary>
    /// Indicates if background check is required
    /// </summary>
    public bool? RequiresBackgroundCheck { get; set; }

    /// <summary>
    /// Instructions for application
    /// </summary>
    [MaxLength(256)]
    public string? ApplicationInstructions { get; set; }
    /// <summary>
    /// Record capture date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    [JsonIgnore]
    public ListingDto? Listing { get; set; }
}