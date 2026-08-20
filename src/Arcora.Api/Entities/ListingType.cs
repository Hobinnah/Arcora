// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents different types of property listings available.
/// </summary>
[Table("ListingType")]
public class ListingType
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingTypeID { get; set; }

    /// <summary>
    /// Name of the listing type
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