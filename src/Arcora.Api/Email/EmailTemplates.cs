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

        /// <summary>
        /// Builds the branded HTML body for a guarantor invitation email. The email informs the
        /// recipient that a tenant wishes to name them as a guarantor and provides accept/decline links.
        /// </summary>
        /// <param name="guarantorName">The guarantor's display name.</param>
        /// <param name="tenantName">The applicant/tenant's full name.</param>
        /// <param name="tenantPhone">The applicant/tenant's phone number.</param>
        /// <param name="listingTitle">The title of the listing being applied for.</param>
        /// <param name="monthlyRent">The formatted monthly rent (including currency).</param>
        /// <param name="securityDeposit">The formatted security deposit (including currency).</param>
        /// <param name="listingImageUrl">An optional cover image URL for the listing.</param>
        /// <param name="acceptUrl">The link the guarantor clicks to accept.</param>
        /// <param name="declineUrl">The link the guarantor clicks to decline.</param>
        /// <param name="companyName">The company/brand name used in the email header and footer.</param>
        /// <param name="supportEmail">The support/contact email shown in the footer.</param>
        /// <returns>A complete HTML document representing the email body.</returns>
        public static string BuildGuarantorInvitationEmail(
            string guarantorName,
            string tenantName,
            string? tenantPhone,
            string listingTitle,
            string monthlyRent,
            string securityDeposit,
            string? listingImageUrl,
            string acceptUrl,
            string declineUrl,
            string companyName,
            string? supportEmail = null)
        {
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
            var supportLine = string.IsNullOrWhiteSpace(supportEmail)
                ? string.Empty
                : $"<p style=\"margin:0 0 8px;font-family:Arial,sans-serif;font-size:13px;line-height:20px;color:#6b7280;\">Need help? Contact us at <a href=\"mailto:{supportEmail}\" style=\"color:#0d9488;text-decoration:none;\">{supportEmail}</a>.</p>";

            var listingImageBlock = string.IsNullOrWhiteSpace(listingImageUrl)
                ? string.Empty
                : $"<tr><td style=\"padding:8px 40px 0;\"><img src=\"{listingImageUrl}\" alt=\"{System.Net.WebUtility.HtmlEncode(listingTitle)}\" width=\"520\" style=\"width:100%;max-width:520px;border-radius:12px;display:block;\" /></td></tr>";

            var template = LoadTemplate("GuarantorInvitation.html");

            return template
                .Replace("{{Brand}}", brand)
                .Replace("{{GuarantorName}}", System.Net.WebUtility.HtmlEncode(guarantorName))
                .Replace("{{TenantName}}", System.Net.WebUtility.HtmlEncode(tenantName))
                .Replace("{{TenantPhone}}", System.Net.WebUtility.HtmlEncode(tenantPhone ?? "Not provided"))
                .Replace("{{ListingTitle}}", System.Net.WebUtility.HtmlEncode(listingTitle))
                .Replace("{{MonthlyRent}}", System.Net.WebUtility.HtmlEncode(monthlyRent))
                .Replace("{{SecurityDeposit}}", System.Net.WebUtility.HtmlEncode(securityDeposit))
                .Replace("{{ListingImageBlock}}", listingImageBlock)
                .Replace("{{AcceptUrl}}", acceptUrl)
                .Replace("{{DeclineUrl}}", declineUrl)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString())
                .Replace("{{SupportLine}}", supportLine);
        }

        /// <summary>
        /// Builds the branded HTML body for a rental-application confirmation email, sent to both the
        /// tenant (their application was submitted) and the host/landlord (a new application was received).
        /// </summary>
        public static string BuildRentalApplicationSubmittedEmail(
            string subject,
            string eyebrow,
            string headline,
            string intro,
            string listingTitle,
            string applicationCode,
            string monthlyRent,
            string moveInDate,
            string leaseTerm,
            string occupants,
            string? listingImageUrl,
            string? applicantName,
            string? applicantPhone,
            string actionUrl,
            string actionLabel,
            string footnote,
            string companyName,
            string? supportEmail = null)
        {
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
            var supportLine = string.IsNullOrWhiteSpace(supportEmail)
                ? string.Empty
                : $"<p style=\"margin:0 0 8px;font-family:Arial,sans-serif;font-size:13px;line-height:20px;color:#6b7280;\">Need help? Contact us at <a href=\"mailto:{supportEmail}\" style=\"color:#0d9488;text-decoration:none;\">{supportEmail}</a>.</p>";

            var listingImageBlock = string.IsNullOrWhiteSpace(listingImageUrl)
                ? string.Empty
                : $"<tr><td style=\"padding:8px 40px 0;\"><img src=\"{listingImageUrl}\" alt=\"{System.Net.WebUtility.HtmlEncode(listingTitle)}\" width=\"520\" style=\"width:100%;max-width:520px;border-radius:12px;display:block;\" /></td></tr>";

            // The applicant block is only shown on the host copy (when an applicant name is supplied).
            var applicantBlock = string.IsNullOrWhiteSpace(applicantName)
                ? string.Empty
                : "<tr><td style=\"padding:20px 40px 0;\">" +
                  "<h2 style=\"margin:0 0 12px;font-size:16px;color:#0f3a34;font-family:Arial,sans-serif;font-weight:700;\">Applicant</h2>" +
                  "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"font-family:Arial,sans-serif;font-size:14px;color:#4b5563;\">" +
                  $"<tr><td style=\"padding:6px 0;color:#6b7280;width:45%;\">Full name</td><td style=\"padding:6px 0;color:#0f3a34;font-weight:600;\">{System.Net.WebUtility.HtmlEncode(applicantName)}</td></tr>" +
                  $"<tr><td style=\"padding:6px 0;color:#6b7280;\">Phone number</td><td style=\"padding:6px 0;color:#0f3a34;font-weight:600;\">{System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(applicantPhone) ? "Not provided" : applicantPhone)}</td></tr>" +
                  "</table></td></tr>";

            var template = LoadTemplate("RentalApplicationSubmitted.html");

            return template
                .Replace("{{Brand}}", brand)
                .Replace("{{Subject}}", System.Net.WebUtility.HtmlEncode(subject))
                .Replace("{{Eyebrow}}", System.Net.WebUtility.HtmlEncode(eyebrow))
                .Replace("{{Headline}}", System.Net.WebUtility.HtmlEncode(headline))
                .Replace("{{Intro}}", System.Net.WebUtility.HtmlEncode(intro))
                .Replace("{{ListingImageBlock}}", listingImageBlock)
                .Replace("{{ApplicantBlock}}", applicantBlock)
                .Replace("{{ListingTitle}}", System.Net.WebUtility.HtmlEncode(listingTitle))
                .Replace("{{ApplicationCode}}", System.Net.WebUtility.HtmlEncode(applicationCode))
                .Replace("{{MonthlyRent}}", System.Net.WebUtility.HtmlEncode(monthlyRent))
                .Replace("{{MoveInDate}}", System.Net.WebUtility.HtmlEncode(moveInDate))
                .Replace("{{LeaseTerm}}", System.Net.WebUtility.HtmlEncode(leaseTerm))
                .Replace("{{Occupants}}", System.Net.WebUtility.HtmlEncode(occupants))
                .Replace("{{ActionUrl}}", actionUrl)
                .Replace("{{ActionLabel}}", System.Net.WebUtility.HtmlEncode(actionLabel))
                .Replace("{{Footnote}}", System.Net.WebUtility.HtmlEncode(footnote))
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

