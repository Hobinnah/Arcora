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
    public class InspectionController : ControllerBase
    {
        // GET: api/<InspectionController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllInspections")]
        public async Task<IActionResult> Get([FromServices] IInspectionService inspectionService, [FromQuery] Paging paging)
        {
            return Ok(await inspectionService.GetAll(paging));
        }

        // GET api/<InspectionController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetInspectionByID")]
        public async Task<IActionResult> GetInspectionByID([FromServices] IInspectionService inspectionService, Guid id)
        {
            var result = await inspectionService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Inspection with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<InspectionController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateInspection")]
        public async Task<IActionResult> CreateInspection([FromServices] IInspectionService inspectionService, [FromBody] InspectionDto inspectionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await inspectionService.CreateInspection(inspectionDto);
            if (result.InspectionID != null && result.InspectionID != Guid.Empty)
                return CreatedAtRoute("GetInspectionByID", new { id = result.InspectionID }, result);
            return BadRequest(new { message = "Failed to create inspection. A inspection with the same name may already exist." });
        }

        // PUT api/<InspectionController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateInspection")]
        public async Task<IActionResult> UpdateInspection([FromServices] IInspectionService inspectionService, Guid id, [FromBody] InspectionDto inspectionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await inspectionService.UpdateInspection(id, inspectionDto);
            if (result == null)
                return NotFound(new { message = "Inspection with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<InspectionController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteInspection")]
        public async Task<IActionResult> DeleteInspection([FromServices] IInspectionService inspectionService, Guid id)
        {
            try
            {
                await inspectionService.DeleteInspection(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Inspection with the specified ID was not found." });
            }
        }

        // POST: api/inspection/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateInspectionStatus")]
        public async Task<ActionResult> UpdateInspectionStatus([FromServices] IInspectionService inspectionService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var inspection = await inspectionService.UpdateInspectionStatus(id, status);
            if (inspection == null)
                return NotFound($"Inspection with ID {id} not found.");
            return Ok(inspection);
        }
    }
}