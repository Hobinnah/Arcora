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
    public class InvoiceDetailController : ControllerBase
    {
        // GET: api/<InvoiceDetailController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllInvoiceDetails")]
        public async Task<IActionResult> Get([FromServices] IInvoiceDetailService invoicedetailService, [FromQuery] Paging paging)
        {
            return Ok(await invoicedetailService.GetAll(paging));
        }

        // GET api/<InvoiceDetailController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetInvoiceDetailByID")]
        public async Task<IActionResult> GetInvoiceDetailByID([FromServices] IInvoiceDetailService invoicedetailService, Guid id)
        {
            var result = await invoicedetailService.GetID(id);
            if (result == null)
                return NotFound(new { message = "InvoiceDetail with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<InvoiceDetailController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateInvoiceDetail")]
        public async Task<IActionResult> CreateInvoiceDetail([FromServices] IInvoiceDetailService invoicedetailService, [FromBody] InvoiceDetailDto invoicedetailDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await invoicedetailService.CreateInvoiceDetail(invoicedetailDto);
            if (result.InvoiceDetailID != null && result.InvoiceDetailID != Guid.Empty)
                return CreatedAtRoute("GetInvoiceDetailByID", new { id = result.InvoiceDetailID }, result);
            return BadRequest(new { message = "Failed to create invoicedetail. A invoicedetail with the same name may already exist." });
        }

        // PUT api/<InvoiceDetailController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateInvoiceDetail")]
        public async Task<IActionResult> UpdateInvoiceDetail([FromServices] IInvoiceDetailService invoicedetailService, Guid id, [FromBody] InvoiceDetailDto invoicedetailDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await invoicedetailService.UpdateInvoiceDetail(id, invoicedetailDto);
            if (result == null)
                return NotFound(new { message = "InvoiceDetail with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<InvoiceDetailController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteInvoiceDetail")]
        public async Task<IActionResult> DeleteInvoiceDetail([FromServices] IInvoiceDetailService invoicedetailService, Guid id)
        {
            try
            {
                await invoicedetailService.DeleteInvoiceDetail(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "InvoiceDetail with the specified ID was not found." });
            }
        }
    }
}