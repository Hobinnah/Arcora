using Arcora.Api.DTOs;

namespace Arcora.Api.Helpers;

public static class PublicMapLocation
{
    public static PublicListingLocation? From(decimal? latitude, decimal? longitude)
    {
        if (latitude is not decimal lat || longitude is not decimal lon ||
            lat < -90 || lat > 90 || lon < -180 || lon > 180)
            return null;
        // A fixed cell center cannot be averaged across searches to reconstruct a street location.
        return new(Center(lat, 90), Center(lon, 180));
    }

    private static decimal Center(decimal value, decimal maximum) =>
        decimal.Floor(decimal.Min(value, maximum - 0.001m) * 100m) / 100m + 0.005m;
}
