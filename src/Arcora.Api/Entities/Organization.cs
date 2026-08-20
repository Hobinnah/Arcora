// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores information about landlord organizations or individuals managing rental properties.
/// </summary>
[Table("Organization")]
public class Organization
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Legal name of the organization
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? LegalName { get; set; }

    /// <summary>
    /// Display name of the organization
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Business registration number
    /// </summary>
    [MaxLength(100)]
    public string? BusinessNumber { get; set; }

    /// <summary>
    /// Country code (ISO 2-letter)
    /// </summary>
    [Required]
    [MaxLength(2)]
    public string? CountryCode { get; set; } = "CA";

    /// <summary>
    /// Province or state code
    /// </summary>
    [MaxLength(10)]
    public string? ProvinceCode { get; set; }

    /// <summary>
    /// Indicates if the organization is a personal account
    /// </summary>
    [Required]
    public bool IsPersonal { get; set; }

    /// <summary>
    /// Current status of the organization
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";

    /// <summary>
    /// Default currency code (ISO 3-letter)
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? DefaultCurrency { get; set; } = "CAD";

    /// <summary>
    /// Time zone of the organization
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Prefix for invoice numbers
    /// </summary>
    [MaxLength(50)]
    public string? InvoicePrefix { get; set; }

    /// <summary>
    /// Prefix for receipt numbers
    /// </summary>
    [MaxLength(50)]
    public string? ReceiptPrefix { get; set; }

    /// <summary>
    /// Indicates if late fees are enabled
    /// </summary>
    [Required]
    public bool LateFeeEnabled { get; set; }

    /// <summary>
    /// Indicates if automatic invoice generation is enabled
    /// </summary>
    [Required]
    public bool AutoInvoiceGeneration { get; set; }

    /// <summary>
    /// Indicates if automatic payment retry is enabled
    /// </summary>
    [Required]
    public bool AutoPaymentRetry { get; set; }

    /// <summary>
    /// Payment provider name
    /// </summary>
    [MaxLength(100)]
    public string? PaymentProvider { get; set; }

    /// <summary>
    /// URL to brand logo image
    /// </summary>
    [MaxLength(500)]
    public string? BrandLogoUrl { get; set; }

    /// <summary>
    /// Indicates if background checks are required
    /// </summary>
    [Required]
    public bool RequireBackgroundCheck { get; set; }

    /// <summary>
    /// Ranking score of the organization
    /// </summary>
    [MaxLength(5)]
    public decimal? RankingScore { get; set; }
    /// <summary>
    /// Date when the record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when the record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}