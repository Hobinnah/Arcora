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
    public class ListingTypeController : ControllerBase
    {
        // GET: api/<ListingTypeController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingTypes")]
        public async Task<IActionResult> Get([FromServices] IListingTypeService listingtypeService, [FromQuery] Paging paging)
        {
            return Ok(await listingtypeService.GetAll(paging));
        }

        // GET api/<ListingTypeController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingTypeByID")]
        public async Task<IActionResult> GetListingTypeByID([FromServices] IListingTypeService listingtypeService, Guid id)
        {
            var result = await listingtypeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingType with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingTypeController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingType")]
        public async Task<IActionResult> CreateListingType([FromServices] IListingTypeService listingtypeService, [FromBody] ListingTypeDto listingtypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingtypeService.CreateListingType(listingtypeDto);
            if (result.ListingTypeID != null && result.ListingTypeID != Guid.Empty)
                return CreatedAtRoute("GetListingTypeByID", new { id = result.ListingTypeID }, result);
            return BadRequest(new { message = "Failed to create listingtype. A listingtype with the same name may already exist." });
        }

        // PUT api/<ListingTypeController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingType")]
        public async Task<IActionResult> UpdateListingType([FromServices] IListingTypeService listingtypeService, Guid id, [FromBody] ListingTypeDto listingtypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingtypeService.UpdateListingType(id, listingtypeDto);
            if (result == null)
                return NotFound(new { message = "ListingType with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingTypeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingType")]
        public async Task<IActionResult> DeleteListingType([FromServices] IListingTypeService listingtypeService, Guid id)
        {
            try
            {
                await listingtypeService.DeleteListingType(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingType with the specified ID was not found." });
            }
        }
    }
}