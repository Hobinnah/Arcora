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
    public class AutopayMandateController : ControllerBase
    {
        // GET: api/<AutopayMandateController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAutopayMandates")]
        public async Task<IActionResult> Get([FromServices] IAutopayMandateService autopaymandateService, [FromQuery] Paging paging)
        {
            return Ok(await autopaymandateService.GetAll(paging));
        }

        // GET api/<AutopayMandateController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAutopayMandateByID")]
        public async Task<IActionResult> GetAutopayMandateByID([FromServices] IAutopayMandateService autopaymandateService, Guid id)
        {
            var result = await autopaymandateService.GetID(id);
            if (result == null)
                return NotFound(new { message = "AutopayMandate with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<AutopayMandateController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAutopayMandate")]
        public async Task<IActionResult> CreateAutopayMandate([FromServices] IAutopayMandateService autopaymandateService, [FromBody] AutopayMandateDto autopaymandateDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await autopaymandateService.CreateAutopayMandate(autopaymandateDto);
            if (result.AutopayMandateID != null && result.AutopayMandateID != Guid.Empty)
                return CreatedAtRoute("GetAutopayMandateByID", new { id = result.AutopayMandateID }, result);
            return BadRequest(new { message = "Failed to create autopaymandate. A autopaymandate with the same name may already exist." });
        }

        // PUT api/<AutopayMandateController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAutopayMandate")]
        public async Task<IActionResult> UpdateAutopayMandate([FromServices] IAutopayMandateService autopaymandateService, Guid id, [FromBody] AutopayMandateDto autopaymandateDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await autopaymandateService.UpdateAutopayMandate(id, autopaymandateDto);
            if (result == null)
                return NotFound(new { message = "AutopayMandate with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AutopayMandateController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAutopayMandate")]
        public async Task<IActionResult> DeleteAutopayMandate([FromServices] IAutopayMandateService autopaymandateService, Guid id)
        {
            try
            {
                await autopaymandateService.DeleteAutopayMandate(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "AutopayMandate with the specified ID was not found." });
            }
        }

        // POST: api/autopaymandate/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateAutopayMandateStatus")]
        public async Task<ActionResult> UpdateAutopayMandateStatus([FromServices] IAutopayMandateService autopaymandateService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var autopaymandate = await autopaymandateService.UpdateAutopayMandateStatus(id, status);
            if (autopaymandate == null)
                return NotFound($"AutopayMandate with ID {id} not found.");
            return Ok(autopaymandate);
        }
    }
}