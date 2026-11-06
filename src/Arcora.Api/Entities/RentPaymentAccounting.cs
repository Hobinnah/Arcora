using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

public sealed class RentPaymentAccounting
{
    [Key]
    public Guid PaymentIntentID { get; set; }
    public Guid? PaymentID { get; set; }
    public Guid LeaseID { get; set; }
    public Guid OrganizationID { get; set; }
    public Guid ListingID { get; set; }
    public Guid RentalUnitID { get; set; }
    public DateTime RentPeriod { get; set; }
    [MaxLength(3)]
    public string Currency { get; set; } = "CAD";
    public decimal RentAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal TenantFeeAmount { get; set; }
    public decimal PlacementFeeRate { get; set; }
    public decimal MonthlyFeeRate { get; set; }
    public decimal HostPlacementFee { get; set; }
    public decimal HostMonthlyFee { get; set; }
    public decimal HostNetAmount { get; set; }
    public DateTime CapturedDate { get; set; }
    public DateTime? SettledAt { get; set; }
    [MaxLength(256)]
    public string? ReconciliationBy { get; set; }
    [ForeignKey(nameof(PaymentIntentID))]
    public PaymentIntent? PaymentIntent { get; set; }
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }
}
