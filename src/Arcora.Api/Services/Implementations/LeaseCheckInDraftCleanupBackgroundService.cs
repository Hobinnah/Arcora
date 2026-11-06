using Arcora.Api.Services.Interfaces;
using Arcora.Api.Operations;

namespace Arcora.Api.Services.Implementations;

public class LeaseCheckInDraftCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<LeaseCheckInDraftCleanupBackgroundService> logger;
    private readonly OperationsRegistry? health;

    public LeaseCheckInDraftCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<LeaseCheckInDraftCleanupBackgroundService> logger,
        OperationsRegistry? health = null)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        this.health = health;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var run = health?.Begin(WorkerIds.Cleanup);
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILeaseCheckInService>();
                await service.CleanupExpiredDraftEvidenceAsync(stoppingToken);
                run?.Count();
            }
            catch (Exception ex)
            {
                if (stoppingToken.IsCancellationRequested) { run?.Cancel(); break; }
                run?.Fail();
                logger.LogError(ex, "Move-in check-in draft cleanup failed. Timestamp: {Timestamp}", DateTime.UtcNow);
            }

            run?.Dispose();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
