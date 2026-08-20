// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores address details linked optionally to an organization.
/// </summary>
[Table("Address")]
public class Address
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid AddressID { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Type of address
    /// </summary>
    [MaxLength(50)]
    public string? AddressType { get; set; }

    /// <summary>
    /// Address line 1
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? Line1 { get; set; }

    /// <summary>
    /// Address line 2
    /// </summary>
    [MaxLength(255)]
    public string? Line2 { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string? City { get; set; }

    /// <summary>
    /// Province or state code
    /// </summary>
    [MaxLength(10)]
    public string? ProvinceCode { get; set; }

    /// <summary>
    /// Postal or ZIP code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    [Required]
    [MaxLength(2)]
    public string? CountryCode { get; set; } = "CA";
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public decimal? Latitude { get; set; }
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// Place provider name
    /// </summary>
    [MaxLength(50)]
    public string? PlaceProvider { get; set; }

    /// <summary>
    /// Reference ID from place provider
    /// </summary>
    [MaxLength(255)]
    public string? PlaceProviderReferenceID { get; set; }
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by user
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by user
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}