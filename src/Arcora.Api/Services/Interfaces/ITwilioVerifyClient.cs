namespace Arcora.Api.Services.Interfaces;

public sealed record TwilioVerifyResult(bool Succeeded, bool IsApproved, string Status, string? Sid, string? ErrorMessage)
{
    public static TwilioVerifyResult Success(string status, string? sid, bool isApproved = false)
        => new(true, isApproved, status, sid, null);

    public static TwilioVerifyResult Failure(string errorMessage, string status = "failed")
        => new(false, false, status, null, errorMessage);
}

public interface ITwilioVerifyClient
{
    Task<TwilioVerifyResult> StartVerificationAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<TwilioVerifyResult> CheckVerificationAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
}
