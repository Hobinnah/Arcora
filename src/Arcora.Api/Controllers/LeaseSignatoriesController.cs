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
    public class LeaseSignatoriesController : ControllerBase
    {
        // GET: api/<LeaseSignatoriesController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseSignatories")]
        public async Task<IActionResult> Get([FromServices] ILeaseSignatoriesService leasesignatoriesService, [FromQuery] Paging paging)
        {
            return Ok(await leasesignatoriesService.GetAll(paging));
        }

        // GET api/<LeaseSignatoriesController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseSignatoriesByID")]
        public async Task<IActionResult> GetLeaseSignatoriesByID([FromServices] ILeaseSignatoriesService leasesignatoriesService, Guid id)
        {
            var result = await leasesignatoriesService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseSignatories with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseSignatoriesController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseSignatories")]
        public async Task<IActionResult> CreateLeaseSignatories([FromServices] ILeaseSignatoriesService leasesignatoriesService, [FromBody] LeaseSignatoriesDto leasesignatoriesDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasesignatoriesService.CreateLeaseSignatories(leasesignatoriesDto);
            if (result.LeaseSignatoryID != null && result.LeaseSignatoryID != Guid.Empty)
                return CreatedAtRoute("GetLeaseSignatoriesByID", new { id = result.LeaseSignatoryID }, result);
            return BadRequest(new { message = "Failed to create leasesignatories. A leasesignatories with the same name may already exist." });
        }

        // PUT api/<LeaseSignatoriesController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseSignatories")]
        public async Task<IActionResult> UpdateLeaseSignatories([FromServices] ILeaseSignatoriesService leasesignatoriesService, Guid id, [FromBody] LeaseSignatoriesDto leasesignatoriesDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasesignatoriesService.UpdateLeaseSignatories(id, leasesignatoriesDto);
            if (result == null)
                return NotFound(new { message = "LeaseSignatories with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseSignatoriesController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseSignatories")]
        public async Task<IActionResult> DeleteLeaseSignatories([FromServices] ILeaseSignatoriesService leasesignatoriesService, Guid id)
        {
            try
            {
                await leasesignatoriesService.DeleteLeaseSignatories(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseSignatories with the specified ID was not found." });
            }
        }

        // POST: api/leasesignatories/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateLeaseSignatoriesStatus")]
        public async Task<ActionResult> UpdateLeaseSignatoriesStatus([FromServices] ILeaseSignatoriesService leasesignatoriesService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var leasesignatories = await leasesignatoriesService.UpdateLeaseSignatoriesStatus(id, status);
            if (leasesignatories == null)
                return NotFound($"LeaseSignatories with ID {id} not found.");
            return Ok(leasesignatories);
        }
    }
}