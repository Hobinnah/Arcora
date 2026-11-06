using System.Reflection;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs.DtoProfiles;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Arcora.Payments.Stripe;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class RecurringRentCollectionTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private readonly List<CreateChargeRequest> charges = [];
    private readonly Queue<Func<ProviderChargeResult>> outcomes = [];
    private readonly FrozenClock clock = new();
    private DbContextOptions<ArcoraDbContext> options = null!;
    private ServiceProvider services = null!;
    private Lease lease = null!;
    private bool sqlServer;
    private bool createdDatabase;

    public async Task InitializeAsync()
    {
        try
        {
            await InitializeCoreAsync();
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    private async Task InitializeCoreAsync()
    {
        var sql = Environment.GetEnvironmentVariable("ARCORA_RECURRING_TEST_SQLSERVER");
        sqlServer = !string.IsNullOrWhiteSpace(sql);
        if (sqlServer)
        {
            var builder = new SqlConnectionStringBuilder(sql);
            if (!builder.InitialCatalog.StartsWith("ArcoraRecurringTests_", StringComparison.Ordinal) ||
                !Guid.TryParseExact(builder.InitialCatalog["ArcoraRecurringTests_".Length..], "N", out _))
                throw new InvalidOperationException("Only a new ArcoraRecurringTests_<guid N> database is allowed.");
            options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(sql).Options;
        }
        else
        {
            await connection.OpenAsync();
            options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        }
        await using var db = NewContext();
        if (sqlServer && await db.Database.CanConnectAsync())
            throw new InvalidOperationException("The SQL test database must not already exist.");
        createdDatabase = await db.Database.EnsureCreatedAsync();
        if (sqlServer)
            // Only synthetic payment graphs are needed; never run against an existing database.
            await db.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
        lease = new Lease
        {
            LeaseID = Guid.NewGuid(), LeaseCode = "recurring-synthetic", LeaseNumber = "RECURRING-TEST",
            TenantID = Guid.NewGuid(), OrganizationID = Guid.NewGuid(), RentalUnitID = Guid.NewGuid(),
            ListingID = Guid.NewGuid(), BaseRentAmount = 1500m, Currency = "CAD", Status = "ACTIVE",
            StartDate = new DateTime(2026, 9, 30)
        };
        db.Leases.Add(lease);
        db.Tenants.Add(new Tenant { TenantID = lease.TenantID, UserID = 42, Code = "TEST", Description = "Synthetic recurring tenant" });
        foreach (var role in new[] { "PRIMARY", "BACKUP" })
        {
            var method = new PaymentMethod
            {
                PaymentMethodID = Guid.NewGuid(), TenantID = lease.TenantID, MethodRole = role,
                PaymentMethodType = role == "PRIMARY" ? "PAD" : "CARD", IsActive = true,
                VerificationStatus = "VERIFIED", ProviderName = "STRIPE",
                ProviderCustomerID = "cus_synthetic", ProviderPaymentMethodID = $"pm_synthetic_{role}"
            };
            db.PaymentMethods.Add(method);
            if (role == "PRIMARY")
                db.AutopayMandates.Add(new AutopayMandate
                {
                    AutopayMandateID = Guid.NewGuid(), TenantID = lease.TenantID,
                    PaymentMethodID = method.PaymentMethodID, PaymentRail = "PAD", Status = "ACTIVE",
                    ProviderName = "STRIPE", ProviderMandateID = "mandate_synthetic", ConsentVersion = "test"
                });
        }
        AddFee(db, "Host Placement Fee", "PLATFORM-HOST-PLACEMENT", 99.99m);
        AddFee(db, "Host Active Unit Subscription Fee", "PLATFORM-HOST-UNIT-SUBSCRIPTION", 24.99m);
        await db.SaveChangesAsync();
        var provider = DispatchProxy.Create<IPaymentProvider, ProviderProxy>();
        ((ProviderProxy)(object)provider).Charge = request =>
        {
            charges.Add(request);
            return outcomes.Count == 0 ? Result(FailureCategory.Pending) : outcomes.Dequeue()();
        };
        services = BuildServices(provider);
    }

    private ServiceProvider BuildServices(IPaymentProvider provider)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHybridCache();
        return serviceCollection
            .AddScoped(_ => NewContext())
            .AddScoped<ILeaseRepository, LeaseRepository>()
            .AddScoped<IPaymentIntentRepository, PaymentIntentRepository>()
            .AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>()
            .AddScoped<IPaymentMethodRepository, PaymentMethodRepository>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<IAutopayMandateRepository, AutopayMandateRepository>()
            .AddSingleton(provider)
            .AddSingleton<IMapper>(new MapperConfiguration(c => c.AddMaps(typeof(PaymentIntentProfile).Assembly),
                NullLoggerFactory.Instance).CreateMapper())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<RentCollectionOrchestrator>>(NullLogger<RentCollectionOrchestrator>.Instance)
            .AddScoped<IRentCollectionOrchestrator, RentCollectionOrchestrator>()
            .BuildServiceProvider();
    }

    [StripeRecurringSandboxFact]
    public Task StripeSandboxDueMonthlyPadSettlesWithOnlyMonthlyHostFee() => RunStripeSandbox(false);

    [StripeRecurringSandboxFact]
    public Task StripeSandboxDefinitivePadFailureRecoversWithRealBackupCard() => RunStripeSandbox(true);

    private async Task RunStripeSandbox(bool failRecurringPad)
    {
        var key = Environment.GetEnvironmentVariable("ARCORA_RECURRING_TEST_STRIPE_KEY")!;
        Assert.True(key.StartsWith("sk_test_", StringComparison.Ordinal));
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        var ct = timeout.Token;
        var client = new Stripe.StripeClient(key);
        var customers = new Stripe.CustomerService(client);
        var customer = await customers.CreateAsync(new Stripe.CustomerCreateOptions
        {
            Name = "Arcora synthetic recurring integration",
            Metadata = new() { ["arcora_isolated_recurring_test"] = lease.LeaseID.ToString("N") }
        }, cancellationToken: ct);
        Assert.False(customer.Livemode);
        try
        {
            var provider = new StripePaymentProvider(Options.Create(new StripeOptions { SecretKey = key }),
                NullLogger<StripePaymentProvider>.Instance);
            await services.DisposeAsync();
            services = BuildServices(provider);
            var setup = await SetupSandboxPad(client, customer.Id, "pm_acssDebit_success", ct);
            var card = await new Stripe.PaymentMethodService(client).CreateAsync(new Stripe.PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new Stripe.PaymentMethodCardOptions { Token = "tok_visa" }
            }, cancellationToken: ct);
            Assert.False(card.Livemode);
            await provider.SetupPaymentMethodAsync(new SetupPaymentMethodRequest(
                customer.Id, PaymentMethodKind.Card, card.Id, $"recurring_card_{lease.LeaseID:N}"), ct);
            await SaveSandboxMethods(customer.Id, setup.PaymentMethodId, setup.MandateId, card.Id);

            Guid firstId = Guid.Empty;
            await WithOrchestrator(async x =>
            {
                var first = await x.InitiateCollectionAsync(lease.LeaseID, lease.TenantID, Guid.Empty,
                    lease.BaseRentAmount, $"pi_firstcharge_lease_{lease.LeaseID:N}", ct);
                firstId = first.PaymentIntentID!.Value;
            });
            await ReconcileSandboxIntent(client, provider, firstId, ct);
            await using (var firstDb = NewContext())
            {
                var firstIntent = await firstDb.PaymentIntents.SingleAsync(ct);
                Assert.Equal("PAID", firstIntent.Status);
                var firstActual = await new Stripe.PaymentIntentService(client).GetAsync(
                    firstIntent.ProviderPaymentIntentID, cancellationToken: ct);
                Assert.False(firstActual.Livemode);
                Assert.Equal(customer.Id, firstActual.CustomerId);
                Assert.Equal("succeeded", firstActual.Status);
                Assert.Equal(150000L, firstActual.AmountReceived);
                var firstAccounting = await firstDb.RentPaymentAccountings.SingleAsync(ct);
                Assert.Equal(99.99m, firstAccounting.HostPlacementFee);
                Assert.Equal(24.99m, firstAccounting.HostMonthlyFee);
                Assert.NotNull(firstAccounting.SettledAt);
            }

            if (failRecurringPad)
            {
                var failedSetup = await SetupSandboxPad(client, customer.Id, "pm_acssDebit_insufficientFunds", ct);
                await SaveSandboxMethods(customer.Id, failedSetup.PaymentMethodId, failedSetup.MandateId, card.Id);
            }

            await RunCycle();
            Guid recurringId;
            await using (var dueDb = NewContext())
                recurringId = (await dueDb.PaymentIntents.SingleAsync(x =>
                    x.IdempotencyKey == $"pi_{lease.LeaseID:N}_202610", ct)).PaymentIntentID;
            await ReconcileSandboxIntent(client, provider, recurringId, ct);
            await RunCycle();
            await RunCycle();
            await using var db = NewContext();
            var recurring = await db.PaymentIntents.SingleAsync(x => x.PaymentIntentID == recurringId, ct);
            Assert.Equal("PAID", recurring.Status);
            var attempts = await db.PaymentAttempts.Where(x => x.PaymentIntentID == recurringId)
                .OrderBy(x => x.AttemptNumber).ToListAsync(ct);
            Assert.Equal(failRecurringPad ? new[] { "PAD", "CARD" } : new[] { "PAD" }, attempts.Select(x => x.MethodKind));
            foreach (var attempt in attempts)
            {
                var actual = await new Stripe.PaymentIntentService(client).GetAsync(attempt.ProviderAttemptID, cancellationToken: ct);
                Assert.False(actual.Livemode);
                Assert.Equal(customer.Id, actual.CustomerId);
                Assert.Equal(150000L, actual.Amount);
                if (attempt.MethodKind == "PAD" && failRecurringPad)
                {
                    Assert.Equal("requires_payment_method", actual.Status);
                    Assert.NotNull(actual.LastPaymentError);
                    Assert.Contains("insufficient_funds", new[]
                    {
                        actual.LastPaymentError.Code, actual.LastPaymentError.DeclineCode
                    });
                }
                else
                {
                    Assert.Equal("succeeded", actual.Status);
                    Assert.Equal(150000L, actual.AmountReceived);
                }
            }
            var accounting = await db.RentPaymentAccountings.SingleAsync(x => x.PaymentIntentID == recurringId, ct);
            Assert.Equal(0m, accounting.HostPlacementFee);
            Assert.Equal(24.99m, accounting.HostMonthlyFee);
            Assert.Equal(1475.01m, accounting.HostNetAmount);
            Assert.Equal(0m, accounting.DepositAmount);
            Assert.Equal(0m, accounting.TenantFeeAmount);
            Assert.NotNull(accounting.SettledAt);
            Assert.Equal(2, await db.Payments.CountAsync(ct));
            var allAccounting = await db.RentPaymentAccountings.ToListAsync(ct);
            Assert.Equal(99.99m, allAccounting.Sum(x => x.HostPlacementFee));
            Assert.Equal(49.98m, allAccounting.Sum(x => x.HostMonthlyFee));
            var providerPayments = await new Stripe.PaymentIntentService(client).ListAsync(
                new Stripe.PaymentIntentListOptions { Customer = customer.Id, Limit = 10 }, cancellationToken: ct);
            Assert.Equal(failRecurringPad ? 3 : 2, providerPayments.Data.Count);
        }
        finally
        {
            await customers.DeleteAsync(customer.Id);
        }
    }

    private static async Task<Stripe.SetupIntent> SetupSandboxPad(Stripe.StripeClient client, string customer,
        string token, CancellationToken ct)
    {
        var setup = await new Stripe.SetupIntentService(client).CreateAsync(new Stripe.SetupIntentCreateOptions
        {
            Customer = customer, PaymentMethod = token, PaymentMethodTypes = ["acss_debit"],
            Confirm = true, Usage = "off_session",
            MandateData = new Stripe.SetupIntentMandateDataOptions
            {
                CustomerAcceptance = new Stripe.SetupIntentMandateDataCustomerAcceptanceOptions
                {
                    Type = "online",
                    Online = new Stripe.SetupIntentMandateDataCustomerAcceptanceOnlineOptions
                    {
                        IpAddress = "127.0.0.1", UserAgent = "Arcora isolated Stripe sandbox regression"
                    }
                }
            },
            PaymentMethodOptions = new Stripe.SetupIntentPaymentMethodOptionsOptions
            {
                AcssDebit = new Stripe.SetupIntentPaymentMethodOptionsAcssDebitOptions
                {
                    Currency = "cad",
                    MandateOptions = new Stripe.SetupIntentPaymentMethodOptionsAcssDebitMandateOptionsOptions
                    {
                        PaymentSchedule = "interval", IntervalDescription = "Synthetic monthly rent test",
                        TransactionType = "personal"
                    }
                }
            }
        }, cancellationToken: ct);
        Assert.False(setup.Livemode);
        Assert.Equal("succeeded", setup.Status);
        Assert.False(string.IsNullOrWhiteSpace(setup.MandateId));
        return setup;
    }

    private async Task SaveSandboxMethods(string customer, string pad, string mandate, string card)
    {
        await using var db = NewContext();
        foreach (var method in await db.PaymentMethods.ToListAsync())
        {
            method.ProviderCustomerID = customer;
            method.ProviderPaymentMethodID = method.MethodRole == "PRIMARY" ? pad : card;
        }
        (await db.AutopayMandates.SingleAsync()).ProviderMandateID = mandate;
        await db.SaveChangesAsync();
    }

    private async Task ReconcileSandboxIntent(Stripe.StripeClient client, IPaymentProvider provider, Guid id, CancellationToken ct)
    {
        for (var poll = 0; poll < 30; poll++)
        {
            await using var db = NewContext();
            var intent = await db.PaymentIntents.AsNoTracking().SingleAsync(x => x.PaymentIntentID == id, ct);
            if (intent.Status == "PAID")
                return;
            Assert.False(string.IsNullOrWhiteSpace(intent.ProviderPaymentIntentID));
            var actual = await new Stripe.PaymentIntentService(client).GetAsync(intent.ProviderPaymentIntentID, cancellationToken: ct);
            Assert.False(actual.Livemode);
            var result = await provider.GetChargeStatusAsync(actual.Id, ct);
            await WithOrchestrator(x => x.HandlePadResultAsync(id, result.Status, result.FailureCode, result.FailureMessage, ct));
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }
        Assert.Fail("Stripe did not reach a final successful outcome within the bounded polling window; no provider time was advanced.");
    }

    public sealed class StripeRecurringSandboxFactAttribute : FactAttribute
    {
        public StripeRecurringSandboxFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("ARCORA_RECURRING_TEST_STRIPE_KEY")?.StartsWith("sk_test_", StringComparison.Ordinal) != true)
                Skip = "Set ARCORA_RECURRING_TEST_STRIPE_KEY to authorize new isolated Stripe test customers and rent debits.";
        }
    }

    [Fact]
    public async Task DueRentSurvivesRepeatedCyclesAndWorkerRestartWithoutAnotherDebit()
    {
        await RunCycle();
        await RunCycle();
        await using var db = NewContext();
        var intent = await db.PaymentIntents.SingleAsync();
        Assert.Equal($"pi_{lease.LeaseID:N}_202610", intent.IdempotencyKey);
        Assert.Equal("PROCESSING", intent.Status);
        Assert.Equal(1500m, intent.Amount);
        Assert.Single(charges);
        Assert.Equal(PaymentMethodKind.Pad, charges[0].Kind);
        Assert.Equal(150000, charges[0].AmountInMinorUnits);
        Assert.Equal("mandate_synthetic", charges[0].MandateId);
        Assert.Single(await db.PaymentAttempts.ToListAsync());
        Assert.Empty(await db.Payments.ToListAsync());
        Assert.Null((await db.RentPaymentAccountings.SingleAsync()).SettledAt);
    }

    [Theory]
    [InlineData(FailureCategory.Temporary)]
    [InlineData(FailureCategory.Permanent)]
    public async Task DefinitivePadFailureUsesCardOnceAndSettlesOnce(FailureCategory failure)
    {
        outcomes.Enqueue(() => Result(failure));
        outcomes.Enqueue(() => Result(FailureCategory.None, "pi_card_synthetic"));
        await RunCycle();
        await RunCycle();
        var id = await IntentId();
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "succeeded", null, null));
        await WithOrchestrator(async x => Assert.False(await x.ChargeBackupCardAsync(id)));
        await using var db = NewContext();
        Assert.Equal(new[] { PaymentMethodKind.Pad, PaymentMethodKind.Card }, charges.Select(x => x.Kind));
        Assert.Equal(2, charges.Select(x => x.IdempotencyKey).Distinct().Count());
        Assert.Equal("PAID", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Single(await db.Payments.ToListAsync());
        Assert.Equal(2, await db.PaymentAttempts.CountAsync());
        Assert.NotNull((await db.RentPaymentAccountings.SingleAsync()).SettledAt);
    }

    [Fact]
    public async Task BothMethodsFailLeavesOverdueAndDoesNotRetryOnRestart()
    {
        outcomes.Enqueue(() => Result(FailureCategory.Temporary));
        outcomes.Enqueue(() => Result(FailureCategory.Permanent, "pi_card_declined"));
        await RunCycle();
        await RunCycle();
        var id = await IntentId();
        await WithOrchestrator(x => x.HandlePadResultAsync(id,
            "requires_payment_method", "insufficient_funds", "Synthetic decline"));
        await using var db = NewContext();
        Assert.Equal("OVERDUE", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Equal(2, charges.Count);
        Assert.Equal(2, await db.PaymentAttempts.CountAsync());
        Assert.Empty(await db.Payments.ToListAsync());
        Assert.Null((await db.RentPaymentAccountings.SingleAsync()).SettledAt);
    }

    [Fact]
    public async Task LostPadResponseIsUnknownAndNeverDebitsCardOrRetries()
    {
        outcomes.Enqueue(() => throw new HttpRequestException("Synthetic lost provider response"));
        await RunCycle();
        await RunCycle();
        await using var db = NewContext();
        Assert.Equal("PROCESSING", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Equal("UNKNOWN", (await db.PaymentAttempts.SingleAsync()).Status);
        Assert.Single(charges);
        Assert.Empty(await db.Payments.ToListAsync());
        await WithOrchestrator(async x => Assert.False(await x.ChargeBackupCardAsync(await IntentId())));
    }

    [Fact]
    public async Task PendingPadWebhookThenDefinitiveFailureTriggersOnlyOneCard()
    {
        await RunCycle();
        var id = await IntentId();
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "processing", null, null));
        Assert.Single(charges);
        outcomes.Enqueue(() => Result(FailureCategory.None, "pi_card_recovery"));
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "requires_payment_method", "insufficient_funds", "Synthetic decline"));
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "requires_payment_method", "insufficient_funds", "Repeated synthetic decline"));
        await RunCycle();
        Assert.Equal(2, charges.Count);
        await using var db = NewContext();
        Assert.Equal("PAID", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Single(await db.Payments.ToListAsync());
    }

    [Fact]
    public async Task PendingPadSuccessUpdatesMatchingAttemptAndDuplicateIsIdempotent()
    {
        await RunCycle();
        var id = await IntentId();
        await using (var db = NewContext())
        {
            Assert.Equal("PROCESSING", (await db.PaymentAttempts.SingleAsync()).Status);
            Assert.Equal("ISSUED", (await db.InvoiceMasters.SingleAsync()).Status);
            Assert.Equal(1500m, (await db.InvoiceDetails.SingleAsync()).TotalLineAmount);
            Assert.Empty(await db.PaymentAllocations.ToListAsync());
        }

        await WithOrchestrator(x => x.HandlePadResultAsync(id, "succeeded", null, null));
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "succeeded", null, null));

        await using var verified = NewContext();
        var attempt = await verified.PaymentAttempts.SingleAsync();
        Assert.Equal("SUCCEEDED", attempt.Status);
        Assert.Equal("NONE", attempt.FailureCategory);
        Assert.Null(attempt.FailureCode);
        Assert.Null(attempt.FailureMessage);
        Assert.NotNull(attempt.CompletedAt);
        Assert.Equal("PAID", (await verified.PaymentIntents.SingleAsync()).Status);
        Assert.Single(await verified.Payments.ToListAsync());
        Assert.Equal("PAID", (await verified.InvoiceMasters.SingleAsync()).Status);
        Assert.Equal(1500m, (await verified.PaymentAllocations.ToListAsync()).Sum(x => x.AllocatedAmount));
        Assert.Single(charges);
    }

    [Fact]
    public async Task LatePadFailureDoesNotRetryCardAfterBackupAttemptStarts()
    {
        await RunCycle();
        var id = await IntentId();
        outcomes.Enqueue(() => Result(FailureCategory.Pending, "pi_card_pending"));
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "requires_payment_method",
            "insufficient_funds", "Synthetic decline"));
        await WithOrchestrator(x => x.HandlePadResultAsync(id, "requires_payment_method",
            "insufficient_funds", "Repeated synthetic decline", providerPaymentIntentId: "pi_pad_synthetic"));

        await using var db = NewContext();
        Assert.Equal(2, charges.Count);
        Assert.Equal(2, await db.PaymentAttempts.CountAsync());
        Assert.Equal("PROCESSING", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Equal("FAILED", (await db.PaymentAttempts.SingleAsync(x => x.MethodKind == "PAD")).Status);
        Assert.Equal("PROCESSING", (await db.PaymentAttempts.SingleAsync(x => x.MethodKind == "CARD")).Status);
    }

    [Fact]
    public async Task UncertainBackupOutcomeNeverRetriesEitherMethod()
    {
        outcomes.Enqueue(() => Result(FailureCategory.Permanent));
        outcomes.Enqueue(() => throw new HttpRequestException("Synthetic lost card response"));
        await RunCycle();
        await RunCycle();
        await WithOrchestrator(async x => Assert.False(await x.ChargeBackupCardAsync(await IntentId())));
        await using var db = NewContext();
        Assert.Equal("BACKUP_PROCESSING", (await db.PaymentIntents.SingleAsync()).Status);
        Assert.Equal(2, charges.Count);
        Assert.Equal("UNKNOWN", (await db.PaymentAttempts.SingleAsync(x => x.MethodKind == "CARD")).Status);
        Assert.Empty(await db.Payments.ToListAsync());
    }

    [Theory]
    [InlineData("DRAFT", 2026, 9, 30, null, null, null, 12)]
    [InlineData("ACTIVE", 2026, 10, 30, null, null, null, 12)]
    [InlineData("ACTIVE", 2026, 11, 30, null, null, null, 12)]
    [InlineData("ACTIVE", 2026, 9, 29, null, null, null, 12)]
    [InlineData("ACTIVE", 2026, 9, 30, 2026, 10, 29, 12)]
    [InlineData("ACTIVE", 2026, 9, 30, null, null, null, 8)]
    public async Task NotDueOrInactiveLeaseNeverCharges(string status, int year, int month, int day,
        int? endYear, int? endMonth, int? endDay, int hour)
    {
        await using var db = NewContext();
        var saved = await db.Leases.SingleAsync();
        saved.Status = status;
        saved.StartDate = new DateTime(year, month, day);
        saved.EndDate = endYear.HasValue ? new DateTime(endYear.Value, endMonth!.Value, endDay!.Value) : null;
        await db.SaveChangesAsync();
        clock.Now = new DateTimeOffset(2026, 10, 30, hour, 0, 0, TimeSpan.Zero);
        await RunCycle();
        Assert.Empty(charges);
        Assert.Empty(await db.PaymentIntents.ToListAsync());
    }

    [Fact]
    public async Task MonthEndAnniversaryClampsToShortMonth()
    {
        await using var db = NewContext();
        (await db.Leases.SingleAsync()).StartDate = new DateTime(2026, 1, 31);
        await db.SaveChangesAsync();
        clock.Now = new DateTimeOffset(2026, 2, 28, 12, 0, 0, TimeSpan.Zero);
        await RunCycle();
        Assert.Single(charges);
        Assert.Equal($"pi_{lease.LeaseID:N}_202602", (await db.PaymentIntents.SingleAsync()).IdempotencyKey);
    }

    [Fact]
    public async Task UnrelatedKeyContainingCurrentPeriodDoesNotSuppressRent()
    {
        await using var db = NewContext();
        db.PaymentIntents.Add(new PaymentIntent
        {
            PaymentIntentID = Guid.NewGuid(), LeaseID = lease.LeaseID, TenantID = lease.TenantID,
            Amount = 1m, Currency = "CAD", Status = "FAILED", ProviderName = "STRIPE",
            ScheduledChargeAt = DateTime.UtcNow, IdempotencyKey = "unrelated_202610_operation"
        });
        await db.SaveChangesAsync();
        await RunCycle();
        Assert.Single(charges);
        Assert.Equal(2, await db.PaymentIntents.CountAsync());
    }

    private async Task RunCycle()
    {
        using var worker = new RecurringRentCollectionService(NullLogger<RecurringRentCollectionService>.Instance,
            services, Options.Create(new RentCollectionOptions { Enabled = false, ChargeHourUtc = 9 }), timeProvider: clock);
        await (Task)typeof(RecurringRentCollectionService).GetMethod("RunOnceAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(worker, new object[] { CancellationToken.None })!;
    }

    private async Task WithOrchestrator(Func<IRentCollectionOrchestrator, Task> action)
    {
        using var scope = services.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<IRentCollectionOrchestrator>());
    }

    private async Task<Guid> IntentId()
    {
        await using var db = NewContext();
        return (await db.PaymentIntents.SingleAsync()).PaymentIntentID;
    }

    private ArcoraDbContext NewContext() => sqlServer
        ? new ArcoraDbContext(options, new ConfigurationBuilder().Build())
        : new SqliteContext(options, new ConfigurationBuilder().Build());

    private static ProviderChargeResult Result(FailureCategory category, string id = "pi_pad_synthetic") =>
        new(id, null, category == FailureCategory.None ? "succeeded" : category == FailureCategory.Pending ? "processing" : "requires_payment_method",
            category, category == FailureCategory.Temporary ? "insufficient_funds" : category == FailureCategory.Permanent ? "account_closed" : null, null);

    private static void AddFee(ArcoraDbContext db, string name, string code, decimal amount)
    {
        var type = new FeeType { Name = name, IsPlatformFee = true };
        db.FeeTypes.Add(type);
        db.Fees.Add(new Fee
        {
            FeeID = Guid.NewGuid(), FeeType = type, Name = name, Code = code, IsActive = true,
            Currency = "CAD", CalculationType = "FIXED", FixedAmount = amount
        });
    }

    public class ProviderProxy : DispatchProxy
    {
        public Func<CreateChargeRequest, ProviderChargeResult> Charge { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Assert.Equal(nameof(IPaymentProvider.CreateChargeAsync), targetMethod!.Name);
            return Task.FromResult(Charge((CreateChargeRequest)args![0]!));
        }
    }

    private sealed class FrozenClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class SqliteContext(DbContextOptions<ArcoraDbContext> options, IConfiguration config) : ArcoraDbContext(options, config)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
            foreach (var property in builder.Entity<RentPaymentAccounting>().Metadata.GetProperties().Where(x => x.ClrType == typeof(decimal)))
                builder.Entity<RentPaymentAccounting>().Property<decimal>(property.Name).HasConversion<double>();
        }
    }

    public async Task DisposeAsync()
    {
        if (services != null)
            await services.DisposeAsync();
        if (sqlServer && createdDatabase)
        {
            await using var db = NewContext();
            await db.Database.EnsureDeletedAsync();
        }
        await connection.DisposeAsync();
    }
}
