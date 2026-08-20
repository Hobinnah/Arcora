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
    public class ReservationHoldController : ControllerBase
    {
        // GET: api/<ReservationHoldController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllReservationHolds")]
        public async Task<IActionResult> Get([FromServices] IReservationHoldService reservationholdService, [FromQuery] Paging paging)
        {
            return Ok(await reservationholdService.GetAll(paging));
        }

        // GET api/<ReservationHoldController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetReservationHoldByID")]
        public async Task<IActionResult> GetReservationHoldByID([FromServices] IReservationHoldService reservationholdService, Guid id)
        {
            var result = await reservationholdService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ReservationHold with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ReservationHoldController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateReservationHold")]
        public async Task<IActionResult> CreateReservationHold([FromServices] IReservationHoldService reservationholdService, [FromBody] ReservationHoldDto reservationholdDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await reservationholdService.CreateReservationHold(reservationholdDto);
            if (result.ReservationHoldID != null && result.ReservationHoldID != Guid.Empty)
                return CreatedAtRoute("GetReservationHoldByID", new { id = result.ReservationHoldID }, result);
            return BadRequest(new { message = "Failed to create reservationhold. A reservationhold with the same name may already exist." });
        }

        // PUT api/<ReservationHoldController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateReservationHold")]
        public async Task<IActionResult> UpdateReservationHold([FromServices] IReservationHoldService reservationholdService, Guid id, [FromBody] ReservationHoldDto reservationholdDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await reservationholdService.UpdateReservationHold(id, reservationholdDto);
            if (result == null)
                return NotFound(new { message = "ReservationHold with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ReservationHoldController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteReservationHold")]
        public async Task<IActionResult> DeleteReservationHold([FromServices] IReservationHoldService reservationholdService, Guid id)
        {
            try
            {
                await reservationholdService.DeleteReservationHold(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ReservationHold with the specified ID was not found." });
            }
        }

        // POST: api/reservationhold/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateReservationHoldStatus")]
        public async Task<ActionResult> UpdateReservationHoldStatus([FromServices] IReservationHoldService reservationholdService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var reservationhold = await reservationholdService.UpdateReservationHoldStatus(id, status);
            if (reservationhold == null)
                return NotFound($"ReservationHold with ID {id} not found.");
            return Ok(reservationhold);
        }
    }
}