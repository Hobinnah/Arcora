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
    public class OrgSubscriptionController : ControllerBase
    {
        // GET: api/<OrgSubscriptionController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllOrgSubscriptions")]
        public async Task<IActionResult> Get([FromServices] IOrgSubscriptionService orgsubscriptionService, [FromQuery] Paging paging)
        {
            return Ok(await orgsubscriptionService.GetAll(paging));
        }

        // GET api/<OrgSubscriptionController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetOrgSubscriptionByID")]
        public async Task<IActionResult> GetOrgSubscriptionByID([FromServices] IOrgSubscriptionService orgsubscriptionService, Guid id)
        {
            var result = await orgsubscriptionService.GetID(id);
            if (result == null)
                return NotFound(new { message = "OrgSubscription with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<OrgSubscriptionController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateOrgSubscription")]
        public async Task<IActionResult> CreateOrgSubscription([FromServices] IOrgSubscriptionService orgsubscriptionService, [FromBody] OrgSubscriptionDto orgsubscriptionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await orgsubscriptionService.CreateOrgSubscription(orgsubscriptionDto);
            if (result.OrgSubscriptionID != null && result.OrgSubscriptionID != Guid.Empty)
                return CreatedAtRoute("GetOrgSubscriptionByID", new { id = result.OrgSubscriptionID }, result);
            return BadRequest(new { message = "Failed to create orgsubscription. A orgsubscription with the same name may already exist." });
        }

        // PUT api/<OrgSubscriptionController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateOrgSubscription")]
        public async Task<IActionResult> UpdateOrgSubscription([FromServices] IOrgSubscriptionService orgsubscriptionService, Guid id, [FromBody] OrgSubscriptionDto orgsubscriptionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await orgsubscriptionService.UpdateOrgSubscription(id, orgsubscriptionDto);
            if (result == null)
                return NotFound(new { message = "OrgSubscription with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<OrgSubscriptionController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteOrgSubscription")]
        public async Task<IActionResult> DeleteOrgSubscription([FromServices] IOrgSubscriptionService orgsubscriptionService, Guid id)
        {
            try
            {
                await orgsubscriptionService.DeleteOrgSubscription(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "OrgSubscription with the specified ID was not found." });
            }
        }

        // POST: api/orgsubscription/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateOrgSubscriptionStatus")]
        public async Task<ActionResult> UpdateOrgSubscriptionStatus([FromServices] IOrgSubscriptionService orgsubscriptionService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var orgsubscription = await orgsubscriptionService.UpdateOrgSubscriptionStatus(id, status);
            if (orgsubscription == null)
                return NotFound($"OrgSubscription with ID {id} not found.");
            return Ok(orgsubscription);
        }
    }
}