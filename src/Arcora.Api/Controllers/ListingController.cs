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
    public class ListingController : ControllerBase
    {
        // GET: api/<ListingController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListings")]
        public async Task<IActionResult> Get([FromServices] IListingService listingService, [FromQuery] Paging paging)
        {
            return Ok(await listingService.GetAll(paging));
        }

        // GET api/<ListingController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingByID")]
        public async Task<IActionResult> GetListingByID([FromServices] IListingService listingService, Guid id)
        {
            var result = await listingService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Listing with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListing")]
        public async Task<IActionResult> CreateListing([FromServices] IListingService listingService, [FromBody] ListingDto listingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingService.CreateListing(listingDto);
            if (result.ListingID != null && result.ListingID != Guid.Empty)
                return CreatedAtRoute("GetListingByID", new { id = result.ListingID }, result);
            return BadRequest(new { message = "Failed to create listing. A listing with the same name may already exist." });
        }

        // PUT api/<ListingController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListing")]
        public async Task<IActionResult> UpdateListing([FromServices] IListingService listingService, Guid id, [FromBody] ListingDto listingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingService.UpdateListing(id, listingDto);
            if (result == null)
                return NotFound(new { message = "Listing with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListing")]
        public async Task<IActionResult> DeleteListing([FromServices] IListingService listingService, Guid id)
        {
            try
            {
                await listingService.DeleteListing(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Listing with the specified ID was not found." });
            }
        }

        // POST: api/listing/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateListingStatus")]
        public async Task<ActionResult> UpdateListingStatus([FromServices] IListingService listingService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var listing = await listingService.UpdateListingStatus(id, status);
            if (listing == null)
                return NotFound($"Listing with ID {id} not found.");
            return Ok(listing);
        }
    }
}