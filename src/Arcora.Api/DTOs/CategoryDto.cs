// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Defines categories for contractors and maintenance requests.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Key
    /// </summary>
    public int CategoryID { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    /// <summary>
    /// Indicates if category is for maintenance
    /// </summary>
    public bool? Maintenance { get; set; }
    /// <summary>
    /// Indicates if category is for contractors
    /// </summary>
    public bool? Contractor { get; set; }
    /// <summary>
    /// Date category was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the category
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
}