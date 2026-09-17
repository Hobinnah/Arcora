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
    public class DisputeController : ControllerBase
    {
        // GET: api/<DisputeController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllDisputes")]
        public async Task<IActionResult> Get([FromServices] IDisputeService disputeService, [FromQuery] Paging paging)
        {
            return Ok(await disputeService.GetAll(paging));
        }

        // GET api/<DisputeController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetDisputeByID")]
        public async Task<IActionResult> GetDisputeByID([FromServices] IDisputeService disputeService, Guid id)
        {
            var result = await disputeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Dispute with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<DisputeController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateDispute")]
        public async Task<IActionResult> CreateDispute([FromServices] IDisputeService disputeService, [FromBody] DisputeDto disputeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await disputeService.CreateDispute(disputeDto);
            if (result.DisputeID != null && result.DisputeID != Guid.Empty)
                return CreatedAtRoute("GetDisputeByID", new { id = result.DisputeID }, result);
            return BadRequest(new { message = "Failed to create dispute. A dispute with the same name may already exist." });
        }

        // PUT api/<DisputeController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateDispute")]
        public async Task<IActionResult> UpdateDispute([FromServices] IDisputeService disputeService, Guid id, [FromBody] DisputeDto disputeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await disputeService.UpdateDispute(id, disputeDto);
            if (result == null)
                return NotFound(new { message = "Dispute with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<DisputeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteDispute")]
        public async Task<IActionResult> DeleteDispute([FromServices] IDisputeService disputeService, Guid id)
        {
            try
            {
                await disputeService.DeleteDispute(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Dispute with the specified ID was not found." });
            }
        }

        // POST: api/dispute/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateDisputeStatus")]
        public async Task<ActionResult> UpdateDisputeStatus([FromServices] IDisputeService disputeService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var dispute = await disputeService.UpdateDisputeStatus(id, status);
            if (dispute == null)
                return NotFound($"Dispute with ID {id} not found.");
            return Ok(dispute);
        }
    }
}