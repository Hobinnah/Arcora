using System.Security.Claims;
using Arcora.Api.Entities;
using Arcora.Api.Realtime;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Arcora.Api.Tests;

public sealed class MessagingHubTests
{
    [Theory]
    [InlineData(11, true)]
    [InlineData(22, true)]
    [InlineData(33, false)]
    [InlineData(44, false)]
    public async Task Only_own_tenant_or_active_host_can_join_or_emit_typing(long userId, bool allowed)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        await using var db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        var tenantId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        db.Tenants.Add(new Tenant { TenantID = tenantId, UserID = 11, Code = "SYNTHETIC", Description = "Realtime test" });
        db.OrganizationMembers.AddRange(
            new OrganizationMember { OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationId, UserID = 22, Status = "ACTIVE", RoleName = "OWNER" },
            new OrganizationMember { OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationId, UserID = 33, Status = "INACTIVE", RoleName = "OWNER" });
        db.Conversations.Add(new Conversation { ConversationID = conversationId, TenantID = tenantId, OrganizationID = organizationId });
        await db.SaveChangesAsync();
        var groups = new Groups();
        var clients = new HubClients();
        using var hub = new MessagingHub(db) { Context = new Caller(userId), Groups = groups, Clients = clients };
        if (allowed)
        {
            await hub.JoinConversation(conversationId);
            await hub.Typing(conversationId, "Synthetic");
            Assert.Equal(1, groups.Joins);
            Assert.Equal(1, clients.Proxy.Sends);
        }
        else
        {
            await Assert.ThrowsAsync<HubException>(() => hub.JoinConversation(conversationId));
            await Assert.ThrowsAsync<HubException>(() => hub.Typing(conversationId, "Synthetic"));
            Assert.Equal(0, groups.Joins);
            Assert.Equal(0, clients.Proxy.Sends);
        }
        await Assert.ThrowsAsync<HubException>(() => hub.JoinConversation(Guid.NewGuid()));
    }

    private sealed class Caller(long id) : HubCallerContext
    {
        public override string ConnectionId => "synthetic";
        public override string? UserIdentifier => id.ToString();
        public override ClaimsPrincipal User => new(new ClaimsIdentity([new Claim("UserId", id.ToString())], "Test"));
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => default;
        public override void Abort() { }
    }
    private sealed class Groups : IGroupManager
    {
        public int Joins { get; private set; }
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        { Joins++; return Task.CompletedTask; }
        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    private sealed class Proxy : IClientProxy
    {
        public int Sends { get; private set; }
        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        { Sends++; return Task.CompletedTask; }
    }
    private sealed class HubClients : IHubCallerClients
    {
        public Proxy Proxy { get; } = new();
        public IClientProxy All => Proxy;
        public IClientProxy Caller => Proxy;
        public IClientProxy Others => Proxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => Proxy;
        public IClientProxy Client(string connectionId) => Proxy;
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => Proxy;
        public IClientProxy Group(string groupName) => Proxy;
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => Proxy;
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => Proxy;
        public IClientProxy OthersInGroup(string groupName) => Proxy;
        public IClientProxy User(string userId) => Proxy;
        public IClientProxy Users(IReadOnlyList<string> userIds) => Proxy;
    }
    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options)
        : ArcoraDbContext(options, new ConfigurationBuilder().Build())
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entity in builder.Model.GetEntityTypes())
                foreach (var property in entity.GetProperties())
                    if (property.GetColumnType() == "nvarchar(max)") property.SetColumnType("TEXT");
        }
    }
}
