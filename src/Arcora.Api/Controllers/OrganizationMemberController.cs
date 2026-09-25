// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using System.Security.Claims;
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

        // GET api/<OrganizationMemberController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{organizationID}", Name = "GetOrganizationMemberByOrgID")]
        public async Task<IActionResult> GetOrganizationMemberByOrgID([FromServices] IOrganizationMemberService organizationmemberService, Guid organizationID)
        {
            var result = await organizationmemberService.GetOrganizationMemberByOrgID(organizationID);
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

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "InviteCohost")]
        public async Task<IActionResult> InviteCohost([FromServices] IOrganizationMemberService organizationmemberService, [FromBody] CohostInvitationDto cohostInvitationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId) || userId <= 0)
                return BadRequest(new { message = "Unable to resolve the authenticated user." });

            try
            {
                var result = await organizationmemberService.InviteCohostAsync(cohostInvitationDto, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetCohostInvitationsByOrganization")]
        public async Task<IActionResult> GetCohostInvitationsByOrganization([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] Guid organizationID)
        {
            var result = await organizationmemberService.GetCohostInvitationsByOrganizationAsync(organizationID);
            return Ok(result);
        }

        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCohostInvitationsByOrganization")]
        public async Task<IActionResult> GetAllCohostInvitationsByOrganization([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] Guid organizationID)
        {
            var result = await organizationmemberService.GetAllCohostInvitationsByOrganizationAsync(organizationID);
            return Ok(result);
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet(Name = "GetCohostInviteDetails")]
        public async Task<IActionResult> InviteDetails([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "A token is required." });

            var result = await organizationmemberService.GetCohostInviteDetailsAsync(token);
            if (result == null)
                return NotFound(new { message = "The cohost invitation could not be found or the link is invalid or expired." });

            return Ok(result);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "RespondToCohostInvitation")]
        public async Task<IActionResult> RespondInvitation([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] string token, [FromQuery] string response)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(response))
                return BadRequest(new { message = "A token and response are required." });

            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId) || userId <= 0)
                return BadRequest(new { message = "Unable to resolve the authenticated user." });

            try
            {
                var result = await organizationmemberService.RespondToCohostInvitationAsync(token, response, userId);
                if (result == null)
                    return NotFound(new { message = "The cohost invitation could not be found or the link is invalid or expired." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "RevokeCohostInvitation")]
        public async Task<IActionResult> RevokeCohostInvitation([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] Guid cohostInvitationID)
        {
            if (cohostInvitationID == Guid.Empty)
                return BadRequest(new { message = "A valid cohostInvitationID is required." });

            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId) || userId <= 0)
                return BadRequest(new { message = "Unable to resolve the authenticated user." });

            try
            {
                var result = await organizationmemberService.RevokeCohostInvitationAsync(cohostInvitationID, userId);
                if (result == null)
                    return NotFound(new { message = "Cohost invitation not found." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "ReactivateRevokedCohost")]
        public async Task<IActionResult> ReactivateRevokedCohost([FromServices] IOrganizationMemberService organizationmemberService, [FromQuery] Guid cohostInvitationID)
        {
            if (cohostInvitationID == Guid.Empty)
                return BadRequest(new { message = "A valid cohostInvitationID is required." });

            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId) || userId <= 0)
                return BadRequest(new { message = "Unable to resolve the authenticated user." });

            try
            {
                var result = await organizationmemberService.ReactivateRevokedCohostAsync(cohostInvitationID, userId);
                if (result == null)
                    return NotFound(new { message = "Cohost invitation not found." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}