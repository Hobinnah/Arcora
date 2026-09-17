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
    public class OrganizationMemberController : ControllerBase
    {
        // GET: api/<OrganizationMemberController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllOrganizationMembers")]
        public async Task<IActionResult> Get([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] Paging paging)
        {
            return Ok(await organizationmemberService.GetAll(paging));
        }

        // GET api/<OrganizationMemberController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetOrganizationMemberByID")]
        public async Task<IActionResult> GetOrganizationMemberByID([FromServices] IOrganizationMemberService organizationmemberService, Guid id)
        {
            var result = await organizationmemberService.GetID(id);
            if (result == null)
                return NotFound(new { message = "OrganizationMember with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<OrganizationMemberController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateOrganizationMember")]
        public async Task<IActionResult> CreateOrganizationMember([FromServices] IOrganizationMemberService organizationmemberService, [FromBody] OrganizationMemberDto organizationmemberDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationmemberService.CreateOrganizationMember(organizationmemberDto);
            if (result.OrganizationMemberID != null && result.OrganizationMemberID != Guid.Empty)
                return CreatedAtRoute("GetOrganizationMemberByID", new { id = result.OrganizationMemberID }, result);
            return BadRequest(new { message = "Failed to create organizationmember. A organizationmember with the same name may already exist." });
        }

        // PUT api/<OrganizationMemberController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateOrganizationMember")]
        public async Task<IActionResult> UpdateOrganizationMember([FromServices] IOrganizationMemberService organizationmemberService, Guid id, [FromBody] OrganizationMemberDto organizationmemberDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await organizationmemberService.UpdateOrganizationMember(id, organizationmemberDto);
            if (result == null)
                return NotFound(new { message = "OrganizationMember with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<OrganizationMemberController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteOrganizationMember")]
        public async Task<IActionResult> DeleteOrganizationMember([FromServices] IOrganizationMemberService organizationmemberService, Guid id)
        {
            try
            {
                await organizationmemberService.DeleteOrganizationMember(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "OrganizationMember with the specified ID was not found." });
            }
        }

        // POST: api/organizationmember/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateOrganizationMemberStatus")]
        public async Task<ActionResult> UpdateOrganizationMemberStatus([FromServices] IOrganizationMemberService organizationmemberService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var organizationmember = await organizationmemberService.UpdateOrganizationMemberStatus(id, status);
            if (organizationmember == null)
                return NotFound($"OrganizationMember with ID {id} not found.");
            return Ok(organizationmember);
        }
    }
}