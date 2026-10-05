namespace Arcora.Api.DTOs;

public class ListingEarningsBreakdownDto
{
    public Guid ListingID { get; set; }
    public string ListingName { get; set; } = string.Empty;
    public decimal RentCollected { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal NetEarnings { get; set; }
}
