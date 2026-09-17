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
    public class InvoiceMasterController : ControllerBase
    {
        // GET: api/<InvoiceMasterController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllInvoiceMasters")]
        public async Task<IActionResult> Get([FromServices] IInvoiceMasterService invoicemasterService, [FromQuery] Paging paging)
        {
            return Ok(await invoicemasterService.GetAll(paging));
        }

        // GET api/<InvoiceMasterController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetInvoiceMasterByID")]
        public async Task<IActionResult> GetInvoiceMasterByID([FromServices] IInvoiceMasterService invoicemasterService, Guid id)
        {
            var result = await invoicemasterService.GetID(id);
            if (result == null)
                return NotFound(new { message = "InvoiceMaster with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<InvoiceMasterController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateInvoiceMaster")]
        public async Task<IActionResult> CreateInvoiceMaster([FromServices] IInvoiceMasterService invoicemasterService, [FromBody] InvoiceMasterDto invoicemasterDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await invoicemasterService.CreateInvoiceMaster(invoicemasterDto);
            if (result.InvoiceMasterID != null && result.InvoiceMasterID != Guid.Empty)
                return CreatedAtRoute("GetInvoiceMasterByID", new { id = result.InvoiceMasterID }, result);
            return BadRequest(new { message = "Failed to create invoicemaster. A invoicemaster with the same name may already exist." });
        }

        // PUT api/<InvoiceMasterController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateInvoiceMaster")]
        public async Task<IActionResult> UpdateInvoiceMaster([FromServices] IInvoiceMasterService invoicemasterService, Guid id, [FromBody] InvoiceMasterDto invoicemasterDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await invoicemasterService.UpdateInvoiceMaster(id, invoicemasterDto);
            if (result == null)
                return NotFound(new { message = "InvoiceMaster with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<InvoiceMasterController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteInvoiceMaster")]
        public async Task<IActionResult> DeleteInvoiceMaster([FromServices] IInvoiceMasterService invoicemasterService, Guid id)
        {
            try
            {
                await invoicemasterService.DeleteInvoiceMaster(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "InvoiceMaster with the specified ID was not found." });
            }
        }

        // POST: api/invoicemaster/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateInvoiceMasterStatus")]
        public async Task<ActionResult> UpdateInvoiceMasterStatus([FromServices] IInvoiceMasterService invoicemasterService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var invoicemaster = await invoicemasterService.UpdateInvoiceMasterStatus(id, status);
            if (invoicemaster == null)
                return NotFound($"InvoiceMaster with ID {id} not found.");
            return Ok(invoicemaster);
        }
    }
}