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
    public class PayoutItemController : ControllerBase
    {
        // GET: api/<PayoutItemController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPayoutItems")]
        public async Task<IActionResult> Get([FromServices] IPayoutItemService payoutitemService, [FromQuery] Paging paging)
        {
            return Ok(await payoutitemService.GetAll(paging));
        }

        // GET api/<PayoutItemController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPayoutItemByID")]
        public async Task<IActionResult> GetPayoutItemByID([FromServices] IPayoutItemService payoutitemService, long id)
        {
            var result = await payoutitemService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PayoutItem with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PayoutItemController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePayoutItem")]
        public async Task<IActionResult> CreatePayoutItem([FromServices] IPayoutItemService payoutitemService, [FromBody] PayoutItemDto payoutitemDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await payoutitemService.CreatePayoutItem(payoutitemDto);
            if (result != null && result.PayoutItemID != 0)
                return CreatedAtRoute("GetPayoutItemByID", new { id = result.PayoutItemID }, result);
            return BadRequest(new { message = "Failed to create payoutitem. A payoutitem with the same name may already exist." });
        }

        // PUT api/<PayoutItemController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePayoutItem")]
        public async Task<IActionResult> UpdatePayoutItem([FromServices] IPayoutItemService payoutitemService, long id, [FromBody] PayoutItemDto payoutitemDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await payoutitemService.UpdatePayoutItem(id, payoutitemDto);
            if (result == null)
                return NotFound(new { message = "PayoutItem with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PayoutItemController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePayoutItem")]
        public async Task<IActionResult> DeletePayoutItem([FromServices] IPayoutItemService payoutitemService, long id)
        {
            try
            {
                await payoutitemService.DeletePayoutItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PayoutItem with the specified ID was not found." });
            }
        }
    }
}