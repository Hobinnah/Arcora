// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class ListingImportResultDto
{
    public Guid? AddressID { get; set; }
    public Guid? PropertyID { get; set; }
    public List<UnitImportResultItem> Units { get; set; } = new List<UnitImportResultItem>();
}

public class UnitImportResultItem
{
    public string? UnitNumber { get; set; }
    public Guid? RentalUnitID { get; set; }
    public Guid? ListingID { get; set; }
}
