using Arcora.Api.Services.Interfaces;

namespace Arcora.Api.Services.Implementations;

public class LeaseCheckInDraftCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<LeaseCheckInDraftCleanupBackgroundService> logger;

    public LeaseCheckInDraftCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<LeaseCheckInDraftCleanupBackgroundService> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILeaseCheckInService>();
                await service.CleanupExpiredDraftEvidenceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Move-in check-in draft cleanup failed. Timestamp: {Timestamp}", DateTime.UtcNow);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
