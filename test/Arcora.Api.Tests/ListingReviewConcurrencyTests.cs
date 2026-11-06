using System.Reflection;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Realtime;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class ListingReviewConcurrencyTests : IAsyncLifetime
{
    private readonly string connectionString =
        $"Data Source=listing-review-{Guid.NewGuid():N};Mode=Memory;Cache=Shared;Foreign Keys=False;Default Timeout=10";
    private SqliteConnection anchorConnection = null!;
    private ServiceProvider cacheProvider = null!;
    private HybridCache cache = null!;
    private Guid listingID;
    private long firstReviewerID;
    private long secondReviewerID;
    private FakeEmailSender emailSender = null!;

    public async Task InitializeAsync()
    {
        anchorConnection = new SqliteConnection(connectionString);
        await anchorConnection.OpenAsync();
        cacheProvider = CreateCacheProvider();
        cache = cacheProvider.GetRequiredService<HybridCache>();
        emailSender = new FakeEmailSender();

        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();

        var organizationID = Guid.NewGuid();
        listingID = Guid.NewGuid();
        firstReviewerID = 101;
        secondReviewerID = 102;
        db.Users.AddRange(NewUser(1, "host@example.test"), NewUser(firstReviewerID, "first-reviewer@example.test"),
            NewUser(secondReviewerID, "second-reviewer@example.test"));
        db.OrganizationMembers.Add(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(),
            OrganizationID = organizationID,
            UserID = 1,
            RoleName = "LandLord",
            Status = "ACTIVE",
            IsPrimaryOwner = true,
            CapturedDate = DateTime.UtcNow
        });
        db.Listings.Add(new Listing
        {
            ListingID = listingID,
            RentalUnitID = Guid.NewGuid(),
            ListingTypeID = Guid.NewGuid(),
            OrganizationID = organizationID,
            Title = "Test listing",
            BaseMonthlyRentAmount = 1000,
            SecurityDepositAmount = 500,
            YearBuilt = 2000,
            Status = ListingReviewService.PendingReviewStatus,
            Currency = "CAD",
            MinimumLeaseMonths = 1,
            MaximumLeaseMonths = 12,
            Notes = string.Empty,
            AcceptingApplications = true
        });
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await cacheProvider.DisposeAsync();
        await anchorConnection.DisposeAsync();
    }

    [Fact]
    public async Task ConcurrentDecisions_OnlyOneReviewerCommitsAndNotifiesHost()
    {
        await using var firstDb = CreateDbContext();
        await using var secondDb = CreateDbContext();
        var firstService = CreateService(firstDb);
        var secondService = CreateService(secondDb);

        var firstDecision = firstService.DecideAsync(listingID, new ListingReviewDecisionRequest
        {
            Decision = "APPROVED"
        }, firstReviewerID);
        var secondDecision = secondService.DecideAsync(listingID, new ListingReviewDecisionRequest
        {
            Decision = "REJECTED",
            Message = "Please correct the listing details."
        }, secondReviewerID);

        var outcomes = await Task.WhenAll(CaptureDecisionAsync(firstDecision), CaptureDecisionAsync(secondDecision));

        Assert.Single(outcomes, outcome => outcome.Decision is not null);
        Assert.Single(outcomes, outcome => outcome.Error is InvalidOperationException);
        Assert.Equal(1, emailSender.SendCount);

        await using var verifyDb = CreateDbContext();
        var savedListing = await verifyDb.Listings.AsNoTracking().SingleAsync(item => item.ListingID == listingID);
        Assert.Contains(savedListing.Status, new[] { "APPROVED", "REJECTED" });
        Assert.NotNull(savedListing.ReviewedAt);
        Assert.Contains(savedListing.ReviewedByUserID, new long?[] { firstReviewerID, secondReviewerID });
        Assert.Equal(1, await verifyDb.Conversations.CountAsync());
        Assert.Equal(1, await verifyDb.ConversationParticipants.CountAsync());
        Assert.Equal(1, await verifyDb.ConversationMessages.CountAsync());
    }

    private async Task<(ListingReviewDecisionResultDto? Decision, Exception? Error)> CaptureDecisionAsync(
        Task<ListingReviewDecisionResultDto?> decisionTask)
    {
        try
        {
            return (await decisionTask, null);
        }
        catch (Exception exception)
        {
            return (null, exception);
        }
    }

    private ListingReviewService CreateService(ArcoraDbContext db)
    {
        var listingService = DispatchProxy.Create<IListingService, TestListingService>();
        ((TestListingService)(object)listingService).Listing = new ListingDto
        {
            ListingID = listingID,
            Title = "Test listing"
        };
        return new ListingReviewService(db, listingService, cache, emailSender, new ThrowingHubContext(),
            Options.Create(new EmailSettings()), NullLogger<ListingReviewService>.Instance);
    }

    private ArcoraDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connectionString).Options;
        return new ReviewTestDbContext(options, new ConfigurationBuilder().Build());
    }

    private static User NewUser(long id, string email) => new()
    {
        Id = id,
        UserName = email,
        NormalizedUserName = email.ToUpperInvariant(),
        Email = email,
        NormalizedEmail = email.ToUpperInvariant(),
        FirstName = "Test",
        LastName = "User",
        DisplayName = "Test User",
        SecurityStamp = Guid.NewGuid().ToString(),
        ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    private static ServiceProvider CreateCacheProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHybridCache();
        return services.BuildServiceProvider();
    }

    private sealed class ReviewTestDbContext(
        DbContextOptions<ArcoraDbContext> options,
        IConfiguration configuration)
        : ArcoraDbContext(options, configuration)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
        }
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        private int sendCount;

        public int SendCount => Volatile.Read(ref sendCount);

        public Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            Interlocked.Increment(ref sendCount);
            return Task.CompletedTask;
        }
    }

    public class TestListingService : DispatchProxy
    {
        public ListingDto? Listing { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => targetMethod?.Name == nameof(IListingService.GetID)
                ? Task.FromResult(Listing)
                : throw new NotSupportedException(targetMethod?.Name);
    }

    private sealed class ThrowingHubContext : IHubContext<MessagingHub>
    {
        public IHubClients Clients => throw new InvalidOperationException("SignalR is not configured in this test.");
        public IGroupManager Groups => throw new InvalidOperationException("SignalR is not configured in this test.");
    }
}
