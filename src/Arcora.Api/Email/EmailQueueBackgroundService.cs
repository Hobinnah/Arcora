namespace Arcora.Api.Email
{
    /// <summary>
    /// Background worker that continuously drains the <see cref="IEmailQueue"/> and delivers each
    /// queued email using a scoped <see cref="IEmailSender"/>. Running delivery here keeps SMTP latency
    /// off the request path; individual send failures are logged and never stop the worker.
    /// </summary>
    public sealed class EmailQueueBackgroundService : BackgroundService
    {
        private readonly IEmailQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailQueueBackgroundService> _logger;

        public EmailQueueBackgroundService(
            IEmailQueue queue,
            IServiceProvider serviceProvider,
            ILogger<EmailQueueBackgroundService> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email queue dispatcher started.");

            await foreach (var message in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    // IEmailSender is registered as transient/scoped, so resolve it per message.
                    using var scope = _serviceProvider.CreateScope();
                    var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    await sender.SendEmailAsync(message.To, message.Subject, message.HtmlBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deliver queued email to {Recipient} with subject '{Subject}'.", message.To, message.Subject);
                }
            }
        }
    }
}
