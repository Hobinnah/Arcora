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
    public class PaymentAttemptController : ControllerBase
    {
        // GET: api/<PaymentAttemptController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentAttempts")]
        public async Task<IActionResult> Get([FromServices] IPaymentAttemptService paymentattemptService, [FromQuery] Paging paging)
        {
            return Ok(await paymentattemptService.GetAll(paging));
        }

        // GET api/<PaymentAttemptController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentAttemptByID")]
        public async Task<IActionResult> GetPaymentAttemptByID([FromServices] IPaymentAttemptService paymentattemptService, Guid id)
        {
            var result = await paymentattemptService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentAttempt with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentAttemptController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentAttempt")]
        public async Task<IActionResult> CreatePaymentAttempt([FromServices] IPaymentAttemptService paymentattemptService, [FromBody] PaymentAttemptDto paymentattemptDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentattemptService.CreatePaymentAttempt(paymentattemptDto);
            if (result.PaymentAttemptID != null && result.PaymentAttemptID != Guid.Empty)
                return CreatedAtRoute("GetPaymentAttemptByID", new { id = result.PaymentAttemptID }, result);
            return BadRequest(new { message = "Failed to create paymentattempt. A paymentattempt with the same name may already exist." });
        }

        // PUT api/<PaymentAttemptController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentAttempt")]
        public async Task<IActionResult> UpdatePaymentAttempt([FromServices] IPaymentAttemptService paymentattemptService, Guid id, [FromBody] PaymentAttemptDto paymentattemptDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentattemptService.UpdatePaymentAttempt(id, paymentattemptDto);
            if (result == null)
                return NotFound(new { message = "PaymentAttempt with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentAttemptController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentAttempt")]
        public async Task<IActionResult> DeletePaymentAttempt([FromServices] IPaymentAttemptService paymentattemptService, Guid id)
        {
            try
            {
                await paymentattemptService.DeletePaymentAttempt(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentAttempt with the specified ID was not found." });
            }
        }

        // POST: api/paymentattempt/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePaymentAttemptStatus")]
        public async Task<ActionResult> UpdatePaymentAttemptStatus([FromServices] IPaymentAttemptService paymentattemptService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var paymentattempt = await paymentattemptService.UpdatePaymentAttemptStatus(id, status);
            if (paymentattempt == null)
                return NotFound($"PaymentAttempt with ID {id} not found.");
            return Ok(paymentattempt);
        }
    }
}