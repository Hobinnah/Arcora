// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores extracted lease terms from lease documents including dates, amounts, and extraction status.
/// </summary>
[Table("LeaseDocExtractedTerm")]
public class LeaseDocExtractedTerm
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LeaseDocExtractedTermID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public Guid LeaseDocumentID { get; set; }
    /// <summary>
    /// Extracted start date
    /// </summary>
    public DateTime? ExtractedStartDate { get; set; }
    /// <summary>
    /// Extracted end date
    /// </summary>
    public DateTime? ExtractedEndDate { get; set; }
    /// <summary>
    /// Extracted rent amount
    /// </summary>
    public decimal? ExtractedRentAmount { get; set; }
    /// <summary>
    /// Extracted deposit amount
    /// </summary>
    public decimal? ExtractedDepositAmount { get; set; }
    /// <summary>
    /// Extracted renewal date
    /// </summary>
    public DateTime? ExtractedRenewalDate { get; set; }
    /// <summary>
    /// Confidence level of extraction
    /// </summary>
    public decimal? ExtractionConfidence { get; set; }

    /// <summary>
    /// Status of extraction
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ExtractionStatus { get; set; } = "PENDING";

    /// <summary>
    /// Raw extraction JSON data
    /// </summary>
    [MaxLength(256)]
    public string? RawExtractionJson { get; set; }
    /// <summary>
    /// Date when extraction was done
    /// </summary>
    public DateTime? ExtractedDate { get; set; }
    /// <summary>
    /// Date when extraction was reviewed
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// User who reviewed the extraction
    /// </summary>
    [MaxLength(100)]
    public string? ReviewedBy { get; set; }
}