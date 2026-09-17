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
    public class ViewingAppointmentsController : ControllerBase
    {
        // GET: api/<ViewingAppointmentsController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllViewingAppointments")]
        public async Task<IActionResult> Get([FromServices] IViewingAppointmentsService viewingappointmentsService, [FromQuery] Paging paging)
        {
            return Ok(await viewingappointmentsService.GetAll(paging));
        }

        // GET api/<ViewingAppointmentsController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetViewingAppointmentsByID")]
        public async Task<IActionResult> GetViewingAppointmentsByID([FromServices] IViewingAppointmentsService viewingappointmentsService, Guid id)
        {
            var result = await viewingappointmentsService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ViewingAppointments with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ViewingAppointmentsController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateViewingAppointments")]
        public async Task<IActionResult> CreateViewingAppointments([FromServices] IViewingAppointmentsService viewingappointmentsService, [FromBody] ViewingAppointmentsDto viewingappointmentsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await viewingappointmentsService.CreateViewingAppointments(viewingappointmentsDto);
            if (result.ViewingAppointmentID != null && result.ViewingAppointmentID != Guid.Empty)
                return CreatedAtRoute("GetViewingAppointmentsByID", new { id = result.ViewingAppointmentID }, result);
            return BadRequest(new { message = "Failed to create viewingappointments. A viewingappointments with the same name may already exist." });
        }

        // PUT api/<ViewingAppointmentsController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateViewingAppointments")]
        public async Task<IActionResult> UpdateViewingAppointments([FromServices] IViewingAppointmentsService viewingappointmentsService, Guid id, [FromBody] ViewingAppointmentsDto viewingappointmentsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await viewingappointmentsService.UpdateViewingAppointments(id, viewingappointmentsDto);
            if (result == null)
                return NotFound(new { message = "ViewingAppointments with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ViewingAppointmentsController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteViewingAppointments")]
        public async Task<IActionResult> DeleteViewingAppointments([FromServices] IViewingAppointmentsService viewingappointmentsService, Guid id)
        {
            try
            {
                await viewingappointmentsService.DeleteViewingAppointments(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ViewingAppointments with the specified ID was not found." });
            }
        }

        // POST: api/viewingappointments/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateViewingAppointmentsStatus")]
        public async Task<ActionResult> UpdateViewingAppointmentsStatus([FromServices] IViewingAppointmentsService viewingappointmentsService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var viewingappointments = await viewingappointmentsService.UpdateViewingAppointmentsStatus(id, status);
            if (viewingappointments == null)
                return NotFound($"ViewingAppointments with ID {id} not found.");
            return Ok(viewingappointments);
        }
    }
}