using Azure.Identity;
using Arcora.Api.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Arcora.Api.Controllers;

[ApiController]
[Route("api/Maps")]
[AllowAnonymous]
[EnableRateLimiting("MapsPolicy")]
public sealed class MapsController(AzureMapsService maps, ILogger<MapsController> logger) : ControllerBase
{
    [HttpGet("Configuration")]
    public IActionResult Configuration() => string.IsNullOrWhiteSpace(maps.ClientID)
        ? StatusCode(503, new { message = "The interactive map is not configured. Home search remains available." })
        : Ok(new { clientId = maps.ClientID });

    [HttpGet("Token")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Token(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(maps.ClientID))
            return StatusCode(503, new { message = "The interactive map is not configured." });
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));
        try
        {
            var token = await maps.GetTokenAsync(timeout.Token);
            return Ok(new { token = token.Token, expiresOn = token.ExpiresOn });
        }
        catch (AuthenticationFailedException)
        {
            logger.LogWarning("Azure Maps identity authentication failed.");
            return StatusCode(503, new { message = "Map authentication is unavailable. Home search remains available." });
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("Azure Maps identity authentication timed out.");
            return StatusCode(503, new { message = "Map authentication timed out. Try again shortly." });
        }
    }
}
