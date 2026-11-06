// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores address details linked optionally to an organization.
/// </summary>
public class AddressDto : IValidatableObject
{
    public bool LocationConfirmed { get; set; }
    /// <summary>
    /// Key
    /// </summary>
    public Guid? AddressID { get; set; }
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
    [Range(-90, 90)]
    public decimal? Latitude { get; set; }
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Latitude.HasValue != Longitude.HasValue)
            yield return new ValidationResult("Latitude and longitude must be supplied together.", new[] { nameof(Latitude), nameof(Longitude) });
        if (LocationConfirmed && !Latitude.HasValue)
            yield return new ValidationResult("Confirmed locations require coordinates.", new[] { nameof(LocationConfirmed) });
    }

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
    public OrganizationDto? Organization { get; set; }
}