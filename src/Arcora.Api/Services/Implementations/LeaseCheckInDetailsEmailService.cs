using System.Globalization;
using Arcora.Api.Email;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services.Implementations;

public class LeaseCheckInDetailsEmailService : BackgroundService
{
    private const string ProviderName = "INTERNAL_LEASE_CHECKIN";
    private readonly ILogger<LeaseCheckInDetailsEmailService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;

    public LeaseCheckInDetailsEmailService(
        ILogger<LeaseCheckInDetailsEmailService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalHours = Math.Max(1, configuration.GetValue<int?>("LeaseCheckIn:NotificationPollingHours") ?? 6);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(intervalHours));

        do
        {
            try
            {
                await RunCycleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lease check-in details email cycle failed.");
            }
        }
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
        var preferenceService = scope.ServiceProvider.GetRequiredService<IPreferenceService>();
        var emailQueue = scope.ServiceProvider.GetRequiredService<IEmailQueue>();

        var leadDays = Math.Max(1, configuration.GetValue<int?>("LeaseCheckIn:DetailsLeadDays") ?? 3);
        var targetDate = DateTime.UtcNow.Date.AddDays(leadDays);
        var targetEnd = targetDate.AddDays(1);

        var preference = await preferenceService.GetPreference();
        var companyName = string.IsNullOrWhiteSpace(preference?.CompanyName) ? "Arcora" : preference!.CompanyName!;
        var supportEmail = preference?.CompanyEmail;
        var frontendUrl = (configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');

        var leases = await dbContext.Leases
            .AsNoTracking()
            .Include(x => x.Tenant)!.ThenInclude(x => x!.User)
            .Include(x => x.Listing)!
                .ThenInclude(x => x!.RentalUnit)!
                    .ThenInclude(x => x!.Property)!
                        .ThenInclude(x => x!.Address)
            .Where(x => x.Status == "ACTIVE" && x.StartDate >= targetDate && x.StartDate < targetEnd)
            .ToListAsync(cancellationToken);

        foreach (var lease in leases)
        {
            var tenantEmail = lease.Tenant?.User?.Email;
            if (string.IsNullOrWhiteSpace(tenantEmail))
                continue;

            var alreadySent = await dbContext.PaymentProviderEvents
                .AsNoTracking()
                .AnyAsync(x => x.ProviderName == ProviderName && x.ProviderEventID == BuildEventId(lease.LeaseID), cancellationToken);
            if (alreadySent)
                continue;

            var occupancyConflict = await dbContext.Leases
                .AsNoTracking()
                .AnyAsync(x =>
                    x.LeaseID != lease.LeaseID &&
                    x.RentalUnitID == lease.RentalUnitID &&
                    x.Status == "ACTIVE" &&
                    x.StartDate < lease.StartDate.AddDays(1) &&
                    (x.EndDate == null || x.EndDate > lease.StartDate), cancellationToken);

            if (occupancyConflict)
                continue;

            var listing = lease.Listing;
            if (listing == null)
                continue;

            var instruction = await dbContext.ListingAccessInstructions
                .AsNoTracking()
                .Where(x => x.ListingID == listing.ListingID &&
                            x.IsActive &&
                            (x.AvailableFrom == null || x.AvailableFrom <= lease.StartDate) &&
                            (x.AvailableUntil == null || x.AvailableUntil >= lease.StartDate))
                .OrderByDescending(x => x.CapturedDate)
                .Select(x => x.Instructions)
                .FirstOrDefaultAsync(cancellationToken);

            var checkInInstructions = !string.IsNullOrWhiteSpace(instruction)
                ? instruction!
                : (string.IsNullOrWhiteSpace(listing.CheckInDoorCode)
                    ? "Your host will share access instructions shortly."
                    : $"Door code: {listing.CheckInDoorCode}");

            var effectiveDate = lease.StartDate;
            var suiteRules = await dbContext.ListingRules
                .AsNoTracking()
                .Where(x => x.ListingID == listing.ListingID &&
                            (x.EffectiveFrom == null || x.EffectiveFrom <= effectiveDate) &&
                            (x.EffectiveTo == null || x.EffectiveTo >= effectiveDate))
                .OrderBy(x => x.CapturedDate)
                .Select(x => !string.IsNullOrWhiteSpace(x.RuleDescription)
                    ? x.RuleDescription!
                    : (x.RuleTitle ?? string.Empty))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToListAsync(cancellationToken);

            var address = FormatAddress(listing.RentalUnit?.Property?.Address);
            var tenantName = string.Join(" ", new[]
            {
                lease.Tenant?.User?.FirstName,
                lease.Tenant?.User?.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
            if (string.IsNullOrWhiteSpace(tenantName))
                tenantName = "there";

            var actionUrl = string.IsNullOrWhiteSpace(frontendUrl)
                ? "#"
                : $"{frontendUrl}/leases/{lease.LeaseID}";

            var html = EmailTemplates.BuildLeaseCheckInDetailsEmail(
                tenantName: tenantName,
                listingTitle: listing.Title ?? "your home",
                moveInDate: lease.StartDate.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture),
                fullAddress: address,
                checkInInstructions: checkInInstructions,
                wifiNetwork: listing.WIFINetwork,
                wifiPassword: listing.WIFIPassword,
                suiteRules: suiteRules,
                actionUrl: actionUrl,
                companyName: companyName,
                supportEmail: supportEmail);

            await emailQueue.EnqueueAsync(new EmailMessage(
                tenantEmail,
                $"Your check-in details for {listing.Title ?? "your lease"}",
                html), cancellationToken);

            dbContext.PaymentProviderEvents.Add(new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(),
                ProviderName = ProviderName,
                ProviderEventID = BuildEventId(lease.LeaseID),
                EventType = "CHECKIN_DETAILS_EMAIL",
                ProcessingStatus = "PROCESSED",
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    lease.LeaseID,
                    RecipientEmail = tenantEmail,
                    SentAt = DateTime.UtcNow
                }),
                ReceivedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string BuildEventId(Guid leaseId) => $"checkin-details-email:{leaseId:N}";

    private static string FormatAddress(Address? address)
    {
        if (address == null)
            return "Address unavailable";

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(address.Line1)) parts.Add(address.Line1);
        if (!string.IsNullOrWhiteSpace(address.Line2)) parts.Add(address.Line2);

        var cityLine = string.Join(", ",
            new[] { address.City, address.ProvinceCode }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        if (!string.IsNullOrWhiteSpace(cityLine)) parts.Add(cityLine);
        if (!string.IsNullOrWhiteSpace(address.PostalCode)) parts.Add(address.PostalCode);
        if (!string.IsNullOrWhiteSpace(address.CountryCode)) parts.Add(address.CountryCode);

        return parts.Count == 0 ? "Address unavailable" : string.Join(", ", parts);
    }
}
