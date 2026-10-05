namespace Arcora.Api.DTOs;

public class EarningsTransactionDto
{
    public DateTime TransactionDate { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public long? PayoutID { get; set; }
    public Guid? PaymentID { get; set; }
    public Guid? InvoiceMasterID { get; set; }
}
