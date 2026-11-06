using System.Security.Claims;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Controllers;

[ApiController]
[Authorize(Roles = "User, LandLord, Admin")]
[Route("api/leases/{leaseId:guid}/first-payment")]
public sealed class LeaseFirstPaymentController(
    ArcoraDbContext db,
    ILeaseFirstPaymentService service,
    ILogger<LeaseFirstPaymentController> logger) : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Get(Guid leaseId, CancellationToken ct) =>
        ExecuteAsync(leaseId, lease => service.GetAsync(lease, ct), ct);

    [HttpPost]
    public Task<IActionResult> Collect(Guid leaseId, CollectLeaseFirstPaymentRequest request, CancellationToken ct) =>
        ExecuteAsync(leaseId, lease => service.CollectAsync(lease, request.ExpectedAmount, ct), ct);

    [HttpPost("refresh")]
    public Task<IActionResult> Refresh(Guid leaseId, CancellationToken ct) =>
        ExecuteAsync(leaseId, lease => service.RefreshAsync(lease, ct), ct);

    private async Task<IActionResult> ExecuteAsync(
        Guid leaseId, Func<Lease, Task<LeaseFirstPaymentDto>> operation, CancellationToken ct)
    {
        var claim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(claim, out var actor))
            return Unauthorized();
        var lease = await db.Leases.SingleOrDefaultAsync(item => item.LeaseID == leaseId, ct);
        if (lease == null)
            return NotFound();
        if (!await db.Tenants.AnyAsync(tenant => tenant.TenantID == lease.TenantID && tenant.UserID == actor, ct))
            return Forbid();
        try
        {
            return Ok(await operation(lease));
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "First payment rejected for lease {LeaseId}", leaseId);
            return Conflict(new { message = ex.Message });
        }
        catch (Stripe.StripeException ex)
        {
            logger.LogError(ex, "First-payment provider failed for lease {LeaseId}", leaseId);
            return Problem("The payment provider is unavailable. Refresh payment status before trying again.", statusCode: 502);
        }
    }
}
