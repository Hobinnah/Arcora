using Arcora.Api;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Implementations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Arcora.Api.Tests;

public sealed class IdentityVerificationRepositoryTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private IdentityVerificationTestDbContext db = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        db = new IdentityVerificationTestDbContext(options, new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task GetByUserIDAsync_FiltersRecordsInDatabase()
    {
        db.Users.AddRange(NewUser(101), NewUser(202));
        db.IdentityVerifications.AddRange(
            NewVerification(101, "PENDING"),
            NewVerification(101, "APPROVED"),
            NewVerification(202, "APPROVED"));
        await db.SaveChangesAsync();

        var repository = new IdentityVerificationRepository(db);
        var records = await repository.GetByUserIDAsync(101);

        Assert.Equal(2, records.Count);
        Assert.All(records, record => Assert.Equal(101, record.UserID));
    }

    private static User NewUser(long id) => new()
    {
        Id = id,
        UserName = $"user{id}",
        NormalizedUserName = $"USER{id}",
        Email = $"user{id}@example.test",
        NormalizedEmail = $"USER{id}@EXAMPLE.TEST",
        SecurityStamp = Guid.NewGuid().ToString()
    };

    private static IdentityVerification NewVerification(long userID, string status) => new()
    {
        IdentityVerificationID = Guid.NewGuid(),
        UserID = userID,
        VerificationType = "IDENTITY",
        Status = status,
        CapturedDate = DateTime.UtcNow
    };

    private sealed class IdentityVerificationTestDbContext(
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
}