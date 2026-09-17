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
    public class PaymentMethodController : ControllerBase
    {
        // GET: api/<PaymentMethodController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentMethods")]
        public async Task<IActionResult> Get([FromServices] IPaymentMethodService paymentmethodService, [FromQuery] Paging paging)
        {
            return Ok(await paymentmethodService.GetAll(paging));
        }

        // GET api/<PaymentMethodController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentMethodByID")]
        public async Task<IActionResult> GetPaymentMethodByID([FromServices] IPaymentMethodService paymentmethodService, Guid id)
        {
            var result = await paymentmethodService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentMethod with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentMethodController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentMethod")]
        public async Task<IActionResult> CreatePaymentMethod([FromServices] IPaymentMethodService paymentmethodService, [FromBody] PaymentMethodDto paymentmethodDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentmethodService.CreatePaymentMethod(paymentmethodDto);
            if (result.PaymentMethodID != null && result.PaymentMethodID != Guid.Empty)
                return CreatedAtRoute("GetPaymentMethodByID", new { id = result.PaymentMethodID }, result);
            return BadRequest(new { message = "Failed to create paymentmethod. A paymentmethod with the same name may already exist." });
        }

        // PUT api/<PaymentMethodController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentMethod")]
        public async Task<IActionResult> UpdatePaymentMethod([FromServices] IPaymentMethodService paymentmethodService, Guid id, [FromBody] PaymentMethodDto paymentmethodDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentmethodService.UpdatePaymentMethod(id, paymentmethodDto);
            if (result == null)
                return NotFound(new { message = "PaymentMethod with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentMethodController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentMethod")]
        public async Task<IActionResult> DeletePaymentMethod([FromServices] IPaymentMethodService paymentmethodService, Guid id)
        {
            try
            {
                await paymentmethodService.DeletePaymentMethod(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentMethod with the specified ID was not found." });
            }
        }
    }
}