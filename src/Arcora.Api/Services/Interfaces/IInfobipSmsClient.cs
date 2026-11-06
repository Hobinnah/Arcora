namespace Arcora.Api.Services.Interfaces;

public sealed record InfobipSmsResult(bool Succeeded, string Status, string? ErrorMessage)
{
    public static InfobipSmsResult Success(string status = "accepted")
        => new(true, status, null);

    public static InfobipSmsResult Failure(string errorMessage, string status = "failed")
        => new(false, status, errorMessage);
}

public interface IInfobipSmsClient
{
    Task<InfobipSmsResult> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
