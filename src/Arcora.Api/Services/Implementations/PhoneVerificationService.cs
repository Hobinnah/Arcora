using System.Security.Cryptography;
using Arcora.Api.Accounts;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class PhoneVerificationService : IPhoneVerificationService
{
    private readonly UserManager<User> _userManager;
    private readonly ITwilioVerifyClient _twilioVerifyClient;
    private readonly IInfobipSmsClient _infobipSmsClient;
    private readonly PhoneVerificationOptions _phoneVerificationOptions;
    private readonly TwilioVerifyOptions _twilioVerifyOptions;
    private readonly InfobipOptions _infobipOptions;
    private readonly IHostEnvironment? _hostEnvironment;

    public PhoneVerificationService(UserManager<User> userManager, ITwilioVerifyClient twilioVerifyClient)
        : this(
            userManager,
            twilioVerifyClient,
            new DisabledInfobipSmsClient(),
            Options.Create(new PhoneVerificationOptions { Provider = "TWILIO" }),
            Options.Create(new TwilioVerifyOptions
            {
                AccountSid = "legacy",
                AuthToken = "legacy",
                ServiceSid = "legacy",
                Channel = "sms"
            }),
            Options.Create(new InfobipOptions()))
    {
    }

    public PhoneVerificationService(
        UserManager<User> userManager,
        ITwilioVerifyClient twilioVerifyClient,
        IInfobipSmsClient infobipSmsClient,
        IOptions<PhoneVerificationOptions> phoneVerificationOptions,
        IOptions<TwilioVerifyOptions> twilioVerifyOptions,
        IOptions<InfobipOptions> infobipOptions,
        IHostEnvironment? hostEnvironment = null)
    {
        _userManager = userManager;
        _twilioVerifyClient = twilioVerifyClient;
        _infobipSmsClient = infobipSmsClient;
        _phoneVerificationOptions = phoneVerificationOptions.Value ?? new PhoneVerificationOptions();
        _twilioVerifyOptions = twilioVerifyOptions.Value ?? new TwilioVerifyOptions();
        _infobipOptions = infobipOptions.Value ?? new InfobipOptions();
        _hostEnvironment = hostEnvironment;
    }

    public async Task<Result<PhoneVerificationStatusDto>> SendVerificationCodeAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<PhoneVerificationStatusDto>.Fail("User not found.");

        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result<PhoneVerificationStatusDto>.Fail("Add a phone number before requesting verification.");

        if (user.PhoneNumberConfirmed)
            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, "approved", "Phone number already confirmed."));

        var provider = ResolveProvider();
        if (provider == VerificationProvider.Local)
        {
            var localCodeLength = _phoneVerificationOptions.InfobipCodeLength <= 0 ? 5 : _phoneVerificationOptions.InfobipCodeLength;
            var localExpiryMinutes = _phoneVerificationOptions.InfobipCodeExpiryMinutes <= 0 ? 5 : _phoneVerificationOptions.InfobipCodeExpiryMinutes;
            user.SmsCode = GenerateNumericCode(localCodeLength);
            user.SmsCodeExpiresAt = DateTimeOffset.UtcNow.AddMinutes(localExpiryMinutes);
            user.SecurityStamp ??= Guid.NewGuid().ToString("N");
            var localUpdate = await _userManager.UpdateAsync(user);
            if (!localUpdate.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(localUpdate.Errors.Select(item => item.Description).ToArray());

            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, "development", "Development verification code generated."));
        }

        if (provider == VerificationProvider.Twilio)
        {
            var verificationResult = await _twilioVerifyClient.StartVerificationAsync(user.PhoneNumber, cancellationToken);
            if (!verificationResult.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(verificationResult.ErrorMessage ?? "Unable to send verification code.");

            user.SmsCode = null;
            user.SmsCodeExpiresAt = null;
            user.SecurityStamp ??= Guid.NewGuid().ToString("N");
            var twilioUpdate = await _userManager.UpdateAsync(user);
            if (!twilioUpdate.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(twilioUpdate.Errors.Select(item => item.Description).ToArray());

            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, verificationResult.Status, "Verification code sent."));
        }

        if (provider == VerificationProvider.Infobip)
        {
            var codeLength = _phoneVerificationOptions.InfobipCodeLength <= 0 ? 5 : _phoneVerificationOptions.InfobipCodeLength;
            var expiryMinutes = _phoneVerificationOptions.InfobipCodeExpiryMinutes <= 0 ? 5 : _phoneVerificationOptions.InfobipCodeExpiryMinutes;
            var code = GenerateNumericCode(codeLength);
            var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);
            var message = $"Arcora verification code: {code}. It expires in {expiryMinutes} minutes. If you did not request this code, please ignore this message.";

            var sendResult = await _infobipSmsClient.SendAsync(user.PhoneNumber, message, cancellationToken);
            if (!sendResult.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(sendResult.ErrorMessage ?? "Unable to send verification code.");

            user.SmsCode = code;
            user.SmsCodeExpiresAt = expiresAtUtc;
            user.SecurityStamp ??= Guid.NewGuid().ToString("N");
            var infobipUpdate = await _userManager.UpdateAsync(user);
            if (!infobipUpdate.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(infobipUpdate.Errors.Select(item => item.Description).ToArray());

            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, sendResult.Status, "Verification code sent."));
        }

        return Result<PhoneVerificationStatusDto>.Fail("Phone verification is not configured.");
    }

    public async Task<Result<PhoneVerificationStatusDto>> ConfirmPhoneNumberAsync(long userId, string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<PhoneVerificationStatusDto>.Fail("Verification code is required.");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<PhoneVerificationStatusDto>.Fail("User not found.");

        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result<PhoneVerificationStatusDto>.Fail("Add a phone number before confirming it.");

        if (user.PhoneNumberConfirmed)
            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, "approved", "Phone number already confirmed."));

        var provider = ResolveProvider();
        if (provider == VerificationProvider.Twilio)
        {
            var verificationResult = await _twilioVerifyClient.CheckVerificationAsync(user.PhoneNumber, code, cancellationToken);
            if (!verificationResult.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(verificationResult.ErrorMessage ?? "Unable to confirm phone number.");

            if (!verificationResult.IsApproved)
                return Result<PhoneVerificationStatusDto>.Fail("The verification code is invalid or expired.");

            user.SecurityStamp ??= Guid.NewGuid().ToString("N");
            user.SmsCode = null;
            user.SmsCodeExpiresAt = null;
            user.PhoneNumberConfirmed = true;
            var updateResultTwilio = await _userManager.UpdateAsync(user);
            if (!updateResultTwilio.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(updateResultTwilio.Errors.Select(item => item.Description).ToArray());

            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, verificationResult.Status, "Phone number confirmed."));
        }

        if (provider is VerificationProvider.Infobip or VerificationProvider.Local)
        {
            var storedCode = user.SmsCode?.Trim();
            var expiresAtUtc = user.SmsCodeExpiresAt;
            if (string.IsNullOrWhiteSpace(storedCode)
                || expiresAtUtc is null
                || expiresAtUtc <= DateTimeOffset.UtcNow)
            {
                user.SmsCode = null;
                user.SmsCodeExpiresAt = null;
                user.SecurityStamp ??= Guid.NewGuid().ToString("N");
                await _userManager.UpdateAsync(user);
                return Result<PhoneVerificationStatusDto>.Fail("The verification code is invalid or expired.");
            }

            if (!string.Equals(storedCode, code.Trim(), StringComparison.Ordinal))
                return Result<PhoneVerificationStatusDto>.Fail("The verification code is invalid or expired.");

            user.SecurityStamp ??= Guid.NewGuid().ToString("N");
            user.SmsCode = null;
            user.SmsCodeExpiresAt = null;
            user.PhoneNumberConfirmed = true;
            var updateResultInfobip = await _userManager.UpdateAsync(user);
            if (!updateResultInfobip.Succeeded)
                return Result<PhoneVerificationStatusDto>.Fail(updateResultInfobip.Errors.Select(item => item.Description).ToArray());

            return Result<PhoneVerificationStatusDto>.Ok(CreateStatus(user, "approved", "Phone number confirmed."));
        }

        return Result<PhoneVerificationStatusDto>.Fail("Phone verification is not configured.");
    }

    private VerificationProvider ResolveProvider()
    {
        var configuredProvider = (_phoneVerificationOptions.Provider ?? string.Empty).Trim().ToUpperInvariant();

        if (configuredProvider == "LOCAL")
        {
            return _hostEnvironment?.IsDevelopment() == true && _phoneVerificationOptions.EnableDevelopmentOtp
                ? VerificationProvider.Local
                : VerificationProvider.None;
        }

        if (configuredProvider == "TWILIO")
            return VerificationProvider.Twilio;

        if (configuredProvider == "INFOBIP")
            return VerificationProvider.Infobip;

        if (IsInfobipConfigured())
            return VerificationProvider.Infobip;

        if (IsTwilioConfigured())
            return VerificationProvider.Twilio;

        return VerificationProvider.None;
    }

    private bool IsTwilioConfigured()
        => !string.IsNullOrWhiteSpace(_twilioVerifyOptions.AccountSid)
           && !string.IsNullOrWhiteSpace(_twilioVerifyOptions.AuthToken)
           && !string.IsNullOrWhiteSpace(_twilioVerifyOptions.ServiceSid);

    private bool IsInfobipConfigured()
        => !string.IsNullOrWhiteSpace(_infobipOptions.ApiKey)
           && !string.IsNullOrWhiteSpace(_infobipOptions.BaseUrl)
           && !string.IsNullOrWhiteSpace(_infobipOptions.Sender);

    private static string GenerateNumericCode(int length)
    {
        if (length <= 0)
            length = 5;

        Span<char> chars = stackalloc char[length];
        for (var index = 0; index < chars.Length; index++)
            chars[index] = (char)('0' + System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 10));

        return new string(chars);
    }

    private static PhoneVerificationStatusDto CreateStatus(User user, string status, string message)
        => new()
        {
            Phone = user.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Status = status,
            Message = message
        };

    private sealed class DisabledInfobipSmsClient : IInfobipSmsClient
    {
        public Task<InfobipSmsResult> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
            => Task.FromResult(InfobipSmsResult.Failure("Infobip phone verification is not configured."));
    }

    private enum VerificationProvider
    {
        None,
        Twilio,
        Infobip,
        Local
    }
}
