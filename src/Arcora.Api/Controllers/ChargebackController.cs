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
    public class ChargebackController : ControllerBase
    {
        // GET: api/<ChargebackController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllChargebacks")]
        public async Task<IActionResult> Get([FromServices] IChargebackService chargebackService, [FromQuery] Paging paging)
        {
            return Ok(await chargebackService.GetAll(paging));
        }

        // GET api/<ChargebackController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetChargebackByID")]
        public async Task<IActionResult> GetChargebackByID([FromServices] IChargebackService chargebackService, Guid id)
        {
            var result = await chargebackService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Chargeback with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ChargebackController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateChargeback")]
        public async Task<IActionResult> CreateChargeback([FromServices] IChargebackService chargebackService, [FromBody] ChargebackDto chargebackDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await chargebackService.CreateChargeback(chargebackDto);
            if (result.ChargebackID != null && result.ChargebackID != Guid.Empty)
                return CreatedAtRoute("GetChargebackByID", new { id = result.ChargebackID }, result);
            return BadRequest(new { message = "Failed to create chargeback. A chargeback with the same name may already exist." });
        }

        // PUT api/<ChargebackController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateChargeback")]
        public async Task<IActionResult> UpdateChargeback([FromServices] IChargebackService chargebackService, Guid id, [FromBody] ChargebackDto chargebackDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await chargebackService.UpdateChargeback(id, chargebackDto);
            if (result == null)
                return NotFound(new { message = "Chargeback with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ChargebackController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteChargeback")]
        public async Task<IActionResult> DeleteChargeback([FromServices] IChargebackService chargebackService, Guid id)
        {
            try
            {
                await chargebackService.DeleteChargeback(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Chargeback with the specified ID was not found." });
            }
        }

        // POST: api/chargeback/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateChargebackStatus")]
        public async Task<ActionResult> UpdateChargebackStatus([FromServices] IChargebackService chargebackService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var chargeback = await chargebackService.UpdateChargebackStatus(id, status);
            if (chargeback == null)
                return NotFound($"Chargeback with ID {id} not found.");
            return Ok(chargeback);
        }
    }
}