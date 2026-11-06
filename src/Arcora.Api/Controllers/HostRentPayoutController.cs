using Arcora.Api.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Arcora.Api.Controllers;

[ApiController]
[Authorize(Roles = "Viewer,User,LandLord,Admin")]
[Route("api/organizations/{organizationId:guid}/rent-payouts/{paymentId:guid}")]
public sealed class HostRentPayoutController(
    HostRentPayoutService service, ILogger<HostRentPayoutController> logger) : ControllerBase
{
    public sealed record PayoutConsent(decimal ExpectedAmount);

    [HttpPost]
    [Authorize(Roles = "LandLord,Admin")]
    public Task<IActionResult> RequestPayout(Guid organizationId, Guid paymentId, PayoutConsent consent, CancellationToken ct) =>
        Progress(organizationId, paymentId, true, consent.ExpectedAmount, ct);

    [HttpGet("/api/organizations/{organizationId:guid}/rent-payouts")]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!long.TryParse(User.FindFirst("UserId")?.Value, out var actor) || actor <= 0)
            return Forbid();
        try
        {
            return Ok(await service.ListAsync(organizationId, actor, ct));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("refresh")]
    [Authorize(Roles = "LandLord,Admin")]
    public Task<IActionResult> Refresh(Guid organizationId, Guid paymentId, CancellationToken ct) =>
        Progress(organizationId, paymentId, false, null, ct);

    private async Task<IActionResult> Progress(Guid org, Guid payment, bool create, decimal? expected, CancellationToken ct)
    {
        if (!long.TryParse(User.FindFirst("UserId")?.Value, out var actor) || actor <= 0)
            return Forbid();
        try
        {
            var result = await service.ProgressAsync(payment, org, actor, create, expected, ct);
            return Ok(new
            {
                result.PaymentID, result.PayoutID, result.Amount, result.Currency, result.Status,
                result.TransferReference, result.BankPayoutReference, result.FailureReason, result.LastCheckedAt
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Host rent payout rejected for payment {PaymentID}", payment);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex) when (ex is StripeException or HttpRequestException)
        {
            logger.LogError(ex, "Host rent payout provider failure for payment {PaymentID}", payment);
            return StatusCode(502, new { message = "Stripe could not confirm the payout. Refresh its status; do not create another payout." });
        }
    }
}
