// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Exceptions;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class TenantInvitationController : ControllerBase
    {
        // GET: api/<TenantInvitationController>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantInvitations")]
        public async Task<IActionResult> Get([FromServices] ITenantInvitationService tenantinvitationService, [FromQuery] Paging paging)
        {
            return Ok(await tenantinvitationService.GetAll(paging));
        }

        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/api/tenantinvitation/by-organization", Name = "GetTenantInvitationsByOrganization")]
        public async Task<IActionResult> GetByOrganization(
            [FromServices] ITenantInvitationService tenantinvitationService,
            [FromServices] ArcoraDbContext dbContext,
            [FromQuery] Guid organizationID)
        {
            if (organizationID == Guid.Empty)
                return BadRequest(new { message = "A valid organizationID is required." });

            var userIdClaim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var signedInUserId) || signedInUserId <= 0)
                return Forbid();

            var isOrganizationMember = await dbContext.OrganizationMembers
                .AsNoTracking()
                .AnyAsync(member => member.OrganizationID == organizationID
                    && member.UserID == signedInUserId
                    && member.Status == "ACTIVE"
                    && member.DeactivatedAt == null);
            if (!isOrganizationMember)
                return Forbid();

            var invitations = await tenantinvitationService.GetByOrganizationAsync(organizationID);
            return Ok(invitations);
        }

        // GET api/<TenantInvitationController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantInvitationByID")]
        public async Task<IActionResult> GetTenantInvitationByID([FromServices] ITenantInvitationService tenantinvitationService, [FromServices] ArcoraDbContext db, Guid id)
        {
            if (!await InvitationAuthorization.CanManageInvitationAsync(db, id, InvitationAuthorization.GetUserID(User)))
                return Forbid();
            var result = await tenantinvitationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantInvitationController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantInvitation")]
        public async Task<IActionResult> CreateTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, [FromServices] ArcoraDbContext db, [FromBody] CreateTenantInvitationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userID = InvitationAuthorization.GetUserID(User);
            if (!await InvitationAuthorization.CanManageAsync(db, request.OrganizationID, userID))
                return Forbid();
            request.CapturedBy = userID.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var result = await tenantinvitationService.CreateTenantInvitationWithHold(request, userID);
            if (!result.Success)
            {
                if (result.Availability != null)
                    return Conflict(result);

                return BadRequest(result);
            }

            return CreatedAtRoute("GetTenantInvitationByID", new { id = result.TenantInvitation?.TenantInvitationID }, result);
        }

        // PUT api/<TenantInvitationController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantInvitation")]
        public async Task<IActionResult> UpdateTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, [FromServices] ArcoraDbContext db, Guid id, [FromBody] TenantInvitationDto tenantinvitationDto)
        {
            if (!await InvitationAuthorization.CanManageInvitationAsync(db, id, InvitationAuthorization.GetUserID(User)))
                return Forbid();
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantinvitationService.UpdateTenantInvitation(id, tenantinvitationDto);
            if (result == null)
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantInvitationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantInvitation")]
        public async Task<IActionResult> DeleteTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, [FromServices] ArcoraDbContext db, Guid id)
        {
            if (!await InvitationAuthorization.CanManageInvitationAsync(db, id, InvitationAuthorization.GetUserID(User)))
                return Forbid();
            try
            {
                await tenantinvitationService.DeleteTenantInvitation(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            }
        }

        // POST: api/tenantinvitation/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateTenantInvitationStatus")]
        public async Task<ActionResult> UpdateTenantInvitationStatus([FromServices] ITenantInvitationService tenantinvitationService, [FromServices] ArcoraDbContext db, [FromRoute] Guid id, [FromRoute] string status)
        {
            if (!await InvitationAuthorization.CanManageInvitationAsync(db, id, InvitationAuthorization.GetUserID(User)))
                return Forbid();
            var tenantinvitation = await tenantinvitationService.UpdateTenantInvitationStatus(id, status);
            if (tenantinvitation == null)
                return NotFound($"TenantInvitation with ID {id} not found.");
            return Ok(tenantinvitation);
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("/api/tenantinvitation/{id}/retry-email")]
        public async Task<IActionResult> RetryEmail([FromServices] ITenantInvitationService service,
            [FromServices] ArcoraDbContext db, Guid id)
        {
            if (!await InvitationAuthorization.CanManageInvitationAsync(db, id, InvitationAuthorization.GetUserID(User)))
                return Forbid();
            return Ok(await service.RetryInvitationEmailAsync(id));
        }

        [Authorize(Roles = "User")]
        [HttpGet("/api/tenantinvitation/{id}/for-me")]
        public async Task<IActionResult> GetAcceptedInvitationForMe([FromServices] ITenantInvitationService service, Guid id)
        {
            return Ok(await service.GetAcceptedInvitationForUserAsync(id, InvitationAuthorization.GetUserID(User)));
        }

        // POST /api/tenantinvitation/respond?token=...&response=ACCEPT
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("/api/tenantinvitation/respond", Name = "RespondTenantInvitation")]
        public async Task<IActionResult> RespondTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, [FromQuery] string token, [FromQuery] string response)
        {
            var result = await tenantinvitationService.RespondToTenantInvitationAsync(token, response);
            if (!result.Success)
                return BadRequest(result);

            return Ok(new
            {
                tenantInvitationID = result.TenantInvitationID ?? result.TenantInvitation?.TenantInvitationID,
                status = result.Status ?? result.TenantInvitation?.Status
            });
        }

        // POST api/tenantinvitation/{tenantInvitationID}/createLease
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("/api/tenantinvitation/{tenantInvitationID}/createLease", Name = "CreateLeaseFromTenantInvitation")]
        public async Task<IActionResult> CreateLeaseFromTenantInvitation(
            [FromServices] ITenantInvitationService tenantinvitationService,
            [FromRoute] Guid tenantInvitationID,
            [FromBody] CreateLeaseFromTenantInvitationRequestDto request)
        {
            if (request == null || request.TenantID == Guid.Empty)
            {
                return Problem(
                    detail: "A valid tenantID is required.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid tenant");
            }

            var userIdClaim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!long.TryParse(userIdClaim, out var signedInUserId) || signedInUserId <= 0)
            {
                return Problem(
                    detail: "The signed-in user could not be resolved.",
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden");
            }

            var signedInEmail = User.FindFirst("Email")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? string.Empty;

            var capturedBy = User.Identity?.Name;

            try
            {
                var result = await tenantinvitationService.CreateLeaseFromTenantInvitationAsync(
                    tenantInvitationID,
                    request.TenantID,
                    signedInUserId,
                    signedInEmail,
                    capturedBy);

                if (result == null)
                {
                    return Problem(
                        detail: "TenantInvitation with the specified ID was not found.",
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Not found");
                }

                if (result.AlreadyExists)
                {
                    return Ok(new
                    {
                        leaseID = result.LeaseID,
                        status = result.Status
                    });
                }

                return CreatedAtRoute("GetLeaseByID", new { id = result.LeaseID }, new
                {
                    leaseID = result.LeaseID,
                    status = result.Status
                });
            }
            catch (ApiProblemException ex)
            {
                return Problem(
                    detail: ex.Detail,
                    statusCode: ex.StatusCode,
                    title: ex.Title);
            }
        }

        // GET api/tenantinvitation/pending-for-me
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/api/tenantinvitation/pending-for-me", Name = "GetPendingInvitationsForMe")]
        public async Task<IActionResult> GetPendingInvitationsForMe(
            [FromServices] ITenantInvitationService tenantinvitationService,
            [FromServices] ITenantRepository tenantRepository)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!long.TryParse(userIdClaim, out var signedInUserId) || signedInUserId <= 0)
            {
                return Problem(
                    detail: "The signed-in user could not be resolved.",
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden");
            }

            try
            {
                var tenant = await tenantRepository.GetTenantByUserIDAsync(signedInUserId);
                if (tenant == null)
                {
                    return Ok(new List<TenantInvitationDto>());
                }

                var pendingInvitations = await tenantinvitationService.GetPendingInvitationsForTenantAsync(tenant.TenantID);
                return Ok(pendingInvitations);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: $"An error occurred while fetching pending invitations: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal server error");
            }
        }
    }
}