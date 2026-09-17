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
    public class FeeTypeController : ControllerBase
    {
        // GET: api/<FeeTypeController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllFeeTypes")]
        public async Task<IActionResult> Get([FromServices] IFeeTypeService feetypeService, [FromQuery] Paging paging)
        {
            return Ok(await feetypeService.GetAll(paging));
        }

        // GET api/<FeeTypeController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetFeeTypeByID")]
        public async Task<IActionResult> GetFeeTypeByID([FromServices] IFeeTypeService feetypeService, int id)
        {
            var result = await feetypeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "FeeType with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<FeeTypeController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateFeeType")]
        public async Task<IActionResult> CreateFeeType([FromServices] IFeeTypeService feetypeService, [FromBody] FeeTypeDto feetypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await feetypeService.CreateFeeType(feetypeDto);
            if (result != null && result.FeeTypeID != 0)
                return CreatedAtRoute("GetFeeTypeByID", new { id = result.FeeTypeID }, result);
            return BadRequest(new { message = "Failed to create feetype. A feetype with the same name may already exist." });
        }

        // PUT api/<FeeTypeController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateFeeType")]
        public async Task<IActionResult> UpdateFeeType([FromServices] IFeeTypeService feetypeService, int id, [FromBody] FeeTypeDto feetypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await feetypeService.UpdateFeeType(id, feetypeDto);
            if (result == null)
                return NotFound(new { message = "FeeType with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<FeeTypeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteFeeType")]
        public async Task<IActionResult> DeleteFeeType([FromServices] IFeeTypeService feetypeService, int id)
        {
            try
            {
                await feetypeService.DeleteFeeType(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "FeeType with the specified ID was not found." });
            }
        }
    }
}