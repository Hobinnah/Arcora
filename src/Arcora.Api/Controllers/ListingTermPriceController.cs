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
    public class ListingTermPriceController : ControllerBase
    {
        // GET: api/<ListingTermPriceController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListingTermPrices")]
        public async Task<IActionResult> Get([FromServices] IListingTermPriceService listingtermpriceService, [FromQuery] Paging paging)
        {
            return Ok(await listingtermpriceService.GetAll(paging));
        }

        // GET api/<ListingTermPriceController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingTermPriceByID")]
        public async Task<IActionResult> GetListingTermPriceByID([FromServices] IListingTermPriceService listingtermpriceService, Guid id)
        {
            var result = await listingtermpriceService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ListingTermPrice with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ListingTermPriceController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListingTermPrice")]
        public async Task<IActionResult> CreateListingTermPrice([FromServices] IListingTermPriceService listingtermpriceService, [FromBody] ListingTermPriceDto listingtermpriceDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingtermpriceService.CreateListingTermPrice(listingtermpriceDto);
            if (result.ListingTermPriceID != null && result.ListingTermPriceID != Guid.Empty)
                return CreatedAtRoute("GetListingTermPriceByID", new { id = result.ListingTermPriceID }, result);
            return BadRequest(new { message = "Failed to create listingtermprice. A listingtermprice with the same name may already exist." });
        }

        // PUT api/<ListingTermPriceController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListingTermPrice")]
        public async Task<IActionResult> UpdateListingTermPrice([FromServices] IListingTermPriceService listingtermpriceService, Guid id, [FromBody] ListingTermPriceDto listingtermpriceDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await listingtermpriceService.UpdateListingTermPrice(id, listingtermpriceDto);
            if (result == null)
                return NotFound(new { message = "ListingTermPrice with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingTermPriceController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListingTermPrice")]
        public async Task<IActionResult> DeleteListingTermPrice([FromServices] IListingTermPriceService listingtermpriceService, Guid id)
        {
            try
            {
                await listingtermpriceService.DeleteListingTermPrice(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ListingTermPrice with the specified ID was not found." });
            }
        }
    }
}