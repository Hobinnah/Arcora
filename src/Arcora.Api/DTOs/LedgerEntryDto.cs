// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Details individual debit and credit entries for ledger transactions with amounts and descriptions.
/// </summary>
public class LedgerEntryDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? LedgerEntryID { get; set; }

    /// <summary>
    /// FK to LedgerTransaction
    /// </summary>
    [Required]
    public Guid LedgerTransactionID { get; set; }

    /// <summary>
    /// FK to LedgerAccount
    /// </summary>
    [Required]
    public Guid LedgerAccountID { get; set; }

    /// <summary>
    /// Debit amount
    /// </summary>
    [Required]
    public decimal DebitAmount { get; set; } = 0;

    /// <summary>
    /// Credit amount
    /// </summary>
    [Required]
    public decimal CreditAmount { get; set; } = 0;

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Entry description
    /// </summary>
    [MaxLength(255)]
    public string? Description { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to LedgerTransaction
    /// </summary>
    public LedgerTransactionDto? LedgerTransaction { get; set; }
    /// <summary>
    /// FK to LedgerAccount
    /// </summary>
    public LedgerAccountDto? LedgerAccount { get; set; }
}