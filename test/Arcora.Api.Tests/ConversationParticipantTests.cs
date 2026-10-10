using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.DTOs.DtoProfiles;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Repositories.Interfaces;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class ConversationParticipantTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private readonly MemoryCache cache = new(new MemoryCacheOptions());
    private ArcoraDbContext db = null!;
    private ConversationParticipantService service = null!;
    private IMapper mapper = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options,
            new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
        mapper = new MapperConfiguration(config => config.AddProfile<ConversationParticipantProfile>(),
            NullLoggerFactory.Instance).CreateMapper();
        service = new ConversationParticipantService(mapper, cache, Options.Create(new CacheConfiguration()),
            NullLogger<ConversationParticipantService>.Instance, new ConversationParticipantRepository(db));
    }

    public async Task DisposeAsync()
    {
        cache.Dispose();
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    private static ConversationParticipantDto Request(Guid conversationID, Guid tenantID) => new()
    {
        ConversationID = conversationID, TenantID = tenantID,
        ParticipantRole = "TENANT", JoinedAt = DateTime.UtcNow, IsMuted = false
    };

    [Fact]
    public async Task DifferentTenantsWithSameRoleAreSeparateParticipants()
    {
        var conversationID = Guid.NewGuid();
        var first = await service.CreateConversationParticipant(Request(conversationID, Guid.NewGuid()));
        var second = await service.CreateConversationParticipant(Request(conversationID, Guid.NewGuid()));
        Assert.NotEqual(first.ConversationParticipantID, second.ConversationParticipantID);
        Assert.NotEqual(first.TenantID, second.TenantID);
        Assert.Equal(2, await db.ConversationParticipants.CountAsync());
    }

    [Fact]
    public async Task RetryReusesIdentityWithoutOverwritingReadOrMuteState()
    {
        var request = Request(Guid.NewGuid(), Guid.NewGuid());
        var first = await service.CreateConversationParticipant(request);
        var entity = await db.ConversationParticipants.SingleAsync();
        entity.LastReadAt = DateTime.UtcNow;
        entity.IsMuted = true;
        await db.SaveChangesAsync();
        var retry = await service.CreateConversationParticipant(request);
        Assert.Equal(first.ConversationParticipantID, retry.ConversationParticipantID);
        Assert.True(retry.IsMuted);
        Assert.Equal(entity.LastReadAt, retry.LastReadAt);
        Assert.Equal(1, await db.ConversationParticipants.CountAsync());
    }

    [Fact]
    public async Task SameIdentityCanJoinDifferentConversations()
    {
        var tenantID = Guid.NewGuid();
        var first = await service.CreateConversationParticipant(Request(Guid.NewGuid(), tenantID));
        var second = await service.CreateConversationParticipant(Request(Guid.NewGuid(), tenantID));
        Assert.NotEqual(first.ConversationParticipantID, second.ConversationParticipantID);
    }

    [Fact]
    public async Task ConflictingSuppliedIdentitiesReturn409()
    {
        var request = Request(Guid.NewGuid(), Guid.NewGuid());
        request.UserID = 10;
        await service.CreateConversationParticipant(request);
        request.TenantID = Guid.NewGuid();
        var error = await Assert.ThrowsAsync<ApiProblemException>(() => service.CreateConversationParticipant(request));
        Assert.Equal(409, error.StatusCode);
        Assert.Equal(1, await db.ConversationParticipants.CountAsync());
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public async Task InvalidIdentityReturns400(bool missingConversation, bool missingIdentity, bool negativeUser)
    {
        var request = Request(missingConversation ? Guid.Empty : Guid.NewGuid(),
            missingIdentity ? Guid.Empty : Guid.NewGuid());
        if (negativeUser) request.UserID = -1;
        var error = await Assert.ThrowsAsync<ApiProblemException>(() => service.CreateConversationParticipant(request));
        Assert.Equal(400, error.StatusCode);
        Assert.Empty(await db.ConversationParticipants.ToListAsync());
    }

    [Theory]
    [InlineData("tenant")]
    [InlineData("member")]
    [InlineData("user")]
    public async Task DatabaseRejectsDuplicateIdentityWithinConversation(string identity)
    {
        var conversationID = Guid.NewGuid();
        var identityID = Guid.NewGuid();
        ConversationParticipant Entity() => new()
        {
            ConversationParticipantID = Guid.NewGuid(), ConversationID = conversationID,
            TenantID = identity == "tenant" ? identityID : null,
            OrganizationMemberID = identity == "member" ? identityID : null,
            UserID = identity == "user" ? 10 : null, JoinedAt = DateTime.UtcNow
        };
        db.ConversationParticipants.Add(Entity());
        await db.SaveChangesAsync();
        db.ConversationParticipants.Add(Entity());
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void RequestMappingDoesNotAttachNavigationGraphs()
    {
        var entity = mapper.Map<ConversationParticipant>(new ConversationParticipantDto
        {
            ConversationID = Guid.NewGuid(), TenantID = Guid.NewGuid(),
            Conversation = new ConversationDto(), Tenant = new TenantDto(),
            User = new User(), OrganizationMember = new OrganizationMemberDto()
        });
        Assert.Null(entity.Conversation);
        Assert.Null(entity.Tenant);
        Assert.Null(entity.User);
        Assert.Null(entity.OrganizationMember);
    }

    [Fact]
    public async Task UpdateUsesRouteIDWhenPayloadIDIsMissing()
    {
        var request = Request(Guid.NewGuid(), Guid.NewGuid());
        var created = await service.CreateConversationParticipant(request);
        request.LastReadAt = DateTime.UtcNow;
        var updated = await service.UpdateConversationParticipant(created.ConversationParticipantID!.Value, request);
        Assert.Equal(created.ConversationParticipantID, updated!.ConversationParticipantID);
        Assert.Equal(request.LastReadAt, updated.LastReadAt);
    }

    [Fact]
    public void SqlServerMigrationCreatesAllThreeFilteredUniqueIndexes()
    {
        using var sqlContext = new ArcoraDbContext(
            new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(
                "Server=localhost;Database=ParticipantMigrationValidation;Integrated Security=true;TrustServerCertificate=true").Options,
            new ConfigurationBuilder().Build());
        var script = sqlContext.GetService<IMigrator>().GenerateScript(
            "20261008000200_AddTenantInvitationQuote", "20261009000100_AddConversationParticipantIdentityUniqueness");
        foreach (var identity in new[] { "UserID", "TenantID", "OrganizationMemberID" })
        {
            Assert.Contains($"CREATE UNIQUE INDEX [IX_ConversationParticipants_ConversationID_{identity}]", script);
            Assert.Contains($"WHERE [{identity}] IS NOT NULL", script);
        }
    }

    [Fact]
    public async Task ConversationListFailuresAreNotReturnedAsEmptySuccesses()
    {
        await db.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => service.GetAll(new Arcora.Api.Models.Paging()));
        var conversations = new ConversationService(mapper, cache, Options.Create(new CacheConfiguration()),
            NullLogger<ConversationService>.Instance, new ConversationRepository(db));
        var messages = new ConversationMessageService(mapper, cache, Options.Create(new CacheConfiguration()),
            NullLogger<ConversationMessageService>.Instance, new ConversationMessageRepository(db));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => conversations.GetAll(new Arcora.Api.Models.Paging()));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => messages.GetAll(new Arcora.Api.Models.Paging()));
    }

    [SqlServerFact]
    public async Task ConcurrentSqlServerCreatesReturnTheSamePersistedParticipant()
    {
        var connectionString = new SqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER"))
        {
            InitialCatalog = $"ArcoraParticipantConcurrency_{Guid.NewGuid():N}"
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(connectionString).Options;
        var configuration = new ConfigurationBuilder().Build();
        await using var schema = new ArcoraDbContext(options, configuration);
        await schema.Database.EnsureCreatedAsync();
        try
        {
            var conversationID = Guid.NewGuid();
            schema.Conversations.Add(new Conversation { ConversationID = conversationID });
            schema.Users.Add(new User { Id = 10, UserName = "participant@example.test" });
            await schema.SaveChangesAsync();
            var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var reached = 0;
            async Task WaitForBoth()
            {
                if (Interlocked.Increment(ref reached) == 2) gate.SetResult();
                await gate.Task.WaitAsync(TimeSpan.FromSeconds(20));
            }
            await using var firstContext = new ArcoraDbContext(options, configuration);
            await using var secondContext = new ArcoraDbContext(options, configuration);
            ConversationParticipantService Service(ArcoraDbContext context) => new(
                mapper, cache, Options.Create(new CacheConfiguration()),
                NullLogger<ConversationParticipantService>.Instance, new RacingRepository(context, WaitForBoth));
            ConversationParticipantDto CreateRequest() => new()
            {
                ConversationID = conversationID, UserID = 10, ParticipantRole = "USER",
                JoinedAt = DateTime.UtcNow, IsMuted = false
            };
            var results = await Task.WhenAll(
                Service(firstContext).CreateConversationParticipant(CreateRequest()),
                Service(secondContext).CreateConversationParticipant(CreateRequest()));
            Assert.Equal(results[0].ConversationParticipantID, results[1].ConversationParticipantID);
            Assert.Equal(1, await schema.ConversationParticipants.CountAsync());
            Assert.DoesNotContain(firstContext.ChangeTracker.Entries(), entry => entry.State == EntityState.Added);
            Assert.DoesNotContain(secondContext.ChangeTracker.Entries(), entry => entry.State == EntityState.Added);
        }
        finally
        {
            await schema.Database.EnsureDeletedAsync();
        }
    }

    private sealed class RacingRepository(ArcoraDbContext context, Func<Task> wait)
        : ConversationParticipantRepository(context), IConversationParticipantRepository
    {
        private bool firstRead = true;
        async Task<IEnumerable<ConversationParticipant?>> IRepository<ConversationParticipant>.Find(
            System.Linq.Expressions.Expression<Func<ConversationParticipant, bool>> predicate)
        {
            var result = await base.Find(predicate);
            if (firstRead)
            {
                firstRead = false;
                await wait();
            }
            return result;
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
