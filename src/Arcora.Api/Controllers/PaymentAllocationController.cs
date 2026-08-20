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
    public class PaymentAllocationController : ControllerBase
    {
        // GET: api/<PaymentAllocationController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllPaymentAllocations")]
        public async Task<IActionResult> Get([FromServices] IPaymentAllocationService paymentallocationService, [FromQuery] Paging paging)
        {
            return Ok(await paymentallocationService.GetAll(paging));
        }

        // GET api/<PaymentAllocationController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPaymentAllocationByID")]
        public async Task<IActionResult> GetPaymentAllocationByID([FromServices] IPaymentAllocationService paymentallocationService, long id)
        {
            var result = await paymentallocationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "PaymentAllocation with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PaymentAllocationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreatePaymentAllocation")]
        public async Task<IActionResult> CreatePaymentAllocation([FromServices] IPaymentAllocationService paymentallocationService, [FromBody] PaymentAllocationDto paymentallocationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentallocationService.CreatePaymentAllocation(paymentallocationDto);
            if (result != null && result.PaymentAllocationID != 0)
                return CreatedAtRoute("GetPaymentAllocationByID", new { id = result.PaymentAllocationID }, result);
            return BadRequest(new { message = "Failed to create paymentallocation. A paymentallocation with the same name may already exist." });
        }

        // PUT api/<PaymentAllocationController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdatePaymentAllocation")]
        public async Task<IActionResult> UpdatePaymentAllocation([FromServices] IPaymentAllocationService paymentallocationService, long id, [FromBody] PaymentAllocationDto paymentallocationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await paymentallocationService.UpdatePaymentAllocation(id, paymentallocationDto);
            if (result == null)
                return NotFound(new { message = "PaymentAllocation with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PaymentAllocationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeletePaymentAllocation")]
        public async Task<IActionResult> DeletePaymentAllocation([FromServices] IPaymentAllocationService paymentallocationService, long id)
        {
            try
            {
                await paymentallocationService.DeletePaymentAllocation(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "PaymentAllocation with the specified ID was not found." });
            }
        }
    }
}