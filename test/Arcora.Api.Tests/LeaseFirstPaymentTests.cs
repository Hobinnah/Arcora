using System.Reflection;
using System.Security.Claims;
using Arcora.Api.Controllers;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Hybrid;

namespace Arcora.Api.Tests;

public sealed class LeaseFirstPaymentTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private LeaseFirstPaymentService service = null!;
    private RentCollectionOrchestrator orchestrator = null!;
    private ServiceProvider cacheServices = null!;
    private Lease lease = null!;
    private readonly List<CreateChargeRequest> charges = [];
    private bool ready = true;
    private bool throwOnCharge;
    private ProviderChargeResult chargeResult = Result("processing");
    private ProviderChargeResult refreshedResult = Result("processing");

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        db = new TestContext(options, new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
        lease = new Lease
        {
            LeaseID = Guid.NewGuid(), LeaseCode = "test-lease", LeaseNumber = "TEST-001",
            TenantID = Guid.NewGuid(), OrganizationID = Guid.NewGuid(), BaseRentAmount = 1500m,
            Currency = "CAD", Status = "PENDING_FIRST_PAYMENT", SignedAt = DateTime.UtcNow
        };
        db.Leases.Add(lease);
        db.Tenants.Add(new Tenant { TenantID = lease.TenantID, UserID = 42, Code = "TEST", Description = "Synthetic tenant" });
        db.LeaseDocuments.Add(new LeaseDocuments
        {
            LeaseDocumentID = Guid.NewGuid(), LeaseID = lease.LeaseID, DocumentType = "LEASE_AGREEMENT",
            DocumentStatus = "FULLY_SIGNED", FullySignedAt = DateTime.UtcNow,
            OriginalFilename = "synthetic.pdf", StorageReference = "synthetic.pdf", StorageProvider = "TEST"
        });
        db.SecurityDeposits.Add(new SecurityDeposit
        {
            SecurityDepositID = Guid.NewGuid(), LeaseID = lease.LeaseID, TenantID = lease.TenantID,
            OrganizationID = lease.OrganizationID, RequiredAmount = 750m, Currency = "CAD", Status = "EXPECTED"
        });
        AddFee(1, "BOOKING FEE", "PLATFORM-BOOKING-5PCT", 5m, true);
        AddFee(2, "Tenant Placement Fee", "PLATFORM-TENANT-PLACEMENT", 49.99m);
        AddFee(3, "Host Placement Fee", "PLATFORM-HOST-PLACEMENT", 99.99m);
        AddFee(4, "Host Active Unit Subscription Fee", "CUSTOM-HOST-SUBSCRIPTION", 24.99m);
        var pad = new PaymentMethod
        {
            PaymentMethodID = Guid.NewGuid(), TenantID = lease.TenantID, PaymentMethodType = "PAD",
            MethodRole = "PRIMARY", VerificationStatus = "VERIFIED", IsActive = true, ProviderName = "STRIPE",
            ProviderCustomerID = "cus_synthetic", ProviderPaymentMethodID = "pm_pad_synthetic"
        };
        db.PaymentMethods.Add(pad);
        db.PaymentMethods.Add(new PaymentMethod
        {
            PaymentMethodID = Guid.NewGuid(), TenantID = lease.TenantID, PaymentMethodType = "CARD",
            MethodRole = "BACKUP", VerificationStatus = "VERIFIED", IsActive = true, ProviderName = "STRIPE",
            ProviderCustomerID = "cus_synthetic", ProviderPaymentMethodID = "pm_card_synthetic"
        });
        db.AutopayMandates.Add(new AutopayMandate
        {
            AutopayMandateID = Guid.NewGuid(), TenantID = lease.TenantID, PaymentMethodID = pad.PaymentMethodID,
            PaymentRail = "PAD", Status = "ACTIVE", ProviderName = "STRIPE", ProviderMandateID = "mandate_synthetic",
            ConsentVersion = "test"
        });
        await db.SaveChangesAsync();
        var provider = Proxy<IPaymentProvider>((method, args) =>
        {
            if (method.Name == nameof(IPaymentProvider.GetChargeStatusAsync))
                return Task.FromResult(refreshedResult);
            Assert.Equal(nameof(IPaymentProvider.CreateChargeAsync), method.Name);
            charges.Add((CreateChargeRequest)args[0]!);
            if (throwOnCharge)
                throw new HttpRequestException("Provider response lost.");
            return Task.FromResult(charges.Count > 1 ? Result("succeeded", "pi_backup") : chargeResult);
        });
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(Arcora.Api.DTOs.DtoProfiles.PaymentIntentProfile).Assembly),
            NullLoggerFactory.Instance).CreateMapper();
        var cacheCollection = new ServiceCollection();
        cacheCollection.AddHybridCache();
        cacheServices = cacheCollection.BuildServiceProvider();
        orchestrator = new RentCollectionOrchestrator(mapper, NullLogger<RentCollectionOrchestrator>.Instance, provider,
            new PaymentIntentRepository(db), new PaymentAttemptRepository(db), new PaymentMethodRepository(db),
            new PaymentRepository(db), new AutopayMandateRepository(db), db,
            cacheServices.GetRequiredService<HybridCache>());
        var onboarding = Proxy<IPaymentOnboardingService>((method, _) =>
        {
            Assert.Equal(nameof(IPaymentOnboardingService.GetStatusAsync), method.Name);
            return Task.FromResult(new PaymentOnboardingStatusResponse
            {
                HasVerifiedPad = ready, HasVerifiedCard = ready, PadMandateActive = ready
            });
        });
        service = new LeaseFirstPaymentService(db, orchestrator, onboarding, provider);
    }

    [Fact]
    public async Task QuoteExcludesHostFeesAndNeverCreatesPayment()
    {
        var quote = await service.GetAsync(lease);
        Assert.Equal(1500m, quote.RentAmount);
        Assert.Equal(750m, quote.SecurityDepositAmount);
        Assert.Equal(124.99m, quote.PlatformFeeAmount);
        Assert.Equal(2374.99m, quote.Amount);
        Assert.Equal("READY", quote.Status);
        Assert.Empty(charges);
        Assert.Empty(await db.PaymentIntents.ToListAsync());
    }

    [Fact]
    public async Task RepeatedCollectionIsIdempotentAndPendingBankNeverFallsBack()
    {
        var first = await service.CollectAsync(lease, 2374.99m);
        var second = await service.CollectAsync(lease, 2374.99m);
        await service.RefreshAsync(lease);
        Assert.Equal(first.PaymentIntentID, second.PaymentIntentID);
        Assert.Equal("PROCESSING", second.Status);
        Assert.Single(charges);
        Assert.Equal(PaymentMethodKind.Pad, charges[0].Kind);
        Assert.Equal(237499, charges[0].AmountInMinorUnits);
        Assert.Equal("mandate_synthetic", charges[0].MandateId);
        Assert.Single(await db.PaymentIntents.ToListAsync());
        var invoice = await db.InvoiceMasters.SingleAsync();
        Assert.Equal("ISSUED", invoice.Status);
        Assert.Equal(2374.99m, invoice.TotalAmount);
        Assert.Equal(0m, invoice.AmountPaid);
        Assert.Equal(2374.99m, invoice.BalanceDue);
        Assert.Equal(4, await db.InvoiceDetails.CountAsync());
        Assert.Empty(await db.PaymentAllocations.ToListAsync());
        Assert.Equal("PENDING_FIRST_PAYMENT", (await db.Leases.AsNoTracking().SingleAsync()).Status);
        Assert.Equal(0m, (await db.SecurityDeposits.SingleAsync()).ReceivedAmount);
    }

    [Fact]
    public async Task ApplicationApprovalFirstChargeKeyCreatesItemizedInvoiceBeforeCharging()
    {
        lease.StartDate = new DateTime(2026, 11, 1);
        await db.SaveChangesAsync();

        await orchestrator.InitiateCollectionAsync(lease.LeaseID, lease.TenantID, Guid.Empty,
            2374.99m, $"pi_firstcharge_{Guid.NewGuid():N}");

        var invoice = await db.InvoiceMasters.SingleAsync();
        Assert.Equal("ISSUED", invoice.Status);
        Assert.Equal(2374.99m, invoice.TotalAmount);
        Assert.Equal(4, await db.InvoiceDetails.CountAsync());
        Assert.Empty(await db.PaymentAllocations.ToListAsync());
        Assert.Single(charges);
    }

    [Theory]
    [InlineData("PENDING_TENANT_SIGNATURE")]
    [InlineData("CANCELLED")]
    [InlineData("EXPIRED")]
    public async Task UnsignedOrClosedLeaseCannotCollect(string status)
    {
        lease.Status = status;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CollectAsync(lease));
        Assert.Empty(charges);
    }

    [Fact]
    public async Task MissingCompletedDocumentCannotCollect()
    {
        (await db.LeaseDocuments.SingleAsync()).DocumentStatus = "SENT_FOR_SIGNATURE";
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CollectAsync(lease));
        Assert.Empty(charges);
    }

    [Fact]
    public async Task ChangedAmountOrIncompleteOnboardingCannotCollect()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CollectAsync(lease, 2250m));
        ready = false;
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CollectAsync(lease, 2374.99m));
        Assert.Empty(charges);
        Assert.Empty(await db.PaymentIntents.ToListAsync());
    }

    [Fact]
    public async Task ConfirmedPaymentActivatesLeaseAndFundsDepositExactlyOnce()
    {
        await service.CollectAsync(lease, 2374.99m);
        refreshedResult = Result("succeeded");
        Assert.Equal("PAID", (await service.RefreshAsync(lease)).Status);
        await service.RefreshAsync(lease);
        Assert.Single(charges);
        Assert.Single(await db.Payments.ToListAsync());
        Assert.Single(await db.SecurityDepositTransactions.ToListAsync());
        var invoice = await db.InvoiceMasters.SingleAsync();
        Assert.Equal("PAID", invoice.Status);
        Assert.Equal(2374.99m, invoice.AmountPaid);
        Assert.Equal(0m, invoice.BalanceDue);
        Assert.Equal(2374.99m, (await db.PaymentAllocations.ToListAsync()).Sum(x => x.AllocatedAmount));
        var lines = await db.InvoiceDetails.OrderBy(x => x.LineType).ToListAsync();
        Assert.Equal(1500m, lines.Single(x => x.LineType == "RENT").TotalLineAmount);
        Assert.Equal(750m, lines.Single(x => x.LineType == "SECURITY_DEPOSIT").TotalLineAmount);
        Assert.Equal(75m, lines.Single(x => x.LineType == "BOOKING_FEE").TotalLineAmount);
        Assert.Equal(49.99m, lines.Single(x => x.LineType == "PLATFORM_FEE").TotalLineAmount);
        Assert.Equal(750m, (await db.SecurityDeposits.SingleAsync()).ReceivedAmount);
        Assert.Equal("HELD", (await db.SecurityDeposits.SingleAsync()).Status);
        Assert.Equal("ACTIVE", (await db.Leases.AsNoTracking().SingleAsync()).Status);
    }

    [Fact]
    public async Task DefinitiveBankFailureUsesBackupOnceAndLateEventsCannotRecharge()
    {
        chargeResult = new("pi_test", null, "requires_payment_method", FailureCategory.Permanent, "account_closed", "Closed account");
        Assert.Equal("PAID", (await service.CollectAsync(lease, 2374.99m)).Status);
        await orchestrator.HandlePadResultAsync((await db.PaymentIntents.SingleAsync()).PaymentIntentID,
            "requires_payment_method", "account_closed", "Late duplicate event");
        Assert.Equal(2, charges.Count);
        Assert.Equal(PaymentMethodKind.Card, charges[1].Kind);
        Assert.Single(await db.Payments.ToListAsync());
    }

    [Fact]
    public async Task UncertainProviderOutcomeNeverTriggersBackupOrAnotherDebit()
    {
        throwOnCharge = true;
        var first = await service.CollectAsync(lease, 2374.99m);
        Assert.Equal("PROCESSING", first.Status);
        Assert.Contains("uncertain", first.FailureReason);
        await service.CollectAsync(lease, 2374.99m);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RefreshAsync(lease));
        Assert.Single(charges);
    }

    [Fact]
    public async Task DatabaseRejectsDuplicateCollectionKeyAndMismatchedReuse()
    {
        await service.CollectAsync(lease, 2374.99m);
        var intent = await db.PaymentIntents.AsNoTracking().SingleAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.InitiateCollectionAsync(
            lease.LeaseID, lease.TenantID, Guid.Empty, 1m, intent.IdempotencyKey));
        intent.PaymentIntentID = Guid.NewGuid();
        db.PaymentIntents.Add(intent);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.Single(charges);
    }

    [Theory]
    [InlineData(42, true)]
    [InlineData(99, false)]
    public async Task OnlyOwningTenantCanAccessFirstPayment(long actor, bool allowed)
    {
        var controller = new LeaseFirstPaymentController(db, service, NullLogger<LeaseFirstPaymentController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("UserId", actor.ToString())], "test"))
                }
            }
        };
        var result = await controller.Get(lease.LeaseID, CancellationToken.None);
        Assert.Equal(allowed, result is OkObjectResult);
        if (!allowed)
        {
            Assert.IsType<ForbidResult>(result);
            Assert.IsType<ForbidResult>(await controller.Collect(lease.LeaseID, new(2374.99m), CancellationToken.None));
            Assert.IsType<ForbidResult>(await controller.Refresh(lease.LeaseID, CancellationToken.None));
        }
        Assert.Empty(charges);
    }

    [Theory]
    [InlineData("2026-10-29", "2026-10-29", false)]
    [InlineData("2026-10-29", "2026-11-29", true)]
    [InlineData("2026-10-29", "2026-09-29", false)]
    [InlineData("2026-01-31", "2026-02-28", true)]
    public void RecurringRentDoesNotDebitInitialMonthAgain(string starts, string today, bool expected)
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        using var scheduler = new RecurringRentCollectionService(NullLogger<RecurringRentCollectionService>.Instance,
            services, Options.Create(new RentCollectionOptions { ChargeHourUtc = 0 }));
        var due = typeof(RecurringRentCollectionService).GetMethod("IsDueToday", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var result = due.Invoke(scheduler, [new Lease { StartDate = DateTime.Parse(starts) }, DateTime.Parse(today)]);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task HostTestPayoutReservesOnlyNetRentAndRefreshNeverDuplicatesItems()
    {
        var (payouts, payment, calls) = await HostPayoutFixture();
        await payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None);
        await payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None);
        Assert.Equal(137502, calls[0].Amount);
        Assert.Equal(237499, calls[0].CollectedAmount);
        Assert.Equal(calls[0], calls[1]);
        var saved = await db.RentDisbursements.AsNoTracking().SingleAsync();
        Assert.Equal("WAITING_FUNDS", saved.Status);
        Assert.Equal("tr_synthetic", saved.TransferReference);
        Assert.Null(saved.BankPayoutReference);
        Assert.Null((await db.Payouts.AsNoTracking().SingleAsync()).PaidAt);
        var item = await db.PayoutItems.AsNoTracking().SingleAsync();
        Assert.Equal(1500m, item.GrossAmount);
        Assert.Equal(124.98m, item.DeductionAmount);
        Assert.Equal(1375.02m, item.NetPayoutAmount);
        Assert.Single(charges);
    }

    [Fact]
    public async Task HostPayoutRequiresConsentOnboardingTestModeAndOrganizationManagement()
    {
        var (payouts, payment, calls) = await HostPayoutFixture();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            payouts.ProgressAsync(payment, lease.OrganizationID, 99, true, 1375.02m, CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            payouts.ProgressAsync(payment, Guid.NewGuid(), 42, true, 1375.02m, CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            payouts.ProgressAsync(payment, lease.OrganizationID, 42, false, null, CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1500m, CancellationToken.None));
        Assert.Empty(calls);
        Assert.Empty(await db.RentDisbursements.ToListAsync());
        var disabled = new HostRentPayoutService(db, HostAccounts(),
            Proxy<IHostPayoutProvider>((_, _) => throw new InvalidOperationException("Must not call")),
            new ConfigurationBuilder().Build(), NullLogger<HostRentPayoutService>.Instance);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            disabled.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None));
    }

    [Fact]
    public async Task RefundedPaymentCannotStartHostPayout()
    {
        var (payouts, payment, calls) = await HostPayoutFixture();
        (await db.Payments.SingleAsync()).RefundedAmount = 1m;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None));
        Assert.Empty(calls);
        Assert.Empty(await db.Payouts.ToListAsync());
    }

    [Fact]
    public async Task BankPayoutIsOnlyCreatedAfterDurablyRecordingItsAttemptAndProviderSuccess()
    {
        var (payouts, payment, _) = await HostPayoutFixture((_, allowPayout) =>
        {
            if (!allowPayout)
                return new("tr_synthetic", null, "READY_FOR_PAYOUT");
            Assert.NotNull(db.RentDisbursements.AsNoTracking().Single().PayoutAttemptedAt);
            return new("tr_synthetic", "po_synthetic", "PENDING");
        });
        await payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None);
        Assert.Null((await db.RentDisbursements.AsNoTracking().SingleAsync()).PayoutAttemptedAt);
        var pending = await payouts.ProgressAsync(payment, lease.OrganizationID, 42, false, null, CancellationToken.None);
        Assert.Equal("PENDING", pending.Status);
        Assert.Equal("po_synthetic", pending.BankPayoutReference);
        Assert.Null((await db.Payouts.AsNoTracking().SingleAsync()).PaidAt);
        Assert.Single(await db.PayoutItems.ToListAsync());
    }

    [Fact]
    public async Task UncertainPayoutPersistsReservationAndExpiredRetriesCannotCreateAgain()
    {
        var (payouts, payment, _) = await HostPayoutFixture((_, _) => throw new HttpRequestException("Response lost"));
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None));
        var saved = await db.RentDisbursements.SingleAsync();
        Assert.Equal("PROVIDER_UNCERTAIN", saved.Status);
        saved.CapturedDate = DateTime.UtcNow.AddDays(-2);
        saved.PayoutAttemptedAt = DateTime.UtcNow.AddDays(-2);
        await db.SaveChangesAsync();
        var provider = Proxy<IHostPayoutProvider>((_, args) =>
        {
            Assert.False((bool)args[1]!);
            Assert.False((bool)args[2]!);
            return Task.FromResult(new HostPayoutResult("tr_synthetic", null, "READY_FOR_PAYOUT"));
        });
        var retry = new HostRentPayoutService(db, HostAccounts(), provider, HostPayoutConfiguration(),
            NullLogger<HostRentPayoutService>.Instance);
        Assert.Equal("REVIEW_REQUIRED", (await retry.ProgressAsync(payment, lease.OrganizationID,
            42, false, null, CancellationToken.None)).Status);
        Assert.Single(await db.Payouts.ToListAsync());
    }

    [Theory]
    [InlineData("PAID", true)]
    [InlineData("FAILED", false)]
    public async Task ProviderBankPayoutOutcomeIsMirroredWithoutAnotherReservation(string status, bool paid)
    {
        var (payouts, payment, _) = await HostPayoutFixture((_, _) => new("tr_synthetic", "po_synthetic", status));
        await payouts.ProgressAsync(payment, lease.OrganizationID, 42, true, 1375.02m, CancellationToken.None);
        await payouts.ProgressAsync(payment, lease.OrganizationID, 42, false, null, CancellationToken.None);
        var payout = await db.Payouts.AsNoTracking().SingleAsync();
        Assert.Equal(status, payout.Status);
        Assert.Equal(paid, payout.PaidAt.HasValue);
        Assert.Single(await db.RentDisbursements.ToListAsync());
        Assert.Single(await db.PayoutItems.ToListAsync());
    }

    private IConfiguration HostPayoutConfiguration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["HostPayouts:EnableTestPayouts"] = "true", ["Stripe:SecretKey"] = "sk_test_synthetic"
        }).Build();

    private IOrgPayoutAccountService HostAccounts() => Proxy<IOrgPayoutAccountService>((method, args) =>
    {
        if ((long)args[1]! != 42)
            throw new UnauthorizedAccessException();
        if (method.Name == nameof(IOrgPayoutAccountService.GetOrganizationPayoutAccountStatus))
            Assert.True((bool)args[2]!);
        return Task.FromResult<PayoutAccountStatusDto?>(new PayoutAccountStatusDto
        {
            OrganizationID = lease.OrganizationID, PayoutsEnabled = true, DetailsSubmitted = true,
            StripeAccountID = "acct_synthetic"
        });
    });

    private async Task<(HostRentPayoutService Service, Guid Payment, List<HostPayoutRequest> Calls)> HostPayoutFixture(
        Func<bool, bool, HostPayoutResult>? result = null)
    {
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var payment = (await db.Payments.AsNoTracking().SingleAsync()).PaymentID;
        db.OrgPayoutAccounts.Add(new OrgPayoutAccount
        {
            OrganizationID = lease.OrganizationID, ProviderName = "Stripe", ProviderAccountID = "acct_synthetic",
            StripeAccountID = "acct_synthetic", Currency = "CAD", VerificationStatus = "VERIFIED", IsActive = true
        });
        await db.SaveChangesAsync();
        var calls = new List<HostPayoutRequest>();
        var provider = Proxy<IHostPayoutProvider>((_, args) =>
        {
            calls.Add((HostPayoutRequest)args[0]!);
            return Task.FromResult(result?.Invoke((bool)args[1]!, (bool)args[2]!) ??
                new HostPayoutResult("tr_synthetic", null, "WAITING_FUNDS"));
        });
        return (new HostRentPayoutService(db, HostAccounts(), provider, HostPayoutConfiguration(),
            NullLogger<HostRentPayoutService>.Instance), payment, calls);
    }

    [Fact]
    public async Task AccountingSnapshotsRatesAndOnlyPostsAfterConfirmedSuccess()
    {
        await service.CollectAsync(lease, 2374.99m);
        var snapshot = await db.RentPaymentAccountings.AsNoTracking().SingleAsync();
        Assert.Null(snapshot.SettledAt);
        Assert.Null(snapshot.PaymentID);
        Assert.Equal(0m, snapshot.HostNetAmount);
        foreach (var fee in await db.Fees.ToListAsync())
            fee.FixedAmount = 999m;
        await db.SaveChangesAsync();
        var quote = await service.GetAsync(lease);
        Assert.Equal(124.99m, quote.PlatformFeeAmount);
        refreshedResult = Result("succeeded");
        await service.RefreshAsync(lease);
        await service.RefreshAsync(lease);
        var settled = await db.RentPaymentAccountings.AsNoTracking().SingleAsync();
        Assert.Equal(1500m, settled.RentAmount);
        Assert.Equal(750m, settled.DepositAmount);
        Assert.Equal(124.99m, settled.TenantFeeAmount);
        Assert.Equal(99.99m, settled.HostPlacementFee);
        Assert.Equal(24.99m, settled.HostMonthlyFee);
        Assert.Equal(1375.02m, settled.HostNetAmount);
        Assert.NotNull(settled.SettledAt);
        Assert.Single(charges);
    }

    [Fact]
    public async Task SubsequentMonthDeductsOnlyMonthlyFeeAndDuplicateEventsDoNotRededuct()
    {
        lease.StartDate = new DateTime(2026, 10, 29);
        await db.SaveChangesAsync();
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var next = await orchestrator.InitiateCollectionAsync(lease.LeaseID, lease.TenantID, Guid.Empty,
            1500m, $"pi_{lease.LeaseID:N}_202611");
        Assert.NotNull(next.PaymentIntentID);
        await orchestrator.HandlePadResultAsync(next.PaymentIntentID.Value, "succeeded", null, null);
        var monthly = await db.RentPaymentAccountings.AsNoTracking()
            .SingleAsync(x => x.PaymentIntentID == next.PaymentIntentID);
        Assert.Equal(0m, monthly.HostPlacementFee);
        Assert.Equal(24.99m, monthly.HostMonthlyFee);
        Assert.Equal(1475.01m, monthly.HostNetAmount);
        Assert.Equal(0m, monthly.DepositAmount);
        Assert.Equal(0m, monthly.TenantFeeAmount);
        Assert.Equal(99.99m, (await db.RentPaymentAccountings.ToListAsync()).Sum(x => x.HostPlacementFee));
        Assert.Equal(2, charges.Count);
    }

    [Fact]
    public async Task SameRentPeriodCannotDeductMonthlyFeeTwice()
    {
        lease.StartDate = new DateTime(2026, 10, 29);
        await db.SaveChangesAsync();
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var duplicatePeriod = await orchestrator.InitiateCollectionAsync(lease.LeaseID, lease.TenantID,
            Guid.Empty, 1500m, $"pi_{lease.LeaseID:N}_202610");
        var allocation = await db.RentPaymentAccountings.AsNoTracking()
            .SingleAsync(x => x.PaymentIntentID == duplicatePeriod.PaymentIntentID);
        Assert.Equal(0m, allocation.HostMonthlyFee);
        Assert.Equal(0m, allocation.HostPlacementFee);
    }

    [Fact]
    public async Task MonthlyUnitFeeIsNotRepeatedForAnotherLeaseInTheSameMonth()
    {
        lease.StartDate = new DateTime(2026, 10, 29);
        await db.SaveChangesAsync();
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var replacement = new Lease
        {
            LeaseID = Guid.NewGuid(), LeaseCode = "replacement", LeaseNumber = "TEST-002",
            TenantID = lease.TenantID, OrganizationID = lease.OrganizationID, RentalUnitID = lease.RentalUnitID,
            BaseRentAmount = 1500m, Currency = "CAD", StartDate = new DateTime(2026, 10, 30)
        };
        db.Leases.Add(replacement);
        await db.SaveChangesAsync();
        var result = await orchestrator.InitiateCollectionAsync(replacement.LeaseID, lease.TenantID,
            Guid.Empty, 1624.99m, $"pi_firstcharge_lease_{replacement.LeaseID:N}");
        var allocation = await db.RentPaymentAccountings.AsNoTracking()
            .SingleAsync(x => x.PaymentIntentID == result.PaymentIntentID);
        Assert.Equal(0m, allocation.HostMonthlyFee);
        Assert.Equal(99.99m, allocation.HostPlacementFee);
    }

    [Fact]
    public async Task MissingOrAmbiguousHostFeesBlockCollectionBeforeProviderDebit()
    {
        var monthly = await db.Fees.SingleAsync(x => x.Code == "CUSTOM-HOST-SUBSCRIPTION");
        monthly.IsActive = false;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CollectAsync(lease, 2374.99m));
        Assert.Empty(charges);
        Assert.Empty(await db.PaymentIntents.ToListAsync());
    }

    [Fact]
    public async Task ExplicitLegacyReconciliationRequiresMatchingAmountsAndIsIdempotent()
    {
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var payment = await db.Payments.AsNoTracking().SingleAsync();
        db.RentPaymentAccountings.RemoveRange(await db.RentPaymentAccountings.ToListAsync());
        await db.SaveChangesAsync();
        var accounting = new RentPaymentAccountingService(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => accounting.ReconcileAsync(payment.PaymentID,
            new(1500m, 750m, 124.99m, 1m, 24.99m), "TEST", CancellationToken.None));
        db.ChangeTracker.Clear();
        var approved = new RentPaymentReconciliation(1500m, 750m, 124.99m, 99.99m, 24.99m);
        await accounting.ReconcileAsync(payment.PaymentID, approved, "TEST", CancellationToken.None);
        await accounting.ReconcileAsync(payment.PaymentID, approved, "TEST", CancellationToken.None);
        var allocation = await db.RentPaymentAccountings.AsNoTracking().SingleAsync();
        Assert.Equal("TEST", allocation.ReconciliationBy);
        Assert.Equal(1375.02m, allocation.HostNetAmount);
        Assert.Single(charges);
        Assert.Single(await db.Payments.ToListAsync());
    }

    [Fact]
    public async Task ForeignOrganizationCannotReadAnyEarningsSurface()
    {
        var controller = new PayoutController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("UserId", "42")], "test"))
                }
            }
        };
        var reads = 0;
        var accounts = Proxy<IOrgPayoutAccountService>((_, _) => throw new UnauthorizedAccessException());
        var payouts = Proxy<IPayoutService>((_, _) => { reads++; throw new InvalidOperationException(); });
        Assert.IsType<ForbidResult>(await controller.GetEarningsSummary(payouts, accounts, Guid.NewGuid(), null));
        Assert.IsType<ForbidResult>(await controller.GetListingBreakdown(payouts, accounts, Guid.NewGuid(), null));
        Assert.IsType<ForbidResult>(await controller.ExportStatement(payouts, accounts, Guid.NewGuid(), null));
        Assert.Equal(0, reads);
    }

    [Fact]
    public async Task DatabaseRejectsDuplicateSettledHostDeductions()
    {
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var allocation = await db.RentPaymentAccountings.AsNoTracking().SingleAsync();
        allocation.PaymentIntentID = Guid.NewGuid();
        allocation.PaymentID = null;
        db.RentPaymentAccountings.Add(allocation);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task EarningsSummaryBreakdownAndStatementAgreeAndExcludeOtherOrganizations()
    {
        lease.ListingID = Guid.NewGuid();
        db.Listings.Add(new Listing
        {
            ListingID = lease.ListingID, OrganizationID = lease.OrganizationID,
            Title = "Synthetic accounting listing", Currency = "CAD", Notes = "Synthetic"
        });
        await db.SaveChangesAsync();
        chargeResult = Result("succeeded");
        await service.CollectAsync(lease, 2374.99m);
        var services = new ServiceCollection();
        services.AddHybridCache();
        using var provider = services.BuildServiceProvider();
        var payoutService = new PayoutService(Proxy<IMapper>((_, _) => throw new NotSupportedException()),
            provider.GetRequiredService<HybridCache>(), Options.Create(new CacheConfiguration()),
            NullLogger<PayoutService>.Instance, new PayoutRepository(db), db);
        var summary = await payoutService.GetEarningsSummary(lease.OrganizationID, null);
        var breakdown = Assert.Single(await payoutService.GetListingBreakdown(lease.OrganizationID, null));
        var statement = (await payoutService.GetStatementTransactions(lease.OrganizationID, null)).ToList();
        Assert.Equal(1500m, summary.RentCollected);
        Assert.Equal(124.98m, summary.ServiceFees);
        Assert.Equal(1375.02m, summary.NetEarnings);
        Assert.Equal(summary.RentCollected, breakdown.RentCollected);
        Assert.Equal(summary.NetEarnings, breakdown.NetEarnings);
        Assert.Equal(3, statement.Count);
        Assert.Equal(summary.NetEarnings, statement.Sum(x => x.Amount));
        Assert.Equal(0m, (await payoutService.GetEarningsSummary(Guid.NewGuid(), null)).NetEarnings);
        Assert.Empty(await payoutService.GetListingBreakdown(Guid.NewGuid(), null));
        Assert.Empty(await payoutService.GetStatementTransactions(Guid.NewGuid(), null));
        Assert.False(summary.RefundReviewRequired);
        Assert.Equal(0, summary.UnallocatedPaymentCount);
        var allocation = await db.RentPaymentAccountings.SingleAsync();
        var paidAt = allocation.SettledAt!.Value;
        allocation.SettledAt = paidAt.AddYears(-2);
        await db.SaveChangesAsync();
        Assert.Equal(0m, (await payoutService.GetEarningsSummary(lease.OrganizationID, "This month")).RentCollected);
        allocation.SettledAt = paidAt;
        await db.SaveChangesAsync();
        db.Refunds.Add(new Refund
        {
            RefundID = Guid.NewGuid(), PaymentID = (await db.Payments.SingleAsync()).PaymentID,
            TenantID = lease.TenantID, Amount = 750m, Status = "SUCCEEDED", Currency = "CAD"
        });
        await db.SaveChangesAsync();
        Assert.True((await payoutService.GetEarningsSummary(lease.OrganizationID, null)).RefundReviewRequired);
        Assert.Equal(99.99m, (await db.RentPaymentAccountings.AsNoTracking().SingleAsync()).HostPlacementFee);
        db.RentPaymentAccountings.RemoveRange(await db.RentPaymentAccountings.ToListAsync());
        await db.SaveChangesAsync();
        Assert.Equal(1, (await payoutService.GetEarningsSummary(lease.OrganizationID, null)).UnallocatedPaymentCount);
    }

    [Fact]
    public void TenantFeesUseRentOnlyAndApplyBoundsAndMinorUnitRounding()
    {
        var fees = new[]
        {
            new Fee { CalculationType = "PERCENTAGE", PercentageRate = 5m, MaximumFeeAmount = 20m },
            new Fee { CalculationType = "FIXED", FixedAmount = 1.005m },
            new Fee { CalculationType = "FIXED", FixedAmount = -5m },
            new Fee { Code = "PLATFORM-HOST-PLACEMENT", CalculationType = "FIXED", FixedAmount = 99.99m }
        };
        Assert.Equal(21.01m, TenantBookingFees.Calculate(fees, 1500m));
    }

    private void AddFee(int id, string name, string code, decimal amount, bool percentage = false)
    {
        var type = new FeeType { FeeTypeID = id, Name = name, IsPlatformFee = true };
        db.FeeTypes.Add(type);
        db.Fees.Add(new Fee
        {
            FeeID = Guid.NewGuid(), FeeType = type, Code = code, Name = name, IsActive = true,
            Currency = "CAD", CalculationType = percentage ? "PERCENTAGE" : "FIXED",
            FixedAmount = percentage ? null : amount, PercentageRate = percentage ? amount : null
        });
    }

    private static ProviderChargeResult Result(string status, string id = "pi_test") =>
        new(id, null, status, status == "succeeded" ? FailureCategory.None : FailureCategory.Pending, null, null);

    private static T Proxy<T>(Func<MethodInfo, object?[], object?> handler) where T : class
    {
        var proxy = DispatchProxy.Create<T, InvocationProxy>();
        ((InvocationProxy)(object)proxy).Handler = handler;
        return proxy;
    }

    public class InvocationProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args!);
    }

    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration config) : ArcoraDbContext(options, config)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
            // SQLite stores unmapped decimals as TEXT, which breaks numeric filtered indexes.
            foreach (var property in builder.Entity<RentPaymentAccounting>().Metadata.GetProperties()
                .Where(x => x.ClrType == typeof(decimal)))
                builder.Entity<RentPaymentAccounting>().Property<decimal>(property.Name).HasConversion<double>();
        }
    }

    public async Task DisposeAsync()
    {
        if (cacheServices != null)
            await cacheServices.DisposeAsync();
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }
}
