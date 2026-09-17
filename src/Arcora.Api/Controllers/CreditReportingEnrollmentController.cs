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
    public class CreditReportingEnrollmentController : ControllerBase
    {
        // GET: api/<CreditReportingEnrollmentController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCreditReportingEnrollments")]
        public async Task<IActionResult> Get([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, [FromQuery] Paging paging)
        {
            return Ok(await creditreportingenrollmentService.GetAll(paging));
        }

        // GET api/<CreditReportingEnrollmentController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetCreditReportingEnrollmentByID")]
        public async Task<IActionResult> GetCreditReportingEnrollmentByID([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, Guid id)
        {
            var result = await creditreportingenrollmentService.GetID(id);
            if (result == null)
                return NotFound(new { message = "CreditReportingEnrollment with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<CreditReportingEnrollmentController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateCreditReportingEnrollment")]
        public async Task<IActionResult> CreateCreditReportingEnrollment([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, [FromBody] CreditReportingEnrollmentDto creditreportingenrollmentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingenrollmentService.CreateCreditReportingEnrollment(creditreportingenrollmentDto);
            if (result.CreditReportingEnrollmentID != null && result.CreditReportingEnrollmentID != Guid.Empty)
                return CreatedAtRoute("GetCreditReportingEnrollmentByID", new { id = result.CreditReportingEnrollmentID }, result);
            return BadRequest(new { message = "Failed to create creditreportingenrollment. A creditreportingenrollment with the same name may already exist." });
        }

        // PUT api/<CreditReportingEnrollmentController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateCreditReportingEnrollment")]
        public async Task<IActionResult> UpdateCreditReportingEnrollment([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, Guid id, [FromBody] CreditReportingEnrollmentDto creditreportingenrollmentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await creditreportingenrollmentService.UpdateCreditReportingEnrollment(id, creditreportingenrollmentDto);
            if (result == null)
                return NotFound(new { message = "CreditReportingEnrollment with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<CreditReportingEnrollmentController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteCreditReportingEnrollment")]
        public async Task<IActionResult> DeleteCreditReportingEnrollment([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, Guid id)
        {
            try
            {
                await creditreportingenrollmentService.DeleteCreditReportingEnrollment(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "CreditReportingEnrollment with the specified ID was not found." });
            }
        }

        // POST: api/creditreportingenrollment/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateCreditReportingEnrollmentStatus")]
        public async Task<ActionResult> UpdateCreditReportingEnrollmentStatus([FromServices] ICreditReportingEnrollmentService creditreportingenrollmentService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var creditreportingenrollment = await creditreportingenrollmentService.UpdateCreditReportingEnrollmentStatus(id, status);
            if (creditreportingenrollment == null)
                return NotFound($"CreditReportingEnrollment with ID {id} not found.");
            return Ok(creditreportingenrollment);
        }
    }
}