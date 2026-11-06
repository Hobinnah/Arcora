using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Entities;

public sealed class RentDisbursement
{
    [Key]
    public Guid PaymentID { get; set; }
    public Guid OrganizationID { get; set; }
    public long OrgPayoutAccountID { get; set; }
    public long PayoutID { get; set; }
    [MaxLength(255)]
    public string AccountReference { get; set; } = "";
    [MaxLength(255)]
    public string PaymentIntentReference { get; set; } = "";
    [MaxLength(255)]
    public string? TransferReference { get; set; }
    [MaxLength(255)]
    public string? BankPayoutReference { get; set; }
    [MaxLength(50)]
    public string Status { get; set; } = "REQUESTED";
    [MaxLength(1000)]
    public string? FailureReason { get; set; }
    public decimal Amount { get; set; }
    public decimal CollectedAmount { get; set; }
    [MaxLength(3)]
    public string Currency { get; set; } = "CAD";
    public DateTime CapturedDate { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public DateTime? PayoutAttemptedAt { get; set; }
    [MaxLength(256)]
    public string CapturedBy { get; set; } = "";
}
