// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

[Table("PostalLookupSuggestion")]
public class PostalLookupSuggestion
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(255)]
    public string? Text { get; set; }

    [MaxLength(255)]
    public string? Line1 { get; set; }

    [MaxLength(255)]
    public string? Line2 { get; set; }

    [MaxLength(120)]
    public string? City { get; set; }

    [MaxLength(10)]
    public string? ProvinceCode { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(2)]
    public string? CountryCode { get; set; }

    [MaxLength(50)]
    public string? PlaceProvider { get; set; }

    [MaxLength(255)]
    public string? PlaceProviderReferenceID { get; set; }

    [MaxLength(50)]
    public string? LookupKey { get; set; }

    public DateTime? CapturedDate { get; set; }
}
