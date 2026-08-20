// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents property records.
/// </summary>
public class PropertyDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? PropertyID { get; set; }

    /// <summary>
    /// Organization identifier
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Address identifier
    /// </summary>
    [Required]
    public Guid AddressID { get; set; }

    /// <summary>
    /// Property name
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Property type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? PropertyType { get; set; }
    /// <summary>
    /// Year the property was built
    /// </summary>
    public int? YearBuilt { get; set; }

    /// <summary>
    /// Property time zone
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Property description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Property status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record captured by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// Navigation property for Organization entity.
    /// </summary>
    public OrganizationDto? Organization { get; set; }
    /// <summary>
    /// Navigation property for Address entity.
    /// </summary>
    public AddressDto? Address { get; set; }
}