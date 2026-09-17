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
    public class CreditReportingConsentAuditController : ControllerBase
    {
        // GET: api/<CreditReportingConsentAuditController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCreditReportingConsentAudits")]
        public async Task<IActionResult> Get([FromServices] ICreditReportingConsentAuditService creditreportingconsentauditService, [FromQuery] Paging paging)
        {
            return Ok(await creditreportingconsentauditService.GetAll(paging));
        }

        // GET api/<CreditReportingConsentAuditController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetCreditReportingConsentAuditByID")]
        public async Task<IActionResult> GetCreditReportingConsentAuditByID([FromServices] ICreditReportingConsentAuditService creditreportingconsentauditService, Guid id)
        {
            var result = await creditreportingconsentauditService.GetID(id);
            if (result == null)
                return NotFound(new { message = "CreditReportingConsentAudit with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<CreditReportingConsentAuditController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateCreditReportingConsentAudit")]
        public async Task<IActionResult> CreateCreditReportingConsentAudit([FromServices] ICreditReportingConsentAuditService creditreportingconsentauditService, [FromBody] CreditReportingConsentAuditDto creditreportingconsentauditDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingconsentauditService.CreateCreditReportingConsentAudit(creditreportingconsentauditDto);
            if (result.ConsentAuditID != null && result.ConsentAuditID != Guid.Empty)
                return CreatedAtRoute("GetCreditReportingConsentAuditByID", new { id = result.ConsentAuditID }, result);
            return BadRequest(new { message = "Failed to create creditreportingconsentaudit. A creditreportingconsentaudit with the same name may already exist." });
        }

        // PUT api/<CreditReportingConsentAuditController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateCreditReportingConsentAudit")]
        public async Task<IActionResult> UpdateCreditReportingConsentAudit([FromServices] ICreditReportingConsentAuditService creditreportingconsentauditService, Guid id, [FromBody] CreditReportingConsentAuditDto creditreportingconsentauditDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingconsentauditService.UpdateCreditReportingConsentAudit(id, creditreportingconsentauditDto);
            if (result == null)
                return NotFound(new { message = "CreditReportingConsentAudit with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<CreditReportingConsentAuditController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteCreditReportingConsentAudit")]
        public async Task<IActionResult> DeleteCreditReportingConsentAudit([FromServices] ICreditReportingConsentAuditService creditreportingconsentauditService, Guid id)
        {
            try
            {
                await creditreportingconsentauditService.DeleteCreditReportingConsentAudit(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "CreditReportingConsentAudit with the specified ID was not found." });
            }
        }
    }
}