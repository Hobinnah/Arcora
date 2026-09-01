namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Represents the SMTP email settings used to send outgoing emails.
    /// Bound from the "EmailSettings" section in configuration.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP server host name.
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port.
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Friendly display name shown as the sender.
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Email address emails are sent from.
        /// </summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// SMTP authentication user name.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// SMTP authentication password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether SSL/TLS should be enabled for the SMTP connection.
        /// </summary>
        public bool EnableSsl { get; set; } = true;
    }
}
