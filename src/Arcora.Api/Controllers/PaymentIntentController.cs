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
    public class PaymentIntentController : ControllerBase
    {
        // GET: api/<PaymentIntentController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentIntents")]
        public async Task<IActionResult> Get([FromServices] IPaymentIntentService paymentintentService, [FromQuery] Paging paging)
        {
            return Ok(await paymentintentService.GetAll(paging));
        }

        // GET api/<PaymentIntentController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentIntentByID")]
        public async Task<IActionResult> GetPaymentIntentByID([FromServices] IPaymentIntentService paymentintentService, Guid id)
        {
            var result = await paymentintentService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentIntent with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentIntentController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentIntent")]
        public async Task<IActionResult> CreatePaymentIntent([FromServices] IPaymentIntentService paymentintentService, [FromBody] PaymentIntentDto paymentintentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentintentService.CreatePaymentIntent(paymentintentDto);
            if (result.PaymentIntentID != null && result.PaymentIntentID != Guid.Empty)
                return CreatedAtRoute("GetPaymentIntentByID", new { id = result.PaymentIntentID }, result);
            return BadRequest(new { message = "Failed to create paymentintent. A paymentintent with the same name may already exist." });
        }

        // PUT api/<PaymentIntentController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentIntent")]
        public async Task<IActionResult> UpdatePaymentIntent([FromServices] IPaymentIntentService paymentintentService, Guid id, [FromBody] PaymentIntentDto paymentintentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentintentService.UpdatePaymentIntent(id, paymentintentDto);
            if (result == null)
                return NotFound(new { message = "PaymentIntent with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentIntentController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentIntent")]
        public async Task<IActionResult> DeletePaymentIntent([FromServices] IPaymentIntentService paymentintentService, Guid id)
        {
            try
            {
                await paymentintentService.DeletePaymentIntent(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentIntent with the specified ID was not found." });
            }
        }

        // POST: api/paymentintent/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePaymentIntentStatus")]
        public async Task<ActionResult> UpdatePaymentIntentStatus([FromServices] IPaymentIntentService paymentintentService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var paymentintent = await paymentintentService.UpdatePaymentIntentStatus(id, status);
            if (paymentintent == null)
                return NotFound($"PaymentIntent with ID {id} not found.");
            return Ok(paymentintent);
        }
    }
}