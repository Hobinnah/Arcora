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
    public class AutopayConsentAuditController : ControllerBase
    {
        // GET: api/<AutopayConsentAuditController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAutopayConsentAudits")]
        public async Task<IActionResult> Get([FromServices] IAutopayConsentAuditService autopayconsentauditService, [FromQuery] Paging paging)
        {
            return Ok(await autopayconsentauditService.GetAll(paging));
        }

        // GET api/<AutopayConsentAuditController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAutopayConsentAuditByID")]
        public async Task<IActionResult> GetAutopayConsentAuditByID([FromServices] IAutopayConsentAuditService autopayconsentauditService, Guid id)
        {
            var result = await autopayconsentauditService.GetID(id);
            if (result == null)
                return NotFound(new { message = "AutopayConsentAudit with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<AutopayConsentAuditController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAutopayConsentAudit")]
        public async Task<IActionResult> CreateAutopayConsentAudit([FromServices] IAutopayConsentAuditService autopayconsentauditService, [FromBody] AutopayConsentAuditDto autopayconsentauditDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await autopayconsentauditService.CreateAutopayConsentAudit(autopayconsentauditDto);
            if (result.AutopayConsentAuditID != null && result.AutopayConsentAuditID != Guid.Empty)
                return CreatedAtRoute("GetAutopayConsentAuditByID", new { id = result.AutopayConsentAuditID }, result);
            return BadRequest(new { message = "Failed to create autopayconsentaudit. A autopayconsentaudit with the same name may already exist." });
        }

        // PUT api/<AutopayConsentAuditController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAutopayConsentAudit")]
        public async Task<IActionResult> UpdateAutopayConsentAudit([FromServices] IAutopayConsentAuditService autopayconsentauditService, Guid id, [FromBody] AutopayConsentAuditDto autopayconsentauditDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await autopayconsentauditService.UpdateAutopayConsentAudit(id, autopayconsentauditDto);
            if (result == null)
                return NotFound(new { message = "AutopayConsentAudit with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AutopayConsentAuditController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAutopayConsentAudit")]
        public async Task<IActionResult> DeleteAutopayConsentAudit([FromServices] IAutopayConsentAuditService autopayconsentauditService, Guid id)
        {
            try
            {
                await autopayconsentauditService.DeleteAutopayConsentAudit(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "AutopayConsentAudit with the specified ID was not found." });
            }
        }
    }
}