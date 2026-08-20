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
    public class PayoutController : ControllerBase
    {
        // GET: api/<PayoutController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPayouts")]
        public async Task<IActionResult> Get([FromServices] IPayoutService payoutService, [FromQuery] Paging paging)
        {
            return Ok(await payoutService.GetAll(paging));
        }

        // GET api/<PayoutController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPayoutByID")]
        public async Task<IActionResult> GetPayoutByID([FromServices] IPayoutService payoutService, long id)
        {
            var result = await payoutService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Payout with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PayoutController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePayout")]
        public async Task<IActionResult> CreatePayout([FromServices] IPayoutService payoutService, [FromBody] PayoutDto payoutDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await payoutService.CreatePayout(payoutDto);
            if (result != null && result.PayoutID != 0)
                return CreatedAtRoute("GetPayoutByID", new { id = result.PayoutID }, result);
            return BadRequest(new { message = "Failed to create payout. A payout with the same name may already exist." });
        }

        // PUT api/<PayoutController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePayout")]
        public async Task<IActionResult> UpdatePayout([FromServices] IPayoutService payoutService, long id, [FromBody] PayoutDto payoutDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await payoutService.UpdatePayout(id, payoutDto);
            if (result == null)
                return NotFound(new { message = "Payout with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PayoutController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePayout")]
        public async Task<IActionResult> DeletePayout([FromServices] IPayoutService payoutService, long id)
        {
            try
            {
                await payoutService.DeletePayout(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Payout with the specified ID was not found." });
            }
        }

        // POST: api/payout/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePayoutStatus")]
        public async Task<ActionResult> UpdatePayoutStatus([FromServices] IPayoutService payoutService, [FromRoute] long id, [FromRoute] string status)
        {
            var payout = await payoutService.UpdatePayoutStatus(id, status);
            if (payout == null)
                return NotFound($"Payout with ID {id} not found.");
            return Ok(payout);
        }
    }
}