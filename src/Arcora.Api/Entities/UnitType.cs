// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Defines types of units available for rent or lease.
/// </summary>
[Table("UnitType")]
public class UnitType
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid UnitTypeID { get; set; }

    /// <summary>
    /// Name of the unit type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }
    /// <summary>
    /// Date the record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
}