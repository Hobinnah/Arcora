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
    public class LeaseController : ControllerBase
    {
        // GET: api/<LeaseController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeases")]
        public async Task<IActionResult> Get([FromServices] ILeaseService leaseService, [FromQuery] Paging paging)
        {
            return Ok(await leaseService.GetAll(paging));
        }

        // GET api/<LeaseController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseByID")]
        public async Task<IActionResult> GetLeaseByID([FromServices] ILeaseService leaseService, Guid id)
        {
            var result = await leaseService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Lease with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLease")]
        public async Task<IActionResult> CreateLease([FromServices] ILeaseService leaseService, [FromBody] LeaseDto leaseDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaseService.CreateLease(leaseDto);
            if (result.LeaseID != null && result.LeaseID != Guid.Empty)
                return CreatedAtRoute("GetLeaseByID", new { id = result.LeaseID }, result);
            return BadRequest(new { message = "Failed to create lease. A lease with the same name may already exist." });
        }

        // PUT api/<LeaseController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLease")]
        public async Task<IActionResult> UpdateLease([FromServices] ILeaseService leaseService, Guid id, [FromBody] LeaseDto leaseDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaseService.UpdateLease(id, leaseDto);
            if (result == null)
                return NotFound(new { message = "Lease with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLease")]
        public async Task<IActionResult> DeleteLease([FromServices] ILeaseService leaseService, Guid id)
        {
            try
            {
                await leaseService.DeleteLease(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Lease with the specified ID was not found." });
            }
        }

        // POST: api/lease/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateLeaseStatus")]
        public async Task<ActionResult> UpdateLeaseStatus([FromServices] ILeaseService leaseService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var lease = await leaseService.UpdateLeaseStatus(id, status);
            if (lease == null)
                return NotFound($"Lease with ID {id} not found.");
            return Ok(lease);
        }
    }
}