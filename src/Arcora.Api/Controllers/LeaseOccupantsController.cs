// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LeaseOccupantsController : ControllerBase
    {
        // GET: api/<LeaseOccupantsController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseOccupants")]
        public async Task<IActionResult> Get([FromServices] ILeaseOccupantsService leaseoccupantsService, [FromQuery] Paging paging)
        {
            return Ok(await leaseoccupantsService.GetAll(paging));
        }

        // GET api/<LeaseOccupantsController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseOccupantsByID")]
        public async Task<IActionResult> GetLeaseOccupantsByID([FromServices] ILeaseOccupantsService leaseoccupantsService, Guid id)
        {
            var result = await leaseoccupantsService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseOccupants with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseOccupantsController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseOccupants")]
        public async Task<IActionResult> CreateLeaseOccupants([FromServices] ILeaseOccupantsService leaseoccupantsService, [FromBody] LeaseOccupantsDto leaseoccupantsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaseoccupantsService.CreateLeaseOccupants(leaseoccupantsDto);
            if (result.LeaseOccupantID != null && result.LeaseOccupantID != Guid.Empty)
                return CreatedAtRoute("GetLeaseOccupantsByID", new { id = result.LeaseOccupantID }, result);
            return BadRequest(new { message = "Failed to create leaseoccupants. A leaseoccupants with the same name may already exist." });
        }

        // PUT api/<LeaseOccupantsController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseOccupants")]
        public async Task<IActionResult> UpdateLeaseOccupants([FromServices] ILeaseOccupantsService leaseoccupantsService, Guid id, [FromBody] LeaseOccupantsDto leaseoccupantsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaseoccupantsService.UpdateLeaseOccupants(id, leaseoccupantsDto);
            if (result == null)
                return NotFound(new { message = "LeaseOccupants with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseOccupantsController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseOccupants")]
        public async Task<IActionResult> DeleteLeaseOccupants([FromServices] ILeaseOccupantsService leaseoccupantsService, Guid id)
        {
            try
            {
                await leaseoccupantsService.DeleteLeaseOccupants(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseOccupants with the specified ID was not found." });
            }
        }

        // POST: api/leaseoccupants/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateLeaseOccupantsStatus")]
        public async Task<ActionResult> UpdateLeaseOccupantsStatus([FromServices] ILeaseOccupantsService leaseoccupantsService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var leaseoccupants = await leaseoccupantsService.UpdateLeaseOccupantsStatus(id, status);
            if (leaseoccupants == null)
                return NotFound($"LeaseOccupants with ID {id} not found.");
            return Ok(leaseoccupants);
        }
    }
}