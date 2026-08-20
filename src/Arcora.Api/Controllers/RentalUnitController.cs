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
    public class RentalUnitController : ControllerBase
    {
        // GET: api/<RentalUnitController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllRentalUnits")]
        public async Task<IActionResult> Get([FromServices] IRentalUnitService rentalunitService, [FromQuery] Paging paging)
        {
            return Ok(await rentalunitService.GetAll(paging));
        }

        // GET api/<RentalUnitController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRentalUnitByID")]
        public async Task<IActionResult> GetRentalUnitByID([FromServices] IRentalUnitService rentalunitService, Guid id)
        {
            var result = await rentalunitService.GetID(id);
            if (result == null)
                return NotFound(new { message = "RentalUnit with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<RentalUnitController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateRentalUnit")]
        public async Task<IActionResult> CreateRentalUnit([FromServices] IRentalUnitService rentalunitService, [FromBody] RentalUnitDto rentalunitDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await rentalunitService.CreateRentalUnit(rentalunitDto);
            if (result.RentalUnitID != null && result.RentalUnitID != Guid.Empty)
                return CreatedAtRoute("GetRentalUnitByID", new { id = result.RentalUnitID }, result);
            return BadRequest(new { message = "Failed to create rentalunit. A rentalunit with the same name may already exist." });
        }

        // PUT api/<RentalUnitController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateRentalUnit")]
        public async Task<IActionResult> UpdateRentalUnit([FromServices] IRentalUnitService rentalunitService, Guid id, [FromBody] RentalUnitDto rentalunitDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await rentalunitService.UpdateRentalUnit(id, rentalunitDto);
            if (result == null)
                return NotFound(new { message = "RentalUnit with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<RentalUnitController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteRentalUnit")]
        public async Task<IActionResult> DeleteRentalUnit([FromServices] IRentalUnitService rentalunitService, Guid id)
        {
            try
            {
                await rentalunitService.DeleteRentalUnit(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "RentalUnit with the specified ID was not found." });
            }
        }

        // POST: api/rentalunit/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateRentalUnitStatus")]
        public async Task<ActionResult> UpdateRentalUnitStatus([FromServices] IRentalUnitService rentalunitService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var rentalunit = await rentalunitService.UpdateRentalUnitStatus(id, status);
            if (rentalunit == null)
                return NotFound($"RentalUnit with ID {id} not found.");
            return Ok(rentalunit);
        }
    }
}