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
    public class CalendarEventController : ControllerBase
    {
        // GET: api/<CalendarEventController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCalendarEvents")]
        public async Task<IActionResult> Get([FromServices] ICalendarEventService calendareventService, [FromQuery] Paging paging)
        {
            return Ok(await calendareventService.GetAll(paging));
        }

        // GET api/<CalendarEventController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetCalendarEventByID")]
        public async Task<IActionResult> GetCalendarEventByID([FromServices] ICalendarEventService calendareventService, Guid id)
        {
            var result = await calendareventService.GetID(id);
            if (result == null)
                return NotFound(new { message = "CalendarEvent with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<CalendarEventController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateCalendarEvent")]
        public async Task<IActionResult> CreateCalendarEvent([FromServices] ICalendarEventService calendareventService, [FromBody] CalendarEventDto calendareventDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await calendareventService.CreateCalendarEvent(calendareventDto);
            if (result.CalendarEventID != null && result.CalendarEventID != Guid.Empty)
                return CreatedAtRoute("GetCalendarEventByID", new { id = result.CalendarEventID }, result);
            return BadRequest(new { message = "Failed to create calendarevent. A calendarevent with the same name may already exist." });
        }

        // PUT api/<CalendarEventController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateCalendarEvent")]
        public async Task<IActionResult> UpdateCalendarEvent([FromServices] ICalendarEventService calendareventService, Guid id, [FromBody] CalendarEventDto calendareventDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await calendareventService.UpdateCalendarEvent(id, calendareventDto);
            if (result == null)
                return NotFound(new { message = "CalendarEvent with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<CalendarEventController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteCalendarEvent")]
        public async Task<IActionResult> DeleteCalendarEvent([FromServices] ICalendarEventService calendareventService, Guid id)
        {
            try
            {
                await calendareventService.DeleteCalendarEvent(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "CalendarEvent with the specified ID was not found." });
            }
        }

        // POST: api/calendarevent/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateCalendarEventStatus")]
        public async Task<ActionResult> UpdateCalendarEventStatus([FromServices] ICalendarEventService calendareventService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var calendarevent = await calendareventService.UpdateCalendarEventStatus(id, status);
            if (calendarevent == null)
                return NotFound($"CalendarEvent with ID {id} not found.");
            return Ok(calendarevent);
        }
    }
}