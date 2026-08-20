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
    public class LeaseDocExtractedTermController : ControllerBase
    {
        // GET: api/<LeaseDocExtractedTermController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseDocExtractedTerms")]
        public async Task<IActionResult> Get([FromServices] ILeaseDocExtractedTermService leasedocextractedtermService, [FromQuery] Paging paging)
        {
            return Ok(await leasedocextractedtermService.GetAll(paging));
        }

        // GET api/<LeaseDocExtractedTermController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseDocExtractedTermByID")]
        public async Task<IActionResult> GetLeaseDocExtractedTermByID([FromServices] ILeaseDocExtractedTermService leasedocextractedtermService, Guid id)
        {
            var result = await leasedocextractedtermService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseDocExtractedTerm with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseDocExtractedTermController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseDocExtractedTerm")]
        public async Task<IActionResult> CreateLeaseDocExtractedTerm([FromServices] ILeaseDocExtractedTermService leasedocextractedtermService, [FromBody] LeaseDocExtractedTermDto leasedocextractedtermDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasedocextractedtermService.CreateLeaseDocExtractedTerm(leasedocextractedtermDto);
            if (result.LeaseDocExtractedTermID != null && result.LeaseDocExtractedTermID != Guid.Empty)
                return CreatedAtRoute("GetLeaseDocExtractedTermByID", new { id = result.LeaseDocExtractedTermID }, result);
            return BadRequest(new { message = "Failed to create leasedocextractedterm. A leasedocextractedterm with the same name may already exist." });
        }

        // PUT api/<LeaseDocExtractedTermController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseDocExtractedTerm")]
        public async Task<IActionResult> UpdateLeaseDocExtractedTerm([FromServices] ILeaseDocExtractedTermService leasedocextractedtermService, Guid id, [FromBody] LeaseDocExtractedTermDto leasedocextractedtermDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasedocextractedtermService.UpdateLeaseDocExtractedTerm(id, leasedocextractedtermDto);
            if (result == null)
                return NotFound(new { message = "LeaseDocExtractedTerm with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseDocExtractedTermController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseDocExtractedTerm")]
        public async Task<IActionResult> DeleteLeaseDocExtractedTerm([FromServices] ILeaseDocExtractedTermService leasedocextractedtermService, Guid id)
        {
            try
            {
                await leasedocextractedtermService.DeleteLeaseDocExtractedTerm(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseDocExtractedTerm with the specified ID was not found." });
            }
        }
    }
}