using Arcora.Api;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Arcora.Api.Tests;

public sealed class ReservationConcurrencyGuardTests : IDisposable
{
    private static readonly IConfiguration EmptyConfiguration = new ConfigurationBuilder().Build();
    private static readonly DateTime Start = new(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly SqliteConnection connection;
    private readonly DbContextOptions<ArcoraDbContext> options;
    private readonly Guid listingID = Guid.NewGuid();

    public ReservationConcurrencyGuardTests()
    {
        // Foreign keys are disabled so reservation rows can be tested without seeding the full listing graph.
        connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        connection.Open();
        options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        using var db = NewContext();
        CreateReservationTables(db);
    }

    // The full model uses SQL Server column types, so only the reservation tables are created from the generated script.
    private static void CreateReservationTables(ArcoraDbContext db)
    {
        var tables = new[] { typeof(Lease), typeof(ReservationHold), typeof(CalendarEvent), typeof(Preference) }
            .Select(t => Microsoft.EntityFrameworkCore.RelationalEntityTypeExtensions.GetTableName(db.Model.FindEntityType(t)!)!)
            .ToArray();
        var script = db.Database.GenerateCreateScript();
        var statements = tables
            .Select(t => System.Text.RegularExpressions.Regex.Match(script, $"CREATE TABLE \"{t}\" \\(.*?\\n\\);", System.Text.RegularExpressions.RegexOptions.Singleline))
            .Where(m => m.Success)
            .Select(m => System.Text.RegularExpressions.Regex.Replace(m.Value, @"\(max\)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            .ToList();
        Assert.Equal(tables.Length, statements.Count);
        foreach (var statement in statements)
            db.Database.ExecuteSqlRaw(statement);
    }

    public void Dispose() => connection.Dispose();

    private ArcoraDbContext NewContext() => new(options, EmptyConfiguration);

    [Fact]
    public async Task OverlappingHold_IsRejectedWith409_AndNothingIsSaved()
    {
        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start, Start.AddMonths(6))));

        var ex = await Assert.ThrowsAsync<ApiProblemException>(() =>
            SaveAsync(db => db.ReservationHolds.Add(Hold(Start.AddMonths(3), Start.AddMonths(9)))));

        Assert.Equal(409, ex.StatusCode);
        await using var verify = NewContext();
        Assert.Equal(1, await verify.ReservationHolds.CountAsync());
    }

