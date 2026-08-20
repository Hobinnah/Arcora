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
    public class TenantGuarantorController : ControllerBase
    {
        // GET: api/<TenantGuarantorController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantGuarantors")]
        public async Task<IActionResult> Get([FromServices] ITenantGuarantorService tenantguarantorService, [FromQuery] Paging paging)
        {
            return Ok(await tenantguarantorService.GetAll(paging));
        }

        // GET api/<TenantGuarantorController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantGuarantorByID")]
        public async Task<IActionResult> GetTenantGuarantorByID([FromServices] ITenantGuarantorService tenantguarantorService, Guid id)
        {
            var result = await tenantguarantorService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantGuarantor with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantGuarantorController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantGuarantor")]
        public async Task<IActionResult> CreateTenantGuarantor([FromServices] ITenantGuarantorService tenantguarantorService, [FromBody] TenantGuarantorDto tenantguarantorDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantguarantorService.CreateTenantGuarantor(tenantguarantorDto);
            if (result.TenantGuarantorID != null && result.TenantGuarantorID != Guid.Empty)
                return CreatedAtRoute("GetTenantGuarantorByID", new { id = result.TenantGuarantorID }, result);
            return BadRequest(new { message = "Failed to create tenantguarantor. A tenantguarantor with the same name may already exist." });
        }

        // PUT api/<TenantGuarantorController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantGuarantor")]
        public async Task<IActionResult> UpdateTenantGuarantor([FromServices] ITenantGuarantorService tenantguarantorService, Guid id, [FromBody] TenantGuarantorDto tenantguarantorDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantguarantorService.UpdateTenantGuarantor(id, tenantguarantorDto);
            if (result == null)
                return NotFound(new { message = "TenantGuarantor with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantGuarantorController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantGuarantor")]
        public async Task<IActionResult> DeleteTenantGuarantor([FromServices] ITenantGuarantorService tenantguarantorService, Guid id)
        {
            try
            {
                await tenantguarantorService.DeleteTenantGuarantor(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantGuarantor with the specified ID was not found." });
            }
        }

        // POST: api/tenantguarantor/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateTenantGuarantorStatus")]
        public async Task<ActionResult> UpdateTenantGuarantorStatus([FromServices] ITenantGuarantorService tenantguarantorService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var tenantguarantor = await tenantguarantorService.UpdateTenantGuarantorStatus(id, status);
            if (tenantguarantor == null)
                return NotFound($"TenantGuarantor with ID {id} not found.");
            return Ok(tenantguarantor);
        }
    }
}