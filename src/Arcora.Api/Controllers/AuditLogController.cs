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
    public class AuditLogController : ControllerBase
    {
        // GET: api/<AuditLogController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAuditLogs")]
        public async Task<IActionResult> Get([FromServices] IAuditLogService auditlogService, [FromQuery] Paging paging)
        {
            return Ok(await auditlogService.GetAll(paging));
        }

        // GET api/<AuditLogController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAuditLogByID")]
        public async Task<IActionResult> GetAuditLogByID([FromServices] IAuditLogService auditlogService, Guid id)
        {
            var result = await auditlogService.GetID(id);
            if (result == null)
                return NotFound(new { message = "AuditLog with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<AuditLogController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAuditLog")]
        public async Task<IActionResult> CreateAuditLog([FromServices] IAuditLogService auditlogService, [FromBody] AuditLogDto auditlogDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await auditlogService.CreateAuditLog(auditlogDto);
            if (result.AuditLogID != null && result.AuditLogID != Guid.Empty)
                return CreatedAtRoute("GetAuditLogByID", new { id = result.AuditLogID }, result);
            return BadRequest(new { message = "Failed to create auditlog. A auditlog with the same name may already exist." });
        }

        // PUT api/<AuditLogController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAuditLog")]
        public async Task<IActionResult> UpdateAuditLog([FromServices] IAuditLogService auditlogService, Guid id, [FromBody] AuditLogDto auditlogDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await auditlogService.UpdateAuditLog(id, auditlogDto);
            if (result == null)
                return NotFound(new { message = "AuditLog with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AuditLogController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAuditLog")]
        public async Task<IActionResult> DeleteAuditLog([FromServices] IAuditLogService auditlogService, Guid id)
        {
            try
            {
                await auditlogService.DeleteAuditLog(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "AuditLog with the specified ID was not found." });
            }
        }
    }
}