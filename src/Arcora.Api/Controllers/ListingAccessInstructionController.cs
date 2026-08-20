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
    public class ListingAccessInstructionController : ControllerBase
    {
        // GET: api/<ListingAccessInstructionController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingAccessInstructions")]
        public async Task<IActionResult> Get([FromServices] IListingAccessInstructionService listingaccessinstructionService, [FromQuery] Paging paging)
        {
            return Ok(await listingaccessinstructionService.GetAll(paging));
        }

        // GET api/<ListingAccessInstructionController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingAccessInstructionByID")]
        public async Task<IActionResult> GetListingAccessInstructionByID([FromServices] IListingAccessInstructionService listingaccessinstructionService, Guid id)
        {
            var result = await listingaccessinstructionService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingAccessInstruction with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingAccessInstructionController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingAccessInstruction")]
        public async Task<IActionResult> CreateListingAccessInstruction([FromServices] IListingAccessInstructionService listingaccessinstructionService, [FromBody] ListingAccessInstructionDto listingaccessinstructionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingaccessinstructionService.CreateListingAccessInstruction(listingaccessinstructionDto);
            if (result.ListingAccessInstructionID != null && result.ListingAccessInstructionID != Guid.Empty)
                return CreatedAtRoute("GetListingAccessInstructionByID", new { id = result.ListingAccessInstructionID }, result);
            return BadRequest(new { message = "Failed to create listingaccessinstruction. A listingaccessinstruction with the same name may already exist." });
        }

        // PUT api/<ListingAccessInstructionController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingAccessInstruction")]
        public async Task<IActionResult> UpdateListingAccessInstruction([FromServices] IListingAccessInstructionService listingaccessinstructionService, Guid id, [FromBody] ListingAccessInstructionDto listingaccessinstructionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingaccessinstructionService.UpdateListingAccessInstruction(id, listingaccessinstructionDto);
            if (result == null)
                return NotFound(new { message = "ListingAccessInstruction with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingAccessInstructionController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingAccessInstruction")]
        public async Task<IActionResult> DeleteListingAccessInstruction([FromServices] IListingAccessInstructionService listingaccessinstructionService, Guid id)
        {
            try
            {
                await listingaccessinstructionService.DeleteListingAccessInstruction(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingAccessInstruction with the specified ID was not found." });
            }
        }
    }
}