// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Defines types of fees applicable in the system.
/// </summary>
public class FeeTypeDto
{
    /// <summary>
    /// Key
    /// </summary>
    public int FeeTypeID { get; set; }

    /// <summary>
    /// Fee type name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    /// <summary>
    /// Indicates if fee type is platform fee
    /// </summary>
    public bool? IsPlatformFee { get; set; }
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
}