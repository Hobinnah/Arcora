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
    public class OrganizationStatementController : ControllerBase
    {
        // GET: api/<OrganizationStatementController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllOrganizationStatements")]
        public async Task<IActionResult> Get([FromServices] IOrganizationStatementService organizationstatementService, [FromQuery] Paging paging)
        {
            return Ok(await organizationstatementService.GetAll(paging));
        }

        // GET api/<OrganizationStatementController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetOrganizationStatementByID")]
        public async Task<IActionResult> GetOrganizationStatementByID([FromServices] IOrganizationStatementService organizationstatementService, Guid id)
        {
            var result = await organizationstatementService.GetID(id);
            if (result == null)
                return NotFound(new { message = "OrganizationStatement with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<OrganizationStatementController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateOrganizationStatement")]
        public async Task<IActionResult> CreateOrganizationStatement([FromServices] IOrganizationStatementService organizationstatementService, [FromBody] OrganizationStatementDto organizationstatementDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationstatementService.CreateOrganizationStatement(organizationstatementDto);
            if (result.OrganizationStatementID != null && result.OrganizationStatementID != Guid.Empty)
                return CreatedAtRoute("GetOrganizationStatementByID", new { id = result.OrganizationStatementID }, result);
            return BadRequest(new { message = "Failed to create organizationstatement. A organizationstatement with the same name may already exist." });
        }

        // PUT api/<OrganizationStatementController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateOrganizationStatement")]
        public async Task<IActionResult> UpdateOrganizationStatement([FromServices] IOrganizationStatementService organizationstatementService, Guid id, [FromBody] OrganizationStatementDto organizationstatementDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationstatementService.UpdateOrganizationStatement(id, organizationstatementDto);
            if (result == null)
                return NotFound(new { message = "OrganizationStatement with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<OrganizationStatementController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteOrganizationStatement")]
        public async Task<IActionResult> DeleteOrganizationStatement([FromServices] IOrganizationStatementService organizationstatementService, Guid id)
        {
            try
            {
                await organizationstatementService.DeleteOrganizationStatement(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "OrganizationStatement with the specified ID was not found." });
            }
        }
    }
}