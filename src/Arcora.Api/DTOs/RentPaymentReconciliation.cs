namespace Arcora.Api.DTOs;

public sealed record RentPaymentReconciliation(
    decimal RentAmount, decimal DepositAmount, decimal TenantFeeAmount,
    decimal PlacementFeeRate, decimal MonthlyFeeRate);
