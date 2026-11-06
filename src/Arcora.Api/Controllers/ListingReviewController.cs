using System.Security.Claims;
using Arcora.Api.DTOs;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers;

[ApiController]
[Route("api/[controller]/[Action]")]
public sealed class ListingReviewController : ControllerBase
{
    [Authorize(Roles = "CustomerRep, Admin")]
    [HttpGet]
    public async Task<IActionResult> Queue([FromServices] IListingReviewService service, CancellationToken cancellationToken)
        => Ok(await service.GetQueueAsync(cancellationToken));

    [Authorize(Roles = "CustomerRep, Admin")]
    [HttpGet("{listingID:guid}")]
    public async Task<IActionResult> Detail([FromServices] IListingReviewService service, Guid listingID, CancellationToken cancellationToken)
    {
        var listing = await service.GetPendingListingAsync(listingID, cancellationToken);
        return listing == null ? NotFound() : Ok(listing);
    }

    [Authorize(Roles = "User, LandLord, Admin")]
    [HttpPost("{listingID:guid}")]
    public async Task<IActionResult> Submit([FromServices] IListingReviewService service, Guid listingID, CancellationToken cancellationToken)
    {
        if (!TryGetUserID(out var userID))
            return Unauthorized();
        try
        {
            var listing = await service.SubmitForReviewAsync(listingID, userID, User.IsInRole("Admin"), cancellationToken);
            return listing == null ? NotFound() : Ok(listing);
        }
        catch (UnauthorizedAccessException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [Authorize(Roles = "CustomerRep, Admin")]
    [HttpPost("{listingID:guid}")]
    public async Task<IActionResult> Decision([FromServices] IListingReviewService service, Guid listingID, [FromBody] ListingReviewDecisionRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserID(out var userID))
            return Unauthorized();
        try
        {
            var result = await service.DecideAsync(listingID, request, userID, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private bool TryGetUserID(out long userID)
    {
        var claim = User.FindFirst("UserId")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return long.TryParse(claim, out userID) && userID > 0;
    }
}