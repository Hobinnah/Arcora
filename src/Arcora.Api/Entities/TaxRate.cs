// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores tax rate information by country and province with validity periods.
/// </summary>
[Table("TaxRate")]
public class TaxRate
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaxID { get; set; }

    /// <summary>
    /// Tax code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// Tax name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    [Required]
    [MaxLength(2)]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Province code
    /// </summary>
    [MaxLength(10)]
    public string? ProvinceCode { get; set; }

    /// <summary>
    /// Tax rate
    /// </summary>
    [Required]
    public decimal Rate { get; set; }

    /// <summary>
    /// Tax rate effective start date
    /// </summary>
    [Required]
    public DateTime EffectiveFrom { get; set; }
    /// <summary>
    /// Tax rate effective end date
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Indicates if tax rate is active
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