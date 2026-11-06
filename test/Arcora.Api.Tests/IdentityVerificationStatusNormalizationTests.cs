using Arcora.Api;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class IdentityVerificationStatusNormalizationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:;Foreign Keys=False");
    private IdentityVerificationStatusTestDbContext _db = null!;
    private readonly ServiceProvider _cacheProvider = CreateCacheProvider();

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(_connection).Options;
        _db = new IdentityVerificationStatusTestDbContext(options, new ConfigurationBuilder().Build());
        await _db.Database.EnsureCreatedAsync();

        _db.Users.Add(new User
        {
            Id = 42,
            UserName = "status-user",
            NormalizedUserName = "STATUS-USER",
            Email = "status@example.test",
            NormalizedEmail = "STATUS@EXAMPLE.TEST",
            SecurityStamp = Guid.NewGuid().ToString()
        });
        await _db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _cacheProvider.DisposeAsync();
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task UpdateIdentityVerificationStatus_Compeleted_NormalizesToApprovedAndSetsVerifiedAt()
    {
        var entity = new IdentityVerification
        {
            IdentityVerificationID = Guid.NewGuid(),
            UserID = 42,
            VerificationType = "IDENTITY",
            Status = "PENDING",
            ProviderName = "PERSONA",
            ProviderReferenceID = "inq_1",
            CapturedDate = DateTime.UtcNow
        };

        _db.IdentityVerifications.Add(entity);
        await _db.SaveChangesAsync();

        var service = CreateService();
        var result = await service.UpdateIdentityVerificationStatus(entity.IdentityVerificationID, "completed");

        Assert.NotNull(result);
        Assert.Equal("APPROVED", result!.Status);

        var reloaded = await _db.IdentityVerifications.FindAsync(entity.IdentityVerificationID);
        Assert.NotNull(reloaded);
        Assert.NotNull(reloaded!.VerifiedAt);
        Assert.Equal("APPROVED", reloaded.Status);
    }

    [Fact]
    public async Task UpdateProviderVerificationStatus_UnknownStatus_NormalizesToPending()
    {
        var entity = new IdentityVerification
        {
            IdentityVerificationID = Guid.NewGuid(),
            UserID = 42,
            VerificationType = "IDENTITY",
            Status = "PENDING",
            ProviderName = "PERSONA",
            ProviderReferenceID = "inq_2",
            CapturedDate = DateTime.UtcNow
        };

        _db.IdentityVerifications.Add(entity);
        await _db.SaveChangesAsync();

        var service = CreateService();
        var result = await service.UpdateProviderVerificationStatusAsync(
            "PERSONA",
            "inq_2",
            "unrecognized-provider-status",
            null);

        Assert.NotNull(result);
        Assert.Equal("PENDING", result!.Status);
    }

    [Fact]
    public async Task UpdateProviderVerificationStatus_PersonaCompleted_RemainsSubmittedUntilApproved()
    {
        var entity = new IdentityVerification
        {
            IdentityVerificationID = Guid.NewGuid(),
            UserID = 42,
            VerificationType = "IDENTITY",
            Status = "PENDING",
            ProviderName = "PERSONA",
            ProviderReferenceID = "inq_3",
            CapturedDate = DateTime.UtcNow
        };

        _db.IdentityVerifications.Add(entity);
        await _db.SaveChangesAsync();

        var result = await CreateService().UpdateProviderVerificationStatusAsync(
            "PERSONA",
            "inq_3",
            "completed",
            null);

        Assert.NotNull(result);
        Assert.Equal("SUBMITTED", result!.Status);
        Assert.Null(result.VerifiedAt);
    }

    private IdentityVerificationService CreateService()
    {
        var mapper = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<IdentityVerification, IdentityVerificationDto>();
            configuration.CreateMap<IdentityVerificationDto, IdentityVerification>();
        }, NullLoggerFactory.Instance).CreateMapper();

        return new IdentityVerificationService(
            mapper,
            _cacheProvider.GetRequiredService<HybridCache>(),
            Options.Create(new CacheConfiguration { ExpirationTimeInMinutes = 5 }),
            NullLogger<IdentityVerificationService>.Instance,
            new IdentityVerificationRepository(_db),
            userManager: null!,
            identityVerificationProviderFactory: null!);
    }

    private static ServiceProvider CreateCacheProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddHybridCache();
        return services.BuildServiceProvider();
    }

    private sealed class IdentityVerificationStatusTestDbContext(
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
