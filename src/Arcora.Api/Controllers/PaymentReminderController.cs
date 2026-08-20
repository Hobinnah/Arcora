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
    public class PaymentReminderController : ControllerBase
    {
        // GET: api/<PaymentReminderController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentReminders")]
        public async Task<IActionResult> Get([FromServices] IPaymentReminderService paymentreminderService, [FromQuery] Paging paging)
        {
            return Ok(await paymentreminderService.GetAll(paging));
        }

        // GET api/<PaymentReminderController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentReminderByID")]
        public async Task<IActionResult> GetPaymentReminderByID([FromServices] IPaymentReminderService paymentreminderService, Guid id)
        {
            var result = await paymentreminderService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentReminder with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentReminderController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentReminder")]
        public async Task<IActionResult> CreatePaymentReminder([FromServices] IPaymentReminderService paymentreminderService, [FromBody] PaymentReminderDto paymentreminderDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentreminderService.CreatePaymentReminder(paymentreminderDto);
            if (result.PaymentReminderID != null && result.PaymentReminderID != Guid.Empty)
                return CreatedAtRoute("GetPaymentReminderByID", new { id = result.PaymentReminderID }, result);
            return BadRequest(new { message = "Failed to create paymentreminder. A paymentreminder with the same name may already exist." });
        }

        // PUT api/<PaymentReminderController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentReminder")]
        public async Task<IActionResult> UpdatePaymentReminder([FromServices] IPaymentReminderService paymentreminderService, Guid id, [FromBody] PaymentReminderDto paymentreminderDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentreminderService.UpdatePaymentReminder(id, paymentreminderDto);
            if (result == null)
                return NotFound(new { message = "PaymentReminder with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentReminderController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentReminder")]
        public async Task<IActionResult> DeletePaymentReminder([FromServices] IPaymentReminderService paymentreminderService, Guid id)
        {
            try
            {
                await paymentreminderService.DeletePaymentReminder(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentReminder with the specified ID was not found." });
            }
        }

        // POST: api/paymentreminder/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePaymentReminderStatus")]
        public async Task<ActionResult> UpdatePaymentReminderStatus([FromServices] IPaymentReminderService paymentreminderService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var paymentreminder = await paymentreminderService.UpdatePaymentReminderStatus(id, status);
            if (paymentreminder == null)
                return NotFound($"PaymentReminder with ID {id} not found.");
            return Ok(paymentreminder);
        }
    }
}