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
    public class PropertyController : ControllerBase
    {
        // GET: api/<PropertyController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllProperties")]
        public async Task<IActionResult> Get([FromServices] IPropertyService propertyService, [FromQuery] Paging paging)
        {
            return Ok(await propertyService.GetAll(paging));
        }

        // GET api/<PropertyController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPropertyByID")]
        public async Task<IActionResult> GetPropertyByID([FromServices] IPropertyService propertyService, Guid id)
        {
            var result = await propertyService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Property with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<PropertyController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateProperty")]
        public async Task<IActionResult> CreateProperty([FromServices] IPropertyService propertyService, [FromBody] PropertyDto propertyDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await propertyService.CreateProperty(propertyDto);
            if (result.PropertyID != null && result.PropertyID != Guid.Empty)
                return CreatedAtRoute("GetPropertyByID", new { id = result.PropertyID }, result);
            return BadRequest(new { message = "Failed to create property. A property with the same name may already exist." });
        }

        // PUT api/<PropertyController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateProperty")]
        public async Task<IActionResult> UpdateProperty([FromServices] IPropertyService propertyService, Guid id, [FromBody] PropertyDto propertyDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await propertyService.UpdateProperty(id, propertyDto);
            if (result == null)
                return NotFound(new { message = "Property with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<PropertyController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteProperty")]
        public async Task<IActionResult> DeleteProperty([FromServices] IPropertyService propertyService, Guid id)
        {
            try
            {
                await propertyService.DeleteProperty(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Property with the specified ID was not found." });
            }
        }

        // POST: api/property/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdatePropertyStatus")]
        public async Task<ActionResult> UpdatePropertyStatus([FromServices] IPropertyService propertyService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var @property = await propertyService.UpdatePropertyStatus(id, status);
            if (@property == null)
                return NotFound($"Property with ID {id} not found.");
            return Ok(@property);
        }
    }
}