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
    public class UnitTypeController : ControllerBase
    {
        // GET: api/<UnitTypeController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllUnitTypes")]
        public async Task<IActionResult> Get([FromServices] IUnitTypeService unittypeService, [FromQuery] Paging paging)
        {
            return Ok(await unittypeService.GetAll(paging));
        }

        // GET api/<UnitTypeController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetUnitTypeByID")]
        public async Task<IActionResult> GetUnitTypeByID([FromServices] IUnitTypeService unittypeService, Guid id)
        {
            var result = await unittypeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "UnitType with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<UnitTypeController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateUnitType")]
        public async Task<IActionResult> CreateUnitType([FromServices] IUnitTypeService unittypeService, [FromBody] UnitTypeDto unittypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await unittypeService.CreateUnitType(unittypeDto);
            if (result.UnitTypeID != null && result.UnitTypeID != Guid.Empty)
                return CreatedAtRoute("GetUnitTypeByID", new { id = result.UnitTypeID }, result);
            return BadRequest(new { message = "Failed to create unittype. A unittype with the same name may already exist." });
        }

        // PUT api/<UnitTypeController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateUnitType")]
        public async Task<IActionResult> UpdateUnitType([FromServices] IUnitTypeService unittypeService, Guid id, [FromBody] UnitTypeDto unittypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await unittypeService.UpdateUnitType(id, unittypeDto);
            if (result == null)
                return NotFound(new { message = "UnitType with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<UnitTypeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteUnitType")]
        public async Task<IActionResult> DeleteUnitType([FromServices] IUnitTypeService unittypeService, Guid id)
        {
            try
            {
                await unittypeService.DeleteUnitType(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "UnitType with the specified ID was not found." });
            }
        }
    }
}