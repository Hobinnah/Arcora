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
    public class InspectionItemController : ControllerBase
    {
        // GET: api/<InspectionItemController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllInspectionItems")]
        public async Task<IActionResult> Get([FromServices] IInspectionItemService inspectionitemService, [FromQuery] Paging paging)
        {
            return Ok(await inspectionitemService.GetAll(paging));
        }

        // GET api/<InspectionItemController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetInspectionItemByID")]
        public async Task<IActionResult> GetInspectionItemByID([FromServices] IInspectionItemService inspectionitemService, Guid id)
        {
            var result = await inspectionitemService.GetID(id);
            if (result == null)
                return NotFound(new { message = "InspectionItem with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<InspectionItemController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateInspectionItem")]
        public async Task<IActionResult> CreateInspectionItem([FromServices] IInspectionItemService inspectionitemService, [FromBody] InspectionItemDto inspectionitemDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await inspectionitemService.CreateInspectionItem(inspectionitemDto);
            if (result.InspectionItemID != null && result.InspectionItemID != Guid.Empty)
                return CreatedAtRoute("GetInspectionItemByID", new { id = result.InspectionItemID }, result);
            return BadRequest(new { message = "Failed to create inspectionitem. A inspectionitem with the same name may already exist." });
        }

        // PUT api/<InspectionItemController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateInspectionItem")]
        public async Task<IActionResult> UpdateInspectionItem([FromServices] IInspectionItemService inspectionitemService, Guid id, [FromBody] InspectionItemDto inspectionitemDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await inspectionitemService.UpdateInspectionItem(id, inspectionitemDto);
            if (result == null)
                return NotFound(new { message = "InspectionItem with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<InspectionItemController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteInspectionItem")]
        public async Task<IActionResult> DeleteInspectionItem([FromServices] IInspectionItemService inspectionitemService, Guid id)
        {
            try
            {
                await inspectionitemService.DeleteInspectionItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "InspectionItem with the specified ID was not found." });
            }
        }
    }
}