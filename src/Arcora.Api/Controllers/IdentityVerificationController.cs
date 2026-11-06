// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class IdentityVerificationController : ControllerBase
    {
        // GET: api/<IdentityVerificationController>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllIdentityVerifications")]
        public async Task<IActionResult> Get([FromServices] IIdentityVerificationService identityverificationService, [FromQuery] Paging paging)
        {
            return Ok(await identityverificationService.GetAll(paging));
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/api/identityverification/mine", Name = "GetMyIdentityVerifications")]
        public async Task<IActionResult> GetMine(
            [FromServices] IIdentityVerificationService identityverificationService,
            [FromServices] UserManager<User> userManager,
            [FromQuery] Paging paging,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Unauthorized();

            var verifications = await identityverificationService.GetForUser(user.Id, paging);
            var personaVerificationsToRefresh = verifications.Data
                .Where(item => item.IdentityVerificationID.HasValue
                    && string.Equals(item.ProviderName, "PERSONA", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(item.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
                .Select(item => item.IdentityVerificationID!.Value)
                .Distinct()
                .ToList();

            if (personaVerificationsToRefresh.Count > 0)
            {
                foreach (var verificationId in personaVerificationsToRefresh)
                    await identityverificationService.RefreshVerificationStatusAsync(user.Id, verificationId, cancellationToken);

                verifications = await identityverificationService.GetForUser(user.Id, paging);
            }

            return Ok(verifications);
        }

        // GET api/<IdentityVerificationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetIdentityVerificationByID")]
        public async Task<IActionResult> GetIdentityVerificationByID([FromServices] IIdentityVerificationService identityverificationService, Guid id)
        {
            var result = await identityverificationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            return Ok(result);
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "StartIdentityVerification")]
        public async Task<IActionResult> StartIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, [FromServices] UserManager<User> userManager, [FromBody] StartIdentityVerificationRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                    return Unauthorized();

                request.UserID = user.Id;
                var result = await identityverificationService.StartIdentityVerificationAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User with the specified ID was not found." });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        // POST api/<IdentityVerificationController>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateIdentityVerification")]
        public async Task<IActionResult> CreateIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, [FromBody] IdentityVerificationDto identityverificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await identityverificationService.CreateIdentityVerification(identityverificationDto);
            if (result.IdentityVerificationID != null && result.IdentityVerificationID != Guid.Empty)
                return CreatedAtRoute("GetIdentityVerificationByID", new { id = result.IdentityVerificationID }, result);
            return BadRequest(new { message = "Failed to create identityverification. A identityverification with the same name may already exist." });
        }

        // PUT api/<IdentityVerificationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateIdentityVerification")]
        public async Task<IActionResult> UpdateIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, Guid id, [FromBody] IdentityVerificationDto identityverificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await identityverificationService.UpdateIdentityVerification(id, identityverificationDto);
            if (result == null)
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<IdentityVerificationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteIdentityVerification")]
        public async Task<IActionResult> DeleteIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, Guid id)
        {
            try
            {
                await identityverificationService.DeleteIdentityVerification(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            }
        }

        // POST: api/identityverification/{id}/{status}
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateIdentityVerificationStatus")]
        public async Task<ActionResult> UpdateIdentityVerificationStatus([FromServices] IIdentityVerificationService identityverificationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var identityverification = await identityverificationService.UpdateIdentityVerificationStatus(id, status);
            if (identityverification == null)
                return NotFound($"IdentityVerification with ID {id} not found.");
            return Ok(identityverification);
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("/api/identityverification/mine/{id:guid}/refresh", Name = "RefreshMyIdentityVerificationStatus")]
        public async Task<IActionResult> RefreshMine(
            [FromServices] IIdentityVerificationService identityverificationService,
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var refreshed = await identityverificationService.RefreshVerificationStatusAsync(userId, id, cancellationToken);
            if (refreshed is null)
                return NotFound(new { message = "Identity verification with the specified ID was not found." });

            return Ok(refreshed);
        }

        private bool TryGetCurrentUserId(out long userId)
        {
            userId = 0;

            var claimValue = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return long.TryParse(claimValue, out userId) && userId > 0;
        }
    }
}