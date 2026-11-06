using System.Net;
using System.Text;
using System.Text.Json;
using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Arcora.Payments.Stripe;
using Microsoft.Extensions.Options;
using Stripe;

namespace Arcora.Api.Tests;

public sealed class StripeHostPayoutTests
{
    private static readonly HostPayoutRequest Request = new(
        "arcora_rent_synthetic", "acct_synthetic", "pi_synthetic", 137502, 237499, "cad");

    [Fact]
    public async Task TransferUsesSourceChargeAndNetAmountThenWaitsForAvailableFunds()
    {
        using var http = new ProviderHttp();
        var provider = Provider(http);
        var first = await provider.ProgressAsync(Request, true, false);
        Assert.Equal("WAITING_FUNDS", first.Status);
        Assert.Equal("tr_synthetic", first.TransferId);
        Assert.Null(first.PayoutId);
        Assert.Equal(1, http.TransferCreates);
        Assert.Equal(0, http.PayoutCreates);
        Assert.Contains("amount=137502", http.TransferBody);
        Assert.Contains("source_transaction=py_synthetic", http.TransferBody);
        Assert.Contains("destination=acct_synthetic", http.TransferBody);
        Assert.Equal("arcora_rent_synthetic:transfer", http.TransferKey);
        await provider.ProgressAsync(Request, true, false);
        Assert.Equal(1, http.TransferCreates);
    }

    [Fact]
    public async Task AvailableFundsRequireExplicitBankAttemptAndProviderStatusIsRefreshed()
    {
        using var http = new ProviderHttp { Available = true };
        var provider = Provider(http);
        Assert.Equal("READY_FOR_PAYOUT", (await provider.ProgressAsync(Request, true, false)).Status);
        Assert.Equal(0, http.PayoutCreates);
        Assert.Equal("PENDING", (await provider.ProgressAsync(Request, true, true)).Status);
        Assert.Equal(1, http.TransferCreates);
        Assert.Equal(1, http.PayoutCreates);
        Assert.Equal("arcora_rent_synthetic:payout", http.PayoutKey);
        Assert.Equal("acct_synthetic", http.PayoutAccount);
        Assert.Contains("amount=137502", http.PayoutBody);
        http.PayoutStatus = "paid";
        Assert.Equal("PAID", (await provider.ProgressAsync(Request, false, false)).Status);
        http.PayoutStatus = "failed";
        Assert.Equal("FAILED", (await provider.ProgressAsync(Request, false, false)).Status);
        Assert.Equal(1, http.PayoutCreates);
    }

    [Fact]
    public async Task ExpiredUnknownTransferIsNeverCreatedAndInsufficientBalanceNeverPays()
    {
        using var http = new ProviderHttp { Available = true, Balance = 0 };
        var provider = Provider(http);
        Assert.Equal("REVIEW_REQUIRED", (await provider.ProgressAsync(Request, false, false)).Status);
        Assert.Equal(0, http.TransferCreates);
        Assert.Equal("WAITING_FUNDS", (await provider.ProgressAsync(Request, true, true)).Status);
        Assert.Equal(0, http.PayoutCreates);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public async Task LiveRefundedOrDisputedChargeNeverTransfers(bool live, bool refund, bool dispute)
    {
        using var http = new ProviderHttp { Live = live, Refunded = refund, Disputed = dispute };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Provider(http).ProgressAsync(Request, true, true));
        Assert.Equal(0, http.TransferCreates);
        Assert.Equal(0, http.PayoutCreates);
    }

    [Fact]
    public async Task LiveSecretAndMismatchedSourceAmountAreRejected()
    {
        using var http = new ProviderHttp();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Provider(http, "sk_live_synthetic").ProgressAsync(Request, true, true));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Provider(http).ProgressAsync(Request with { CollectedAmount = 99999 }, true, true));
        Assert.Equal(0, http.TransferCreates);
    }

    private static StripeHostPayoutProvider Provider(ProviderHttp handler, string key = "sk_test_synthetic") =>
        new(Options.Create(new StripeOptions { SecretKey = key }),
            new StripeClient(key, httpClient: new SystemNetHttpClient(new HttpClient(handler))));

    private sealed class ProviderHttp : HttpMessageHandler
    {
        public bool Available { get; set; }
        public bool Live { get; set; }
        public bool Refunded { get; set; }
        public bool Disputed { get; set; }
        public long Balance { get; set; } = 137502;
        public int TransferCreates { get; private set; }
        public int PayoutCreates { get; private set; }
        public string PayoutStatus { get; set; } = "pending";
        public string TransferBody { get; private set; } = "";
        public string PayoutBody { get; private set; } = "";
        public string TransferKey { get; private set; } = "";
        public string PayoutKey { get; private set; } = "";
        public string PayoutAccount { get; private set; } = "";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            object payload;
            var path = request.RequestUri!.AbsolutePath;
            var transfer = new
            {
                id = "tr_synthetic", @object = "transfer", livemode = false, amount = 137502,
                currency = "cad", destination = "acct_synthetic", source_transaction = "py_synthetic",
                transfer_group = Request.Reference, reversed = false, amount_reversed = 0
            };
            var payout = new
            {
                id = "po_synthetic", @object = "payout", livemode = false, amount = 137502,
                currency = "cad", status = PayoutStatus,
                metadata = new Dictionary<string, string> { ["arcora_rent_reference"] = Request.Reference }
            };
            if (path == "/v1/accounts/acct_synthetic")
                payload = new
                {
                    id = "acct_synthetic", @object = "account", payouts_enabled = true, details_submitted = true,
                    capabilities = new { transfers = "active" }, settings = new { payouts = new { schedule = new { interval = "manual" } } }
                };
            else if (path == "/v1/payment_intents/pi_synthetic")
                payload = new
                {
                    id = "pi_synthetic", @object = "payment_intent", livemode = Live,
                    status = "succeeded", amount = 237499, currency = "cad",
                    latest_charge = new
                    {
                        id = "py_synthetic", @object = "charge", paid = true, disputed = Disputed,
                        amount_refunded = Refunded ? 1 : 0,
                        balance_transaction = new { id = "txn_synthetic", @object = "balance_transaction", status = Available ? "available" : "pending" }
                    }
                };
            else if (path == "/v1/transfers" && request.Method == HttpMethod.Get)
                payload = new { @object = "list", has_more = false, data = TransferCreates > 0 ? new[] { transfer } : [] };
            else if (path == "/v1/transfers")
            {
                TransferCreates++;
                TransferBody = await request.Content!.ReadAsStringAsync(ct);
                TransferKey = request.Headers.GetValues("Idempotency-Key").Single();
                payload = transfer;
            }
            else if (path == "/v1/balance")
                payload = new { @object = "balance", available = new[] { new { amount = Balance, currency = "cad" } }, pending = Array.Empty<object>() };
            else if (path == "/v1/payouts" && request.Method == HttpMethod.Get)
                payload = new { @object = "list", has_more = false, data = PayoutCreates > 0 ? new[] { payout } : [] };
            else if (path == "/v1/payouts")
            {
                PayoutCreates++;
                PayoutBody = await request.Content!.ReadAsStringAsync(ct);
                PayoutKey = request.Headers.GetValues("Idempotency-Key").Single();
                PayoutAccount = request.Headers.GetValues("Stripe-Account").Single();
                payload = payout;
            }
            else
                throw new InvalidOperationException($"Unexpected provider request: {path}");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
        }
    }
}
