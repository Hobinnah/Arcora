// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class OrgPayoutAccountController : ControllerBase
    {
        // GET: api/<OrgPayoutAccountController>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllOrgPayoutAccounts")]
        public async Task<IActionResult> Get([FromServices] IOrgPayoutAccountService orgpayoutaccountService, [FromQuery] Paging paging)
        {
            return Ok(await orgpayoutaccountService.GetAll(paging));
        }

        // GET api/<OrgPayoutAccountController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetOrgPayoutAccountByID")]
        public async Task<IActionResult> GetOrgPayoutAccountByID([FromServices] IOrgPayoutAccountService orgpayoutaccountService, long id)
        {
            var result = await orgpayoutaccountService.GetID(id);
            if (result == null)
                return NotFound(new { message = "OrgPayoutAccount with the specified ID was not found." });
            return Ok(result);
        }

        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{organizationID:guid}", Name = "GetOrgPayoutAccountStatusByOrganization")]
        public async Task<IActionResult> GetOrganizationPayoutAccountStatus([FromServices] IOrgPayoutAccountService orgpayoutaccountService, Guid organizationID)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Forbid();

            try
            {
                var result = await orgpayoutaccountService.GetOrganizationPayoutAccountStatus(organizationID, actorUserID);
                if (result == null)
                    return NotFound(new { message = "No active payout account found for this organization." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "CreateOrgPayoutOnboardingLink")]
        public async Task<IActionResult> OnboardingLink([FromServices] IOrgPayoutAccountService orgpayoutaccountService, [FromBody] OrgPayoutOnboardingLinkRequestDto request)
        {
            if (!long.TryParse(User.FindFirst("UserId")?.Value, out var actorUserID) || actorUserID <= 0)
                return Forbid();

            try
            {
                return Ok(await orgpayoutaccountService.CreateOnboardingLink(request, actorUserID));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{organizationID:guid}", Name = "RefreshOrgPayoutAccountStatus")]
        public async Task<IActionResult> RefreshStatus([FromServices] IOrgPayoutAccountService orgpayoutaccountService, Guid organizationID)
        {
            if (!TryGetActorUserId(out var actorUserID))
                return Forbid();

            try
            {
                var result = await orgpayoutaccountService.RefreshOrganizationPayoutAccountStatus(organizationID, actorUserID);
                if (result == null)
                    return NotFound(new { message = "No active payout account found for this organization." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST api/<OrgPayoutAccountController>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateOrgPayoutAccount")]
        public async Task<IActionResult> CreateOrgPayoutAccount([FromServices] IOrgPayoutAccountService orgpayoutaccountService, [FromBody] OrgPayoutAccountDto orgpayoutaccountDto)
        {
            var result = await orgpayoutaccountService.CreateOrgPayoutAccount(orgpayoutaccountDto);
            if (result != null && result.OrgPayoutAccountID != 0)
                return CreatedAtRoute("GetOrgPayoutAccountByID", new { id = result.OrgPayoutAccountID }, result);
            return BadRequest(new { message = "Failed to create orgpayoutaccount. A orgpayoutaccount with the same name may already exist." });
        }

        // PUT api/<OrgPayoutAccountController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateOrgPayoutAccount")]
        public async Task<IActionResult> UpdateOrgPayoutAccount([FromServices] IOrgPayoutAccountService orgpayoutaccountService, long id, [FromBody] OrgPayoutAccountDto orgpayoutaccountDto)
        {
            var result = await orgpayoutaccountService.UpdateOrgPayoutAccount(id, orgpayoutaccountDto);
            if (result == null)
                return NotFound(new { message = "OrgPayoutAccount with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<OrgPayoutAccountController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteOrgPayoutAccount")]
        public async Task<IActionResult> DeleteOrgPayoutAccount([FromServices] IOrgPayoutAccountService orgpayoutaccountService, long id)
        {
            try
            {
                await orgpayoutaccountService.DeleteOrgPayoutAccount(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "OrgPayoutAccount with the specified ID was not found." });
            }
        }

        private bool TryGetActorUserId(out long actorUserId)
        {
            actorUserId = 0;
            var claim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return long.TryParse(claim, out actorUserId) && actorUserId > 0;
        }
    }
}