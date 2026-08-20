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
    public class ApplicationOccupantController : ControllerBase
    {
        // GET: api/<ApplicationOccupantController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllApplicationOccupants")]
        public async Task<IActionResult> Get([FromServices] IApplicationOccupantService applicationoccupantService, [FromQuery] Paging paging)
        {
            return Ok(await applicationoccupantService.GetAll(paging));
        }

        // GET api/<ApplicationOccupantController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetApplicationOccupantByID")]
        public async Task<IActionResult> GetApplicationOccupantByID([FromServices] IApplicationOccupantService applicationoccupantService, Guid id)
        {
            var result = await applicationoccupantService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ApplicationOccupant with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ApplicationOccupantController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateApplicationOccupant")]
        public async Task<IActionResult> CreateApplicationOccupant([FromServices] IApplicationOccupantService applicationoccupantService, [FromBody] ApplicationOccupantDto applicationoccupantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await applicationoccupantService.CreateApplicationOccupant(applicationoccupantDto);
            if (result.ApplicationOccupantID != null && result.ApplicationOccupantID != Guid.Empty)
                return CreatedAtRoute("GetApplicationOccupantByID", new { id = result.ApplicationOccupantID }, result);
            return BadRequest(new { message = "Failed to create applicationoccupant. A applicationoccupant with the same name may already exist." });
        }

        // PUT api/<ApplicationOccupantController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateApplicationOccupant")]
        public async Task<IActionResult> UpdateApplicationOccupant([FromServices] IApplicationOccupantService applicationoccupantService, Guid id, [FromBody] ApplicationOccupantDto applicationoccupantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await applicationoccupantService.UpdateApplicationOccupant(id, applicationoccupantDto);
            if (result == null)
                return NotFound(new { message = "ApplicationOccupant with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ApplicationOccupantController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteApplicationOccupant")]
        public async Task<IActionResult> DeleteApplicationOccupant([FromServices] IApplicationOccupantService applicationoccupantService, Guid id)
        {
            try
            {
                await applicationoccupantService.DeleteApplicationOccupant(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ApplicationOccupant with the specified ID was not found." });
            }
        }

        // POST: api/applicationoccupant/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateApplicationOccupantStatus")]
        public async Task<ActionResult> UpdateApplicationOccupantStatus([FromServices] IApplicationOccupantService applicationoccupantService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var applicationoccupant = await applicationoccupantService.UpdateApplicationOccupantStatus(id, status);
            if (applicationoccupant == null)
                return NotFound($"ApplicationOccupant with ID {id} not found.");
            return Ok(applicationoccupant);
        }
    }
}