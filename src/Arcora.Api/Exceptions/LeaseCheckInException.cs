namespace Arcora.Api.Exceptions;

public class LeaseCheckInException : Exception
{
    public LeaseCheckInException(int statusCode, string title, string detail, string? reasonCode = null, IDictionary<string, string[]>? errors = null)
        : base(detail)
    {
        StatusCode = statusCode;
        Title = title;
        Detail = detail;
        ReasonCode = reasonCode;
        Errors = errors;
    }

    public int StatusCode { get; }

    public string Title { get; }

    public string Detail { get; }

    public string? ReasonCode { get; }

    public IDictionary<string, string[]>? Errors { get; }
}
