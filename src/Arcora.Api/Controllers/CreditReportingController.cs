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
    public class CreditReportingController : ControllerBase
    {
        // GET: api/<CreditReportingController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCreditReportings")]
        public async Task<IActionResult> Get([FromServices] ICreditReportingService creditreportingService, [FromQuery] Paging paging)
        {
            return Ok(await creditreportingService.GetAll(paging));
        }

        // GET api/<CreditReportingController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetCreditReportingByID")]
        public async Task<IActionResult> GetCreditReportingByID([FromServices] ICreditReportingService creditreportingService, Guid id)
        {
            var result = await creditreportingService.GetID(id);
            if (result == null)
                return NotFound(new { message = "CreditReporting with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<CreditReportingController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateCreditReporting")]
        public async Task<IActionResult> CreateCreditReporting([FromServices] ICreditReportingService creditreportingService, [FromBody] CreditReportingDto creditreportingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingService.CreateCreditReporting(creditreportingDto);
            if (result.CreditReportingID != null && result.CreditReportingID != Guid.Empty)
                return CreatedAtRoute("GetCreditReportingByID", new { id = result.CreditReportingID }, result);
            return BadRequest(new { message = "Failed to create creditreporting. A creditreporting with the same name may already exist." });
        }

        // PUT api/<CreditReportingController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateCreditReporting")]
        public async Task<IActionResult> UpdateCreditReporting([FromServices] ICreditReportingService creditreportingService, Guid id, [FromBody] CreditReportingDto creditreportingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingService.UpdateCreditReporting(id, creditreportingDto);
            if (result == null)
                return NotFound(new { message = "CreditReporting with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<CreditReportingController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteCreditReporting")]
        public async Task<IActionResult> DeleteCreditReporting([FromServices] ICreditReportingService creditreportingService, Guid id)
        {
            try
            {
                await creditreportingService.DeleteCreditReporting(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "CreditReporting with the specified ID was not found." });
            }
        }
    }
}