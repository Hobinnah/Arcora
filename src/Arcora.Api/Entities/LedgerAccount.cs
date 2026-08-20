// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents ledger accounts used for financial transactions with categorization and status.
/// </summary>
[Table("LedgerAccount")]
public class LedgerAccount
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LedgerAccountID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Account code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? AccountCode { get; set; }

    /// <summary>
    /// Type of account
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? AccountType { get; set; }

    /// <summary>
    /// Category of account
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? AccountCategory { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Currency { get; set; }

    /// <summary>
    /// Account name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Indicates if system account
    /// </summary>
    [Required]
    public bool IsSystemAccount { get; set; }

    /// <summary>
    /// Indicates if account is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Date record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}