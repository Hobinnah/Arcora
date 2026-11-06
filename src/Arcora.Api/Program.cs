using Arcora.Api.Extensions;
using Arcora.Api.TokenServices;
using Arcora.Api.Middleware;
using Arcora.Api;
using Arcora.Api.Data;
using Arcora.Api.Accounts;
using Arcora.Api.Operations;
using Arcora.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
using var startupLogFactory = LoggerFactory.Create(logging => logging.AddConsole());
using var startupCancellation = new CancellationTokenSource();
ConsoleCancelEventHandler cancelStartup = (_, _) => startupCancellation.Cancel();
Console.CancelKeyPress += cancelStartup;
KeyVaultStartupState keyVaultStartup;
try
{
    keyVaultStartup = await KeyVaultStartup.LoadAsync(builder.Configuration, builder.Environment,
        startupLogFactory.CreateLogger("KeyVaultStartup"), startupCancellation.Token);
}
finally
{
    Console.CancelKeyPress -= cancelStartup;
}
builder.Services.AddSingleton(keyVaultStartup);

// Add services to the container.
builder.Services.AddScoped<Arcora.Api.Services.Implementations.PropertyLocationAccess>();
builder.Services.AddSingleton<Arcora.Api.Services.Implementations.AzureMapsService>();
builder.Services.AddControllers(options => options.Filters.Add<Arcora.Api.Filters.PropertyLocationPrivacyFilter>()).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
var signalRBuilder = builder.Services.AddSignalR();
var azureSignalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];
if (!string.IsNullOrWhiteSpace(azureSignalRConnectionString))
{
    signalRBuilder.AddAzureSignalR(azureSignalRConnectionString);
}
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("AzureMaps", client => client.Timeout = TimeSpan.FromSeconds(10)).RemoveAllLoggers();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("MapsPolicy", context => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
        { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddOperationsMonitoring(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Arcora Web API",
        Description = "ASP.NET Core Web API",
        TermsOfService = new Uri("https://arcora.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "",
            Email = string.Empty,
            Url = new Uri("https://arcora.com/csr/spboyer"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://arcora.com/csr/license"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: Authorization: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

if (args.Contains("--reconcile-rent-payment", StringComparer.Ordinal))
{
    var index = Array.IndexOf(args, "--reconcile-rent-payment");
    if (args.Length != index + 8 || !Guid.TryParse(args[index + 1], out var paymentId))
        throw new ArgumentException("Usage: --reconcile-rent-payment paymentId rent deposit tenantFees placementFee monthlyFee auditActor");
    var amounts = args.Skip(index + 2).Take(5).Select(value =>
        decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
    await new Arcora.Api.Services.Implementations.RentPaymentAccountingService(db).ReconcileAsync(
        paymentId, new(amounts[0], amounts[1], amounts[2], amounts[3], amounts[4]), args[index + 7], CancellationToken.None);
    app.Logger.LogInformation("Rent payment {PaymentID} accounting reconciled. No provider debit or payout was initiated.", paymentId);
    return;
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    ValidateCriticalVerificationConfiguration(app.Configuration, app.Logger, app.Environment);
}

if (args.Contains("--normalize-account-phones", StringComparer.Ordinal))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
    if (!db.Database.IsSqlServer())
    {
        Console.Error.WriteLine("Phone preflight is supported only for the configured SQL Server database.");
        Environment.ExitCode = 1;
        return;
    }

    var result = await AccountPhoneNormalizationPreflight.RunAsync(db);
    if (!result.Succeeded)
    {
        Console.Error.WriteLine("Phone preflight made no changes. Correct invalid phone values or resolve duplicate accounts, then rerun.");
        foreach (var issue in result.Issues)
        {
            var issueType = issue.IsDuplicate ? "Duplicate canonical number" : "Invalid phone number";
            Console.Error.WriteLine($"{issueType}; account IDs: {string.Join(", ", issue.AccountIds)}");
        }

        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine($"Phone preflight succeeded. Canonicalized {result.UpdatedCount} account phone number(s); no phone values were output.");
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arcora API v1");
        c.RoutePrefix = string.Empty;
    });

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

    await context.Database.MigrateAsync();

    var forceReseed = builder.Configuration.GetValue<bool>("SeedData:ForceReseed");
    await DbSeeder.SeedAsync(context, seedLogger, forceReseed);
}

app.UseHttpsRedirection();
app.UseMiddleware<OperationsRequestMetricsMiddleware>();
app.UseGlobalExceptionMiddleware();
app.UseCors("EnableCORS");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseMiddleware<TokenManagerMiddleware>();

app.MapControllers();
app.MapHub<Arcora.Api.Realtime.MessagingHub>("/hubs/messaging", options =>
    options.CloseOnAuthenticationExpiration = true);

app.Run();

static void ValidateCriticalVerificationConfiguration(IConfiguration configuration, ILogger logger, IHostEnvironment environment)
{
    var missingKeys = new List<string>();

    var defaultProvider = configuration["IdentityVerification:DefaultProvider"]?.Trim();
    if (string.IsNullOrWhiteSpace(defaultProvider))
    {
        missingKeys.Add("IdentityVerification:DefaultProvider");
        defaultProvider = string.Empty;
    }

    var normalizedProvider = defaultProvider.ToUpperInvariant();
    if (!string.IsNullOrWhiteSpace(defaultProvider)
        && normalizedProvider is not ("PERSONA" or "ONFIDO"))
    {
        missingKeys.Add("IdentityVerification:DefaultProvider (must be PERSONA or ONFIDO)");
    }

    if (normalizedProvider == "PERSONA")
    {
        AddIfMissing("Persona:ApiKey");
        AddIfMissing("Persona:TemplateId");
        AddIfMissing("Persona:WebhookSecret");
    }

    if (normalizedProvider == "ONFIDO")
    {
        AddIfMissing("Onfido:ApiKey");
        AddIfMissing("Onfido:WebhookSecret");
    }

    ValidatePhoneVerificationConfiguration();

    if (missingKeys.Count == 0)
        return;

    var message = $"Missing or invalid configuration keys: {string.Join(", ", missingKeys)}";
    logger.LogCritical("{Message}", message);
    throw new InvalidOperationException(message);

    void ValidatePhoneVerificationConfiguration()
    {
        var provider = configuration["PhoneVerification:Provider"]?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(provider))
            provider = "INFOBIP";

        if (provider == "LOCAL")
        {
            if (!environment.IsDevelopment())
                missingKeys.Add("PhoneVerification:Provider LOCAL is allowed only in Development.");
            if (!configuration.GetValue<bool>("PhoneVerification:EnableDevelopmentOtp"))
                missingKeys.Add("PhoneVerification:EnableDevelopmentOtp must be true for LOCAL.");
            return;
        }

        if (provider is not ("AUTO" or "TWILIO" or "INFOBIP"))
        {
            missingKeys.Add("PhoneVerification:Provider (must be AUTO, TWILIO, INFOBIP, or LOCAL in Development)");
            return;
        }

        var twilioConfigured = IsTwilioConfigured();
        var infobipConfigured = IsInfobipConfigured();

        if (provider == "TWILIO" && !twilioConfigured)
        {
            missingKeys.Add("Twilio:Verify:AccountSid");
            missingKeys.Add("Twilio:Verify:AuthToken");
            missingKeys.Add("Twilio:Verify:ServiceSid");
        }

        if (provider == "INFOBIP" && !infobipConfigured)
        {
            missingKeys.Add("Infobip:ApiKey");
            missingKeys.Add("Infobip:BaseUrl");
            missingKeys.Add("Infobip:Sender");
        }

        if (provider == "AUTO" && !twilioConfigured && !infobipConfigured)
        {
            missingKeys.Add("Twilio:Verify:* or Infobip:* (configure at least one provider)");
        }
    }

    bool IsTwilioConfigured()
        => !string.IsNullOrWhiteSpace(configuration["Twilio:Verify:AccountSid"])
           && !string.IsNullOrWhiteSpace(configuration["Twilio:Verify:AuthToken"])
           && !string.IsNullOrWhiteSpace(configuration["Twilio:Verify:ServiceSid"]);

    bool IsInfobipConfigured()
        => !string.IsNullOrWhiteSpace(configuration["Infobip:ApiKey"])
           && !string.IsNullOrWhiteSpace(configuration["Infobip:BaseUrl"])
           && !string.IsNullOrWhiteSpace(configuration["Infobip:Sender"]);

    void AddIfMissing(string key)
    {
        if (string.IsNullOrWhiteSpace(configuration[key]))
            missingKeys.Add(key);
    }
}

public partial class Program { }
