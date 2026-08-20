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
    public class PaymentProviderEventController : ControllerBase
    {
        // GET: api/<PaymentProviderEventController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentProviderEvents")]
        public async Task<IActionResult> Get([FromServices] IPaymentProviderEventService paymentprovidereventService, [FromQuery] Paging paging)
        {
            return Ok(await paymentprovidereventService.GetAll(paging));
        }

        // GET api/<PaymentProviderEventController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentProviderEventByID")]
        public async Task<IActionResult> GetPaymentProviderEventByID([FromServices] IPaymentProviderEventService paymentprovidereventService, Guid id)
        {
            var result = await paymentprovidereventService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentProviderEvent with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentProviderEventController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentProviderEvent")]
        public async Task<IActionResult> CreatePaymentProviderEvent([FromServices] IPaymentProviderEventService paymentprovidereventService, [FromBody] PaymentProviderEventDto paymentprovidereventDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentprovidereventService.CreatePaymentProviderEvent(paymentprovidereventDto);
            if (result.PaymentProviderEventID != null && result.PaymentProviderEventID != Guid.Empty)
                return CreatedAtRoute("GetPaymentProviderEventByID", new { id = result.PaymentProviderEventID }, result);
            return BadRequest(new { message = "Failed to create paymentproviderevent. A paymentproviderevent with the same name may already exist." });
        }

        // PUT api/<PaymentProviderEventController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentProviderEvent")]
        public async Task<IActionResult> UpdatePaymentProviderEvent([FromServices] IPaymentProviderEventService paymentprovidereventService, Guid id, [FromBody] PaymentProviderEventDto paymentprovidereventDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentprovidereventService.UpdatePaymentProviderEvent(id, paymentprovidereventDto);
            if (result == null)
                return NotFound(new { message = "PaymentProviderEvent with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentProviderEventController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentProviderEvent")]
        public async Task<IActionResult> DeletePaymentProviderEvent([FromServices] IPaymentProviderEventService paymentprovidereventService, Guid id)
        {
            try
            {
                await paymentprovidereventService.DeletePaymentProviderEvent(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentProviderEvent with the specified ID was not found." });
            }
        }
    }
}