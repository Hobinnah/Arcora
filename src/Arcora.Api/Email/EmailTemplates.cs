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

        /// <summary>
        /// Builds a branded HTML notification for a new chat message. This template is fully self-contained
        /// (it does not depend on an on-disk HTML file) so it can be dispatched reliably from the messaging service.
        /// </summary>
        /// <param name="recipientName">The display name of the person receiving the notification.</param>
        /// <param name="senderName">The display name of the person who sent the message.</param>
        /// <param name="messagePreview">The message body (or a preview of it).</param>
        /// <param name="conversationSubject">An optional conversation subject/title.</param>
        /// <param name="actionUrl">The link the recipient clicks to open the conversation.</param>
        /// <param name="companyName">The company/brand name used in the email header and footer.</param>
        /// <param name="supportEmail">The support/contact email shown in the footer.</param>
        /// <returns>A complete HTML document representing the email body.</returns>
        public static string BuildNewMessageEmail(
            string recipientName,
            string senderName,
            string messagePreview,
            string? conversationSubject,
            string actionUrl,
            string companyName,
            string? supportEmail = null)
        {
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
            var greetingName = string.IsNullOrWhiteSpace(recipientName) ? "there" : recipientName;
            var subjectLine = string.IsNullOrWhiteSpace(conversationSubject)
                ? string.Empty
                : $"<p style=\"margin:0 0 12px;font-family:Arial,sans-serif;font-size:13px;line-height:20px;color:#6b7280;\">Re: {System.Net.WebUtility.HtmlEncode(conversationSubject)}</p>";
            var supportLine = string.IsNullOrWhiteSpace(supportEmail)
                ? string.Empty
                : $"<p style=\"margin:0 0 8px;font-family:Arial,sans-serif;font-size:13px;line-height:20px;color:#6b7280;\">Need help? Contact us at <a href=\"mailto:{supportEmail}\" style=\"color:#0d9488;text-decoration:none;\">{supportEmail}</a>.</p>";
            var link = string.IsNullOrWhiteSpace(actionUrl) ? "#" : actionUrl;

            return
                "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"></head>" +
                "<body style=\"margin:0;padding:0;background:#f3f4f6;\">" +
                "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#f3f4f6;padding:24px 0;\"><tr><td align=\"center\">" +
                "<table role=\"presentation\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 1px 3px rgba(0,0,0,0.08);\">" +
                $"<tr><td style=\"background:#0f3a34;padding:20px 28px;\"><span style=\"font-family:Arial,sans-serif;font-size:18px;font-weight:700;color:#ffffff;\">{System.Net.WebUtility.HtmlEncode(brand)}</span></td></tr>" +
                "<tr><td style=\"padding:28px;\">" +
                $"<p style=\"margin:0 0 6px;font-family:Arial,sans-serif;font-size:12px;letter-spacing:.08em;text-transform:uppercase;color:#0d9488;font-weight:700;\">New message</p>" +
                $"<h1 style=\"margin:0 0 12px;font-family:Arial,sans-serif;font-size:22px;line-height:28px;color:#0f3a34;\">Hi {System.Net.WebUtility.HtmlEncode(greetingName)}, you have a new message</h1>" +
                subjectLine +
                $"<p style=\"margin:0 0 16px;font-family:Arial,sans-serif;font-size:14px;line-height:22px;color:#374151;\"><strong style=\"color:#0f3a34;\">{System.Net.WebUtility.HtmlEncode(senderName)}</strong> wrote:</p>" +
                $"<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"background:#f9fafb;border-left:4px solid #0d9488;border-radius:6px;padding:14px 16px;font-family:Arial,sans-serif;font-size:14px;line-height:22px;color:#374151;\">{System.Net.WebUtility.HtmlEncode(messagePreview)}</td></tr></table>" +
                $"<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin:24px 0 8px;\"><tr><td style=\"border-radius:8px;background:#0d9488;\"><a href=\"{link}\" style=\"display:inline-block;padding:12px 24px;font-family:Arial,sans-serif;font-size:14px;font-weight:600;color:#ffffff;text-decoration:none;border-radius:8px;\">Open conversation</a></td></tr></table>" +
                "</td></tr>" +
                "<tr><td style=\"padding:20px 28px;border-top:1px solid #e5e7eb;\">" +
                supportLine +
                $"<p style=\"margin:0;font-family:Arial,sans-serif;font-size:12px;line-height:18px;color:#9ca3af;\">&copy; {DateTime.UtcNow.Year} {System.Net.WebUtility.HtmlEncode(brand)}. All rights reserved.</p>" +
                "</td></tr></table></td></tr></table></body></html>";
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

