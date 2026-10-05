namespace Arcora.Api.DTOs;

public class PayoutAccountStatusDto
{
    public Guid OrganizationID { get; set; }
    public bool IsConnected { get; set; }
    public string? Provider { get; set; }
    public string? StripeAccountID { get; set; }
    public string? VerificationStatus { get; set; }
    public string? MaskedAccount { get; set; }
    public string? BankName { get; set; }
    public string? AccountType { get; set; }
    public bool IsDefault { get; set; }
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public bool DetailsSubmitted { get; set; }
    public List<string> RequirementsCurrentlyDue { get; set; } = new();
    public List<string> RequirementsEventuallyDue { get; set; } = new();
}
