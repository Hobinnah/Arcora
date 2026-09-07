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
    public class TenantController : ControllerBase
    {
        // GET: api/<TenantController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenants")]
        public async Task<IActionResult> Get([FromServices] ITenantService tenantService, [FromQuery] Paging paging)
        {
            return Ok(await tenantService.GetAll(paging));
        }

        // GET api/<TenantController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantByID")]
        public async Task<IActionResult> GetTenantByID([FromServices] ITenantService tenantService, Guid id)
        {
            var result = await tenantService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Tenant with the specified ID was not found." });
            return Ok(result);
        }

        // GET api/<TenantController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{userID}", Name = "GetTenantByUserID")]
        public async Task<IActionResult> GetTenantByUserID([FromServices] ITenantService tenantService, long userID)
        {
            var result = await tenantService.GetTenantByUserID(userID);
            if (result == null)
                return NotFound(new { message = "Tenant with the specified user ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenant")]
        public async Task<IActionResult> CreateTenant([FromServices] ITenantService tenantService, [FromBody] TenantDto tenantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantService.CreateTenant(tenantDto);
            if (result.TenantID != null && result.TenantID != Guid.Empty)
                return CreatedAtRoute("GetTenantByID", new { id = result.TenantID }, result);
            return BadRequest(new { message = "Failed to create tenant. A tenant with the same name may already exist." });
        }

        // PUT api/<TenantController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenant")]
        public async Task<IActionResult> UpdateTenant([FromServices] ITenantService tenantService, Guid id, [FromBody] TenantDto tenantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantService.UpdateTenant(id, tenantDto);
            if (result == null)
                return NotFound(new { message = "Tenant with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenant")]
        public async Task<IActionResult> DeleteTenant([FromServices] ITenantService tenantService, Guid id)
        {
            try
            {
                await tenantService.DeleteTenant(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Tenant with the specified ID was not found." });
            }
        }
    }
}