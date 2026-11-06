using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Arcora.Api.Tests;

public sealed class ConversationDetailsTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private TestDbContext db = null!;
    private ConversationDetailsService service = null!;
    private readonly Guid listingID = Guid.NewGuid();
    private readonly Guid tenantID = Guid.NewGuid();
    private readonly Guid organizationID = Guid.NewGuid();
    private readonly Guid conversationID = Guid.NewGuid();
    private readonly Guid memberID = Guid.NewGuid();
    private readonly Guid secondMemberID = Guid.NewGuid();

    private const long RenterUserID = 42;
    private const long HostUserID = 43;
    private const long SecondHostUserID = 44;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["LeaseCheckIn:DetailsLeadDays"] = "3" }).Build();
        db = new TestDbContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();

        var addressID = Guid.NewGuid();
        var propertyID = Guid.NewGuid();
        var unitID = Guid.NewGuid();
        db.Addresses.Add(RequiredStrings(new Address
        {
            AddressID = addressID, Line1 = "12 King St W", City = "Toronto", ProvinceCode = "ON", PostalCode = "M5H 1A1",
            CountryCode = "CA", Latitude = 43.65m, Longitude = -79.38m
        }));
        db.Properties.Add(RequiredStrings(new Property { PropertyID = propertyID, OrganizationID = organizationID, AddressID = addressID }));
        db.RentalUnits.Add(RequiredStrings(new RentalUnit { RentalUnitID = unitID, PropertyID = propertyID }));
        db.Listings.Add(RequiredStrings(new Listing
        {
            ListingID = listingID, OrganizationID = organizationID, RentalUnitID = unitID, Title = "Harbour loft",
            Status = "PUBLISHED", Currency = "CAD", BaseMonthlyRentAmount = 2000, SecurityDepositAmount = 1000,
            MinimumLeaseMonths = 3, CheckInDoorCode = "4821", ShortTermCancellationPolicy = "Flexible",
            LongTermCancellationPolicy = "Firm Long Term"
        }));
        db.ListingPolicies.Add(RequiredStrings(new ListingPolicy { ListingPolicyID = Guid.NewGuid(), ListingID = listingID, AllowsPets = false, MaximumOccupants = 2 }));
        db.ListingRules.AddRange(
            Rule("CHECK_IN", "15:00"),
            Rule("CHECK_OUT", "11:00"),
            Rule("QUIET_HOURS_START", "22:00"),
            Rule("QUIET_HOURS_END", "07:00"),
            Rule("PARTIES", "Parties"));
        db.Organizations.Add(RequiredStrings(new Organization { OrganizationID = organizationID, DisplayName = "Grinding Breeze", CapturedDate = new DateTime(2021, 5, 1) }));
        db.Users.AddRange(
            NewUser(RenterUserID, "renter", "Elijah", "Stone"),
            NewUser(HostUserID, "host", "Mia", "Lane"),
            NewUser(SecondHostUserID, "cohost", "Noah", "Park"));
        db.Tenants.Add(RequiredStrings(new Tenant { TenantID = tenantID, UserID = RenterUserID, PhoneNumber = "+1 416 555 0100" }));
        db.OrganizationMembers.AddRange(
            RequiredStrings(new OrganizationMember { OrganizationMemberID = memberID, OrganizationID = organizationID, UserID = HostUserID, Status = "ACTIVE", IsPrimaryOwner = true }),
            RequiredStrings(new OrganizationMember { OrganizationMemberID = secondMemberID, OrganizationID = organizationID, UserID = SecondHostUserID, Status = "ACTIVE" }));
        db.Conversations.Add(RequiredStrings(new Conversation { ConversationID = conversationID, TenantID = tenantID, OrganizationID = organizationID, Status = "OPEN" }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var preferences = Stub<IPreferenceService>((method, _) => method.Name == nameof(IPreferenceService.GetPreference)
            ? Task.FromResult<Preference?>(RequiredStrings(new Preference { CompanyName = "Arcora", CompanyEmail = "support@arcora.test" }))
            : throw new NotSupportedException(method.Name));
        service = new ConversationDetailsService(db, configuration, preferences, Stub<IFileStorageService>((method, args) => method.Name == nameof(IFileStorageService.GetReadSasUrlAsync)
            ? Task.FromResult<string?>($"https://blob.test/images/{args![1]}?sig=read")
            : throw new NotSupportedException(method.Name)), NullLogger<ConversationDetailsService>.Instance);
    }

    [Fact]
    public async Task Host_enquiry_details_use_the_card_dates_and_preserve_the_hosts_own_address()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20);
        await AddCardAsync(start, start.AddMonths(6));

        var details = await service.GetDetailsAsync(conversationID, Host());

        Assert.Equal(ConversationDetailsDto.KindEnquiry, details.Kind);
        Assert.Equal("HOST", details.ViewerRole);
        Assert.Equal(start, details.Enquiry!.ProposedStartDate);
        Assert.Null(details.Reservation);
        Assert.Equal("Harbour loft", details.Listing!.Title);
        Assert.Equal((short)3, details.Listing.MinimumLeaseMonths);
        Assert.Equal("Firm Long Term", details.Listing.CancellationPolicy);
        Assert.Equal("3:00 PM", details.Listing.CheckInTime);
        Assert.Equal("11:00 AM", details.Listing.CheckOutTime);
        Assert.Equal("12 King St W", details.Listing.Address!.Line1);
        Assert.Equal("Toronto", details.Listing.Address.City);
        Assert.Null(details.Renter!.PhoneNumber);
        Assert.Equal("4821", details.Access!.DoorCode);
        Assert.Equal("Grinding Breeze", details.Host!.Name);
        Assert.Equal("Mia", details.Host.FirstName);
        Assert.Equal("Hosting since 2021", details.Host.Subtitle);
        Assert.Equal(new[] { "2 guests maximum", "No pets", "Quiet hours 10:00 PM – 7:00 AM" }, details.HouseRules);
        Assert.Equal("support@arcora.test", details.Support!.Email);
    }

    [Fact]
    public async Task Short_enquiries_use_the_short_term_cancellation_policy()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20);
        await AddCardAsync(start, start.AddDays(20));

        var details = await service.GetDetailsAsync(conversationID, Tenant());

        Assert.Equal("Flexible", details.Listing!.CancellationPolicy);
        Assert.Equal("TENANT", details.ViewerRole);
        Assert.Null(details.Note);
        Assert.Null(details.Payout);
    }

    [Fact]
    public async Task Reservation_details_come_from_the_lease()
    {
        await AddCardAsync(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20), null);
        var lease = await AddLeaseAsync("ACTIVE", DateTime.UtcNow.Date.AddDays(30));
        await AddPayoutAsync(lease.LeaseID, 1800m);

        var details = await service.GetDetailsAsync(conversationID, Host());

        Assert.Equal(ConversationDetailsDto.KindReservation, details.Kind);
        Assert.Equal(lease.LeaseID, details.Reservation!.LeaseID);
        Assert.True(details.Reservation.IsConfirmed);
        Assert.Equal("ARC-123", details.Reservation.ConfirmationCode);
        Assert.Equal(6, details.Reservation.TermMonths);
        Assert.Equal(12000m, details.Reservation.Total);
        Assert.Equal("12 King St W", details.Listing!.Address!.Line1);
        Assert.Equal("+1 416 555 0100", details.Renter!.PhoneNumber);
        Assert.Equal("4821", details.Access!.DoorCode);
        Assert.Equal(1800m, details.Payout!.Amount);
        Assert.Equal("PAID", details.Payout.Status);
    }

    [Fact]
    public async Task Cancelled_leases_do_not_turn_an_enquiry_into_a_reservation()
    {
        await AddCardAsync(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20), null);
        await AddLeaseAsync("CANCELLED", DateTime.UtcNow.Date.AddDays(30));

        var details = await service.GetDetailsAsync(conversationID, Host());

        Assert.Equal(ConversationDetailsDto.KindEnquiry, details.Kind);
        Assert.Null(details.Reservation);
    }

    [Fact]
    public async Task Unconfirmed_leases_hide_the_code_address_and_renter_access()
    {
        await AddCardAsync(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20), null);
        var lease = await AddLeaseAsync("PENDING_TENANT_SIGNATURE", DateTime.UtcNow.Date.AddDays(2));

        var tenant = await service.GetDetailsAsync(conversationID, Tenant());
        Assert.Equal(ConversationDetailsDto.KindReservation, tenant.Kind);
        Assert.False(tenant.Reservation!.IsConfirmed);
        Assert.Null(tenant.Reservation.ConfirmationCode);
        Assert.Null(tenant.Listing!.Address!.Line1);
        Assert.Equal("Toronto", tenant.Listing.Address.City);
        Assert.Null(tenant.Access);

        var host = await service.GetDetailsAsync(conversationID, Host());
        Assert.Null(host.Reservation!.ConfirmationCode);
        Assert.Equal("12 King St W", host.Listing!.Address!.Line1);
        Assert.Equal("4821", host.Access!.DoorCode);

        // Active but unpaid is still not confirmed.
        var tracked = await db.Leases.SingleAsync(x => x.LeaseID == lease.LeaseID);
        tracked.Status = "ACTIVE";
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        Assert.False((await service.GetDetailsAsync(conversationID, Tenant())).Reservation!.IsConfirmed);

        await AddPaidIntentAsync(lease.LeaseID);
        var paid = await service.GetDetailsAsync(conversationID, Tenant());
        Assert.True(paid.Reservation!.IsConfirmed);
        Assert.Equal("ARC-123", paid.Reservation.ConfirmationCode);
        Assert.Equal("12 King St W", paid.Listing!.Address!.Line1);
        Assert.Equal("4821", paid.Access!.DoorCode);
    }

    [Fact]
    public async Task Stored_logo_references_are_resolved_and_missing_policies_use_listing_defaults()
    {
        var organization = await db.Organizations.SingleAsync(x => x.OrganizationID == organizationID);
        organization.BrandLogoUrl = "2026-10-03/logo.png";
        var listing = await db.Listings.SingleAsync(x => x.ListingID == listingID);
        listing.ShortTermCancellationPolicy = null;
        listing.LongTermCancellationPolicy = "  ";
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(20);
        await AddCardAsync(start, start.AddMonths(4));

        var details = await service.GetDetailsAsync(conversationID, Tenant());

        Assert.Equal("https://blob.test/images/2026-10-03/logo.png?sig=read", details.Host!.PhotoUrl);
        Assert.Equal("Firm Long Term", details.Listing!.CancellationPolicy);
    }

    [Fact]
    public async Task Renters_only_see_the_door_code_close_to_move_in()
    {
        var lease = await AddLeaseAsync("ACTIVE", DateTime.UtcNow.Date.AddDays(30));
        await AddPaidIntentAsync(lease.LeaseID);

        var early = await service.GetDetailsAsync(conversationID, Tenant());
        Assert.Null(early.Access!.DoorCode);
        Assert.Equal(lease.StartDate.Date.AddDays(-3), early.Access.AvailableFrom);
        Assert.Null(early.Renter!.PhoneNumber);
        Assert.Null(early.Payout);

        var tracked = await db.Leases.SingleAsync(x => x.LeaseID == lease.LeaseID);
        tracked.StartDate = DateTime.UtcNow.Date.AddDays(2);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var near = await service.GetDetailsAsync(conversationID, Tenant());
        Assert.Equal("4821", near.Access!.DoorCode);
    }

    [Fact]
    public async Task Reviews_are_resolved_for_the_viewer()
    {
        var lease = await AddLeaseAsync("ACTIVE", DateTime.UtcNow.Date.AddDays(-60));
        db.Ratings.Add(RequiredStrings(new Rating
        {
            RatingID = Guid.NewGuid(), LeaseID = lease.LeaseID, ReviewerUserID = RenterUserID, SubjectType = "HOST",
            SubjectReferenceID = organizationID.ToString(), OverallRating = 5, IsPublic = true, PublishedAt = DateTime.UtcNow.AddDays(-1)
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var host = await service.GetDetailsAsync(conversationID, Host());
        var tenant = await service.GetDetailsAsync(conversationID, Tenant());

        Assert.Equal((short)5, host.Review!.OverallRating);
        Assert.Equal("Elijah", host.Review.ReviewerFirstName);
        Assert.Null(tenant.Review);
    }

    [Fact]
    public async Task Outsiders_and_mismatched_users_are_rejected()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetDetailsAsync(conversationID, new ConversationActor { UserID = HostUserID, TenantID = tenantID }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetDetailsAsync(conversationID, new ConversationActor { UserID = RenterUserID, OrganizationMemberID = memberID }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetDetailsAsync(conversationID, new ConversationActor { TenantID = tenantID }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDetailsAsync(Guid.NewGuid(), Tenant()));
    }

    [Fact]
    public async Task Notes_are_shared_by_the_host_team_and_hidden_from_renters()
    {
        var saved = await service.SaveNoteAsync(conversationID, Host(), "  Prefers a morning viewing  ");
        Assert.Equal("Prefers a morning viewing", saved!.Note);
        Assert.Equal("Mia", saved.UpdatedByName);

        var cohost = await service.GetDetailsAsync(conversationID, new ConversationActor { UserID = SecondHostUserID, OrganizationMemberID = secondMemberID });
        Assert.Equal("Prefers a morning viewing", cohost.Note!.Note);

        var updated = await service.SaveNoteAsync(conversationID, new ConversationActor { UserID = SecondHostUserID, OrganizationMemberID = secondMemberID }, "Viewing booked");
        Assert.Equal("Noah", updated!.UpdatedByName);
        Assert.Equal(1, await db.ConversationNotes.CountAsync());

        Assert.Null((await service.GetDetailsAsync(conversationID, Tenant())).Note);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SaveNoteAsync(conversationID, Tenant(), "hi"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.SaveNoteAsync(conversationID, Host(), new string('x', ConversationNote.MaxLength + 1)));

        Assert.Null(await service.SaveNoteAsync(conversationID, Host(), "   "));
        Assert.Equal(0, await db.ConversationNotes.CountAsync());
    }

    private ConversationActor Host() => new() { UserID = HostUserID, OrganizationMemberID = memberID };
    private ConversationActor Tenant() => new() { UserID = RenterUserID, TenantID = tenantID };

    private async Task AddCardAsync(DateOnly? start, DateOnly? end)
    {
        db.ConversationMessages.Add(RequiredStrings(new ConversationMessage
        {
            ConversationMessageID = Guid.NewGuid(), ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = RenterUserID,
            MessageType = "ENQUIRY_CARD", SentAt = DateTime.UtcNow,
            Message = JsonSerializer.Serialize(new ListingEnquiryCard(listingID, "Harbour loft", 2000, "CAD", null, start, end, 1000), new JsonSerializerOptions(JsonSerializerDefaults.Web))
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    private async Task<Lease> AddLeaseAsync(string status, DateTime start)
    {
        var lease = RequiredStrings(new Lease
        {
            LeaseID = Guid.NewGuid(), OrganizationID = organizationID, ListingID = listingID, TenantID = tenantID, Status = status,
            StartDate = start, EndDate = start.AddMonths(6), LeaseTermMonths = 6, BaseRentAmount = 2000, Currency = "CAD", LeaseCode = "ARC-123"
        });
        db.Leases.Add(lease);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return lease;
    }

    private async Task AddPaidIntentAsync(Guid leaseID)
    {
        var intentID = Guid.NewGuid();
        db.PaymentIntents.Add(RequiredStrings(new PaymentIntent { PaymentIntentID = intentID, LeaseID = leaseID, TenantID = tenantID,
            IdempotencyKey = $"pi_firstcharge_lease_{leaseID:N}", Amount = 2000, Status = "PAID", ScheduledChargeAt = DateTime.UtcNow }));
        db.Payments.Add(RequiredStrings(new Payment { PaymentID = Guid.NewGuid(), PaymentIntentID = intentID, TenantID = tenantID,
            GrossAmount = 2000, NetAmount = 2000, PaidAt = DateTime.UtcNow, Status = "PAID" }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    private async Task AddPayoutAsync(Guid leaseID, decimal net)
    {
        var intentID = Guid.NewGuid();
        var paymentID = Guid.NewGuid();
        db.PaymentIntents.Add(RequiredStrings(new PaymentIntent { PaymentIntentID = intentID, LeaseID = leaseID, TenantID = tenantID, Amount = 2000, ScheduledChargeAt = DateTime.UtcNow }));
        db.Payments.Add(RequiredStrings(new Payment { PaymentID = paymentID, PaymentIntentID = intentID, TenantID = tenantID, GrossAmount = 2000, NetAmount = 1900, Status = "SUCCEEDED" }));
        db.Payouts.Add(RequiredStrings(new Payout { PayoutID = 7, OrganizationID = organizationID, Amount = net, Currency = "CAD", Status = "PAID", RequestedAt = DateTime.UtcNow, PaidAt = DateTime.UtcNow }));
        db.PayoutItems.Add(RequiredStrings(new PayoutItem { PayoutItemID = 9, PayoutID = 7, PaymentID = paymentID, GrossAmount = 2000, NetPayoutAmount = net }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    private ListingRule Rule(string type, string title) =>
        RequiredStrings(new ListingRule { ListingRuleID = Guid.NewGuid(), ListingID = listingID, RuleType = type, RuleTitle = title, RuleDescription = title, IsAllowed = true });

    private static User NewUser(long id, string userName, string firstName, string lastName) => new()
    {
        Id = id, UserName = userName, NormalizedUserName = userName.ToUpperInvariant(), Email = $"{userName}@example.test",
        SecurityStamp = Guid.NewGuid().ToString(), FirstName = firstName, LastName = lastName
    };

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    private static T RequiredStrings<T>(T entity)
    {
        foreach (var property in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string)
                     && p.GetCustomAttribute<RequiredAttribute>() != null && p.GetValue(entity) == null))
            property.SetValue(entity, "test");
        return entity;
    }

    private static T Stub<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var stub = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)stub).Handler = handler;
        return stub;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!, args);
    }

    private sealed class TestDbContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration)
        : ArcoraDbContext(options, configuration)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
                property.SetColumnType(null);
        }
    }
}
