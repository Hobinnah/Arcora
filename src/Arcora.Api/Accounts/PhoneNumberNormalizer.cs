using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace Arcora.Api.Accounts;

public static class PhoneNumberNormalizer
{
    public const string DuplicatePhoneNumberError = "This phone number is already registered to another account.";
    public const string InvalidPhoneNumberError = "Enter a valid international phone number, including its country calling code.";
    public const string PhoneNumberIndexName = "IX_AspNetUsers_PhoneNumber";

    private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();

    public static bool TryNormalize(string? phoneNumber, out string normalizedPhoneNumber)
    {
        normalizedPhoneNumber = string.Empty;
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var trimmedPhoneNumber = phoneNumber.Trim();
        if (!trimmedPhoneNumber.StartsWith('+'))
            return false;

        try
        {
            var parsedNumber = PhoneUtil.Parse(trimmedPhoneNumber, null);
            if (!string.IsNullOrEmpty(parsedNumber.Extension) || !PhoneUtil.IsValidNumber(parsedNumber))
                return false;

            normalizedPhoneNumber = PhoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);
            return true;
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    public static bool IsPhoneNumberUniqueConstraintViolation(DbUpdateException exception)
    {
        for (Exception? innerException = exception; innerException is not null; innerException = innerException.InnerException)
        {
            if (innerException is SqlException sqlException
                && sqlException.Number is 2601 or 2627
                && sqlException.Message.Contains(PhoneNumberIndexName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
