using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services.Implementations;

public sealed class LeaseFirstPaymentService(
    ArcoraDbContext db,
    IRentCollectionOrchestrator orchestrator,
    IPaymentOnboardingService onboarding,
    IPaymentProvider provider) : ILeaseFirstPaymentService
{
    private static string Key(Lease lease) => $"pi_firstcharge_lease_{lease.LeaseID:N}";

    private Task<PaymentIntent?> FindIntentAsync(Lease lease, CancellationToken ct) =>
        db.PaymentIntents.AsNoTracking().SingleOrDefaultAsync(
            intent => intent.LeaseID == lease.LeaseID && intent.IdempotencyKey == Key(lease), ct);

    private Task<bool> IsSignedAsync(Lease lease, CancellationToken ct) =>
        db.LeaseDocuments.AnyAsync(document =>
            document.LeaseID == lease.LeaseID && document.DocumentType == "LEASE_AGREEMENT" &&
            document.DocumentStatus == "FULLY_SIGNED" && document.FullySignedAt != null, ct);

    public async Task<LeaseFirstPaymentDto> GetAsync(Lease lease, CancellationToken cancellationToken = default)
    {
        var savedIntent = await FindIntentAsync(lease, cancellationToken);
        if (savedIntent != null)
        {
            var allocation = await db.RentPaymentAccountings.AsNoTracking().SingleOrDefaultAsync(
                x => x.PaymentIntentID == savedIntent.PaymentIntentID, cancellationToken);
            if (allocation != null)
                return new(allocation.RentAmount, allocation.DepositAmount, allocation.TenantFeeAmount,
                    savedIntent.Amount, allocation.Currency, savedIntent.Status ?? "UNAVAILABLE",
                    savedIntent.PaymentIntentID, savedIntent.FailureReason);
        }
        var deposit = await db.SecurityDeposits.AsNoTracking()
            .Where(item => item.LeaseID == lease.LeaseID)
            .OrderByDescending(item => item.CapturedDate)
            .Select(item => item.RequiredAmount).FirstOrDefaultAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var fees = await db.Fees.AsNoTracking().Include(fee => fee.FeeType).Where(fee =>
            fee.IsActive && fee.FeeType != null && fee.FeeType.IsPlatformFee == true &&
            (fee.OrganizationID == null || fee.OrganizationID == lease.OrganizationID) &&
            (fee.EffectiveFrom == null || fee.EffectiveFrom <= now) &&
            (fee.EffectiveTo == null || fee.EffectiveTo >= now)).ToListAsync(cancellationToken);
        var tenantFees = fees.Where(TenantBookingFees.IsTenantFee).ToList();
        if (tenantFees.Any(fee => !string.Equals(fee.Currency, lease.Currency, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("The first-payment fees must use the lease currency.");
        var feeAmount = TenantBookingFees.Calculate(tenantFees, lease.BaseRentAmount);
        var intent = savedIntent;
        var ready = lease.Status == "PENDING_FIRST_PAYMENT" && lease.SignedAt.HasValue &&
            await IsSignedAsync(lease, cancellationToken);
        return new(lease.BaseRentAmount, deposit, feeAmount,
            intent?.Amount ?? lease.BaseRentAmount + deposit + feeAmount,
            lease.Currency ?? "CAD", intent?.Status ?? (ready ? "READY" : lease.Status ?? "UNAVAILABLE"),
            intent?.PaymentIntentID, intent?.FailureReason);
    }

    public async Task<LeaseFirstPaymentDto> CollectAsync(
        Lease lease, decimal? expectedAmount = null, CancellationToken cancellationToken = default)
    {
        var quote = await GetAsync(lease, cancellationToken);
        // Never retry an uncertain or failed debit by creating a second local/provider intent.
        if (quote.PaymentIntentID.HasValue)
            return quote;
        if (quote.Status != "READY")
            throw new InvalidOperationException("All lease signatures must be complete before the first payment.");
        if (quote.Amount <= 0m || lease.BaseRentAmount <= 0m || quote.Currency != "CAD")
            throw new InvalidOperationException("A positive CAD first-payment amount is required.");
        if (expectedAmount.HasValue && expectedAmount.Value != quote.Amount)
            throw new InvalidOperationException("The payment amount has changed. Refresh the breakdown before continuing.");
        if (!(await onboarding.GetStatusAsync(lease.TenantID, cancellationToken)).IsReady)
            throw new InvalidOperationException("Verify your bank, backup card and active PAD mandate before collecting payment.");
        await orchestrator.InitiateCollectionAsync(lease.LeaseID, lease.TenantID, Guid.Empty, quote.Amount,
            Key(lease), cancellationToken);
        return await GetAsync(lease, cancellationToken);
    }

    public async Task<LeaseFirstPaymentDto> RefreshAsync(Lease lease, CancellationToken cancellationToken = default)
    {
        var intent = await FindIntentAsync(lease, cancellationToken);
        if (intent == null)
            throw new InvalidOperationException("No first payment has been initiated.");
        if (string.IsNullOrWhiteSpace(intent.ProviderPaymentIntentID))
            throw new InvalidOperationException("The provider has not confirmed the debit reference. Do not start another payment; contact support.");
        var result = await provider.GetChargeStatusAsync(intent.ProviderPaymentIntentID, cancellationToken);
        if (string.IsNullOrWhiteSpace(result.Status))
            throw new InvalidOperationException("The provider returned no payment status.");
        await orchestrator.HandlePadResultAsync(intent.PaymentIntentID, result.Status, result.FailureCode,
            result.FailureMessage, cancellationToken, intent.ProviderPaymentIntentID);
        return await GetAsync(lease, cancellationToken);
    }
}
