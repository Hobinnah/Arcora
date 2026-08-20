// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Defines types of tenancy agreements including duration limits and periodicity.
/// </summary>
public class TenancyTypeDto
{
    /// <summary>
    /// Key
    /// </summary>
    public int TenancyTypeID { get; set; }

    /// <summary>
    /// Code for tenancy type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// Name of tenancy type
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Description of tenancy type
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    /// <summary>
    /// Minimum months for tenancy
    /// </summary>
    public short? MinimumMonths { get; set; }
    /// <summary>
    /// Maximum months for tenancy
    /// </summary>
    public short? MaximumMonths { get; set; }

    /// <summary>
    /// Indicates if tenancy is periodic
    /// </summary>
    [Required]
    public bool IsPeriodic { get; set; }

    /// <summary>
    /// Indicates if tenancy type is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
}