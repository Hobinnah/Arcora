using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arcora.Api;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Email;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Arcora.Api.Services;
using Arcora.Api.Services.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class TenantInvitationSafetyTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private readonly IDataProtectionProvider protection = new EphemeralDataProtectionProvider();
    private ArcoraDbContext db = null!;
    private TenantInvitationService service = null!;
    private const string SigningKey = "test-only-invitation-signing-key";

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JwtSettings:Key"] = SigningKey
        }).Build();
        db = new SqliteTestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, config);
        await db.Database.EnsureCreatedAsync();
        var mapper = new MapperConfiguration(c => c.CreateMap<TenantInvitation, TenantInvitationDto>(MemberList.None),
            NullLoggerFactory.Instance).CreateMapper();
        service = new TenantInvitationService(mapper, new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheConfiguration()), NullLogger<TenantInvitationService>.Instance,
            null!, db, null!, null!, null!, config, null!, protection);
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Theory]
    [InlineData("Full access", false, "ACTIVE", false, true)]
    [InlineData("FULL ACCESS", false, "active", false, true)]
    [InlineData("Calendar access", false, "ACTIVE", false, false)]
    [InlineData("Calendar and message access", false, "ACTIVE", false, false)]
    [InlineData("OWNER", true, "ACTIVE", false, true)]
    [InlineData("Full access", false, "INVITED", false, false)]
    [InlineData("Full access", false, "ACTIVE", true, false)]
    public async Task ManagementRequiresActiveAuthorizedMembership(string role, bool owner, string status, bool deactivated, bool expected)
    {
        var org = Guid.NewGuid();
        db.OrganizationMembers.Add(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(), OrganizationID = org, UserID = 7,
            RoleName = role, IsPrimaryOwner = owner, Status = status,
            DeactivatedAt = deactivated ? DateTime.UtcNow : null
        });
        await db.SaveChangesAsync();
        Assert.Equal(expected, await InvitationAuthorization.CanManageAsync(db, org, 7));
        Assert.False(await InvitationAuthorization.CanManageAsync(db, Guid.NewGuid(), 7));
        Assert.False(await InvitationAuthorization.CanManageAsync(db, org, 8));
    }

    [Fact]
    public async Task CreationRejectsNonMemberBeforeCreatingAnything()
    {
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.CreateTenantInvitationWithHold(new CreateTenantInvitationRequestDto
            {
                OrganizationID = Guid.NewGuid(), ListingID = Guid.NewGuid(),
                LeaseTermMonths = 6, Email = "tenant@example.test"
            }, 7));
        Assert.Equal(403, error.StatusCode);
        Assert.Empty(await db.TenantInvitations.ToListAsync());
        Assert.Empty(await db.ReservationHolds.ToListAsync());
        Assert.Empty(await db.TenantInvitationEmails.ToListAsync());
    }

    [Theory]
    [InlineData("REVOKED")]
    [InlineData("DECLINED")]
    [InlineData("EXPIRED")]
    public async Task TerminalInvitationCannotBeAccepted(string status)
    {
        var (invitation, token) = await SeedInvitationAsync(status);
        var response = await service.RespondToTenantInvitationAsync(token, "ACCEPT");
        Assert.False(response.Success);
        Assert.Equal(status, invitation.Status);
        await Assert.ThrowsAsync<ApiProblemException>(() => service.CreateLeaseFromTenantInvitationAsync(
            invitation.TenantInvitationID, Guid.NewGuid(), 7, invitation.Email!, "7"));
        Assert.False(await db.Leases.AnyAsync());
    }

    [Fact]
    public async Task SameResponseIsIdempotentButOppositeResponseFails()
    {
        var (invitation, token) = await SeedInvitationAsync("ACCEPTED");
        Assert.True((await service.RespondToTenantInvitationAsync(token, "ACCEPT")).Success);
        Assert.False((await service.RespondToTenantInvitationAsync(token, "DECLINE")).Success);
        Assert.Equal("ACCEPTED", invitation.Status);
    }

    [Fact]
    public async Task PendingInvitationAcceptsOnce()
    {
        var (invitation, token) = await SeedInvitationAsync("PENDING");
        Assert.True((await service.RespondToTenantInvitationAsync(token, "ACCEPT")).Success);
        Assert.Equal("ACCEPTED", invitation.Status);
        Assert.NotNull(invitation.AcceptedAt);
    }

    [Fact]
    public async Task ExpiredPendingInvitationCannotAccept()
    {
        var (invitation, token) = await SeedInvitationAsync("PENDING", expired: true);
        Assert.False((await service.RespondToTenantInvitationAsync(token, "ACCEPT")).Success);
        Assert.Equal("EXPIRED", invitation.Status);
    }

    [Fact]
    public async Task LandlordCannotReviveRevokedInvitationThroughStatusEndpoint()
    {
        var (invitation, _) = await SeedInvitationAsync("REVOKED");
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.UpdateTenantInvitationStatus(invitation.TenantInvitationID, "Revert"));
        Assert.Equal(400, error.StatusCode);
        Assert.Equal("REVOKED", invitation.Status);
    }

    [Fact]
    public async Task DeliveryFailurePersistsJobAndRetryDoesNotCreateAnotherInvitation()
    {
        var (invitation, _) = await SeedInvitationAsync("PENDING");
        var hold = await SeedHoldAsync(invitation);
        var email = SeedEmail(invitation);
        await db.SaveChangesAsync();
        using var services = DeliveryServices(new TestSender(fail: true));
        await TenantInvitationEmailWorker.DispatchAsync(services, invitation.TenantInvitationID, default);
        Assert.Equal("FAILED", email.Status);
        Assert.Equal(1, email.Attempts);
        Assert.NotEmpty(email.ProtectedMessage);
        Assert.True(email.NextAttemptAt > DateTime.UtcNow);
        Assert.Equal("ACTIVE", hold.Status);
        Assert.Null(hold.ReleasedAt);

        email.NextAttemptAt = DateTime.UtcNow.AddSeconds(-1);
        await db.SaveChangesAsync();
        using var retryServices = DeliveryServices(new TestSender(fail: false));
        await TenantInvitationEmailWorker.DispatchAsync(retryServices, invitation.TenantInvitationID, default);
        Assert.Equal("SENT", email.Status);
        Assert.NotNull(email.SentAt);
        Assert.Empty(email.ProtectedMessage);
        Assert.Single(await db.TenantInvitations.ToListAsync());
        Assert.Single(await db.TenantInvitationEmails.ToListAsync());
        Assert.Single(await db.ReservationHolds.ToListAsync());
    }

    [Theory]
    [InlineData("REVOKED")]
    [InlineData("DECLINED")]
    public async Task RevocationAndDeclineReleaseHoldWithoutAllowingLaterAcceptance(string status)
    {
        var (invitation, token) = await SeedInvitationAsync("PENDING");
        var hold = await SeedHoldAsync(invitation);
        if (status == "REVOKED")
            await service.UpdateTenantInvitationStatus(invitation.TenantInvitationID, status);
        else
            Assert.True((await service.RespondToTenantInvitationAsync(token, "DECLINE")).Success);
        Assert.Equal("RELEASED", hold.Status);
        Assert.NotNull(hold.ReleasedAt);
        Assert.False((await service.RespondToTenantInvitationAsync(token, "ACCEPT")).Success);
        Assert.Equal(status, invitation.Status);
    }

    [Theory]
    [InlineData("REVOKED", false)]
    [InlineData("PENDING", true)]
    public async Task UnusableInvitationCancelsDelivery(string status, bool expired)
    {
        var (invitation, _) = await SeedInvitationAsync(status, expired);
        var email = SeedEmail(invitation);
        await db.SaveChangesAsync();
        var sender = new TestSender(fail: false);
        using var services = DeliveryServices(sender);
        await TenantInvitationEmailWorker.DispatchAsync(services, invitation.TenantInvitationID, default);
        Assert.Equal("CANCELLED", email.Status);
        Assert.Equal(0, sender.Calls);
    }

    [SqlServerFact]
    public async Task SqlServerCommittedRevocationCannotBeOverwrittenByWaitingAcceptance()
    {
        var connectionString = Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString)) return;
        var sqlConnection = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = $"ArcoraInvitationSafety_{Guid.NewGuid():N}"
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JwtSettings:Key"] = SigningKey
        }).Build();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(sqlConnection.ConnectionString).Options;
        await using var setup = new ArcoraDbContext(options, config);
        await setup.Database.EnsureCreatedAsync();
        try
        {
            var id = Guid.NewGuid();
            var expires = DateTime.UtcNow.AddDays(1);
            var payload = $"{id:N}.tenant@example.test.{new DateTimeOffset(expires).ToUnixTimeSeconds()}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SigningKey));
            var token = Base64Url(Encoding.UTF8.GetBytes($"{payload}.{Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes($"tenant-invite:{payload}")))}"));
            var invitation = new TenantInvitation
            {
                TenantInvitationID = id, ListingID = Guid.NewGuid(),
                Email = "tenant@example.test", Status = "PENDING", ExpiresAt = expires,
                InvitationPurpose = "Direct lease invitation",
                TokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)))
            };
            setup.TenantInvitations.Add(invitation);
            await setup.SaveChangesAsync();
            await using var transaction = await setup.Database.BeginTransactionAsync();
            await setup.AcquireListingLockAsync(invitation.ListingID.Value);

            var waitingAcceptance = Task.Run(async () =>
            {
                await using var second = new ArcoraDbContext(options, config);
                var mapper = new MapperConfiguration(c => c.CreateMap<TenantInvitation, TenantInvitationDto>(MemberList.None),
                    NullLoggerFactory.Instance).CreateMapper();
                var secondService = new TenantInvitationService(mapper, new MemoryCache(new MemoryCacheOptions()),
                    Options.Create(new CacheConfiguration()), NullLogger<TenantInvitationService>.Instance,
                    null!, second, null!, null!, null!, config, null!, protection);
                return await secondService.RespondToTenantInvitationAsync(token, "ACCEPT");
            });
            await Task.Delay(300);
            Assert.False(waitingAcceptance.IsCompleted);
            invitation.Status = "REVOKED";
            invitation.RevokedAt = DateTime.UtcNow;
            await setup.SaveChangesAsync();
            await transaction.CommitAsync();
            Assert.False((await waitingAcceptance).Success);
            await setup.Entry(invitation).ReloadAsync();
            Assert.Equal("REVOKED", invitation.Status);
        }
        finally
        {
            await setup.Database.EnsureDeletedAsync();
        }
    }

    [SqlServerFact]
    public async Task SqlServerConcurrentEmailWorkersSendOnce()
    {
        var connectionString = Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString)) return;
        var sqlConnection = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = $"ArcoraInvitationEmailSafety_{Guid.NewGuid():N}"
        };
        var config = new ConfigurationBuilder().Build();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(sqlConnection.ConnectionString).Options;
        await using var setup = new ArcoraDbContext(options, config);
        await setup.Database.EnsureCreatedAsync();
        try
        {
            var invitation = new TenantInvitation
            {
                TenantInvitationID = Guid.NewGuid(), Email = "tenant@example.test", Status = "PENDING",
                ExpiresAt = DateTime.UtcNow.AddDays(1), InvitationPurpose = "Direct lease invitation", TokenHash = "test-hash"
            };
            setup.TenantInvitations.Add(invitation);
            setup.TenantInvitationEmails.Add(new TenantInvitationEmail
            {
                TenantInvitationID = invitation.TenantInvitationID, NextAttemptAt = DateTime.UtcNow.AddSeconds(-1),
                ProtectedMessage = protection.CreateProtector(TenantInvitationEmailWorker.ProtectionPurpose)
                    .Protect(JsonSerializer.Serialize(new EmailMessage("tenant@example.test", "Invitation", "Private link")))
            });
            await setup.SaveChangesAsync();
            var sender = new TestSender(fail: false);
            async Task Deliver()
            {
                await using var context = new ArcoraDbContext(options, config);
                using var services = new ServiceCollection().AddSingleton(context).AddSingleton(protection)
                    .AddSingleton<IEmailSender>(sender)
                    .AddSingleton<Microsoft.Extensions.Logging.ILogger<TenantInvitationEmailWorker>>(NullLogger<TenantInvitationEmailWorker>.Instance)
                    .BuildServiceProvider();
                await TenantInvitationEmailWorker.DispatchAsync(services, invitation.TenantInvitationID, default);
            }

            await Task.WhenAll(Task.Run(Deliver), Task.Run(Deliver));
            Assert.Equal(1, sender.Calls);
            var email = await setup.TenantInvitationEmails.AsNoTracking().SingleAsync();
            Assert.Equal("SENT", email.Status);
            Assert.Equal(1, email.Attempts);
        }
        finally
        {
            await setup.Database.EnsureDeletedAsync();
        }
    }

    private async Task<ReservationHold> SeedHoldAsync(TenantInvitation invitation)
    {
        invitation.ListingID = Guid.NewGuid();
        var hold = new ReservationHold
        {
            ReservationHoldID = Guid.NewGuid(), ListingID = invitation.ListingID.Value,
            StartDate = DateTime.UtcNow.Date.AddMonths(1), EndDate = DateTime.UtcNow.Date.AddMonths(7),
            ExpiresAt = invitation.ExpiresAt, Status = "ACTIVE",
            HoldReason = $"Tenant invitation {invitation.TenantInvitationID} | persisted terms"
        };
        db.ReservationHolds.Add(hold);
        await db.SaveChangesAsync();
        return hold;
    }

    private TenantInvitationEmail SeedEmail(TenantInvitation invitation)
    {
        var email = new TenantInvitationEmail
        {
            TenantInvitationID = invitation.TenantInvitationID,
            NextAttemptAt = DateTime.UtcNow.AddSeconds(-1),
            ProtectedMessage = protection.CreateProtector(TenantInvitationEmailWorker.ProtectionPurpose)
                .Protect(JsonSerializer.Serialize(new EmailMessage("tenant@example.test", "Invitation", "<p>Private link</p>")))
        };
        db.TenantInvitationEmails.Add(email);
        return email;
    }

    private ServiceProvider DeliveryServices(TestSender sender) => new ServiceCollection()
        .AddSingleton(db).AddSingleton(protection).AddSingleton<IEmailSender>(sender)
        .AddSingleton<Microsoft.Extensions.Logging.ILogger<TenantInvitationEmailWorker>>(NullLogger<TenantInvitationEmailWorker>.Instance)
        .BuildServiceProvider();

    private async Task<(TenantInvitation Invitation, string Token)> SeedInvitationAsync(string status, bool expired = false)
    {
        var id = Guid.NewGuid();
        var expires = DateTime.UtcNow.AddDays(expired ? -1 : 1);
        var payload = $"{id:N}.tenant@example.test.{new DateTimeOffset(expires).ToUnixTimeSeconds()}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SigningKey));
        var signature = Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes($"tenant-invite:{payload}")));
        var token = Base64Url(Encoding.UTF8.GetBytes($"{payload}.{signature}"));
        var invitation = new TenantInvitation
        {
            TenantInvitationID = id, Email = "tenant@example.test", Status = status,
            InvitationPurpose = "Direct lease invitation", ExpiresAt = expires,
            TokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)))
        };
        db.TenantInvitations.Add(invitation);
        await db.SaveChangesAsync();
        return (invitation, token);
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');

    private sealed class TestSender(bool fail) : IEmailSender
    {
        private int calls;
        public int Calls => calls;
        public Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            Interlocked.Increment(ref calls);
            return fail ? Task.FromException(new InvalidOperationException("Simulated SMTP outage")) : Task.CompletedTask;
        }

    }

    private sealed class SqliteTestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration)
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
