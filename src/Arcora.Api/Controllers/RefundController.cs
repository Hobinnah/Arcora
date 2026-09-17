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
    public class RefundController : ControllerBase
    {
        // GET: api/<RefundController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllRefunds")]
        public async Task<IActionResult> Get([FromServices] IRefundService refundService, [FromQuery] Paging paging)
        {
            return Ok(await refundService.GetAll(paging));
        }

        // GET api/<RefundController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRefundByID")]
        public async Task<IActionResult> GetRefundByID([FromServices] IRefundService refundService, Guid id)
        {
            var result = await refundService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Refund with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<RefundController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateRefund")]
        public async Task<IActionResult> CreateRefund([FromServices] IRefundService refundService, [FromBody] RefundDto refundDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await refundService.CreateRefund(refundDto);
            if (result.RefundID != null && result.RefundID != Guid.Empty)
                return CreatedAtRoute("GetRefundByID", new { id = result.RefundID }, result);
            return BadRequest(new { message = "Failed to create refund. A refund with the same name may already exist." });
        }

        // PUT api/<RefundController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateRefund")]
        public async Task<IActionResult> UpdateRefund([FromServices] IRefundService refundService, Guid id, [FromBody] RefundDto refundDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await refundService.UpdateRefund(id, refundDto);
            if (result == null)
                return NotFound(new { message = "Refund with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<RefundController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteRefund")]
        public async Task<IActionResult> DeleteRefund([FromServices] IRefundService refundService, Guid id)
        {
            try
            {
                await refundService.DeleteRefund(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Refund with the specified ID was not found." });
            }
        }

        // POST: api/refund/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateRefundStatus")]
        public async Task<ActionResult> UpdateRefundStatus([FromServices] IRefundService refundService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var refund = await refundService.UpdateRefundStatus(id, status);
            if (refund == null)
                return NotFound($"Refund with ID {id} not found.");
            return Ok(refund);
        }
    }
}