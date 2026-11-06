using System.Data;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Microsoft.EntityFrameworkCore;
using Stripe;
using LocalPayout = Arcora.Api.Entities.Payout;

namespace Arcora.Api.Services.Implementations;

public sealed class HostRentPayoutService(
    ArcoraDbContext db, IOrgPayoutAccountService accounts, IHostPayoutProvider provider,
    IConfiguration configuration, ILogger<HostRentPayoutService> logger)
{
    public async Task<object> ListAsync(Guid organizationId, long actor, CancellationToken ct)
    {
        await accounts.GetOrganizationPayoutAccountStatus(organizationId, actor);
        var enabled = configuration.GetValue<bool>("HostPayouts:EnableTestPayouts") &&
            configuration["Stripe:SecretKey"]?.StartsWith("sk_test_", StringComparison.Ordinal) == true;
        var rows = await (from allocation in db.RentPaymentAccountings.AsNoTracking()
                          join disbursement in db.RentDisbursements.AsNoTracking()
                              on allocation.PaymentID equals disbursement.PaymentID into disbursements
                          from item in disbursements.DefaultIfEmpty()
                          where allocation.OrganizationID == organizationId && allocation.SettledAt != null
                          orderby allocation.SettledAt descending
                          select new
                          {
                              allocation.PaymentID, Amount = allocation.HostNetAmount, allocation.Currency,
                              Status = item == null ? "NOT_REQUESTED" : item.Status,
                              FailureReason = item == null ? null : item.FailureReason
                          }).ToListAsync(ct);
        return new { Enabled = enabled, Collections = rows };
    }

    public async Task<RentDisbursement> ProgressAsync(Guid paymentId, Guid organizationId, long actor,
        bool create, decimal? expectedAmount, CancellationToken ct)
    {
        await accounts.GetOrganizationPayoutAccountStatus(organizationId, actor, requireManageAccess: true);
        if (!configuration.GetValue<bool>("HostPayouts:EnableTestPayouts") ||
            configuration["Stripe:SecretKey"]?.StartsWith("sk_test_", StringComparison.Ordinal) != true)
            throw new InvalidOperationException("Host test payouts are disabled or Stripe is not in test mode.");
        await using (var reservation = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct))
        {
            if (db.Database.IsSqlServer())
                await db.Payments.FromSqlInterpolated(
                    $"SELECT * FROM [Payments] WITH (UPDLOCK, HOLDLOCK) WHERE [PaymentID] = {paymentId}")
                    .AsNoTracking().ToListAsync(ct);
            var allocation = await db.RentPaymentAccountings.AsNoTracking().SingleOrDefaultAsync(
                x => x.PaymentID == paymentId && x.OrganizationID == organizationId && x.SettledAt != null, ct)
                ?? throw new InvalidOperationException("A posted rent allocation for this organization is required.");
            var payment = await db.Payments.AsNoTracking().SingleAsync(x => x.PaymentID == paymentId, ct);
            if (payment.Status != "PAID" || payment.RefundedAmount > 0m ||
                await db.Refunds.AnyAsync(x => x.PaymentID == paymentId && x.Status != "FAILED" && x.Status != "CANCELLED", ct) ||
                await db.ChargeBacks.AnyAsync(x => x.PaymentID == paymentId, ct))
                throw new InvalidOperationException("Refunded, disputed or unpaid collections cannot be disbursed.");
            if (allocation.HostNetAmount <= 0m || allocation.Currency != "CAD")
                throw new InvalidOperationException("A positive CAD host net allocation is required.");
            var item = await db.RentDisbursements.SingleOrDefaultAsync(x => x.PaymentID == paymentId, ct);
            if (item == null)
            {
                if (!create)
                    throw new InvalidOperationException("No payout has been requested for this collection.");
                if (expectedAmount != allocation.HostNetAmount)
                    throw new InvalidOperationException("Confirm the exact current net rent amount before requesting payout.");
                if (await db.PayoutItems.AnyAsync(x => x.PaymentID == paymentId, ct))
                    throw new InvalidOperationException("This payment already belongs to a payout; reconcile it before proceeding.");
                var status = await accounts.RefreshOrganizationPayoutAccountStatus(organizationId, actor);
                if (status?.PayoutsEnabled != true || status.DetailsSubmitted != true)
                    throw new InvalidOperationException("Complete Stripe payout onboarding first.");
                var account = await db.OrgPayoutAccounts.AsNoTracking().SingleAsync(
                    x => x.OrganizationID == organizationId && x.IsActive &&
                        (x.StripeAccountID == status.StripeAccountID || x.ProviderAccountID == status.StripeAccountID), ct);
                var intent = await db.PaymentIntents.AsNoTracking().SingleAsync(
                    x => x.PaymentIntentID == allocation.PaymentIntentID, ct);
                if (string.IsNullOrWhiteSpace(intent.ProviderPaymentIntentID))
                    throw new InvalidOperationException("A provider charge reference is required.");
                var now = DateTime.UtcNow;
                var payout = new LocalPayout
                {
                    OrganizationID = organizationId, OrgPayoutAccountID = account.OrgPayoutAccountID,
                    Amount = allocation.HostNetAmount, Currency = allocation.Currency, Status = "REQUESTED",
                    RequestedAt = now, CapturedDate = now, CapturedBy = actor.ToString(), ProviderName = "Stripe"
                };
                db.Payouts.Add(payout);
                await db.SaveChangesAsync(ct);
                item = new RentDisbursement
                {
                    PaymentID = paymentId, OrganizationID = organizationId, OrgPayoutAccountID = account.OrgPayoutAccountID,
                    PayoutID = payout.PayoutID, AccountReference = status.StripeAccountID!,
                    PaymentIntentReference = intent.ProviderPaymentIntentID, Amount = allocation.HostNetAmount,
                    CollectedAmount = payment.GrossAmount, Currency = allocation.Currency,
                    CapturedDate = now, CapturedBy = actor.ToString()
                };
                db.RentDisbursements.Add(item);
                db.PayoutItems.Add(new PayoutItem
                {
                    PayoutID = payout.PayoutID, PaymentID = paymentId, GrossAmount = allocation.RentAmount,
                    DeductionAmount = allocation.HostPlacementFee + allocation.HostMonthlyFee,
                    NetPayoutAmount = allocation.HostNetAmount, Description = "Net rent only; deposit and tenant fees excluded",
                    CapturedDate = now, CapturedBy = actor.ToString()
                });
                await db.SaveChangesAsync(ct);
            }
            if (item.Status == "READY_FOR_PAYOUT" && !item.PayoutAttemptedAt.HasValue)
            {
                item.PayoutAttemptedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
            }
            await reservation.CommitAsync(ct);
        }

        db.ChangeTracker.Clear();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        if (db.Database.IsSqlServer())
            await db.RentDisbursements.FromSqlInterpolated(
                $"SELECT * FROM [RentDisbursements] WITH (UPDLOCK, HOLDLOCK) WHERE [PaymentID] = {paymentId}")
                .AsNoTracking().ToListAsync(ct);
        var saved = await db.RentDisbursements.SingleAsync(x => x.PaymentID == paymentId, ct);
        var request = new HostPayoutRequest($"arcora_rent_{paymentId:N}", saved.AccountReference,
            saved.PaymentIntentReference, Minor(saved.Amount), Minor(saved.CollectedAmount), saved.Currency.ToLowerInvariant());
        var allowTransfer = DateTime.UtcNow - saved.CapturedDate < TimeSpan.FromHours(23);
        try
        {
            var allowPayout = saved.PayoutAttemptedAt.HasValue &&
                DateTime.UtcNow - saved.PayoutAttemptedAt.Value < TimeSpan.FromHours(23);
            var result = await provider.ProgressAsync(request, allowTransfer, allowPayout, ct);
            if (result.Status == "READY_FOR_PAYOUT" && saved.PayoutAttemptedAt.HasValue && !allowPayout)
                result = result with { Status = "REVIEW_REQUIRED", FailureReason = "The safe payout retry window has expired; contact support." };
            saved.TransferReference = result.TransferId;
            saved.BankPayoutReference = result.PayoutId;
            saved.Status = result.Status;
            saved.FailureReason = result.FailureReason;
            saved.LastCheckedAt = DateTime.UtcNow;
            await UpdatePayoutAsync(saved, ct);
            await transaction.CommitAsync(ct);
            return saved;
        }
        catch (Exception ex) when (ex is StripeException or HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "Host payout provider outcome needs reconciliation for payment {PaymentID}", paymentId);
            saved.Status = "PROVIDER_UNCERTAIN";
            saved.FailureReason = "Provider outcome is uncertain. Refresh this payout; do not create another payout.";
            saved.LastCheckedAt = DateTime.UtcNow;
            await UpdatePayoutAsync(saved, CancellationToken.None);
            await transaction.CommitAsync(CancellationToken.None);
            throw;
        }
    }

    private static long Minor(decimal amount) => checked((long)(amount * 100m));

    private async Task UpdatePayoutAsync(RentDisbursement saved, CancellationToken ct)
    {
        var payout = await db.Payouts.SingleAsync(x => x.PayoutID == saved.PayoutID, ct);
        payout.Status = saved.Status;
        payout.ProviderPayoutID = saved.BankPayoutReference;
        payout.FailureReason = saved.FailureReason;
        payout.ProcessedAt = saved.LastCheckedAt;
        if (saved.Status == "PAID")
            payout.PaidAt ??= saved.LastCheckedAt;
        await db.SaveChangesAsync(ct);
    }
}
