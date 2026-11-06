using System.Linq.Expressions;
using System.Reflection;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Implementations;
using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class PaymentOnboardingStatusTests
{
    [Fact]
    public async Task PendingBankExposesHostedVerificationWithoutApprovingTheMethodOrMandate()
    {
        var fixture = new Fixture("requires_action");
        var status = await fixture.Service.GetStatusAsync(fixture.TenantID);
        Assert.False(status.IsReady);
        Assert.False(status.HasVerifiedPad);
        Assert.True(status.HasVerifiedCard);
        Assert.False(status.PadMandateActive);
        Assert.Equal("https://verify.stripe.com/test-fixture", status.HostedBankVerificationUrl);
        Assert.Equal("PENDING", fixture.Mandate.Status);
    }

    [Fact]
    public async Task RefreshReconcilesSuccessfulProviderVerificationAndActivatesTheSavedMandate()
    {
        var fixture = new Fixture("succeeded");
        var status = await fixture.Service.GetStatusAsync(fixture.TenantID);
        Assert.True(status.IsReady);
        Assert.Equal("VERIFIED", fixture.Bank.VerificationStatus);
        Assert.Equal("ACTIVE", fixture.Mandate.Status);
        Assert.NotNull(fixture.Bank.VerifiedAt);
        Assert.NotNull(fixture.Mandate.ActivatedAt);
        Assert.Null(status.HostedBankVerificationUrl);
        fixture.Provider.InvokeHandler = (_, _) => throw new InvalidOperationException("Verified methods must not be re-confirmed.");
        Assert.True((await fixture.Service.GetStatusAsync(fixture.TenantID)).IsReady);
    }

    [Fact]
    public async Task FailedProviderVerificationCannotMakeTheTenantReady()
    {
        var fixture = new Fixture("requires_payment_method");
        Assert.False((await fixture.Service.GetStatusAsync(fixture.TenantID)).IsReady);
        Assert.Equal("FAILED", fixture.Bank.VerificationStatus);
        Assert.Equal("PENDING", fixture.Mandate.Status);
    }

    [Fact]
    public async Task ProviderReadFailuresPropagateWithoutChangingStoredVerification()
    {
        var fixture = new Fixture("requires_action");
        fixture.Provider.InvokeHandler = (_, _) => throw new InvalidOperationException("Provider unavailable.");
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.GetStatusAsync(fixture.TenantID));
        Assert.Equal("PENDING", fixture.Bank.VerificationStatus);
        Assert.Equal("PENDING", fixture.Mandate.Status);
    }

    private sealed class Fixture
    {
        public Guid TenantID { get; } = Guid.NewGuid();
        public PaymentMethod Bank { get; }
        public AutopayMandate Mandate { get; }
        public InvocationProxy Provider { get; }
        public PaymentOnboardingService Service { get; }

        public Fixture(string providerStatus)
        {
            Bank = new PaymentMethod
            {
                PaymentMethodID = Guid.NewGuid(), TenantID = TenantID, MethodRole = "PRIMARY",
                VerificationStatus = "PENDING", IsActive = true, ProviderName = "STRIPE",
                ProviderCustomerID = "cus_fixture", ProviderPaymentMethodID = "pm_fixture"
            };
            Mandate = new AutopayMandate
            {
                AutopayMandateID = Guid.NewGuid(), TenantID = TenantID,
                PaymentMethodID = Bank.PaymentMethodID, Status = "PENDING"
            };
            var card = new PaymentMethod
            {
                PaymentMethodID = Guid.NewGuid(), TenantID = TenantID,
                MethodRole = "BACKUP", VerificationStatus = "VERIFIED", IsActive = true
            };
            var provider = DispatchProxy.Create<IPaymentProvider, InvocationProxy>();
            Provider = (InvocationProxy)(object)provider;
            Provider.InvokeHandler = (method, args) =>
            {
                Assert.Equal(nameof(IPaymentProvider.GetPadSetupStatusAsync), method.Name);
                Assert.Equal("cus_fixture", args[0]);
                Assert.Equal("pm_fixture", args[1]);
                return Task.FromResult(new ProviderMandateResult("mandate_fixture", "pm_fixture", providerStatus,
                    providerStatus == "requires_action" ? "https://verify.stripe.com/test-fixture" : null));
            };
            Service = new PaymentOnboardingService(null!, NullLogger<PaymentOnboardingService>.Instance,
                provider, Options.Create(new StripeOptions()), null!, null!,
                Repository<IPaymentMethodRepository, PaymentMethod>(new() { Bank, card }),
                Repository<IAutopayMandateRepository, AutopayMandate>(new() { Mandate }));
        }
    }

    private static TRepository Repository<TRepository, TEntity>(List<TEntity> rows)
        where TRepository : class
        where TEntity : class
    {
        var repository = DispatchProxy.Create<TRepository, InvocationProxy>();
        ((InvocationProxy)(object)repository).InvokeHandler = (method, args) => method.Name switch
        {
            "Find" => Task.FromResult<IEnumerable<TEntity?>>(rows.Where(((Expression<Func<TEntity, bool>>)args[0]!).Compile()).ToList()),
            "Update" => Task.FromResult((TEntity?)args[0]),
            "Save" => Task.CompletedTask,
            _ => throw new InvalidOperationException($"Unexpected repository call: {method.Name}")
        };
        return repository;
    }

    public class InvocationProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> InvokeHandler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => InvokeHandler(method!, args!);
    }
}
