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
    public class TenantEmergencyContactController : ControllerBase
    {
        // GET: api/<TenantEmergencyContactController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantEmergencyContacts")]
        public async Task<IActionResult> Get([FromServices] ITenantEmergencyContactService tenantemergencycontactService, [FromQuery] Paging paging)
        {
            return Ok(await tenantemergencycontactService.GetAll(paging));
        }

        // GET api/<TenantEmergencyContactController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantEmergencyContactByID")]
        public async Task<IActionResult> GetTenantEmergencyContactByID([FromServices] ITenantEmergencyContactService tenantemergencycontactService, Guid id)
        {
            var result = await tenantemergencycontactService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantEmergencyContact with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantEmergencyContactController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantEmergencyContact")]
        public async Task<IActionResult> CreateTenantEmergencyContact([FromServices] ITenantEmergencyContactService tenantemergencycontactService, [FromBody] TenantEmergencyContactDto tenantemergencycontactDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantemergencycontactService.CreateTenantEmergencyContact(tenantemergencycontactDto);
            if (result.TenantEmergencyContactID != null && result.TenantEmergencyContactID != Guid.Empty)
                return CreatedAtRoute("GetTenantEmergencyContactByID", new { id = result.TenantEmergencyContactID }, result);
            return BadRequest(new { message = "Failed to create tenantemergencycontact. A tenantemergencycontact with the same name may already exist." });
        }

        // PUT api/<TenantEmergencyContactController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantEmergencyContact")]
        public async Task<IActionResult> UpdateTenantEmergencyContact([FromServices] ITenantEmergencyContactService tenantemergencycontactService, Guid id, [FromBody] TenantEmergencyContactDto tenantemergencycontactDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantemergencycontactService.UpdateTenantEmergencyContact(id, tenantemergencycontactDto);
            if (result == null)
                return NotFound(new { message = "TenantEmergencyContact with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantEmergencyContactController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantEmergencyContact")]
        public async Task<IActionResult> DeleteTenantEmergencyContact([FromServices] ITenantEmergencyContactService tenantemergencycontactService, Guid id)
        {
            try
            {
                await tenantemergencycontactService.DeleteTenantEmergencyContact(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantEmergencyContact with the specified ID was not found." });
            }
        }
    }
}