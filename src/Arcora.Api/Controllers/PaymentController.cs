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
    public class PaymentController : ControllerBase
    {
        // GET: api/<PaymentController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPayments")]
        public async Task<IActionResult> Get([FromServices] IPaymentService paymentService, [FromQuery] Paging paging)
        {
            return Ok(await paymentService.GetAll(paging));
        }

        // GET api/<PaymentController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentByID")]
        public async Task<IActionResult> GetPaymentByID([FromServices] IPaymentService paymentService, Guid id)
        {
            var result = await paymentService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Payment with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromServices] IPaymentService paymentService, [FromBody] PaymentDto paymentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentService.CreatePayment(paymentDto);
            if (result.PaymentID != null && result.PaymentID != Guid.Empty)
                return CreatedAtRoute("GetPaymentByID", new { id = result.PaymentID }, result);
            return BadRequest(new { message = "Failed to create payment. A payment with the same name may already exist." });
        }

        // PUT api/<PaymentController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePayment")]
        public async Task<IActionResult> UpdatePayment([FromServices] IPaymentService paymentService, Guid id, [FromBody] PaymentDto paymentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentService.UpdatePayment(id, paymentDto);
            if (result == null)
                return NotFound(new { message = "Payment with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePayment")]
        public async Task<IActionResult> DeletePayment([FromServices] IPaymentService paymentService, Guid id)
        {
            try
            {
                await paymentService.DeletePayment(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Payment with the specified ID was not found." });
            }
        }

        // POST: api/payment/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePaymentStatus")]
        public async Task<ActionResult> UpdatePaymentStatus([FromServices] IPaymentService paymentService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var payment = await paymentService.UpdatePaymentStatus(id, status);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found.");
            return Ok(payment);
        }
    }
}