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
    public class AmenityCatalogController : ControllerBase
    {
        // GET: api/<AmenityCatalogController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAmenityCatalogs")]
        public async Task<IActionResult> Get([FromServices] IAmenityCatalogService amenitycatalogService, [FromQuery] Paging paging)
        {
            return Ok(await amenitycatalogService.GetAll(paging));
        }

        // GET api/<AmenityCatalogController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAmenityCatalogByID")]
        public async Task<IActionResult> GetAmenityCatalogByID([FromServices] IAmenityCatalogService amenitycatalogService, Guid id)
        {
            var result = await amenitycatalogService.GetID(id);
            if (result == null)
                return NotFound(new { message = "AmenityCatalog with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<AmenityCatalogController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAmenityCatalog")]
        public async Task<IActionResult> CreateAmenityCatalog([FromServices] IAmenityCatalogService amenitycatalogService, [FromBody] AmenityCatalogDto amenitycatalogDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await amenitycatalogService.CreateAmenityCatalog(amenitycatalogDto);
            if (result.AmenityID != null && result.AmenityID != Guid.Empty)
                return CreatedAtRoute("GetAmenityCatalogByID", new { id = result.AmenityID }, result);
            return BadRequest(new { message = "Failed to create amenitycatalog. A amenitycatalog with the same name may already exist." });
        }

        // PUT api/<AmenityCatalogController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAmenityCatalog")]
        public async Task<IActionResult> UpdateAmenityCatalog([FromServices] IAmenityCatalogService amenitycatalogService, Guid id, [FromBody] AmenityCatalogDto amenitycatalogDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await amenitycatalogService.UpdateAmenityCatalog(id, amenitycatalogDto);
            if (result == null)
                return NotFound(new { message = "AmenityCatalog with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AmenityCatalogController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAmenityCatalog")]
        public async Task<IActionResult> DeleteAmenityCatalog([FromServices] IAmenityCatalogService amenitycatalogService, Guid id)
        {
            try
            {
                await amenitycatalogService.DeleteAmenityCatalog(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "AmenityCatalog with the specified ID was not found." });
            }
        }
    }
}