using Arcora.Api.Configurations;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Background scheduler that initiates rent collection on each lease's monthly anniversary.
    ///
    /// The first charge happens at host approval (handled elsewhere); this job only drives the
    /// recurring monthly debits. It creates a PaymentIntent per due lease via the orchestrator, which
    /// attempts the PAD first and only falls back to the backup card on a definitive PAD failure.
    /// Idempotency is provided by the orchestrator's deterministic IdempotencyKey per (lease, invoice).
    /// </summary>
    public class RecurringRentCollectionService : BackgroundService
    {
        private readonly ILogger<RecurringRentCollectionService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly RentCollectionOptions options;

        public RecurringRentCollectionService(
            ILogger<RecurringRentCollectionService> logger,
            IServiceProvider serviceProvider,
            IOptions<RentCollectionOptions> options)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!options.Enabled)
            {
                logger.LogInformation("Recurring rent collection scheduler is disabled.");
                return;
            }

            var interval = TimeSpan.FromMinutes(Math.Max(1, options.PollingIntervalMinutes));
            logger.LogInformation("Recurring rent collection scheduler started; polling every {Interval}.", interval);

            using var timer = new PeriodicTimer(interval);
            do
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Recurring rent collection cycle failed.");
                }
            }
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var leaseRepository = scope.ServiceProvider.GetRequiredService<ILeaseRepository>();
            var paymentIntentRepository = scope.ServiceProvider.GetRequiredService<IPaymentIntentRepository>();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IRentCollectionOrchestrator>();

            var now = DateTime.UtcNow;
            var activeLeases = await leaseRepository.Find(x => x.Status == "ACTIVE");
            if (activeLeases == null)
                return;

            foreach (var lease in activeLeases.Where(l => l != null)!)
            {
                if (!IsDueToday(lease!, now))
                    continue;

                // Deterministic key for this month's charge prevents duplicate collection within a cycle.
                var periodKey = $"{now:yyyyMM}";
                var alreadyCharged = await paymentIntentRepository.Find(x =>
                    x.LeaseID == lease!.LeaseID &&
                    x.IdempotencyKey != null &&
                    x.IdempotencyKey.Contains(periodKey));

                if (alreadyCharged != null && alreadyCharged.Any())
                    continue;

                try
                {
                    await orchestrator.InitiateCollectionAsync(
                        lease!.LeaseID,
                        lease.TenantID,
                        Guid.Empty,
                        lease.BaseRentAmount,
                        $"pi_{lease.LeaseID:N}_{periodKey}",
                        cancellationToken);

                    logger.LogInformation("Initiated rent collection for lease {LeaseId} period {Period}.", lease.LeaseID, periodKey);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initiate rent collection for lease {LeaseId}.", lease!.LeaseID);
                }
            }
        }

        /// <summary>
        /// A lease is due on the day-of-month matching its start date (anniversary), at/after the
        /// configured charge hour. Handles short months by clamping the anniversary day.
        /// </summary>
        private bool IsDueToday(Lease lease, DateTime nowUtc)
        {
            var anchor = lease.StartDate;
            var daysInMonth = DateTime.DaysInMonth(nowUtc.Year, nowUtc.Month);
            var anniversaryDay = Math.Min(anchor.Day, daysInMonth);

            return nowUtc.Day == anniversaryDay && nowUtc.Hour >= options.ChargeHourUtc;
        }
    }
}
