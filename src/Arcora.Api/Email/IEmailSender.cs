namespace Arcora.Api
{
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name = "to"></param>
        /// <param name = "subject"></param>
        /// <param name = "htmlBody"></param>
        /// <returns></returns>
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}