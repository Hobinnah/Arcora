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
    public class ListingPhotoController : ControllerBase
    {
        // GET: api/<ListingPhotoController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingPhotos")]
        public async Task<IActionResult> Get([FromServices] IListingPhotoService listingphotoService, [FromQuery] Paging paging)
        {
            return Ok(await listingphotoService.GetAll(paging));
        }

        // GET api/<ListingPhotoController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingPhotoByID")]
        public async Task<IActionResult> GetListingPhotoByID([FromServices] IListingPhotoService listingphotoService, Guid id)
        {
            var result = await listingphotoService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingPhoto with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingPhotoController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingPhoto")]
        public async Task<IActionResult> CreateListingPhoto([FromServices] IListingPhotoService listingphotoService, [FromBody] ListingPhotoDto listingphotoDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingphotoService.CreateListingPhoto(listingphotoDto);
            if (result.ListingPhotoID != null && result.ListingPhotoID != Guid.Empty)
                return CreatedAtRoute("GetListingPhotoByID", new { id = result.ListingPhotoID }, result);
            return BadRequest(new { message = "Failed to create listingphoto. A listingphoto with the same name may already exist." });
        }

        // PUT api/<ListingPhotoController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingPhoto")]
        public async Task<IActionResult> UpdateListingPhoto([FromServices] IListingPhotoService listingphotoService, Guid id, [FromBody] ListingPhotoDto listingphotoDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingphotoService.UpdateListingPhoto(id, listingphotoDto);
            if (result == null)
                return NotFound(new { message = "ListingPhoto with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingPhotoController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingPhoto")]
        public async Task<IActionResult> DeleteListingPhoto([FromServices] IListingPhotoService listingphotoService, Guid id)
        {
            try
            {
                await listingphotoService.DeleteListingPhoto(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingPhoto with the specified ID was not found." });
            }
        }
    }
}