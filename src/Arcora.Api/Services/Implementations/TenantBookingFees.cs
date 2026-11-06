using Arcora.Api.Entities;

namespace Arcora.Api.Services.Implementations;

public static class TenantBookingFees
{
    public static decimal Calculate(IEnumerable<Fee> fees, decimal rent)
    {
        return fees.Where(IsTenantFee)
            .Sum(fee => CalculateAmount(fee, rent));
    }

    public static bool IsTenantFee(Fee fee) =>
        !string.Equals(fee.Code, "PLATFORM-HOST-PLACEMENT", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(fee.Code, "PLATFORM-HOST-UNIT-SUBSCRIPTION", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(fee.FeeType?.Name, "Host Placement Fee", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(fee.FeeType?.Name, "Host Active Unit Subscription Fee", StringComparison.OrdinalIgnoreCase);

    public static decimal CalculateAmount(Fee fee, decimal rent)
    {
        var amount = fee.CalculationType?.Trim().ToUpperInvariant() == "PERCENTAGE"
            ? rent * (fee.PercentageRate ?? 0m) / 100m
            : fee.FixedAmount ?? 0m;
        if (fee.MinimumFeeAmount.HasValue)
            amount = Math.Max(amount, fee.MinimumFeeAmount.Value);
        if (fee.MaximumFeeAmount.HasValue)
            amount = Math.Min(amount, fee.MaximumFeeAmount.Value);
        return Math.Round(Math.Max(0m, amount), 2, MidpointRounding.AwayFromZero);
    }
}
