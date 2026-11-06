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
using System.Linq;

namespace Arcora.Api.Tests;

public sealed class RentalUnitPropertyScopedTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private readonly ServiceProvider cacheProvider = CreateCacheProvider();
    private ArcoraDbContext db = null!;
    private RentalUnitService service = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();
        var mapper = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<RentalUnit, RentalUnitDto>().ReverseMap();
        }, NullLoggerFactory.Instance).CreateMapper();
        service = new RentalUnitService(mapper, cacheProvider.GetRequiredService<HybridCache>(),
            Options.Create(new CacheConfiguration()), NullLogger<RentalUnitService>.Instance,
            new RentalUnitRepository(db));
    }

    public async Task DisposeAsync()
    {
        await cacheProvider.DisposeAsync();
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task UnitNumbersAreUniqueWithinAPropertyAndExistingUnitsAreReturned()
    {
        var firstProperty = Guid.NewGuid();
        var secondProperty = Guid.NewGuid();
        db.RentalUnits.Add(new RentalUnit
        {
            RentalUnitID = Guid.NewGuid(),
            PropertyID = firstProperty,
            UnitNumber = "101",
            Status = "ACTIVE"
        });
        await db.SaveChangesAsync();

        var created = await service.CreateRentalUnit(new RentalUnitDto
        {
            PropertyID = secondProperty,
            UnitNumber = "101",
            Status = "ACTIVE"
        });
        var repeated = await service.CreateRentalUnit(new RentalUnitDto
        {
            PropertyID = secondProperty,
            UnitNumber = "101",
            Status = "ACTIVE"
        });

        Assert.NotEqual(Guid.Empty, created.RentalUnitID);
        Assert.Equal(secondProperty, created.PropertyID);
        Assert.Equal(created.RentalUnitID, repeated.RentalUnitID);
        Assert.Equal(2, await db.RentalUnits.CountAsync());
    }

    private static ServiceProvider CreateCacheProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddHybridCache();
        return services.BuildServiceProvider();
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