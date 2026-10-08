using System.Reflection;
using Arcora.Api;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        organizationMemberRepository = CreateMembershipRepository(new OrganizationMember
        {
            OrganizationID = organizationID,
            UserID = 7,
            RoleName = "OWNER",
            Status = "ACTIVE",
            IsPrimaryOwner = true
        });
        service = new HostingSecurityDepositService(
            db,
            organizationMemberRepository,
            null!,
            null!,
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

    private static IOrganizationMemberRepository CreateMembershipRepository(OrganizationMember member)
    {
        var repository = DispatchProxy.Create<IOrganizationMemberRepository, MembershipRepositoryProxy>();
        ((MembershipRepositoryProxy)(object)repository).Member = member;
        return repository;
    }

    public class MembershipRepositoryProxy : DispatchProxy
    {
        public OrganizationMember Member { get; set; } = null!;

        protected override object? Invoke(MethodInfo? method, object?[]? args)
        {
            if (method?.Name == nameof(IOrganizationMemberRepository.GetMemberOrganizationsAsync))
                return Task.FromResult(new List<OrganizationMember> { Member });
            throw new InvalidOperationException($"Unexpected repository call: {method?.Name}");
        }
    }

    private sealed class FakeRefundProcessor : ISecurityDepositRefundProcessor
    {
        private readonly Guid depositID;

        public FakeRefundProcessor(Guid depositID)
        {
            this.depositID = depositID;
        }

        public int CallCount { get; private set; }

        public Task<SecurityDepositProcessorRefund> CreateRefundAsync(
            string providerPaymentIntentID,
            long amountInMinorUnits,
            Guid requestedDepositID,
            string idempotencyKey,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            Assert.Equal("pi_test_payment", providerPaymentIntentID);
            Assert.Equal(40000, amountInMinorUnits);
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
