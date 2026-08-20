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
    public class LeaseRecurringChargesController : ControllerBase
    {
        // GET: api/<LeaseRecurringChargesController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseRecurringCharges")]
        public async Task<IActionResult> Get([FromServices] ILeaseRecurringChargesService leaserecurringchargesService, [FromQuery] Paging paging)
        {
            return Ok(await leaserecurringchargesService.GetAll(paging));
        }

        // GET api/<LeaseRecurringChargesController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseRecurringChargesByID")]
        public async Task<IActionResult> GetLeaseRecurringChargesByID([FromServices] ILeaseRecurringChargesService leaserecurringchargesService, Guid id)
        {
            var result = await leaserecurringchargesService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseRecurringCharges with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseRecurringChargesController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseRecurringCharges")]
        public async Task<IActionResult> CreateLeaseRecurringCharges([FromServices] ILeaseRecurringChargesService leaserecurringchargesService, [FromBody] LeaseRecurringChargesDto leaserecurringchargesDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaserecurringchargesService.CreateLeaseRecurringCharges(leaserecurringchargesDto);
            if (result.LeaseRecurringChargeID != null && result.LeaseRecurringChargeID != Guid.Empty)
                return CreatedAtRoute("GetLeaseRecurringChargesByID", new { id = result.LeaseRecurringChargeID }, result);
            return BadRequest(new { message = "Failed to create leaserecurringcharges. A leaserecurringcharges with the same name may already exist." });
        }

        // PUT api/<LeaseRecurringChargesController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseRecurringCharges")]
        public async Task<IActionResult> UpdateLeaseRecurringCharges([FromServices] ILeaseRecurringChargesService leaserecurringchargesService, Guid id, [FromBody] LeaseRecurringChargesDto leaserecurringchargesDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leaserecurringchargesService.UpdateLeaseRecurringCharges(id, leaserecurringchargesDto);
            if (result == null)
                return NotFound(new { message = "LeaseRecurringCharges with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseRecurringChargesController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseRecurringCharges")]
        public async Task<IActionResult> DeleteLeaseRecurringCharges([FromServices] ILeaseRecurringChargesService leaserecurringchargesService, Guid id)
        {
            try
            {
                await leaserecurringchargesService.DeleteLeaseRecurringCharges(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseRecurringCharges with the specified ID was not found." });
            }
        }
    }
}