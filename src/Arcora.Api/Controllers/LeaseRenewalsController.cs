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
    public class LeaseRenewalsController : ControllerBase
    {
        // GET: api/<LeaseRenewalsController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseRenewals")]
        public async Task<IActionResult> Get([FromServices] ILeaseRenewalsService leaserenewalsService, [FromQuery] Paging paging)
        {
            return Ok(await leaserenewalsService.GetAll(paging));
        }

        // GET api/<LeaseRenewalsController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseRenewalsByID")]
        public async Task<IActionResult> GetLeaseRenewalsByID([FromServices] ILeaseRenewalsService leaserenewalsService, Guid id)
        {
            var result = await leaserenewalsService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseRenewals with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseRenewalsController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseRenewals")]
        public async Task<IActionResult> CreateLeaseRenewals([FromServices] ILeaseRenewalsService leaserenewalsService, [FromBody] LeaseRenewalsDto leaserenewalsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaserenewalsService.CreateLeaseRenewals(leaserenewalsDto);
            if (result.LeaseRenewalID != null && result.LeaseRenewalID != Guid.Empty)
                return CreatedAtRoute("GetLeaseRenewalsByID", new { id = result.LeaseRenewalID }, result);
            return BadRequest(new { message = "Failed to create leaserenewals. A leaserenewals with the same name may already exist." });
        }

        // PUT api/<LeaseRenewalsController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseRenewals")]
        public async Task<IActionResult> UpdateLeaseRenewals([FromServices] ILeaseRenewalsService leaserenewalsService, Guid id, [FromBody] LeaseRenewalsDto leaserenewalsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaserenewalsService.UpdateLeaseRenewals(id, leaserenewalsDto);
            if (result == null)
                return NotFound(new { message = "LeaseRenewals with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseRenewalsController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseRenewals")]
        public async Task<IActionResult> DeleteLeaseRenewals([FromServices] ILeaseRenewalsService leaserenewalsService, Guid id)
        {
            try
            {
                await leaserenewalsService.DeleteLeaseRenewals(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseRenewals with the specified ID was not found." });
            }
        }

        // POST: api/leaserenewals/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateLeaseRenewalsStatus")]
        public async Task<ActionResult> UpdateLeaseRenewalsStatus([FromServices] ILeaseRenewalsService leaserenewalsService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var leaserenewals = await leaserenewalsService.UpdateLeaseRenewalsStatus(id, status);
            if (leaserenewals == null)
                return NotFound($"LeaseRenewals with ID {id} not found.");
            return Ok(leaserenewals);
        }
    }
}