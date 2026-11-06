using Arcora.Api;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class SecurityDepositRefundTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private FakeRefundProcessor refundProcessor = null!;
    private HostingSecurityDepositService service = null!;
    private IOrganizationMemberRepository organizationMemberRepository = null!;
    private readonly Guid organizationID = Guid.NewGuid();
    private readonly Guid tenantID = Guid.NewGuid();
    private readonly Guid depositID = Guid.NewGuid();
    private readonly Guid paymentID = Guid.NewGuid();
    private const string ProviderRefundID = "re_test_pending";
    private const string IdempotencyKey = "refund-test-key";

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();

        db.Users.Add(new User { Id = 10, UserName = "tenant@example.test", Email = "tenant@example.test" });
        db.Users.Add(new User { Id = 7, UserName = "host@example.test", Email = "host@example.test" });
        db.Organizations.Add(new Organization
        {
            OrganizationID = organizationID, LegalName = "Synthetic host", DisplayName = "Synthetic host",
            TimeZone = "America/Edmonton"
        });
        db.OrganizationMembers.Add(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationID, UserID = 7,
            RoleName = "OWNER", Status = "ACTIVE", IsPrimaryOwner = true
        });
        db.Tenants.Add(new Tenant { TenantID = tenantID, UserID = 10, Code = "tenant", Description = "Tenant" });
        db.SecurityDeposits.Add(new SecurityDeposit
        {
            SecurityDepositID = depositID,
            TenantID = tenantID,
            OrganizationID = organizationID,
            RequiredAmount = 1000m,
            ReceivedAmount = 1000m,
            Currency = "CAD",
            Status = "HELD",
            HeldAt = DateTime.UtcNow
        });
        db.Payments.Add(new Payment
        {
            PaymentID = paymentID,
            PaymentIntentID = Guid.NewGuid(),
            TenantID = tenantID,
            ProviderChargeID = "pi_test_payment",
            Status = "PAID",
            GrossAmount = 1000m,
            NetAmount = 1000m,
            Currency = "CAD",
            PaidAt = DateTime.UtcNow
        });
        db.SecurityDepositTransactions.Add(new SecurityDepositTransaction
        {
            SecurityDepositTransactionID = Guid.NewGuid(),
            SecurityDepositID = depositID,
            PaymentID = paymentID,
            TransactionType = "FUNDING",
            Amount = 1000m,
            Currency = "CAD",
            OccurredAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        refundProcessor = new FakeRefundProcessor(depositID);
        organizationMemberRepository = new OrganizationMemberRepository(db);
        service = new HostingSecurityDepositService(
            db,
            organizationMemberRepository,
            null!,
            new NoopEmailSender(),
            refundProcessor);
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task PendingRefundReservesFundsUntilSucceededAndRetryReplaysResult()
    {
        Assert.Equal(organizationID,
            (await organizationMemberRepository.GetMemberOrganizationsAsync(7)).Single().OrganizationID);
        var request = new SecurityDepositReturnRequestDto
        {
            Amount = 400m,
            Currency = "CAD",
            IdempotencyKey = IdempotencyKey
        };

        var pending = await service.ReturnDeposit(depositID, actorUserID: 7, request);

        Assert.Equal("PENDING", pending.RefundStatus);
        Assert.Equal(0m, pending.ReturnedAmount);
        Assert.Equal(600m, pending.HeldAmount);
        Assert.Equal("RETURN_PENDING", (await db.SecurityDeposits.SingleAsync()).Status);
        Assert.Equal(0m, (await db.SecurityDeposits.SingleAsync()).ReturnedAmount);
        var hostDeposits = await service.GetHostSecurityDeposits(7, 100, 1, null);
        Assert.Single(hostDeposits.Data);
        Assert.Equal(600m, hostDeposits.Summary.HeldAmountTotal);
        Assert.Equal(1, refundProcessor.CallCount);

        await service.ReconcileRefundAsync(ProviderRefundID, "succeeded");

        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(400m, deposit.ReturnedAmount);
        Assert.Equal("PARTIALLY_RETURNED", deposit.Status);
        Assert.Equal("RETURN", (await db.SecurityDepositTransactions.SingleAsync(x => x.RefundID != null)).TransactionType);
        Assert.Equal(600m, (await service.GetHostSecurityDeposits(7, 100, 1, null)).Summary.HeldAmountTotal);

        await service.ReconcileRefundAsync(ProviderRefundID, "pending");
        Assert.Equal(400m, (await db.SecurityDeposits.SingleAsync()).ReturnedAmount);
        Assert.Equal("PROCESSED", (await db.Refunds.SingleAsync()).Status);

        var replay = await service.ReturnDeposit(depositID, actorUserID: 7, request);
        Assert.True(replay.IdempotentReplay);
        Assert.Equal("PROCESSED", replay.RefundStatus);
        Assert.Equal(400m, replay.ReturnedAmount);
        Assert.Equal(600m, replay.HeldAmount);
        Assert.Equal(1, refundProcessor.CallCount);
    }

    [Fact]
    public async Task ForeignOrganizationCannotReturnOrDeductDeposit()
    {
        var foreignOrganizationID = Guid.NewGuid();
        db.Organizations.Add(new Organization
        {
            OrganizationID = foreignOrganizationID, LegalName = "Foreign synthetic host",
            DisplayName = "Foreign synthetic host", TimeZone = "America/Edmonton"
        });
        (await db.OrganizationMembers.SingleAsync()).OrganizationID = foreignOrganizationID;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
            {
                Amount = 400m, Currency = "CAD", IdempotencyKey = IdempotencyKey
            }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.SendSettlementNotice(depositID, 7, "unauthorized", new SecurityDepositSettlementNoticeRequestDto()));
        Assert.Equal(0, refundProcessor.CallCount);
        Assert.Empty(await db.Refunds.ToListAsync());
    }

    [StripeSandboxFact]
    public async Task StripeSandboxFullRefundUsesOnlyANewVerifiedTestPayment()
    {
        var client = await ConfigureNewStripeTestPayment();
        var request = new SecurityDepositReturnRequestDto
        {
            Amount = 1000m, Currency = "CAD", IdempotencyKey = Guid.NewGuid().ToString("N")
        };
        var result = await service.ReturnDeposit(depositID, 7, request);
        var replay = await service.ReturnDeposit(depositID, 7, request);
        Assert.Equal("PROCESSED", result.RefundStatus);
        Assert.True(replay.IdempotentReplay);
        Assert.Equal(result.RefundID, replay.RefundID);
        Assert.Equal(1000m, (await db.SecurityDeposits.SingleAsync()).ReturnedAmount);
        var refund = await new Stripe.RefundService(client).GetAsync((await db.Refunds.SingleAsync()).ProviderRefundID);
        Assert.Equal(100000L, refund.Amount);
        Assert.Equal("succeeded", refund.Status);
    }

    [StripeSandboxFact]
    public async Task StripeSandboxEvidenceDeductionAndPartialRefundDisposeOnlyANewTestPayment()
    {
        var client = await ConfigureNewStripeTestPayment();
        await service.SendSettlementNotice(depositID, 7, Guid.NewGuid().ToString("N"), await CreateSettlementRequest(250m));
        var result = await service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
        {
            Amount = 750m, Currency = "CAD", IdempotencyKey = Guid.NewGuid().ToString("N")
        });
        Assert.Equal("PROCESSED", result.RefundStatus);
        Assert.Equal(0m, result.HeldAmount);
        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(250m, deposit.AppliedAmount);
        Assert.Equal(750m, deposit.ReturnedAmount);
        Assert.Equal("RETURNED", deposit.Status);
        var refund = await new Stripe.RefundService(client).GetAsync((await db.Refunds.SingleAsync()).ProviderRefundID);
        Assert.Equal(75000L, refund.Amount);
        Assert.Equal("succeeded", refund.Status);
    }

    private async Task<Stripe.StripeClient> ConfigureNewStripeTestPayment()
    {
        var key = Environment.GetEnvironmentVariable("ARCORA_DEPOSIT_TEST_STRIPE_KEY")!;
        Assert.True(key.StartsWith("sk_test_", StringComparison.Ordinal));
        var client = new Stripe.StripeClient(key);
        var intent = await new Stripe.PaymentIntentService(client).CreateAsync(new Stripe.PaymentIntentCreateOptions
        {
            Amount = 100000, Currency = "cad", PaymentMethod = "pm_card_visa",
            PaymentMethodTypes = ["card"], Confirm = true,
            Description = $"Arcora isolated deposit lifecycle regression {Guid.NewGuid():N}",
            Metadata = new Dictionary<string, string> { ["arcora_isolated_deposit_test"] = depositID.ToString() }
        });
        Assert.False(intent.Livemode);
        Assert.Equal("succeeded", intent.Status);
        Assert.Equal(100000L, intent.AmountReceived);
        (await db.Payments.SingleAsync()).ProviderChargeID = intent.Id;
        await db.SaveChangesAsync();
        service = new HostingSecurityDepositService(db, organizationMemberRepository, null!, new NoopEmailSender(),
            new StripeSecurityDepositRefundProcessor(Options.Create(new StripeOptions { SecretKey = key })));
        return client;
    }

    [Fact]
    public async Task FullReturnClosesDepositAndDuplicateReconciliationDoesNotDoubleCount()
    {
        refundProcessor.ExpectedAmount = 100000;
        var request = new SecurityDepositReturnRequestDto
        {
            Amount = 1000m, Currency = "CAD", IdempotencyKey = IdempotencyKey
        };
        var pending = await service.ReturnDeposit(depositID, 7, request);
        Assert.Equal(0m, pending.HeldAmount);
        await service.ReconcileRefundAsync(ProviderRefundID, "succeeded");
        await service.ReconcileRefundAsync(ProviderRefundID, "succeeded");
        db.ChangeTracker.Clear();

        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(1000m, deposit.ReturnedAmount);
        Assert.Equal("RETURNED", deposit.Status);
        Assert.NotNull(deposit.ClosedAt);
        Assert.True((await service.ReturnDeposit(depositID, 7, request)).IdempotentReplay);
        Assert.Single(await db.Refunds.ToListAsync());
        Assert.Equal(1, refundProcessor.CallCount);
    }

    [Theory]
    [InlineData(250, 750)]
    [InlineData(1000, 0)]
    public async Task EvidenceBackedDeductionsAndRemainingRefundFullyDisposeDeposit(int deduction, int remaining)
    {
        var request = await CreateSettlementRequest(deduction);
        var notice = await service.SendSettlementNotice(depositID, 7, "disposition-key", request);
        Assert.Equal((decimal)remaining, notice.ReturnAmount);
        Assert.Equal(remaining == 0 ? "CLOSED" : "RETURN_DUE", notice.Status);

        if (remaining > 0)
        {
            refundProcessor.ExpectedAmount = remaining * 100;
            await service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
            {
                Amount = remaining, Currency = "CAD", IdempotencyKey = IdempotencyKey
            });
            await service.ReconcileRefundAsync(ProviderRefundID, "succeeded");
        }

        db.ChangeTracker.Clear();
        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(1000m, deposit.AppliedAmount + deposit.ReturnedAmount);
        Assert.NotNull(deposit.ClosedAt);
        Assert.Equal(0m, (await service.GetHostSecurityDeposits(7, 100, 1, null)).Summary.HeldAmountTotal);
        Assert.Equal(remaining > 0 ? 1 : 0, refundProcessor.CallCount);
    }

    [Fact]
    public async Task PendingRefundCannotAlsoBeDeductedAndDepositStaysOpenUntilReconciled()
    {
        await service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
        {
            Amount = 400m, Currency = "CAD", IdempotencyKey = IdempotencyKey
        });
        var excessive = await CreateSettlementRequest(700m);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.SendSettlementNotice(depositID, 7, "excessive-deduction", excessive));
        Assert.Equal(0m, (await db.SecurityDeposits.SingleAsync()).AppliedAmount);

        var remaining = await CreateSettlementRequest(600m);
        var notice = await service.SendSettlementNotice(depositID, 7, "remaining-deduction", remaining);
        Assert.Equal(0m, notice.ReturnAmount);
        Assert.Equal("RETURN_PENDING", notice.Status);
        Assert.Null((await db.SecurityDeposits.SingleAsync()).ClosedAt);
        await service.ReconcileRefundAsync(ProviderRefundID, "succeeded");
        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(1000m, deposit.AppliedAmount + deposit.ReturnedAmount);
        Assert.Equal("RETURNED", deposit.Status);
    }

    [Theory]
    [InlineData(401, "CAD")]
    [InlineData(400, "USD")]
    public async Task ReturnRejectsChangedPayloadForAnExistingIdempotencyKey(int amount, string currency)
    {
        await service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
        {
            Amount = 400m, Currency = "CAD", IdempotencyKey = IdempotencyKey
        });
        var conflict = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
            {
                Amount = amount, Currency = currency, IdempotencyKey = IdempotencyKey
            }));
        Assert.Equal(409, conflict.StatusCode);
        Assert.Equal(1, refundProcessor.CallCount);
        Assert.Single(await db.Refunds.ToListAsync());
    }

    [Fact]
    public async Task FailedRefundReleasesReservedBalanceWithoutRecordingMoneyReturned()
    {
        await service.ReturnDeposit(depositID, 7, new SecurityDepositReturnRequestDto
        {
            Amount = 400m, Currency = "CAD", IdempotencyKey = IdempotencyKey
        });
        await service.ReconcileRefundAsync(ProviderRefundID, "failed", "Synthetic provider failure");
        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(0m, deposit.ReturnedAmount);
        Assert.Equal("HELD", deposit.Status);
        Assert.Equal(1000m, (await service.GetHostSecurityDeposits(7, 100, 1, null)).Summary.HeldAmountTotal);
        Assert.Equal("RETURN_FAILED", (await db.SecurityDepositTransactions.SingleAsync(x => x.RefundID != null)).TransactionType);
    }

    [Fact]
    public async Task EvidenceBelongingToAnotherDepositCannotSupportADeduction()
    {
        var request = await CreateSettlementRequest(100m);
        (await db.Attachments.SingleAsync()).EntityID = Guid.NewGuid().ToString();
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.SendSettlementNotice(depositID, 7, "foreign-evidence", request));
        Assert.Equal(0m, (await db.SecurityDeposits.SingleAsync()).AppliedAmount);
        Assert.Empty(await db.SecurityDepositTransactions.Where(x => x.TransactionType == "SETTLEMENT_NOTICE").ToListAsync());
    }

    private async Task<SecurityDepositSettlementNoticeRequestDto> CreateSettlementRequest(decimal amount)
    {
        var evidenceID = Guid.NewGuid();
        db.Attachments.Add(new Attachment
        {
            AttachmentID = evidenceID, EntityType = "SECURITY_DEPOSIT_EVIDENCE",
            EntityID = depositID.ToString(), FileName = "synthetic-receipt.pdf",
            StorageReference = $"synthetic-evidence/{evidenceID}.pdf"
        });
        await db.SaveChangesAsync();
        return new SecurityDepositSettlementNoticeRequestDto
        {
            Currency = "CAD",
            Deductions = [new SecurityDepositDeductionDto
            {
                Description = "Synthetic repair", Amount = amount, EvidenceAttachmentIDs = [evidenceID]
            }]
        };
    }

    [Fact]
    public async Task SettlementNoticeReplaysOnlyForTheSameRequest()
    {
        var evidenceID = Guid.NewGuid();
        db.Attachments.Add(new Attachment
        {
            AttachmentID = evidenceID,
            EntityType = "SECURITY_DEPOSIT_EVIDENCE",
            EntityID = depositID.ToString(),
            FileName = "receipt.pdf",
            StorageReference = "evidence/receipt.pdf"
        });
        await db.SaveChangesAsync();

        var request = new SecurityDepositSettlementNoticeRequestDto
        {
            Currency = "CAD",
            Deductions = [new SecurityDepositDeductionDto
            {
                Description = "Repair",
                Amount = 100m,
                EvidenceAttachmentIDs = [evidenceID]
            }]
        };

        var first = await service.SendSettlementNotice(depositID, 7, "settlement-key", request);
        var replay = await service.SendSettlementNotice(depositID, 7, "settlement-key", request);

        Assert.Equal(first.SentAtUtc, replay.SentAtUtc);
        Assert.Equal(100m, (await db.SecurityDeposits.SingleAsync()).AppliedAmount);
        Assert.Single(await db.SecurityDepositTransactions.Where(x => x.TransactionType == "SETTLEMENT_NOTICE").ToListAsync());

        request.Deductions[0].Amount = 150m;
        var conflict = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.SendSettlementNotice(depositID, 7, "settlement-key", request));
        Assert.Equal(409, conflict.StatusCode);
        Assert.Equal(100m, (await db.SecurityDeposits.SingleAsync()).AppliedAmount);
    }

    [Fact]
    public async Task SettlementIdempotencyEventKeyIsUniqueInTheDatabase()
    {
        db.PaymentProviderEvents.AddRange(
            new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(), ProviderName = "INTERNAL_SECURITY_DEPOSIT",
                ProviderEventID = "settlement-notice-key", EventType = "SETTLEMENT_NOTICE",
                ProcessingStatus = "RECEIVED", Payload = "{}", ReceivedAt = DateTime.UtcNow
            },
            new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(), ProviderName = "INTERNAL_SECURITY_DEPOSIT",
                ProviderEventID = "settlement-notice-key", EventType = "SETTLEMENT_NOTICE",
                ProcessingStatus = "RECEIVED", Payload = "{}", ReceivedAt = DateTime.UtcNow
            });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    public sealed class StripeSandboxFactAttribute : FactAttribute
    {
        public StripeSandboxFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("ARCORA_DEPOSIT_TEST_STRIPE_KEY")?.StartsWith("sk_test_", StringComparison.Ordinal) != true)
                Skip = "Set ARCORA_DEPOSIT_TEST_STRIPE_KEY to explicitly authorize new isolated Stripe test payments/refunds.";
        }
    }

    private sealed class NoopEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string subject, string htmlBody) => Task.CompletedTask;
    }

    private sealed class FakeRefundProcessor : ISecurityDepositRefundProcessor
    {
        private readonly Guid depositID;

        public FakeRefundProcessor(Guid depositID)
        {
            this.depositID = depositID;
        }

        public int CallCount { get; private set; }
        public long ExpectedAmount { get; set; } = 40000;

        public Task<SecurityDepositProcessorRefund> CreateRefundAsync(
            string providerPaymentIntentID,
            long amountInMinorUnits,
            Guid requestedDepositID,
            string idempotencyKey,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            Assert.Equal("pi_test_payment", providerPaymentIntentID);
            Assert.Equal(ExpectedAmount, amountInMinorUnits);
            Assert.Equal(depositID, requestedDepositID);
            Assert.Contains(IdempotencyKey, idempotencyKey);
            return Task.FromResult(new SecurityDepositProcessorRefund(ProviderRefundID, "pending", null));
        }
    }

    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration)
        : ArcoraDbContext(options, configuration)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
        }
    }
}
