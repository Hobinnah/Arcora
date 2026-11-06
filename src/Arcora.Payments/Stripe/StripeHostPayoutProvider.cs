using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Microsoft.Extensions.Options;
using Stripe;

namespace Arcora.Payments.Stripe;

public sealed class StripeHostPayoutProvider(IOptions<StripeOptions> options, IStripeClient? stripeClient = null) : IHostPayoutProvider
{
    public async Task<HostPayoutResult> ProgressAsync(HostPayoutRequest request, bool allowTransferCreate, bool allowPayoutCreate,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.SecretKey.StartsWith("sk_test_", StringComparison.Ordinal))
            throw new InvalidOperationException("Host test payouts require a Stripe test secret.");
        if (request.Amount <= 0 || request.Amount > request.CollectedAmount || request.Currency != "cad")
            throw new InvalidOperationException("A positive CAD payout within the collected amount is required.");
        var client = stripeClient ?? new StripeClient(options.Value.SecretKey);
        var account = await new AccountService(client).GetAsync(request.AccountId, cancellationToken: cancellationToken);
        if (!account.PayoutsEnabled || account.Capabilities?.Transfers != "active" || !account.DetailsSubmitted)
            throw new InvalidOperationException("Complete the host's Stripe payout verification first.");
        var intent = await new PaymentIntentService(client).GetAsync(request.PaymentIntentId,
            new PaymentIntentGetOptions { Expand = ["latest_charge.balance_transaction"] }, cancellationToken: cancellationToken);
        var charge = intent.LatestCharge;
        if (intent.Livemode || intent.Status != "succeeded" || intent.Currency != request.Currency ||
            intent.Amount != request.CollectedAmount || charge == null || !charge.Paid || charge.AmountRefunded != 0 ||
            charge.Disputed)
            throw new InvalidOperationException("Only a matching, successful, unrefunded test charge can fund this payout.");
        var transfers = new TransferService(client);
        var matches = await transfers.ListAsync(new TransferListOptions
        {
            TransferGroup = request.Reference, Limit = 2
        }, cancellationToken: cancellationToken);
        if (matches.HasMore || matches.Data.Count > 1)
            throw new InvalidOperationException("Multiple provider transfers require reconciliation.");
        var transfer = matches.Data.SingleOrDefault();
        if (account.Settings?.Payouts?.Schedule?.Interval != "manual")
        {
            if (transfer != null || !allowTransferCreate)
                throw new InvalidOperationException("This test account is not configured for manual payouts. Review its automatic payout history before proceeding.");
            await new AccountService(client).UpdateAsync(request.AccountId, new AccountUpdateOptions
            {
                Settings = new AccountSettingsOptions
                {
                    Payouts = new AccountSettingsPayoutsOptions
                    {
                        Schedule = new AccountSettingsPayoutsScheduleOptions { Interval = "manual" }
                    }
                }
            }, new RequestOptions { IdempotencyKey = $"{request.Reference}:manual-schedule" }, cancellationToken);
        }
        if (transfer == null && allowTransferCreate)
            transfer = await transfers.CreateAsync(new TransferCreateOptions
            {
                Amount = request.Amount, Currency = request.Currency, Destination = request.AccountId,
                SourceTransaction = charge.Id, TransferGroup = request.Reference,
                Metadata = new() { ["arcora_rent_reference"] = request.Reference }
            }, new RequestOptions { IdempotencyKey = $"{request.Reference}:transfer" }, cancellationToken);
        if (transfer == null)
            return new(null, null, "REVIEW_REQUIRED", "No transfer found. The safe retry window has expired; contact support.");
        if (transfer.Livemode || transfer.Amount != request.Amount || transfer.Currency != request.Currency ||
            transfer.DestinationId != request.AccountId || transfer.SourceTransactionId != charge.Id)
            throw new InvalidOperationException("The provider transfer does not match the saved host allocation.");
        if (transfer.Reversed || transfer.AmountReversed > 0)
            return new(transfer.Id, null, "REVERSED", "The transfer has been reversed; accounting review is required.");

        var payoutService = new PayoutService(client);
        var connected = new RequestOptions { StripeAccount = request.AccountId };
        Payout? payout = null;
        await foreach (var candidate in payoutService.ListAutoPagingAsync(
            new PayoutListOptions { Limit = 100 }, connected, cancellationToken))
        {
            if (candidate.Metadata.GetValueOrDefault("arcora_rent_reference") != request.Reference)
                continue;
            if (payout != null)
                throw new InvalidOperationException("Multiple provider payouts require reconciliation.");
            payout = candidate;
        }
        if (payout == null)
        {
            if (charge.BalanceTransaction?.Status != "available")
                return new(transfer.Id, null, "WAITING_FUNDS", "Stripe has not yet made this bank debit available for payout.");
            var balance = await new BalanceService(client).GetAsync(requestOptions: connected, cancellationToken: cancellationToken);
            if ((balance.Available.Where(x => x.Currency == request.Currency).Sum(x => x.Amount)) < request.Amount)
                return new(transfer.Id, null, "WAITING_FUNDS", "The connected account has insufficient available CAD funds. Automatic payouts may already have moved funds; review Stripe before retrying.");
            if (!allowPayoutCreate)
                return new(transfer.Id, null, "READY_FOR_PAYOUT");
            payout = await payoutService.CreateAsync(new PayoutCreateOptions
            {
                Amount = request.Amount, Currency = request.Currency,
                Metadata = new() { ["arcora_rent_reference"] = request.Reference }
            }, new RequestOptions { StripeAccount = request.AccountId, IdempotencyKey = $"{request.Reference}:payout" },
                cancellationToken);
        }
        if (payout.Livemode || payout.Amount != request.Amount || payout.Currency != request.Currency)
            throw new InvalidOperationException("The provider bank payout does not match the saved allocation.");
        return new(transfer.Id, payout.Id, payout.Status.ToUpperInvariant(), payout.FailureMessage);
    }
}
