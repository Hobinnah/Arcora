namespace Arcora.Api.DTOs;

public class EarningsSummaryDto
{
    public Guid OrganizationID { get; set; }
    public string Period { get; set; } = "all";
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public decimal RentCollected { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal PastDue { get; set; }
    public decimal NetEarnings { get; set; }
}
