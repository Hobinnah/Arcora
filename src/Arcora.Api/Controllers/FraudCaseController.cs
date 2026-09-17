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
    public class FraudCaseController : ControllerBase
    {
        // GET: api/<FraudCaseController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllFraudCases")]
        public async Task<IActionResult> Get([FromServices] IFraudCaseService fraudcaseService, [FromQuery] Paging paging)
        {
            return Ok(await fraudcaseService.GetAll(paging));
        }

        // GET api/<FraudCaseController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetFraudCaseByID")]
        public async Task<IActionResult> GetFraudCaseByID([FromServices] IFraudCaseService fraudcaseService, Guid id)
        {
            var result = await fraudcaseService.GetID(id);
            if (result == null)
                return NotFound(new { message = "FraudCase with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<FraudCaseController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateFraudCase")]
        public async Task<IActionResult> CreateFraudCase([FromServices] IFraudCaseService fraudcaseService, [FromBody] FraudCaseDto fraudcaseDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await fraudcaseService.CreateFraudCase(fraudcaseDto);
            if (result.FraudCaseID != null && result.FraudCaseID != Guid.Empty)
                return CreatedAtRoute("GetFraudCaseByID", new { id = result.FraudCaseID }, result);
            return BadRequest(new { message = "Failed to create fraudcase. A fraudcase with the same name may already exist." });
        }

        // PUT api/<FraudCaseController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateFraudCase")]
        public async Task<IActionResult> UpdateFraudCase([FromServices] IFraudCaseService fraudcaseService, Guid id, [FromBody] FraudCaseDto fraudcaseDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await fraudcaseService.UpdateFraudCase(id, fraudcaseDto);
            if (result == null)
                return NotFound(new { message = "FraudCase with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<FraudCaseController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteFraudCase")]
        public async Task<IActionResult> DeleteFraudCase([FromServices] IFraudCaseService fraudcaseService, Guid id)
        {
            try
            {
                await fraudcaseService.DeleteFraudCase(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "FraudCase with the specified ID was not found." });
            }
        }

        // POST: api/fraudcase/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateFraudCaseStatus")]
        public async Task<ActionResult> UpdateFraudCaseStatus([FromServices] IFraudCaseService fraudcaseService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var fraudcase = await fraudcaseService.UpdateFraudCaseStatus(id, status);
            if (fraudcase == null)
                return NotFound($"FraudCase with ID {id} not found.");
            return Ok(fraudcase);
        }
    }
}