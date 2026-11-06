namespace Arcora.Api.DTOs;

public sealed record PublicListingLocation(decimal Latitude, decimal Longitude);
public sealed record GeocodedAddressLocationDto(decimal Latitude, decimal Longitude);
public sealed record ListingMapPinDto(Guid ListingID, string? Title, decimal Latitude, decimal Longitude,
    decimal Price, string? Currency);
public sealed record ListingMapResultDto(IReadOnlyList<ListingMapPinDto> Data, int TotalCount, int MissingLocationCount);
