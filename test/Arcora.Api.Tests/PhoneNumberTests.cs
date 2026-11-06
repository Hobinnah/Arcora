using Arcora.Api;
using Arcora.Api.Accounts;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Arcora.Api.Tests;

public sealed class PhoneNumberTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private PhoneTestDbContext db = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        db = new PhoneTestDbContext(options, new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Theory]
    [InlineData("+1 (415) 555-2671", "+14155552671")]
    [InlineData("+44 20 8366 1177", "+442083661177")]
    [InlineData("+44 (0)20 8366 1177", "+442083661177")]
    [InlineData("+234 803 123 4567", "+2348031234567")]
    public void TryNormalize_ValidInternationalNumber_ReturnsCanonicalE164(string input, string expected)
    {
        var succeeded = PhoneNumberNormalizer.TryNormalize(input, out var normalized);

        Assert.True(succeeded);
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData("4155552671")]
    [InlineData("+1 123")]
    [InlineData("+1 415 555 2671 ext 23")]
    [InlineData("")]
    [InlineData(null)]
    public void TryNormalize_InvalidOrNonInternationalNumber_IsRejected(string? input)
    {
        var succeeded = PhoneNumberNormalizer.TryNormalize(input, out var normalized);

        Assert.False(succeeded);
        Assert.Empty(normalized);
    }

    [Fact]
    public async Task PhoneIndex_RejectsDistinctLegacyFormatsAfterCanonicalNormalization()
    {
        var index = db.Model.FindEntityType(typeof(User))!.GetIndexes()
            .Single(item => item.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(User.PhoneNumber) }));
        Assert.True(index.IsUnique);
        Assert.Equal("[PhoneNumber] IS NOT NULL", index.GetFilter());

        var firstNumber = Normalize("+44 (0)20 8366 1177");
        var secondNumber = Normalize("+44 20 8366 1177");
        Assert.Equal(firstNumber, secondNumber);

        db.Users.Add(NewUser(1, firstNumber));
        await db.SaveChangesAsync();

        db.Users.Add(NewUser(2, secondNumber));
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());

        db.ChangeTracker.Clear();
        Assert.Single(await db.Users.ToListAsync());
    }

    [Fact]
    public async Task Preflight_DetectsParserEquivalentLegacyNumbersWithoutChangingAnyAccount()
    {
        db.Users.AddRange(
            NewUser(11, "+44 (0)20 8366 1177"),
            NewUser(12, "+44 20 8366 1177"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var result = await AccountPhoneNormalizationPreflight.RunAsync(db);

        Assert.False(result.Succeeded);
        Assert.Equal(0, result.UpdatedCount);
        var duplicate = Assert.Single(result.Issues, issue => issue.IsDuplicate);
        Assert.Equal(new long[] { 11, 12 }, duplicate.AccountIds);
        Assert.Equal(new[] { "+44 (0)20 8366 1177", "+44 20 8366 1177" },
            (await db.Users.OrderBy(user => user.Id).Select(user => user.PhoneNumber).ToListAsync()).ToArray());
    }

    [Fact]
    public async Task Preflight_NormalizesLegacyNumberWhenNoConflictExists()
    {
        db.Users.Add(NewUser(21, "+1 (415) 555-2671"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var result = await AccountPhoneNormalizationPreflight.RunAsync(db);

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.UpdatedCount);
        Assert.Equal("+14155552671", await db.Users.Where(user => user.Id == 21).Select(user => user.PhoneNumber).SingleAsync());
    }

    [Fact]
    public async Task Preflight_ReportsInvalidAccountIdsAndDoesNotPartiallyNormalize()
    {
        db.Users.AddRange(NewUser(25, "+1 (415) 555-2671"), NewUser(26, "not-an-international-number"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var result = await AccountPhoneNormalizationPreflight.RunAsync(db);

        Assert.False(result.Succeeded);
        Assert.Equal(0, result.UpdatedCount);
        var invalid = Assert.Single(result.Issues, issue => !issue.IsDuplicate);
        Assert.Equal(new long[] { 26 }, invalid.AccountIds);
        Assert.Equal("+1 (415) 555-2671", await db.Users.Where(user => user.Id == 25).Select(user => user.PhoneNumber).SingleAsync());
    }

    [Fact]
    public async Task ProfileUpdate_RejectsPhoneThatNormalizesToAnotherAccount()
    {
        db.Users.AddRange(NewUser(31, "+442083661177"), NewUser(32, null));
        await db.SaveChangesAsync();
        var userManager = CreateUserManager(db);
        var controller = new AccountController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, "32") }, "Test"))
                }
            }
        };

        var response = await controller.UpdateProfile(userManager, new AccountProfileUpdateDto
        {
            Phone = "+44 (0)20 8366 1177"
        });

        Assert.IsType<ConflictObjectResult>(response);
        Assert.Equal("+442083661177", await db.Users.Where(user => user.Id == 31).Select(user => user.PhoneNumber).SingleAsync());
        Assert.Null(await db.Users.Where(user => user.Id == 32).Select(user => user.PhoneNumber).SingleAsync());
    }

    [Fact]
    public async Task ProfileUpdate_ChangingPhone_ResetsPhoneConfirmation()
    {
        var user = NewUser(41, "+14155552671");
        user.PhoneNumberConfirmed = true;
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var userManager = CreateUserManager(db);
        var controller = new AccountController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, "41") }, "Test"))
                }
            }
        };

        var response = await controller.UpdateProfile(userManager, new AccountProfileUpdateDto
        {
            Phone = "+1 (306) 555-0100"
        });

        Assert.IsType<OkObjectResult>(response);
        var updatedUser = await db.Users.SingleAsync(item => item.Id == 41);
        Assert.Equal("+13065550100", updatedUser.PhoneNumber);
        Assert.False(updatedUser.PhoneNumberConfirmed);
    }

    [Theory]
    [InlineData("+1 (306) 555-0100", "+13065550100")]
    [InlineData("", null)]
    public async Task ProfileUpdate_ChangingPhone_InvalidatesPendingInfobipCode(string newPhone, string? expectedPhone)
    {
        db.Users.Add(NewUser(45, "+14155552671"));
        await db.SaveChangesAsync();

        var userManager = CreateUserManager(db);
        var smsClient = new FakeInfobipSmsClient();
        var phoneVerificationService = new PhoneVerificationService(
            userManager,
            new FakeTwilioVerifyClient(),
            smsClient,
            Options.Create(new PhoneVerificationOptions { Provider = "INFOBIP" }),
            Options.Create(new TwilioVerifyOptions()),
            Options.Create(new InfobipOptions()));
        var sendResult = await phoneVerificationService.SendVerificationCodeAsync(45);

        Assert.True(sendResult.Succeeded);
        Assert.Equal("+14155552671", smsClient.LastPhoneNumber);
        var sentCode = await db.Users.Where(user => user.Id == 45).Select(user => user.SmsCode).SingleAsync();
        Assert.False(string.IsNullOrWhiteSpace(sentCode));

        var controller = CreateAccountController(45);
        var response = await controller.UpdateProfile(userManager, new AccountProfileUpdateDto
        {
            Phone = newPhone
        });

        Assert.IsType<OkObjectResult>(response);
        var updatedUser = await db.Users.SingleAsync(user => user.Id == 45);
        Assert.Equal(expectedPhone, updatedUser.PhoneNumber);
        Assert.False(updatedUser.PhoneNumberConfirmed);
        Assert.Null(updatedUser.SmsCode);
        Assert.Null(updatedUser.SmsCodeExpiresAt);

        var confirmResult = await phoneVerificationService.ConfirmPhoneNumberAsync(45, sentCode!);

        Assert.False(confirmResult.Succeeded);
        Assert.False((await db.Users.SingleAsync(user => user.Id == 45)).PhoneNumberConfirmed);
    }

    [Fact]
    public async Task ConfirmPhoneNumberAsync_WhenPhoneChangesDuringTwilioCheck_DoesNotConfirmNewNumber()
    {
        db.Users.Add(NewUser(46, "+14155552671"));
        await db.SaveChangesAsync();

        var userManager = CreateUserManager(db);
        var twilioVerifyClient = new FakeTwilioVerifyClient();
        twilioVerifyClient.CheckHandler = async (phoneNumber, _, cancellationToken) =>
        {
            Assert.Equal("+14155552671", phoneNumber);
            await using var concurrentDb = new PhoneTestDbContext(
                new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options,
                new ConfigurationBuilder().Build());
            var concurrentUserManager = CreateUserManager(concurrentDb);
            var concurrentUser = await concurrentUserManager.FindByIdAsync("46");
            Assert.NotNull(concurrentUser);
            concurrentUser!.PhoneNumber = "+13065550100";
            concurrentUser.PhoneNumberConfirmed = false;
            concurrentUser.SmsCode = null;
            concurrentUser.SmsCodeExpiresAt = null;
            var updateResult = await concurrentUserManager.UpdateAsync(concurrentUser);
            Assert.True(updateResult.Succeeded);
            return TwilioVerifyResult.Success("approved", "VE123", true);
        };
        var service = new PhoneVerificationService(userManager, twilioVerifyClient);

        var result = await service.ConfirmPhoneNumberAsync(46, "123456");

        Assert.False(result.Succeeded);
        var savedUser = await db.Users.AsNoTracking().SingleAsync(user => user.Id == 46);
        Assert.Equal("+13065550100", savedUser.PhoneNumber);
        Assert.False(savedUser.PhoneNumberConfirmed);
    }

    [Fact]
    public async Task ConfirmPhoneNumberAsync_ApprovedCode_MarksPhoneAsConfirmed()
    {
        db.Users.Add(NewUser(51, "+14155552671"));
        await db.SaveChangesAsync();

        var userManager = CreateUserManager(db);
        var twilioVerifyClient = new FakeTwilioVerifyClient
        {
            CheckResult = TwilioVerifyResult.Success("approved", "VE123", true)
        };
        var service = new PhoneVerificationService(userManager, twilioVerifyClient);

        var result = await service.ConfirmPhoneNumberAsync(51, "123456");

        Assert.True(result.Succeeded);
        Assert.True(result.Value!.PhoneNumberConfirmed);
        Assert.True(await db.Users.Where(user => user.Id == 51).Select(user => user.PhoneNumberConfirmed).SingleAsync());
    }

    [Fact]
    public async Task SendVerificationCodeAsync_AlreadyConfirmed_SkipsTwilioRequest()
    {
        var user = NewUser(61, "+14155552671");
        user.PhoneNumberConfirmed = true;
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var userManager = CreateUserManager(db);
        var twilioVerifyClient = new FakeTwilioVerifyClient();
        var service = new PhoneVerificationService(userManager, twilioVerifyClient);

        var result = await service.SendVerificationCodeAsync(61);

        Assert.True(result.Succeeded);
        Assert.Equal(0, twilioVerifyClient.StartCalls);
        Assert.True(result.Value!.PhoneNumberConfirmed);
    }

    private static string Normalize(string input)
    {
        Assert.True(PhoneNumberNormalizer.TryNormalize(input, out var normalized));
        return normalized;
    }

    private static User NewUser(long id, string? phoneNumber) => new()
    {
        Id = id,
        UserName = $"user{id}",
        NormalizedUserName = $"USER{id}",
        Email = $"user{id}@example.test",
        NormalizedEmail = $"USER{id}@EXAMPLE.TEST",
        FirstName = "Test",
        LastName = $"User{id}",
        DisplayName = $"Test User{id}",
        SecurityStamp = Guid.NewGuid().ToString("N"),
        ConcurrencyStamp = Guid.NewGuid().ToString("N"),
        PhoneNumber = phoneNumber
    };

    private static AccountController CreateAccountController(long userId) => new()
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"))
            }
        }
    };

    private static UserManager<User> CreateUserManager(ArcoraDbContext context)
    {
        var store = new UserStore<User, Role, ArcoraDbContext, long>(context);
        return new UserManager<User>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(),
            new IUserValidator<User>[] { new UserValidator<User>() },
            new IPasswordValidator<User>[] { new PasswordValidator<User>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new ServiceCollection().BuildServiceProvider(),
            NullLogger<UserManager<User>>.Instance);
    }

    private sealed class FakeTwilioVerifyClient : ITwilioVerifyClient
    {
        public int StartCalls { get; private set; }
        public int CheckCalls { get; private set; }
        public TwilioVerifyResult StartResult { get; set; } = TwilioVerifyResult.Success("pending", "VE123");
        public TwilioVerifyResult CheckResult { get; set; } = TwilioVerifyResult.Success("approved", "VE123", true);
        public Func<string, string, CancellationToken, Task<TwilioVerifyResult>>? CheckHandler { get; set; }

        public Task<TwilioVerifyResult> StartVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            StartCalls++;
            return Task.FromResult(StartResult);
        }

        public Task<TwilioVerifyResult> CheckVerificationAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
        {
            CheckCalls++;
            return CheckHandler is null
                ? Task.FromResult(CheckResult)
                : CheckHandler(phoneNumber, code, cancellationToken);
        }
    }

    private sealed class FakeInfobipSmsClient : IInfobipSmsClient
    {
        public string? LastPhoneNumber { get; private set; }

        public Task<InfobipSmsResult> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            LastPhoneNumber = phoneNumber;
            return Task.FromResult(InfobipSmsResult.Success());
        }
    }

    private sealed class PhoneTestDbContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration)
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
