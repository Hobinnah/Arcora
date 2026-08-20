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
    public class TenantInvitationController : ControllerBase
    {
        // GET: api/<TenantInvitationController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantInvitations")]
        public async Task<IActionResult> Get([FromServices] ITenantInvitationService tenantinvitationService, [FromQuery] Paging paging)
        {
            return Ok(await tenantinvitationService.GetAll(paging));
        }

        // GET api/<TenantInvitationController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantInvitationByID")]
        public async Task<IActionResult> GetTenantInvitationByID([FromServices] ITenantInvitationService tenantinvitationService, Guid id)
        {
            var result = await tenantinvitationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantInvitationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantInvitation")]
        public async Task<IActionResult> CreateTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, [FromBody] TenantInvitationDto tenantinvitationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantinvitationService.CreateTenantInvitation(tenantinvitationDto);
            if (result.TenantInvitationID != null && result.TenantInvitationID != Guid.Empty)
                return CreatedAtRoute("GetTenantInvitationByID", new { id = result.TenantInvitationID }, result);
            return BadRequest(new { message = "Failed to create tenantinvitation. A tenantinvitation with the same name may already exist." });
        }

        // PUT api/<TenantInvitationController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantInvitation")]
        public async Task<IActionResult> UpdateTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, Guid id, [FromBody] TenantInvitationDto tenantinvitationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantinvitationService.UpdateTenantInvitation(id, tenantinvitationDto);
            if (result == null)
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantInvitationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantInvitation")]
        public async Task<IActionResult> DeleteTenantInvitation([FromServices] ITenantInvitationService tenantinvitationService, Guid id)
        {
            try
            {
                await tenantinvitationService.DeleteTenantInvitation(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantInvitation with the specified ID was not found." });
            }
        }

        // POST: api/tenantinvitation/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateTenantInvitationStatus")]
        public async Task<ActionResult> UpdateTenantInvitationStatus([FromServices] ITenantInvitationService tenantinvitationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var tenantinvitation = await tenantinvitationService.UpdateTenantInvitationStatus(id, status);
            if (tenantinvitation == null)
                return NotFound($"TenantInvitation with ID {id} not found.");
            return Ok(tenantinvitation);
        }
    }
}