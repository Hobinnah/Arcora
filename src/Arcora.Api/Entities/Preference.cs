// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
[Table("Preference")]
public class Preference
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PreferenceID { get; set; }

    [MaxLength(256)]
    public string? CompanyName { get; set; }
    public decimal? TaxRate { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(256)]
    public string? CompanyEmail { get; set; }

    [MaxLength(50)]
    public string? CompanyPhone { get; set; }

    [MaxLength(256)]
    public string? CompanyWebsite { get; set; }

    [MaxLength(500)]
    public string? CompanyLogo { get; set; }

    [MaxLength(100)]
    public string? TaxIdentificationNumber { get; set; }

    [MaxLength(100)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(3)]
    public string? DefaultCurrency { get; set; }
    public bool? RequirePaymentBeforeCaseSubmission { get; set; }
    public int? AutoAssignAgentID { get; set; }
    public int? MaxDraftExpiryDays { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}