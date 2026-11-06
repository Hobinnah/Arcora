using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Arcora.Api.Configurations;

namespace Arcora.Api
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task SendEmailAsync(string to, string subject, string htmlBody) =>
            SendEmailAsync(to, subject, htmlBody, CancellationToken.None);

        public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
            {
                _logger.LogWarning("SMTP host is not configured. Email to {Recipient} was not sent.", to);
                throw new InvalidOperationException("SMTP host is not configured.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}'.", to, subject);
        }
    }
}
