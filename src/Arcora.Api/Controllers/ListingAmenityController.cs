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
    public class ListingAmenityController : ControllerBase
    {
        // GET: api/<ListingAmenityController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingAmenities")]
        public async Task<IActionResult> Get([FromServices] IListingAmenityService listingamenityService, [FromQuery] Paging paging)
        {
            return Ok(await listingamenityService.GetAll(paging));
        }

        // GET api/<ListingAmenityController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingAmenityByID")]
        public async Task<IActionResult> GetListingAmenityByID([FromServices] IListingAmenityService listingamenityService, Guid id)
        {
            var result = await listingamenityService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingAmenity with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingAmenityController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingAmenity")]
        public async Task<IActionResult> CreateListingAmenity([FromServices] IListingAmenityService listingamenityService, [FromBody] ListingAmenityDto listingamenityDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingamenityService.CreateListingAmenity(listingamenityDto);
            if (result.ListingAmenityID != null && result.ListingAmenityID != Guid.Empty)
                return CreatedAtRoute("GetListingAmenityByID", new { id = result.ListingAmenityID }, result);
            return BadRequest(new { message = "Failed to create listingamenity. A listingamenity with the same name may already exist." });
        }

        // PUT api/<ListingAmenityController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingAmenity")]
        public async Task<IActionResult> UpdateListingAmenity([FromServices] IListingAmenityService listingamenityService, Guid id, [FromBody] ListingAmenityDto listingamenityDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingamenityService.UpdateListingAmenity(id, listingamenityDto);
            if (result == null)
                return NotFound(new { message = "ListingAmenity with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingAmenityController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingAmenity")]
        public async Task<IActionResult> DeleteListingAmenity([FromServices] IListingAmenityService listingamenityService, Guid id)
        {
            try
            {
                await listingamenityService.DeleteListingAmenity(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingAmenity with the specified ID was not found." });
            }
        }
    }
}