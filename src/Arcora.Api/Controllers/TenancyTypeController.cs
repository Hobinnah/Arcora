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
    public class TenancyTypeController : ControllerBase
    {
        // GET: api/<TenancyTypeController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenancyTypes")]
        public async Task<IActionResult> Get([FromServices] ITenancyTypeService tenancytypeService, [FromQuery] Paging paging)
        {
            return Ok(await tenancytypeService.GetAll(paging));
        }

        // GET api/<TenancyTypeController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenancyTypeByID")]
        public async Task<IActionResult> GetTenancyTypeByID([FromServices] ITenancyTypeService tenancytypeService, int id)
        {
            var result = await tenancytypeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenancyType with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenancyTypeController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenancyType")]
        public async Task<IActionResult> CreateTenancyType([FromServices] ITenancyTypeService tenancytypeService, [FromBody] TenancyTypeDto tenancytypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenancytypeService.CreateTenancyType(tenancytypeDto);
            if (result != null && result.TenancyTypeID != 0)
                return CreatedAtRoute("GetTenancyTypeByID", new { id = result.TenancyTypeID }, result);
            return BadRequest(new { message = "Failed to create tenancytype. A tenancytype with the same name may already exist." });
        }

        // PUT api/<TenancyTypeController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenancyType")]
        public async Task<IActionResult> UpdateTenancyType([FromServices] ITenancyTypeService tenancytypeService, int id, [FromBody] TenancyTypeDto tenancytypeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenancytypeService.UpdateTenancyType(id, tenancytypeDto);
            if (result == null)
                return NotFound(new { message = "TenancyType with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenancyTypeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenancyType")]
        public async Task<IActionResult> DeleteTenancyType([FromServices] ITenancyTypeService tenancytypeService, int id)
        {
            try
            {
                await tenancytypeService.DeleteTenancyType(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenancyType with the specified ID was not found." });
            }
        }
    }
}