// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RentalApplicationController : ControllerBase
    {
        // GET: api/<RentalApplicationController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllRentalApplications")]
        public async Task<IActionResult> Get([FromServices] IRentalApplicationService rentalapplicationService, [FromQuery] Paging paging)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Unauthorized(new { message = "Authenticated user context is invalid." });

            var isAdmin = User.IsInRole("Admin");
            return Ok(await rentalapplicationService.GetAllForActor(paging, actorUserID, isAdmin));
        }

        // GET api/<RentalApplicationController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRentalApplicationByID")]
        public async Task<IActionResult> GetRentalApplicationByID([FromServices] IRentalApplicationService rentalapplicationService, Guid id)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Unauthorized(new { message = "Authenticated user context is invalid." });

            var isAdmin = User.IsInRole("Admin");
            var result = await rentalapplicationService.GetIDForActor(id, actorUserID, isAdmin);
            if (result == null)
                return NotFound(new { message = "RentalApplication with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<RentalApplicationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateRentalApplication")]
        public async Task<IActionResult> CreateRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, [FromBody] RentalApplicationDto rentalapplicationDto)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Unauthorized(new { message = "Authenticated user context is invalid." });

            try
            {
                var result = await rentalapplicationService.CreateRentalApplicationForActor(
                    rentalapplicationDto, actorUserID, User.IsInRole("Admin"));
                if (result.RentalApplicationID != null && result.RentalApplicationID != Guid.Empty)
                    return CreatedAtRoute("GetRentalApplicationByID", new { id = result.RentalApplicationID }, result);
                return BadRequest(new { message = "Failed to create rental application." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/<RentalApplicationController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateRentalApplication")]
        public async Task<IActionResult> UpdateRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, Guid id, [FromBody] RentalApplicationDto rentalapplicationDto)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Unauthorized(new { message = "Authenticated user context is invalid." });

            try
            {
                var result = await rentalapplicationService.UpdateRentalApplicationForActor(
                    id, rentalapplicationDto, actorUserID, User.IsInRole("Admin"));
                if (result == null)
                    return NotFound(new { message = "RentalApplication with the specified ID was not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/<RentalApplicationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteRentalApplication")]
        public async Task<IActionResult> DeleteRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, Guid id)
        {
            try
            {
                await rentalapplicationService.DeleteRentalApplication(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "RentalApplication with the specified ID was not found." });
            }
        }

        // POST: api/rentalapplication/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateRentalApplicationStatus")]
        public async Task<ActionResult> UpdateRentalApplicationStatus([FromServices] IRentalApplicationService rentalapplicationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Unauthorized(new { message = "Authenticated user context is invalid." });

            try
            {
                var rentalapplication = await rentalapplicationService.UpdateRentalApplicationStatusForActor(id, status, actorUserID, User.IsInRole("Admin"));
                if (rentalapplication == null)
                    return NotFound($"RentalApplication with ID {id} not found.");
                return Ok(rentalapplication);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // GET: api/RentalApplication/GetApplicationStages
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetApplicationStages")]
        public ActionResult<IReadOnlyList<RentalApplicationStageDto>> GetApplicationStages([FromServices] IRentalApplicationService rentalapplicationService)
        {
            return Ok(rentalapplicationService.GetApplicationStages());
        }

        private bool TryGetActorUserId(out long actorUserId)
        {
            actorUserId = 0;
            var claim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return long.TryParse(claim, out actorUserId) && actorUserId > 0;
        }
    }
}