using System.Reflection;
using System.Threading.Channels;
using Arcora.Api;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Signing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Arcora.Api.Tests;

public sealed class SigningPreparationTests
{
    [Theory]
    [InlineData("PENDING_TENANT_SIGNATURE", false, false, "PENDING_FIRST_PAYMENT")]
    [InlineData("PENDING_FIRST_PAYMENT", true, false, "ACTIVE")]
    [InlineData("PENDING_TENANT_SIGNATURE", false, true, "PENDING_FIRST_PAYMENT")]
    [InlineData("CANCELLED", false, false, "CANCELLED")]
    [InlineData("EXPIRED", false, false, "EXPIRED")]
    [InlineData("ACTIVE", false, false, "ACTIVE")]
    public async Task CompletionReconcilesLeaseWithoutChargingOrReopeningClosedLeases(
        string originalStatus, bool firstPaymentPaid, bool otherPaymentPaid, string expectedStatus)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        await using var db = new TestContext(options, configuration);
        await db.Database.EnsureCreatedAsync();
        var lease = new Lease
        {
            LeaseID = Guid.NewGuid(), LeaseCode = "test-lease", LeaseNumber = "TEST-001", Status = originalStatus
        };
        db.Leases.Add(lease);
        if (firstPaymentPaid || otherPaymentPaid)
            db.PaymentIntents.Add(new PaymentIntent
            {
                PaymentIntentID = Guid.NewGuid(), LeaseID = lease.LeaseID,
                IdempotencyKey = firstPaymentPaid ? $"pi_firstcharge_lease_{lease.LeaseID:N}" : "unrelated-invoice",
                Status = "PAID", ProviderName = "STRIPE"
            });
        await db.SaveChangesAsync();
        var signedAt = DateTime.UtcNow.AddMinutes(-1);
        var requiresCollection = await LeaseSigningCompletion.ReconcileAsync(db, lease, signedAt, "TEST_REFRESH");
        await db.SaveChangesAsync();
        Assert.Equal(expectedStatus, lease.Status);
        Assert.Equal(expectedStatus == "PENDING_FIRST_PAYMENT", requiresCollection);
        if (requiresCollection || expectedStatus == "ACTIVE" && firstPaymentPaid)
            Assert.Equal(signedAt, lease.SignedAt);
        Assert.Equal(firstPaymentPaid || otherPaymentPaid ? 1 : 0, await db.PaymentIntents.CountAsync());
        Assert.Equal(requiresCollection, await LeaseSigningCompletion.ReconcileAsync(db, lease, signedAt, "TEST_REFRESH"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PreparationFailurePersistsFailedStatusInsteadOfLeavingRequestQueued(bool missingTemplate)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        var leaseID = Guid.NewGuid();
        var documentID = Guid.NewGuid();
        await using (var setup = new TestContext(options, configuration))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.LeaseDocuments.Add(new LeaseDocuments
            {
                LeaseDocumentID = documentID, LeaseID = leaseID, DocumentType = "LEASE_AGREEMENT",
                DocumentStatus = "QUEUED", OriginalFilename = "synthetic.pdf",
                StorageReference = "pending/synthetic.pdf", StorageProvider = "AZURE_BLOB"
            });
            await setup.SaveChangesAsync();
        }

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddScoped<ArcoraDbContext>(_ => new TestContext(options, configuration))
            .AddSingleton(Proxy<ILeaseContractTemplateService>((method, _) =>
            {
                Assert.Equal("RenderContract", method.Name);
                return Task.FromResult(missingTemplate ? null : new LeaseContractRenderResultDto { RenderedHtml = "<html>Test lease</html>" });
            }))
            .AddSingleton(Proxy<IHtmlToPdfService>((method, _) =>
            {
                Assert.Equal("ConvertHtmlToPdfWithAnchorsAsync", method.Name);
                throw new InvalidOperationException("Chromium is unavailable.");
            }))
            .AddSingleton(Proxy<ISignwellClient>((_, _) => throw new InvalidOperationException("Provider must not be called.")))
            .AddSingleton(Proxy<IFileStorageService>((_, _) => throw new InvalidOperationException("Storage must not be called.")));
        using var provider = services.BuildServiceProvider();
        var queue = new CompletedQueue();
        await queue.EnqueueAsync(new SigningWorkItem { LeaseId = leaseID, LeaseDocumentId = documentID, SigningRequestId = Guid.NewGuid() });
        queue.Complete();
        using var worker = new SigningQueueBackgroundService(queue, provider, NullLogger<SigningQueueBackgroundService>.Instance);
        await worker.StartAsync(CancellationToken.None);
        await worker.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(10));

        await using var verification = new TestContext(options, configuration);
        var document = await verification.LeaseDocuments.SingleAsync();
        Assert.Equal("FAILED", document.DocumentStatus);
        Assert.NotNull(document.UpdatedDate);
        Assert.Null(document.SentForSignatureAt);
    }

    private static T Proxy<T>(Func<MethodInfo, object?[], object?> handler) where T : class
    {
        var value = DispatchProxy.Create<T, InvocationProxy>();
        ((InvocationProxy)(object)value).Handler = handler;
        return value;
    }

    public class InvocationProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!, args!);
    }

    private sealed class CompletedQueue : ISigningQueue
    {
        private readonly Channel<SigningWorkItem> channel = Channel.CreateUnbounded<SigningWorkItem>();
        public ChannelReader<SigningWorkItem> Reader => channel.Reader;
        public ValueTask EnqueueAsync(SigningWorkItem item, CancellationToken cancellationToken = default)
            => channel.Writer.WriteAsync(item, cancellationToken);
        public void Complete() => channel.Writer.Complete();
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
