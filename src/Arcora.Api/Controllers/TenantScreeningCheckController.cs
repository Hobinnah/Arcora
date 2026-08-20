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
    public class TenantScreeningCheckController : ControllerBase
    {
        // GET: api/<TenantScreeningCheckController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTenantScreeningChecks")]
        public async Task<IActionResult> Get([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, [FromQuery] Paging paging)
        {
            return Ok(await tenantscreeningcheckService.GetAll(paging));
        }

        // GET api/<TenantScreeningCheckController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTenantScreeningCheckByID")]
        public async Task<IActionResult> GetTenantScreeningCheckByID([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, Guid id)
        {
            var result = await tenantscreeningcheckService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TenantScreeningCheck with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TenantScreeningCheckController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTenantScreeningCheck")]
        public async Task<IActionResult> CreateTenantScreeningCheck([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, [FromBody] TenantScreeningCheckDto tenantscreeningcheckDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantscreeningcheckService.CreateTenantScreeningCheck(tenantscreeningcheckDto);
            if (result.TenantScreeningCheckID != null && result.TenantScreeningCheckID != Guid.Empty)
                return CreatedAtRoute("GetTenantScreeningCheckByID", new { id = result.TenantScreeningCheckID }, result);
            return BadRequest(new { message = "Failed to create tenantscreeningcheck. A tenantscreeningcheck with the same name may already exist." });
        }

        // PUT api/<TenantScreeningCheckController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTenantScreeningCheck")]
        public async Task<IActionResult> UpdateTenantScreeningCheck([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, Guid id, [FromBody] TenantScreeningCheckDto tenantscreeningcheckDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await tenantscreeningcheckService.UpdateTenantScreeningCheck(id, tenantscreeningcheckDto);
            if (result == null)
                return NotFound(new { message = "TenantScreeningCheck with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TenantScreeningCheckController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTenantScreeningCheck")]
        public async Task<IActionResult> DeleteTenantScreeningCheck([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, Guid id)
        {
            try
            {
                await tenantscreeningcheckService.DeleteTenantScreeningCheck(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TenantScreeningCheck with the specified ID was not found." });
            }
        }

        // POST: api/tenantscreeningcheck/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateTenantScreeningCheckStatus")]
        public async Task<ActionResult> UpdateTenantScreeningCheckStatus([FromServices] ITenantScreeningCheckService tenantscreeningcheckService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var tenantscreeningcheck = await tenantscreeningcheckService.UpdateTenantScreeningCheckStatus(id, status);
            if (tenantscreeningcheck == null)
                return NotFound($"TenantScreeningCheck with ID {id} not found.");
            return Ok(tenantscreeningcheck);
        }
    }
}