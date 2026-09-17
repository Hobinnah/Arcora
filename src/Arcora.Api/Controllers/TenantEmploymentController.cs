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
    public class TenantEmploymentController : ControllerBase
    {
        // GET: api/<TenantEmploymentController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantEmployments")]
        public async Task<IActionResult> Get([FromServices] ITenantEmploymentService tenantemploymentService, [FromQuery] Paging paging)
        {
            return Ok(await tenantemploymentService.GetAll(paging));
        }

        // GET api/<TenantEmploymentController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantEmploymentByID")]
        public async Task<IActionResult> GetTenantEmploymentByID([FromServices] ITenantEmploymentService tenantemploymentService, Guid id)
        {
            var result = await tenantemploymentService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantEmployment with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantEmploymentController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantEmployment")]
        public async Task<IActionResult> CreateTenantEmployment([FromServices] ITenantEmploymentService tenantemploymentService, [FromBody] TenantEmploymentDto tenantemploymentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantemploymentService.CreateTenantEmployment(tenantemploymentDto);
            if (result.TenantEmploymentID != null && result.TenantEmploymentID != Guid.Empty)
                return CreatedAtRoute("GetTenantEmploymentByID", new { id = result.TenantEmploymentID }, result);
            return BadRequest(new { message = "Failed to create tenantemployment. A tenantemployment with the same name may already exist." });
        }

        // PUT api/<TenantEmploymentController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantEmployment")]
        public async Task<IActionResult> UpdateTenantEmployment([FromServices] ITenantEmploymentService tenantemploymentService, Guid id, [FromBody] TenantEmploymentDto tenantemploymentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantemploymentService.UpdateTenantEmployment(id, tenantemploymentDto);
            if (result == null)
                return NotFound(new { message = "TenantEmployment with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantEmploymentController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantEmployment")]
        public async Task<IActionResult> DeleteTenantEmployment([FromServices] ITenantEmploymentService tenantemploymentService, Guid id)
        {
            try
            {
                await tenantemploymentService.DeleteTenantEmployment(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantEmployment with the specified ID was not found." });
            }
        }
    }
}