using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Data.Common;
using Arcora.Api;
using Arcora.Api.Controllers;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs.DtoProfiles;
using Arcora.Api.Entities;
using Arcora.Api.Email;
using Arcora.Api.Exceptions;
using Arcora.Api.Models;
using Arcora.Api.Realtime;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class MessagingEnquiryTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new($"Data Source=messaging-{Guid.NewGuid():N};Mode=Memory;Cache=Shared;Foreign Keys=False");
    private TestDbContext db = null!;
    private MessagingService service = null!;
    private readonly Guid listingID = Guid.NewGuid();
    private readonly Guid tenantID = Guid.NewGuid();
    private readonly Guid organizationID = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().Build();
        db = new TestDbContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();
        var listing = RequiredStrings(new Listing
        {
            ListingID = listingID, OrganizationID = organizationID, Title = "Enquiry home",
            Status = "PUBLISHED", Currency = "CAD", BaseMonthlyRentAmount = 1850, SecurityDepositAmount = 900,
            MinimumLeaseMonths = 6
        });
        db.Listings.Add(listing);
        db.Users.Add(new User
        {
            Id = 42, UserName = "enquiry-renter", NormalizedUserName = "ENQUIRY-RENTER",
            Email = "enquiry@example.test", SecurityStamp = Guid.NewGuid().ToString()
        });
        db.Tenants.Add(RequiredStrings(new Tenant { TenantID = tenantID, UserID = 42 }));
        await db.SaveChangesAsync();
        var mapper = new MapperConfiguration(c =>
        {
            c.AddProfile<ConversationProfile>();
            c.AddProfile<ConversationMessageProfile>();
            c.AddProfile<TenantProfile>();
            c.AddProfile<OrganizationMemberProfile>();
            c.AddProfile<OrganizationProfile>();
        }, NullLoggerFactory.Instance).CreateMapper();
        var client = Stub<IClientProxy>((_, _) => Task.CompletedTask);
        var clients = Stub<IHubClients>((_, _) => client);
        hub = Stub<IHubContext<MessagingHub>>((method, _) =>
            method.Name == "get_Clients" ? clients : throw new NotSupportedException(method.Name));
        this.mapper = mapper;
        this.configuration = configuration;
        service = CreateService();
    }

    private IMapper mapper = null!;
    private IConfiguration configuration = null!;
    private IHubContext<MessagingHub> hub = null!;

    private MessagingService CreateService(IFileStorageService? storage = null, IPreferenceService? preferences = null, ArcoraDbContext? context = null, IEmailQueue? emailQueue = null) =>
        new(mapper, NullLogger<MessagingService>.Instance, configuration,
            new ConversationRepository(context ?? db), new ConversationMessageRepository(context ?? db),
            new ConversationParticipantRepository(context ?? db), new TenantRepository(context ?? db),
            new OrganizationMemberRepository(context ?? db), preferences!, hub, new ListingRepository(context ?? db), new ListingPhotoRepository(context ?? db),
            new ConversationMessageReactionRepository(context ?? db), fileStorageService: storage, emailQueue: emailQueue);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MessagingAwaitsNotificationDbWorkBeforeReturning(bool enquiry)
    {
        var thread = await service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            TenantID = tenantID, OrganizationID = organizationID, Subject = "Synthetic thread"
        });
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource<Preference?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var preferences = Stub<IPreferenceService>((_, _) =>
        {
            started.TrySetResult();
            return resume.Task;
        });
        var sender = CreateService(preferences: preferences);
        Task work = enquiry
            ? sender.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)
            : sender.SendMessageAsync(new SendMessageRequest
            {
                ConversationID = thread.ConversationID!.Value, SenderTenantID = tenantID,
                SenderUserID = 42, Message = "Synthetic check-in notification"
            });
        try
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(work.IsCompleted);
        }
        finally
        {
            resume.TrySetResult(null);
            await work.WaitAsync(TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task ListingEnquiry_PostsRenterMessageThenDatedCardAndReusesThread()
    {
        var first = await service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            ListingID = listingID, TenantID = Guid.NewGuid(), OrganizationID = Guid.NewGuid(),
            Message = "  Is parking included?  ", ProposedStartDate = Start, ProposedEndDate = End
        }, 42);
        var second = await service.GetOrCreateDirectThreadAsync(Enquiry(listingID, "Second question"), 42);
        Assert.Equal(first.ConversationID, second.ConversationID);
        db.ChangeTracker.Clear();
        var thread = Assert.Single(await db.Conversations.ToListAsync());
        Assert.Equal(tenantID, thread.TenantID);
        Assert.Equal(organizationID, thread.OrganizationID);
        Assert.Equal(listingID, thread.ListingID);
        Assert.NotNull(thread.LastMessageAt);
        var messages = await OrderedMessagesAsync();
        Assert.Equal(new[] { "TEXT", "ENQUIRY_CARD", "TEXT", "ENQUIRY_CARD" }, messages.Select(m => m.MessageType));
        Assert.Equal("Is parking included?", messages[0].Message);
        Assert.Equal("Second question", messages[2].Message);
        Assert.All(messages, m => { Assert.Equal(tenantID, m.SenderTenantID); Assert.Equal(42, m.SenderUserID); });
        var card = ReadCard(messages[1]);
        Assert.Equal(listingID, card.ListingID);
        Assert.Equal(1850, card.MonthlyRent);
        Assert.Equal(900, card.SecurityDeposit);
        Assert.Equal("Enquiry home", card.Title);
        Assert.Equal(Start, card.ProposedStartDate);
        Assert.Equal(End, card.ProposedEndDate);
        Assert.Null(card.PhotoID);
    }

    [Fact]
    public async Task ListingEnquiry_CreatesSeparateThreadsPerListingForSameTenantAndOrganization()
    {
        var secondListingID = Guid.NewGuid();
        db.Listings.Add(RequiredStrings(new Listing
        {
            ListingID = secondListingID, OrganizationID = organizationID, Title = "Second enquiry home",
            Status = "PUBLISHED", Currency = "CAD", BaseMonthlyRentAmount = 1950, SecurityDepositAmount = 950,
            MinimumLeaseMonths = 6
        }));
        await db.SaveChangesAsync();

        var firstListingThread = await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        var secondListingThread = await service.GetOrCreateDirectThreadAsync(Enquiry(secondListingID), 42);
        var repeatedSecondListingThread = await service.GetOrCreateDirectThreadAsync(Enquiry(secondListingID, "Another question"), 42);

        Assert.NotEqual(firstListingThread.ConversationID, secondListingThread.ConversationID);
        Assert.Equal(secondListingThread.ConversationID, repeatedSecondListingThread.ConversationID);
        db.ChangeTracker.Clear();
        var threads = await db.Conversations.OrderBy(conversation => conversation.ListingID).ToListAsync();
        Assert.Equal(2, threads.Count);
        Assert.Equal(new[] { listingID, secondListingID }.OrderBy(id => id), threads.Select(thread => thread.ListingID!.Value));
        Assert.Equal(6, await db.ConversationMessages.CountAsync());
    }

    [Fact]
    public async Task ListingScopedDirectThreadTuple_IsUniqueInTheDatabase()
    {
        await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        db.ChangeTracker.Clear();
        db.Conversations.Add(new Conversation
        {
            ConversationID = Guid.NewGuid(), ConversationType = "DIRECT", TenantID = tenantID,
            OrganizationID = organizationID, ListingID = listingID, Subject = "Duplicate",
            Status = "OPEN", CapturedDate = DateTime.UtcNow
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Theory]
    [InlineData(null, true, true)]
    [InlineData("   ", true, true)]
    [InlineData("Hello", false, true)]
    [InlineData("Hello", true, false)]
    public async Task ListingEnquiry_RequiresMessageAndBothDates(string? message, bool hasStart, bool hasEnd)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            ListingID = listingID, Message = message,
            ProposedStartDate = hasStart ? Start : null, ProposedEndDate = hasEnd ? End : null
        }, 42));
        Assert.Empty(await db.ConversationMessages.ToListAsync());
    }

    [Fact]
    public async Task ListingEnquiry_RejectsEndDateOnOrBeforeStartDate()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            ListingID = listingID, Message = "Hi", ProposedStartDate = Start, ProposedEndDate = Start
        }, 42));
        Assert.Empty(await db.Conversations.ToListAsync());
    }

    [Fact]
    public async Task ListingEnquiry_RejectsStayShorterThanMinimumLease()
    {
        var error = await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            ListingID = listingID, Message = "Hi", ProposedStartDate = Start, ProposedEndDate = Start.AddMonths(6).AddDays(-1)
        }, 42));
        Assert.Contains("at least 6 months", error.Message);
        Assert.Empty(await db.Conversations.ToListAsync());
    }

    [Theory]
    [InlineData(null, 10, 40, "A message to the host is required.")]
    [InlineData("Hi", 10, 10, "Proposed end date must be after the start date.")]
    [InlineData("Hi", -5, 40, "Proposed start date cannot be in the past.")]
    public async Task StartThreadEndpoint_ValidatesEnquiryBeforeCallingService(string? message, int startOffset, int endOffset, string expected)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var controller = new MessagingController();
        var result = await controller.StartThread(null!, new StartThreadRequest
        {
            ListingID = listingID, Message = message,
            ProposedStartDate = today.AddDays(startOffset), ProposedEndDate = today.AddDays(endOffset)
        }, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains(expected, JsonSerializer.Serialize(badRequest.Value));
    }

    [Fact]
    public async Task ListingEnquiry_UsesExistingTenantResolverWhenUserHasMultipleProfiles()
    {
        db.Tenants.Add(RequiredStrings(new Tenant { TenantID = Guid.NewGuid(), UserID = 42 }));
        await db.SaveChangesAsync();
        var expected = await new TenantRepository(db).GetTenantByUserIDAsync(42);
        var thread = await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        Assert.Equal(expected!.TenantID, thread.TenantID);
        Assert.Equal(2, (await db.ConversationMessages.ToListAsync()).Count);
    }

    [Fact]
    public async Task ListingEnquiry_SupportsUrlOnlyPhotosWithoutBlobMetadata()
    {
        var photoID = Guid.NewGuid();
        db.ListingPhotos.Add(new ListingPhoto
        {
            ListingPhotoID = photoID, ListingID = listingID, Url = "https://example.test/home.jpg",
            StorageProvider = null, StorageReference = null, IsCoverPhoto = true
        });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        db.ChangeTracker.Clear();
        var card = ReadCard((await OrderedMessagesAsync())[1]);
        Assert.Equal(photoID, card.PhotoID);
        var photo = await new ListingPhotoRepository(db).GetByID(photoID);
        Assert.Equal("https://example.test/home.jpg", photo!.Url);
        Assert.Null(photo.StorageReference);
    }

    [Fact]
    public async Task ListingEnquiry_PreservesLongTitlesBeyondTheOldMessageLimit()
    {
        var listing = await db.Listings.FindAsync(listingID);
        listing!.Title = new string('A', 200);
        await db.SaveChangesAsync();
        await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        db.ChangeTracker.Clear();
        var message = (await OrderedMessagesAsync())[1];
        Assert.True(message.Message!.Length > 256);
        Assert.Null(db.Model.FindEntityType(typeof(ConversationMessage))!.FindProperty(nameof(ConversationMessage.Message))!.GetMaxLength());
        Assert.Equal(new string('A', 200), ReadCard(message).Title);
    }

    [Fact]
    public async Task ListingEnquiry_RequiresAuthenticatedUserAndTenant()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetOrCreateDirectThreadAsync(Enquiry(listingID)));
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 999));
        Assert.Equal(409, error.StatusCode);
        Assert.Empty(await db.Conversations.ToListAsync());
    }

    [Fact]
    public async Task ListingEnquiry_RejectsUnavailableListing()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetOrCreateDirectThreadAsync(Enquiry(Guid.NewGuid()), 42));
        Assert.Empty(await db.Conversations.ToListAsync());
    }

    [Fact]
    public async Task LegacyThreadAndTextMessage_StillPersist()
    {
        var thread = await service.GetOrCreateDirectThreadAsync(new StartThreadRequest
        {
            TenantID = tenantID, OrganizationID = organizationID, Subject = "Existing flow"
        });
        Assert.Empty(await db.ConversationMessages.ToListAsync());
        await service.SendMessageAsync(new SendMessageRequest
        {
            ConversationID = thread.ConversationID!.Value, SenderTenantID = tenantID,
            SenderUserID = 42, Message = "Hello host"
        });
        db.ChangeTracker.Clear();
        Assert.Equal("Hello host", Assert.Single(await db.ConversationMessages.ToListAsync()).Message);
    }

    [Fact]
    public async Task Reactions_ToggleReplaceAndAppearInThread()
    {
        var thread = await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        var memberID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = memberID, OrganizationID = organizationID, Status = "ACTIVE"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var messageID = (await OrderedMessagesAsync())[0].ConversationMessageID;
        var tenant = new ConversationActor { TenantID = tenantID };
        var host = new ConversationActor { OrganizationMemberID = memberID };

        Assert.Equal("👍", Assert.Single((await service.ReactToMessageAsync(messageID, "👍", tenant))!).Emoji);
        Assert.Equal(2, (await service.ReactToMessageAsync(messageID, "❤️", host))!.Count);
        var replaced = (await service.ReactToMessageAsync(messageID, "😂", tenant))!;
        Assert.Equal(new[] { "❤️", "😂" }, replaced.Select(r => r.Emoji).OrderBy(e => e, StringComparer.Ordinal));

        var page = await service.GetThreadMessagesAsync(thread.ConversationID!.Value, 1, 30);
        var reacted = page.Data.Single(m => m.ConversationMessageID == messageID);
        Assert.Contains(reacted.Reactions, r => r.ReactorTenantID == tenantID && r.Emoji == "😂");
        Assert.Contains(reacted.Reactions, r => r.ReactorOrganizationMemberID == memberID && r.Emoji == "❤️");
        Assert.Empty(page.Data.Single(m => m.ConversationMessageID != messageID).Reactions);

        var removed = (await service.ReactToMessageAsync(messageID, "😂", tenant))!;
        Assert.Equal(memberID, Assert.Single(removed).ReactorOrganizationMemberID);
    }

    [Fact]
    public async Task Reactions_RejectOutsidersUnsupportedEmojiAndMissingMessages()
    {
        await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        db.ChangeTracker.Clear();
        var messageID = (await OrderedMessagesAsync())[0].ConversationMessageID;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.ReactToMessageAsync(messageID, "👍", new ConversationActor { TenantID = Guid.NewGuid() }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.ReactToMessageAsync(messageID, "👍", new ConversationActor { OrganizationMemberID = Guid.NewGuid() }));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ReactToMessageAsync(messageID, "<script>", new ConversationActor { TenantID = tenantID }));
        Assert.Null(await service.ReactToMessageAsync(Guid.NewGuid(), "👍", new ConversationActor { TenantID = tenantID }));
        Assert.Empty(await db.ConversationMessageReactions.ToListAsync());

        var badRequest = await new MessagingController().ReactToMessage(null!, messageID, new ReactToMessageRequest { Emoji = "nope" });
        Assert.IsType<BadRequestObjectResult>(badRequest);
    }

    [Fact]
    public async Task EditMessage_OnlyAllowsSendersTextMessages()
    {
        await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        db.ChangeTracker.Clear();
        var messages = await OrderedMessagesAsync();
        var tenant = new ConversationActor { TenantID = tenantID };
        var edited = await service.EditMessageAsync(messages[0].ConversationMessageID, "Updated question", tenant);
        Assert.Equal("Updated question", edited!.Message);
        Assert.NotNull(edited.EditedAt);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EditMessageAsync(messages[1].ConversationMessageID, "{}", tenant));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.EditMessageAsync(messages[0].ConversationMessageID, "Hijack", new ConversationActor { TenantID = Guid.NewGuid() }));
    }

    [Fact]
    public async Task Names_InboxListsParticipantsCounterpartyFirstAndThreadCarriesSenderFirstName()
    {
        var renter = await db.Users.SingleAsync(u => u.Id == 42);
        renter.FirstName = "Elijah";
        renter.LastName = "Stone";
        db.Organizations.Add(RequiredStrings(new Organization { OrganizationID = organizationID, DisplayName = "Grinding Breeze" }));
        db.Users.Add(new User
        {
            Id = 43, UserName = "host-mia", NormalizedUserName = "HOST-MIA", Email = "mia@example.test",
            SecurityStamp = Guid.NewGuid().ToString(), FirstName = "Mia", LastName = "Hart"
        });
        var memberID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = memberID, OrganizationID = organizationID, UserID = 43, Status = "ACTIVE"
        }));
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationID, UserID = 43, Status = "REMOVED"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var thread = await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42);
        var reply = await service.SendMessageAsync(new SendMessageRequest
        {
            ConversationID = thread.ConversationID!.Value, SenderOrganizationMemberID = memberID, Message = "Welcome!"
        });
        Assert.Equal("Mia", reply.SenderName);

        var renterInbox = Assert.Single((await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, PageNumber = 1, PageSize = 10 })).Data);
        Assert.Equal(new[] { "Mia", "Elijah" }, renterInbox.ParticipantNames);
        var hostInbox = Assert.Single((await service.GetInboxAsync(new InboxQuery { OrganizationID = organizationID, PageNumber = 1, PageSize = 10 })).Data);
        Assert.Equal(new[] { "Elijah", "Mia" }, hostInbox.ParticipantNames);
        Assert.All(new[] { renterInbox, hostInbox }, row =>
        {
            Assert.Equal("Elijah", row.TenantFirstName);
            Assert.Equal("Grinding Breeze", row.OrganizationName);
        });

        var page = await service.GetThreadMessagesAsync(thread.ConversationID!.Value, 1, 30);
        Assert.Equal(new[] { "Elijah", "Elijah", "Mia" }, page.Data.Select(m => m.SenderName).OrderBy(n => n));
    }

    [Fact]
    public async Task ReadReceipts_TrackEachSideByRoleAndIncludeMembersWhoJoinedLater()
    {
        var renter = await db.Users.SingleAsync(u => u.Id == 42);
        renter.FirstName = "Elijah";
        db.Organizations.Add(RequiredStrings(new Organization { OrganizationID = organizationID }));
        db.Users.Add(new User
        {
            Id = 43, UserName = "host-mia", NormalizedUserName = "HOST-MIA", Email = "mia@example.test",
            SecurityStamp = Guid.NewGuid().ToString(), FirstName = "Mia"
        });
        var miaID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = miaID, OrganizationID = organizationID, UserID = 43, Status = "ACTIVE"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var conversationID = (await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)).ConversationID!.Value;
        Assert.Empty(await service.GetReadReceiptsAsync(conversationID));

        // Same person on both sides: the tenant role must win over the shared user ID.
        await service.MarkReadAsync(conversationID, new ConversationActor { TenantID = tenantID, UserID = 43 });
        var receipts = await service.GetReadReceiptsAsync(conversationID);
        Assert.Equal(tenantID, Assert.Single(receipts).TenantID);
        Assert.Equal("Elijah", receipts[0].FirstName);

        db.Users.Add(new User
        {
            Id = 44, UserName = "host-noah", NormalizedUserName = "HOST-NOAH", Email = "noah@example.test",
            SecurityStamp = Guid.NewGuid().ToString(), FirstName = "Noah"
        });
        var noahID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = noahID, OrganizationID = organizationID, UserID = 44, Status = "ACTIVE"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        await service.MarkReadAsync(conversationID, new ConversationActor { OrganizationMemberID = miaID, UserID = 43 });
        await service.MarkReadAsync(conversationID, new ConversationActor { OrganizationMemberID = noahID, UserID = 44 });

        receipts = await service.GetReadReceiptsAsync(conversationID);
        Assert.Equal(new[] { "Elijah", "Mia", "Noah" }, receipts.Select(r => r.FirstName).OrderBy(n => n));
        Assert.Equal(miaID, receipts.Single(r => r.FirstName == "Mia").OrganizationMemberID);
        Assert.Equal(noahID, receipts.Single(r => r.FirstName == "Noah").OrganizationMemberID);
    }

    [Fact]
    public async Task Unread_CountsOnlyTheOtherSidesMessagesAndClearsOnRead()
    {
        db.Organizations.Add(RequiredStrings(new Organization { OrganizationID = organizationID }));
        db.Users.Add(new User
        {
            Id = 43, UserName = "host-mia", NormalizedUserName = "HOST-MIA", Email = "mia@example.test",
            SecurityStamp = Guid.NewGuid().ToString(), FirstName = "Mia"
        });
        var memberID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = memberID, OrganizationID = organizationID, UserID = 43, Status = "ACTIVE"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var conversationID = (await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)).ConversationID!.Value;

        // The renter's own enquiry never makes their own dot show.
        Assert.Equal(0, Assert.Single((await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, PageNumber = 1, PageSize = 10 })).Data).UnreadCount);
        Assert.Equal(0, await service.CountUnreadConversationsAsync(tenantID, null));
        Assert.True(Assert.Single((await service.GetInboxAsync(new InboxQuery { OrganizationID = organizationID, PageNumber = 1, PageSize = 10 })).Data).UnreadCount > 0);
        Assert.Equal(1, await service.CountUnreadConversationsAsync(null, organizationID));

        await service.MarkReadAsync(conversationID, new ConversationActor { OrganizationMemberID = memberID, UserID = 43 });
        await service.SendMessageAsync(new SendMessageRequest { ConversationID = conversationID, SenderOrganizationMemberID = memberID, Message = "Welcome!" });

        // The host's reply is unread for the renter only.
        Assert.Equal(0, await service.CountUnreadConversationsAsync(null, organizationID));
        Assert.Equal(1, Assert.Single((await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, PageNumber = 1, PageSize = 10 })).Data).UnreadCount);
        Assert.Equal(1, await service.CountUnreadConversationsAsync(tenantID, null));

        await service.MarkReadAsync(conversationID, new ConversationActor { TenantID = tenantID, UserID = 42 });
        Assert.Equal(0, await service.CountUnreadConversationsAsync(tenantID, null));
        Assert.Equal(0, await service.CountUnreadConversationsAsync(null, null));
    }

    [Fact]
    public async Task Inbox_UnreadAndSearchFiltersApplyBeforePaging()
    {
        var orgs = new[] { ("Alpha Homes", Guid.NewGuid()), ("Beta Lofts", Guid.NewGuid()), ("Gamma Rentals", Guid.NewGuid()) };
        var threads = new Dictionary<string, Guid>();
        foreach (var (name, id) in orgs)
        {
            db.Organizations.Add(RequiredStrings(new Organization { OrganizationID = id, DisplayName = name }));
            await db.SaveChangesAsync();
            threads[name] = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = id })).ConversationID!.Value;
        }
        await service.SendMessageAsync(new SendMessageRequest { ConversationID = threads["Alpha Homes"], SenderOrganizationMemberID = Guid.NewGuid(), Message = "Hi from Alpha" });
        foreach (var name in new[] { "Beta Lofts", "Gamma Rentals" })
            await service.SendMessageAsync(new SendMessageRequest { ConversationID = threads[name], SenderTenantID = tenantID, SenderUserID = 42, Message = "Tenant note" });
        db.ChangeTracker.Clear();

        var unread = await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, UnreadOnly = true, PageNumber = 1, PageSize = 1 });
        Assert.Equal(1, unread.TotalCount);
        Assert.Equal(threads["Alpha Homes"], Assert.Single(unread.Data).ConversationID);

        var search = await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, Search = "  beta ", PageNumber = 1, PageSize = 1 });
        Assert.Equal(1, search.TotalCount);
        Assert.Equal(threads["Beta Lofts"], Assert.Single(search.Data).ConversationID);

        var all = await service.GetInboxAsync(new InboxQuery { TenantID = tenantID, PageNumber = 1, PageSize = 10 });
        Assert.Equal(3, all.TotalCount);
    }

    [Fact]
    public async Task Thread_FirstPageIsNewestInReadingOrderAndBeforeLoadsOlder()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        var origin = DateTime.UtcNow.AddHours(-1);
        for (var i = 0; i < 5; i++)
            db.ConversationMessages.Add(RequiredStrings(new ConversationMessage
            {
                ConversationMessageID = Guid.NewGuid(), ConversationID = conversationID, MessageType = "TEXT",
                Message = $"m{i}", SenderTenantID = tenantID, SentAt = origin.AddMinutes(i), CapturedDate = origin.AddMinutes(i)
            }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var newest = await service.GetThreadMessagesAsync(conversationID, 1, 2);
        Assert.Equal(5, newest.TotalCount);
        Assert.Equal(new[] { "m3", "m4" }, newest.Data.Select(m => m.Message));

        var older = await service.GetThreadMessagesAsync(conversationID, 1, 2, newest.Data.First().SentAt);
        Assert.Equal(3, older.TotalCount);
        Assert.Equal(new[] { "m1", "m2" }, older.Data.Select(m => m.Message));
    }

    [Fact]
    public async Task SendUserMessage_VerifiesSenderAndForcesPlainText()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)).ConversationID!.Value;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderTenantID = Guid.NewGuid(), SenderUserID = 42, Message = "Outsider"
        }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 999, Message = "Impersonation"
        }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = Guid.NewGuid(), SenderUserID = 42, Message = "Fake host"
        }));

        var sent = await service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, MessageType = "ENQUIRY_CARD", Message = "  Hello  "
        });
        Assert.Equal("TEXT", sent.MessageType);
        Assert.Equal("Hello", sent.Message);
        Assert.Equal(3, (await db.ConversationMessages.ToListAsync()).Count);
    }

    [Fact]
    public async Task Replies_MustQuoteSameThreadAndCarryQuotedPreview()
    {
        var hostID = await SeedHostAsync();
        var conversationID = (await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)).ConversationID!.Value;
        var otherOrg = Guid.NewGuid();
        var otherThread = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = otherOrg })).ConversationID!.Value;
        var foreign = await service.SendMessageAsync(new SendMessageRequest { ConversationID = otherThread, SenderTenantID = tenantID, Message = "Elsewhere" });
        db.ChangeTracker.Clear();
        var question = (await OrderedMessagesAsync()).First(m => m.ConversationID == conversationID);

        await Assert.ThrowsAsync<ArgumentException>(() => service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = hostID, SenderUserID = 43, Message = "Re", ReplyToMessageID = foreign.ConversationMessageID
        }));

        var reply = await service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = hostID, SenderUserID = 43, Message = "Yes it is", ReplyToMessageID = question.ConversationMessageID
        });
        Assert.Equal("Is this still available?", reply.ReplyToPreview);
        Assert.Equal("Elijah", reply.ReplyToSenderName);
        var fromThread = (await service.GetThreadMessagesAsync(conversationID, 1, 30)).Data.Single(m => m.ConversationMessageID == reply.ConversationMessageID);
        Assert.Equal("Is this still available?", fromThread.ReplyToPreview);
    }

    [Fact]
    public async Task Block_StopsBothSidesAndOnlyTheBlockerCanUnblock()
    {
        var hostID = await SeedHostAsync();
        var conversationID = (await service.GetOrCreateDirectThreadAsync(Enquiry(listingID), 42)).ConversationID!.Value;
        var tenant = new ConversationActor { TenantID = tenantID, UserID = 42 };
        var host = new ConversationActor { OrganizationMemberID = hostID, UserID = 43 };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SetBlockedAsync(conversationID, new ConversationActor { TenantID = tenantID, UserID = 999 }, true));
        Assert.Equal(ConversationStatuses.BlockedByTenant, await service.SetBlockedAsync(conversationID, tenant, true));
        Assert.Equal(ConversationStatuses.BlockedByTenant, await service.SetBlockedAsync(conversationID, host, true));

        var send = await Assert.ThrowsAsync<ApiProblemException>(() => service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = hostID, SenderUserID = 43, Message = "Hello?"
        }));
        Assert.Equal(409, send.StatusCode);
        var enquiry = await Assert.ThrowsAsync<ApiProblemException>(() => service.GetOrCreateDirectThreadAsync(Enquiry(listingID, "Again"), 42));
        Assert.Equal(409, enquiry.StatusCode);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SetBlockedAsync(conversationID, host, false));
        Assert.Equal(ConversationStatuses.Open, await service.SetBlockedAsync(conversationID, tenant, false));
        await service.SendUserMessageAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = hostID, SenderUserID = 43, Message = "Back again"
        });
    }

    [Fact]
    public async Task SendUserMessage_RateLimitsBursts()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        SendMessageRequest Next(int i) => new() { ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, Message = $"#{i}" };
        for (var i = 0; i < 20; i++)
            await service.SendUserMessageAsync(Next(i));
        var error = await Assert.ThrowsAsync<ApiProblemException>(() => service.SendUserMessageAsync(Next(20)));
        Assert.Equal(429, error.StatusCode);
        Assert.Equal(20, (await db.ConversationMessages.ToListAsync()).Count);
    }

    [Fact]
    public async Task Attachments_TrustFileBytesAndExposeReadUrl()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        SendMessageRequest Request() => new() { ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, AttachmentSendKey = Guid.NewGuid() };

        var unavailable = await Assert.ThrowsAsync<ApiProblemException>(() => service.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length));
        Assert.Equal(503, unavailable.StatusCode);

        var uploads = new List<string>();
        var storage = Stub<IFileStorageService>((method, args) => method.Name switch
        {
            nameof(IFileStorageService.UploadAsync) => Task.FromResult(Record(uploads, (string)args![1]!)),
            nameof(IFileStorageService.GetReadSasUrlAsync) => Task.FromResult<string?>($"https://files.test/{args![1]}"),
            _ => throw new NotSupportedException(method.Name)
        });
        var withStorage = CreateService(storage);

        var script = "<script>alert(1)</script>"u8.ToArray();
        await Assert.ThrowsAsync<ArgumentException>(() => withStorage.SendAttachmentAsync(Request(), new MemoryStream(script), "photo.png", script.Length));
        Assert.Empty(uploads);

        var sent = await withStorage.SendAttachmentAsync(Request(), new MemoryStream(Png), "../../evil name.exe", Png.Length);
        Assert.Equal("ATTACHMENT", sent.MessageType);
        var stored = Assert.Single(uploads);
        Assert.EndsWith(".png", stored);
        Assert.DoesNotContain("/", stored);
        Assert.Equal("https://files.test/blob-1", sent.AttachmentUrl);
        var row = Assert.Single((await withStorage.GetInboxAsync(new InboxQuery { TenantID = tenantID, PageNumber = 1, PageSize = 10 })).Data);
        Assert.StartsWith("\U0001F4CE ", row.LastMessagePreview);
    }

    [Fact]
    public async Task Attachments_LostResponseReplaysDurablyWithoutUploadOrNotifications()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        var key = Guid.NewGuid();
        var uploads = new List<string>();
        var notifications = 0;
        var pushes = 0;
        var client = Stub<IClientProxy>((_, _) => { pushes++; return Task.CompletedTask; });
        var clients = Stub<IHubClients>((_, _) => client);
        hub = Stub<IHubContext<MessagingHub>>((_, _) => clients);
        var preferences = Stub<IPreferenceService>((_, _) => { notifications++; return Task.FromResult<Preference?>(null); });
        var storage = Stub<IFileStorageService>((method, args) => method.Name switch
        {
            nameof(IFileStorageService.UploadAsync) => Task.FromResult(Record(uploads, (string)args![1]!)),
            nameof(IFileStorageService.GetReadSasUrlAsync) => Task.FromResult<string?>($"https://files.test/{args![1]}"),
            _ => throw new NotSupportedException(method.Name)
        });
        SendMessageRequest Request() => new() { ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, AttachmentSendKey = key };
        var first = await CreateService(storage, preferences).SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
        Assert.Equal(1, notifications);
        Assert.Equal(1, pushes);
        db.ChangeTracker.Clear();
        await using var retryDb = new TestDbContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        var retryService = CreateService(storage, preferences, retryDb);
        // A fresh request scope has no in-memory knowledge of the first, whose response the caller lost.
        for (var i = 0; i < 22; i++)
        {
            var replay = await retryService.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
            Assert.Equal(first.ConversationMessageID, replay.ConversationMessageID);
            Assert.Equal(key, replay.AttachmentSendKey);
            Assert.NotNull(replay.AttachmentUrl);
        }
        Assert.Single(uploads);
        Assert.Single(await db.ConversationMessages.ToListAsync());
        Assert.Equal(1, pushes);
        Assert.Equal(1, notifications);

        foreach (var request in new[] { Request(), Request(), Request() }.Select((request, i) =>
        {
            if (i == 0) request.ReplyToMessageID = first.ConversationMessageID;
            return (request, index: i);
        }))
        {
            var bytes = request.index == 2 ? Png.Concat(new byte[] { 1 }).ToArray() : Png;
            var error = await Assert.ThrowsAsync<ApiProblemException>(() => retryService.SendAttachmentAsync(
                request.request, new MemoryStream(bytes), request.index == 1 ? "other.png" : "a.png", bytes.Length));
            Assert.Equal(409, error.StatusCode);
        }
        var unauthorized = Request();
        unauthorized.SenderUserID = 999;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => retryService.SendAttachmentAsync(unauthorized, new MemoryStream(Png), "a.png", Png.Length));
        Assert.Single(uploads);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddHybridCache();
        using var provider = services.BuildServiceProvider();
        var legacy = new ConversationMessageService(mapper, provider.GetRequiredService<HybridCache>(),
            Options.Create(new CacheConfiguration()), NullLogger<ConversationMessageService>.Instance,
            new ConversationMessageRepository(db));
        // Legacy CRUD must not overwrite or hard-delete the durable replay identity.
        await Assert.ThrowsAsync<ApiProblemException>(() => legacy.UpdateConversationMessage(first.ConversationMessageID!.Value, first));
        Assert.Null(mapper.Map<ConversationMessage>(first).AttachmentSendKey);
        await legacy.DeleteConversationMessage(first.ConversationMessageID!.Value);
        var deletedReplay = await retryService.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
        Assert.Equal(first.ConversationMessageID, deletedReplay.ConversationMessageID);
        Assert.NotNull(deletedReplay.DeletedAt);
        Assert.Null(deletedReplay.AttachmentUrl);
        Assert.Single(uploads);
        Assert.Equal(1, notifications);
    }

    [Fact]
    public async Task Attachments_ConcurrentRequestsInSeparateScopesUploadAndNotifyOnce()
    {
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        var key = Guid.NewGuid();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var uploads = 0;
        var notifications = 0;
        var storage = Stub<IFileStorageService>((method, _) => method.Name switch
        {
            nameof(IFileStorageService.UploadAsync) => Upload(),
            nameof(IFileStorageService.GetReadSasUrlAsync) => Task.FromResult<string?>("https://files.test/blob"),
            _ => throw new NotSupportedException(method.Name)
        });
        async Task<BlobUploadResult> Upload()
        {
            Interlocked.Increment(ref uploads);
            started.TrySetResult();
            await resume.Task;
            return new BlobUploadResult { BlobName = "blob" };
        }
        var preferences = Stub<IPreferenceService>((_, _) => { Interlocked.Increment(ref notifications); return Task.FromResult<Preference?>(null); });
        SendMessageRequest Request() => new() { ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, AttachmentSendKey = key };
        var first = CreateService(storage, preferences).SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await using var retryDb = new TestDbContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection.ConnectionString).Options, configuration);
        var second = Task.Run(() => CreateService(storage, preferences, retryDb).SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length));
        try
        {
            await Task.Delay(150);
            Assert.Equal(1, uploads);
        }
        finally { resume.TrySetResult(); }
        var saved = await Task.WhenAll(first, second).WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(saved[0].ConversationMessageID, saved[1].ConversationMessageID);
        Assert.Equal(1, uploads);
        Assert.Equal(1, notifications);
        Assert.Single(await db.ConversationMessages.ToListAsync());
    }

    [SqlServerFact]
    public async Task Attachments_SqlServerConcurrentSendAndAuthorizedReplayAreIdempotent()
    {
        var databaseName = $"ArcoraAttachmentConcurrency_{Guid.NewGuid():N}";
        var connectionString = new SqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER"))
        {
            InitialCatalog = databaseName
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(connectionString).Options;
        await using var schema = new ArcoraDbContext(options, configuration);
        try
        {
            await schema.Database.EnsureCreatedAsync();
            var renter = new User { UserName = "attachment-renter", Email = "attachment-renter@example.test", FirstName = "Renter" };
            var host = new User { UserName = "attachment-host", Email = "attachment-host@example.test", FirstName = "Host" };
            schema.Users.AddRange(renter, host);
            schema.Organizations.Add(RequiredStrings(new Organization { OrganizationID = organizationID }));
            await schema.SaveChangesAsync();
            schema.Tenants.Add(RequiredStrings(new Tenant { TenantID = tenantID, UserID = renter.Id }));
            schema.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
            {
                OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationID, UserID = host.Id, Status = "ACTIVE"
            }));
            var conversationID = Guid.NewGuid();
            schema.Conversations.Add(new Conversation
            {
                ConversationID = conversationID, TenantID = tenantID, OrganizationID = organizationID, Status = ConversationStatuses.Open
            });
            await schema.SaveChangesAsync();
            var key = Guid.NewGuid();
            var uploadStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseUpload = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var lockAttempt = new AttachmentLockInterceptor();
            var uploads = 0;
            var emails = 0;
            var pushes = 0;
            var client = Stub<IClientProxy>((_, _) => { Interlocked.Increment(ref pushes); return Task.CompletedTask; });
            var clients = Stub<IHubClients>((_, _) => client);
            hub = Stub<IHubContext<MessagingHub>>((_, _) => clients);
            var queue = Stub<IEmailQueue>((_, _) => { Interlocked.Increment(ref emails); return ValueTask.CompletedTask; });
            var preferences = Stub<IPreferenceService>((_, _) => Task.FromResult<Preference?>(null));
            var storage = Stub<IFileStorageService>((method, _) => method.Name switch
            {
                nameof(IFileStorageService.UploadAsync) => Upload(),
                nameof(IFileStorageService.GetReadSasUrlAsync) => Task.FromResult<string?>("https://files.test/sql-attachment"),
                _ => throw new NotSupportedException(method.Name)
            });
            async Task<BlobUploadResult> Upload()
            {
                Interlocked.Increment(ref uploads);
                uploadStarted.TrySetResult();
                await releaseUpload.Task.WaitAsync(TimeSpan.FromSeconds(20));
                return new BlobUploadResult { BlobName = "sql-attachment" };
            }
            SendMessageRequest Request() => new()
            {
                ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = renter.Id, AttachmentSendKey = key
            };
            await using var firstDb = new ArcoraDbContext(options, configuration);
            await using var secondDb = new ArcoraDbContext(
                new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(connectionString).AddInterceptors(lockAttempt).Options, configuration);
            var firstService = CreateService(storage, preferences, firstDb, queue);
            var secondService = CreateService(storage, preferences, secondDb, queue);
            var first = firstService.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
            Task<Arcora.Api.DTOs.ConversationMessageDto>? second = null;
            try
            {
                await uploadStarted.Task.WaitAsync(TimeSpan.FromSeconds(10));
                second = secondService.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
                await lockAttempt.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
                await Task.Delay(150);
                Assert.False(second.IsCompleted, "The second SQL session must wait on the first attachment transaction.");
                Assert.Equal(1, uploads);
            }
            finally { releaseUpload.TrySetResult(); }
            var saved = await Task.WhenAll(first, second!).WaitAsync(TimeSpan.FromSeconds(20));
            Assert.Equal(saved[0].ConversationMessageID, saved[1].ConversationMessageID);
            Assert.Equal(1, uploads);
            Assert.Equal(1, emails);
            Assert.Equal(1, pushes);
            Assert.Equal(1, await schema.ConversationMessages.CountAsync());

            await using var replayDb = new ArcoraDbContext(options, configuration);
            var replayService = CreateService(storage, preferences, replayDb, queue);
            var replay = await replayService.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
            Assert.Equal(saved[0].ConversationMessageID, replay.ConversationMessageID);
            Assert.NotNull(replay.AttachmentUrl);
            var changed = Png.Concat(new byte[] { 1 }).ToArray();
            var conflict = await Assert.ThrowsAsync<ApiProblemException>(() => replayService.SendAttachmentAsync(
                Request(), new MemoryStream(changed), "a.png", changed.Length));
            Assert.Equal(409, conflict.StatusCode);
            var impersonator = Request();
            impersonator.SenderUserID = host.Id;
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => replayService.SendAttachmentAsync(
                impersonator, new MemoryStream(Png), "a.png", Png.Length));
            Assert.Equal(1, uploads);
            Assert.Equal(1, emails);
            Assert.Equal(1, pushes);
            Assert.Equal(1, await schema.ConversationMessages.CountAsync());
        }
        finally
        {
            Assert.Equal(databaseName, schema.Database.GetDbConnection().Database);
            await schema.Database.EnsureDeletedAsync();
        }
    }

    private sealed class AttachmentLockInterceptor : DbCommandInterceptor
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (command.CommandText.Contains("sp_getapplock", StringComparison.Ordinal))
                Started.TrySetResult();
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }
    }

    [Fact]
    public async Task Attachments_KeyIsScopedToSenderAndConversationAndRequired()
    {
        var memberID = await SeedHostAsync();
        var conversationID = (await service.GetOrCreateDirectThreadAsync(new StartThreadRequest { TenantID = tenantID, OrganizationID = organizationID })).ConversationID!.Value;
        var key = Guid.NewGuid();
        var uploads = new List<string>();
        var storage = Stub<IFileStorageService>((method, args) => method.Name switch
        {
            nameof(IFileStorageService.UploadAsync) => Task.FromResult(Record(uploads, (string)args![1]!)),
            nameof(IFileStorageService.GetReadSasUrlAsync) => Task.FromResult<string?>("https://files.test/blob"),
            _ => throw new NotSupportedException(method.Name)
        });
        var sender = CreateService(storage);
        SendMessageRequest Request() => new() { ConversationID = conversationID, SenderTenantID = tenantID, SenderUserID = 42, AttachmentSendKey = key };
        var missing = Request();
        missing.AttachmentSendKey = null;
        await Assert.ThrowsAsync<ArgumentException>(() => sender.SendAttachmentAsync(missing, new MemoryStream(Png), "a.png", Png.Length));
        var empty = Request();
        empty.AttachmentSendKey = Guid.Empty;
        await Assert.ThrowsAsync<ArgumentException>(() => sender.SendAttachmentAsync(empty, new MemoryStream(Png), "a.png", Png.Length));
        Assert.Empty(uploads);
        await sender.SendAttachmentAsync(Request(), new MemoryStream(Png), "a.png", Png.Length);
        await sender.SendAttachmentAsync(new SendMessageRequest
        {
            ConversationID = conversationID, SenderOrganizationMemberID = memberID, SenderUserID = 43, AttachmentSendKey = key
        }, new MemoryStream(Png), "host.png", Png.Length);
        var secondThread = RequiredStrings(new Conversation { ConversationID = Guid.NewGuid(), TenantID = tenantID, OrganizationID = organizationID, Status = ConversationStatuses.Open });
        db.Conversations.Add(secondThread);
        await db.SaveChangesAsync();
        var otherConversation = Request();
        otherConversation.ConversationID = secondThread.ConversationID;
        await sender.SendAttachmentAsync(otherConversation, new MemoryStream(Png), "different.png", Png.Length);
        Assert.Equal(3, uploads.Count);
        Assert.Equal(3, await db.ConversationMessages.CountAsync());

        var original = (await db.ConversationMessages.AsNoTracking().ToListAsync()).First(m => m.SenderTenantID == tenantID && m.ConversationID == conversationID);
        db.ChangeTracker.Clear();
        original.ConversationMessageID = Guid.NewGuid();
        db.ConversationMessages.Add(original);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task ReportEndpoint_RequiresReason()
    {
        var result = await new MessagingController().Report(null!, Guid.NewGuid(), new ConversationModerationRequest { Reason = "  " });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0x0D, 0x49, 0x48, 0x44, 0x52];

    private static BlobUploadResult Record(List<string> uploads, string fileName)
    {
        uploads.Add(fileName);
        return new BlobUploadResult { BlobName = $"blob-{uploads.Count}" };
    }

    private async Task<Guid> SeedHostAsync()
    {
        var renter = await db.Users.SingleAsync(u => u.Id == 42);
        renter.FirstName = "Elijah";
        db.Users.Add(new User
        {
            Id = 43, UserName = "host-mia", NormalizedUserName = "HOST-MIA", Email = "mia@example.test",
            SecurityStamp = Guid.NewGuid().ToString(), FirstName = "Mia"
        });
        var memberID = Guid.NewGuid();
        db.OrganizationMembers.Add(RequiredStrings(new OrganizationMember
        {
            OrganizationMemberID = memberID, OrganizationID = organizationID, UserID = 43, Status = "ACTIVE"
        }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return memberID;
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    private static readonly DateOnly Start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14);
    private static readonly DateOnly End = Start.AddMonths(6);

    private static StartThreadRequest Enquiry(Guid listing, string message = "Is this still available?") => new()
    {
        ListingID = listing, Message = message, ProposedStartDate = Start, ProposedEndDate = End
    };

    private async Task<List<ConversationMessage>> OrderedMessagesAsync() =>
        (await db.ConversationMessages.ToListAsync()).OrderBy(m => m.SentAt).ToList();

    private static ListingEnquiryCard ReadCard(ConversationMessage message) =>
        JsonSerializer.Deserialize<ListingEnquiryCard>(message.Message!, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;

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
