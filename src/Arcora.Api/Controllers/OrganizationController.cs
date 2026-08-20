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
    public class OrganizationController : ControllerBase
    {
        // GET: api/<OrganizationController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllOrganizations")]
        public async Task<IActionResult> Get([FromServices] IOrganizationService organizationService, [FromQuery] Paging paging)
        {
            return Ok(await organizationService.GetAll(paging));
        }

        // GET api/<OrganizationController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetOrganizationByID")]
        public async Task<IActionResult> GetOrganizationByID([FromServices] IOrganizationService organizationService, Guid id)
        {
            var result = await organizationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Organization with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<OrganizationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateOrganization")]
        public async Task<IActionResult> CreateOrganization([FromServices] IOrganizationService organizationService, [FromBody] OrganizationDto organizationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationService.CreateOrganization(organizationDto);
            if (result.OrganizationID != null && result.OrganizationID != Guid.Empty)
                return CreatedAtRoute("GetOrganizationByID", new { id = result.OrganizationID }, result);
            return BadRequest(new { message = "Failed to create organization. A organization with the same name may already exist." });
        }

        // PUT api/<OrganizationController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateOrganization")]
        public async Task<IActionResult> UpdateOrganization([FromServices] IOrganizationService organizationService, Guid id, [FromBody] OrganizationDto organizationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationService.UpdateOrganization(id, organizationDto);
            if (result == null)
                return NotFound(new { message = "Organization with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<OrganizationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteOrganization")]
        public async Task<IActionResult> DeleteOrganization([FromServices] IOrganizationService organizationService, Guid id)
        {
            try
            {
                await organizationService.DeleteOrganization(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Organization with the specified ID was not found." });
            }
        }

        // POST: api/organization/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateOrganizationStatus")]
        public async Task<ActionResult> UpdateOrganizationStatus([FromServices] IOrganizationService organizationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var organization = await organizationService.UpdateOrganizationStatus(id, status);
            if (organization == null)
                return NotFound($"Organization with ID {id} not found.");
            return Ok(organization);
        }
    }
}