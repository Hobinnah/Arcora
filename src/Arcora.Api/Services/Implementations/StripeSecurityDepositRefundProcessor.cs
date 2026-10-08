using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Configuration;
using Microsoft.Extensions.Options;
using Stripe;

namespace Arcora.Api.Services.Implementations;

public sealed class StripeSecurityDepositRefundProcessor : ISecurityDepositRefundProcessor
{
    private readonly Stripe.RefundService refundService;

    public StripeSecurityDepositRefundProcessor(IOptions<StripeOptions> stripeOptions)
    {
        refundService = new Stripe.RefundService(new StripeClient(stripeOptions.Value.SecretKey));
    }

    public async Task<SecurityDepositProcessorRefund> CreateRefundAsync(
        string providerPaymentReference,
        long amountInMinorUnits,
        Guid securityDepositID,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var refundOptions = new RefundCreateOptions
        {
            Amount = amountInMinorUnits,
            Metadata = new Dictionary<string, string>
            {
                ["security_deposit_id"] = securityDepositID.ToString(),
                ["idempotency_key"] = idempotencyKey
            }
        };
        if (providerPaymentReference.StartsWith("pi_", StringComparison.Ordinal))
            refundOptions.PaymentIntent = providerPaymentReference;
        else if (providerPaymentReference.StartsWith("ch_", StringComparison.Ordinal))
            refundOptions.Charge = providerPaymentReference;
        else
            throw new ArgumentException("The linked Stripe payment reference is invalid.", nameof(providerPaymentReference));

        var refund = await refundService.CreateAsync(
            refundOptions,
            new RequestOptions { IdempotencyKey = idempotencyKey },
            cancellationToken);

        return new SecurityDepositProcessorRefund(refund.Id, refund.Status, refund.FailureReason);
    }
}