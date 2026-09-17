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
    public class ListingPolicyController : ControllerBase
    {
        // GET: api/<ListingPolicyController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingPolicies")]
        public async Task<IActionResult> Get([FromServices] IListingPolicyService listingpolicyService, [FromQuery] Paging paging)
        {
            return Ok(await listingpolicyService.GetAll(paging));
        }

        // GET api/<ListingPolicyController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingPolicyByID")]
        public async Task<IActionResult> GetListingPolicyByID([FromServices] IListingPolicyService listingpolicyService, Guid id)
        {
            var result = await listingpolicyService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingPolicy with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingPolicyController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingPolicy")]
        public async Task<IActionResult> CreateListingPolicy([FromServices] IListingPolicyService listingpolicyService, [FromBody] ListingPolicyDto listingpolicyDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingpolicyService.CreateListingPolicy(listingpolicyDto);
            if (result.ListingPolicyID != null && result.ListingPolicyID != Guid.Empty)
                return CreatedAtRoute("GetListingPolicyByID", new { id = result.ListingPolicyID }, result);
            return BadRequest(new { message = "Failed to create listingpolicy. A listingpolicy with the same name may already exist." });
        }

        // PUT api/<ListingPolicyController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingPolicy")]
        public async Task<IActionResult> UpdateListingPolicy([FromServices] IListingPolicyService listingpolicyService, Guid id, [FromBody] ListingPolicyDto listingpolicyDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingpolicyService.UpdateListingPolicy(id, listingpolicyDto);
            if (result == null)
                return NotFound(new { message = "ListingPolicy with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingPolicyController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingPolicy")]
        public async Task<IActionResult> DeleteListingPolicy([FromServices] IListingPolicyService listingpolicyService, Guid id)
        {
            try
            {
                await listingpolicyService.DeleteListingPolicy(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingPolicy with the specified ID was not found." });
            }
        }
    }
}