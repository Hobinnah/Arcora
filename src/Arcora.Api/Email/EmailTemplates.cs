namespace Arcora.Api
{
    /// <summary>
    /// Provides reusable, branded HTML email templates for the application.
    /// </summary>
    public static class EmailTemplates
    {
        /// <summary>
        /// Builds the branded HTML body for a one-time login/sign-up verification code email.
        /// </summary>
        /// <param name="code">The one-time verification code to display to the recipient.</param>
        /// <param name="expiryMinutes">The number of minutes before the code expires.</param>
        /// <param name="companyName">The company/brand name used in the email header and footer.</param>
        /// <param name="supportEmail">The support/contact email shown in the footer.</param>
        /// <returns>A complete HTML document representing the email body.</returns>
        public static string BuildLoginCodeEmail(string code, int expiryMinutes, string companyName, string? supportEmail = null)
        {
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
            var supportLine = string.IsNullOrWhiteSpace(supportEmail)
                ? string.Empty
                : $"<p style=\"margin:0 0 8px;font-family:Arial,sans-serif;font-size:13px;line-height:20px;color:#6b7280;\">Need help? Contact us at <a href=\"mailto:{supportEmail}\" style=\"color:#0d9488;text-decoration:none;\">{supportEmail}</a>.</p>";

            var template = LoadTemplate("EmailConfirmationCode.html");

            return template
                .Replace("{{Brand}}", brand)
                .Replace("{{Code}}", code)
                .Replace("{{ExpiryMinutes}}", expiryMinutes.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString())
                .Replace("{{SupportLine}}", supportLine);
        }

        private static readonly string TemplatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _templateCache = new();

        private static string LoadTemplate(string fileName)
        {
            return _templateCache.GetOrAdd(fileName, name =>
            {
                var path = Path.Combine(TemplatesRoot, name);
                return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            });
        }
    }
}