    [Fact]
    public async Task AdjacentHold_IsAllowed()
    {
        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start, Start.AddMonths(6))));
        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start.AddMonths(6), Start.AddMonths(12))));
    }

    [Fact]
    public async Task SameRangeOnDifferentListing_IsAllowed()
    {
        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start, Start.AddMonths(6))));
        var other = Hold(Start, Start.AddMonths(6));
        other.ListingID = Guid.NewGuid();
        await SaveAsync(db => db.ReservationHolds.Add(other));
    }

    [Theory]
    [InlineData("RELEASED", false, false)]
    [InlineData("EXPIRED", false, false)]
    [InlineData("CANCELLED", false, false)]
    [InlineData("ACTIVE", true, false)]
    [InlineData("ACTIVE", false, true)]
    public async Task NonBlockingHold_DoesNotBlock(string status, bool released, bool expired)
    {
        var hold = Hold(Start, Start.AddMonths(6));
        hold.Status = status;
        hold.ReleasedAt = released ? DateTime.UtcNow : null;
        hold.ExpiresAt = expired ? DateTime.UtcNow.AddDays(-1) : hold.ExpiresAt;
        await SaveAsync(db => db.ReservationHolds.Add(hold));

        await SaveAsync(db => db.Leases.Add(Lease(Start, Start.AddMonths(12), "ACTIVE")));
    }

    [Fact]
    public async Task DraftOrCancelledLeaseAndNonBlockingCalendarEvents_DoNotBlock()
    {
        await SaveAsync(db =>
        {
            db.Leases.Add(Lease(Start, Start.AddMonths(12), "DRAFT"));
            db.Leases.Add(Lease(Start, Start.AddMonths(12), "CANCELLED"));
            db.CalendarEvents.Add(Event(Start, Start.AddMonths(12), blocks: false));
            var cancelled = Event(Start, Start.AddMonths(12));
            cancelled.Status = "CANCELLED";
            db.CalendarEvents.Add(cancelled);
        });

        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start, Start.AddMonths(6))));
    }

    [Fact]
    public async Task CalendarBlock_BlocksNewLease()
    {
        await SaveAsync(db => db.CalendarEvents.Add(Event(Start.AddMonths(2), Start.AddMonths(3))));

        var ex = await Assert.ThrowsAsync<ApiProblemException>(() =>
            SaveAsync(db => db.Leases.Add(Lease(Start, null, "ACTIVE"))));
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task HoldConversion_ReleasesHoldAndAddsLinkedLeaseAndCalendarBlock_InOneSave()
    {
        var hold = Hold(Start, Start.AddMonths(12));
        await SaveAsync(db => db.ReservationHolds.Add(hold));

        await using (var db = NewContext())
        {
            var tracked = await db.ReservationHolds.SingleAsync(h => h.ReservationHoldID == hold.ReservationHoldID);
            var lease = Lease(Start, Start.AddMonths(12), "PENDING_TENANT_SIGNATURE");
            db.Leases.Add(lease);
            tracked.Status = "RELEASED";
            tracked.ReleasedAt = DateTime.UtcNow;
            tracked.ConvertedToLeaseAt = DateTime.UtcNow;
            var evt = Event(lease.StartDate, lease.EndDate!.Value);
            evt.LeaseID = lease.LeaseID;
            evt.ReservationHoldID = hold.ReservationHoldID;
            db.CalendarEvents.Add(evt);

            await using var transaction = await db.Database.BeginTransactionAsync();
            await db.AcquireListingLockAsync(listingID);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        await using var verify = NewContext();
        Assert.Equal(1, await verify.Leases.CountAsync());
        Assert.Equal(1, await verify.CalendarEvents.CountAsync());
    }

    [Fact]
    public async Task LeaseWithoutReleasingHold_IsRejected()
    {
        await SaveAsync(db => db.ReservationHolds.Add(Hold(Start, Start.AddMonths(12))));

        await Assert.ThrowsAsync<ApiProblemException>(() =>
            SaveAsync(db => db.Leases.Add(Lease(Start, Start.AddMonths(12), "PENDING_TENANT_SIGNATURE"))));
    }

    [Fact]
    public async Task DuplicateCalendarEntriesForSameLease_AreAllowed_ButUnlinkedBlockIsRejected()
    {
        var lease = Lease(Start, Start.AddMonths(12), "ACTIVE");
        await SaveAsync(db => db.Leases.Add(lease));

        await SaveAsync(db =>
        {
            var first = Event(Start, Start.AddMonths(12));
            first.LeaseID = lease.LeaseID;
            var duplicate = Event(Start, Start.AddMonths(12));
            duplicate.LeaseID = lease.LeaseID;
            db.CalendarEvents.AddRange(first, duplicate);
        });

        await Assert.ThrowsAsync<ApiProblemException>(() =>
            SaveAsync(db => db.CalendarEvents.Add(Event(Start, Start.AddMonths(1)))));
    }

    [Fact]
    public async Task ModifiedHold_ExpandingIntoLease_IsRejected_ButShrinkingIsAllowed()
    {
        var hold = Hold(Start, Start.AddMonths(6));
        await SaveAsync(db =>
        {
            db.ReservationHolds.Add(hold);
            db.Leases.Add(Lease(Start.AddMonths(6), Start.AddMonths(12), "ACTIVE"));
        });

        await using (var db = NewContext())
        {
            var tracked = await db.ReservationHolds.SingleAsync();
            tracked.EndDate = Start.AddMonths(7);
            await Assert.ThrowsAsync<ApiProblemException>(() => db.SaveChangesAsync());
        }

        await using (var db = NewContext())
        {
            var tracked = await db.ReservationHolds.SingleAsync();
            tracked.EndDate = Start.AddMonths(5);
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task ReactivatingReleasedHold_IntoOverlap_IsRejected()
    {
        var hold = Hold(Start, Start.AddMonths(6));
        hold.Status = "RELEASED";
        hold.ReleasedAt = DateTime.UtcNow;
        await SaveAsync(db =>
        {
            db.ReservationHolds.Add(hold);
            db.Leases.Add(Lease(Start, Start.AddMonths(6), "ACTIVE"));
        });

        await using var db = NewContext();
        var tracked = await db.ReservationHolds.SingleAsync();
        tracked.Status = "ACTIVE";
        tracked.ReleasedAt = null;
        await Assert.ThrowsAsync<ApiProblemException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task InnocuousUpdate_OfLegacyOverlap_IsNotRevalidated()
    {
        var first = Lease(Start, Start.AddMonths(12), "ACTIVE");
        var second = Lease(Start, Start.AddMonths(12), "DRAFT");
        await SaveAsync(db => db.Leases.AddRange(first, second));
        await using (var db = NewContext())
        {
            // Simulate legacy data created before the guard existed.
            await db.Leases.Where(l => l.LeaseID == second.LeaseID)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.Status, "ACTIVE"));
        }

        await using (var db = NewContext())
        {
            var tracked = await db.Leases.SingleAsync(l => l.LeaseID == first.LeaseID);
            tracked.UpdatedBy = "maintenance";
            tracked.Status = "SIGNED";
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public void SynchronousSaveChanges_IsGuarded()
    {
        using (var db = NewContext())
        {
            db.ReservationHolds.Add(Hold(Start, Start.AddMonths(6)));
            db.SaveChanges();
        }

        using var second = NewContext();
        second.ReservationHolds.Add(Hold(Start, Start.AddMonths(6)));
        Assert.Throws<ApiProblemException>(() => second.SaveChanges());
    }

    [Fact]
    public async Task AcquireListingLock_RequiresTransaction()
    {
        await using var db = NewContext();
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.AcquireListingLockAsync(listingID));

        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.AcquireListingLockAsync(listingID);
        await ArcoraDbContext.AcquireListingLockAsync(db, listingID);
    }

    [Fact]
    public async Task UnrelatedSave_DoesNotOpenGuardTransaction()
    {
        await using var db = NewContext();
        db.Preferences.Add(new Preference());
        try { await db.SaveChangesAsync(); } catch (DbUpdateException) { }
        Assert.Null(db.Database.CurrentTransaction);
    }

    private async Task SaveAsync(Action<ArcoraDbContext> change)
    {
        await using var db = NewContext();
        change(db);
        await db.SaveChangesAsync();
    }

    private ReservationHold Hold(DateTime start, DateTime end) => new()
    {
        ReservationHoldID = Guid.NewGuid(),
        ListingID = listingID,
        StartDate = start,
        EndDate = end,
        Status = "ACTIVE",
        ExpiresAt = DateTime.UtcNow.AddDays(7)
    };

    private Lease Lease(DateTime start, DateTime? end, string status) => new()
    {
        LeaseID = Guid.NewGuid(),
        OrganizationID = Guid.NewGuid(),
        ListingID = listingID,
        RentalUnitID = Guid.NewGuid(),
        TenancyTypeID = 1,
        TenantID = Guid.NewGuid(),
        LeaseCode = "L-" + Guid.NewGuid().ToString("N")[..8],
        LeaseNumber = "N-" + Guid.NewGuid().ToString("N")[..8],
        Status = status,
        StartDate = start,
        EndDate = end,
        Currency = "CAD"
    };

    private CalendarEvent Event(DateTime start, DateTime end, bool blocks = true) => new()
    {
        CalendarEventID = Guid.NewGuid(),
        ListingID = listingID,
        EventType = "BLOCK",
        Status = "CONFIRMED",
        StartAt = start,
        EndAt = end,
        IsAllDay = true,
        BlocksAvailability = blocks
    };
}

/// <summary>
/// Real SQL Server lock test. Runs only when ARCORA_GUARD_TEST_SQLSERVER points at a disposable
/// database (it is created and deleted by the test); otherwise it returns immediately.
/// </summary>
public sealed class ReservationConcurrencyGuardSqlServerTests
{
    [SqlServerFact]
    public async Task ConcurrentOverlappingHolds_SecondWaitsForLockThenFails()
    {
        var connectionString = Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        var testConnection = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
        if (!testConnection.InitialCatalog.StartsWith("ArcoraGuardTests_", StringComparison.Ordinal))
            throw new InvalidOperationException("The concurrency test requires a disposable database named ArcoraGuardTests_*. Never use an application database.");

        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(connectionString).Options;
        var configuration = new ConfigurationBuilder().Build();
        await using (var setup = new ArcoraDbContext(options, configuration))
        {
            await setup.Database.EnsureDeletedAsync();
            await setup.Database.EnsureCreatedAsync();
        }

        try
        {
            var listingID = Guid.NewGuid();
            ReservationHold NewHold() => new()
            {
                ReservationHoldID = Guid.NewGuid(),
                ListingID = listingID,
                StartDate = new DateTime(2027, 1, 1),
                EndDate = new DateTime(2027, 7, 1),
                Status = "ACTIVE",
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await using var first = new ArcoraDbContext(options, configuration);
            await using var firstTx = await first.Database.BeginTransactionAsync();
            await first.AcquireListingLockAsync(listingID);

            var secondTask = Task.Run(async () =>
            {
                await using var second = new ArcoraDbContext(options, configuration);
                second.ReservationHolds.Add(NewHold());
                await second.SaveChangesAsync();
            });

            await Task.Delay(500);
            Assert.False(secondTask.IsCompleted);

            // FK constraints are real here, so insert via the lock-holding transaction with checks disabled.
            await first.Database.ExecuteSqlRawAsync("ALTER TABLE [ReservationHolds] NOCHECK CONSTRAINT ALL");
            first.ReservationHolds.Add(NewHold());
            await first.SaveChangesAsync();
            await firstTx.CommitAsync();

            var ex = await Assert.ThrowsAsync<ApiProblemException>(() => secondTask);
            Assert.Equal(409, ex.StatusCode);
        }
        finally
        {
            await using var cleanup = new ArcoraDbContext(options, configuration);
            await cleanup.Database.EnsureDeletedAsync();
        }
    }
}
