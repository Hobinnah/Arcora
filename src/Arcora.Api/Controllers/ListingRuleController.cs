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
    public class ListingRuleController : ControllerBase
    {
        // GET: api/<ListingRuleController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingRules")]
        public async Task<IActionResult> Get([FromServices] IListingRuleService listingruleService, [FromQuery] Paging paging)
        {
            return Ok(await listingruleService.GetAll(paging));
        }

        // GET api/<ListingRuleController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingRuleByID")]
        public async Task<IActionResult> GetListingRuleByID([FromServices] IListingRuleService listingruleService, Guid id)
        {
            var result = await listingruleService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingRule with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingRuleController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingRule")]
        public async Task<IActionResult> CreateListingRule([FromServices] IListingRuleService listingruleService, [FromBody] ListingRuleDto listingruleDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingruleService.CreateListingRule(listingruleDto);
            if (result.ListingRuleID != null && result.ListingRuleID != Guid.Empty)
                return CreatedAtRoute("GetListingRuleByID", new { id = result.ListingRuleID }, result);
            return BadRequest(new { message = "Failed to create listingrule. A listingrule with the same name may already exist." });
        }

        // PUT api/<ListingRuleController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingRule")]
        public async Task<IActionResult> UpdateListingRule([FromServices] IListingRuleService listingruleService, Guid id, [FromBody] ListingRuleDto listingruleDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingruleService.UpdateListingRule(id, listingruleDto);
            if (result == null)
                return NotFound(new { message = "ListingRule with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingRuleController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingRule")]
        public async Task<IActionResult> DeleteListingRule([FromServices] IListingRuleService listingruleService, Guid id)
        {
            try
            {
                await listingruleService.DeleteListingRule(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingRule with the specified ID was not found." });
            }
        }
    }
}