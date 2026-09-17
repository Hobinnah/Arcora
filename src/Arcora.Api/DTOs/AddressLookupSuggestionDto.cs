namespace Arcora.Api.DTOs;

public class AddressLookupSuggestionDto
{
    public string? Id { get; set; }
    public string? Text { get; set; }
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? ProvinceCode { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
}
