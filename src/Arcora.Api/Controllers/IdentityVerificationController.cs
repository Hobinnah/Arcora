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
    public class IdentityVerificationController : ControllerBase
    {
        // GET: api/<IdentityVerificationController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllIdentityVerifications")]
        public async Task<IActionResult> Get([FromServices] IIdentityVerificationService identityverificationService, [FromQuery] Paging paging)
        {
            return Ok(await identityverificationService.GetAll(paging));
        }

        // GET api/<IdentityVerificationController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetIdentityVerificationByID")]
        public async Task<IActionResult> GetIdentityVerificationByID([FromServices] IIdentityVerificationService identityverificationService, Guid id)
        {
            var result = await identityverificationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<IdentityVerificationController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateIdentityVerification")]
        public async Task<IActionResult> CreateIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, [FromBody] IdentityVerificationDto identityverificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await identityverificationService.CreateIdentityVerification(identityverificationDto);
            if (result.IdentityVerificationID != null && result.IdentityVerificationID != Guid.Empty)
                return CreatedAtRoute("GetIdentityVerificationByID", new { id = result.IdentityVerificationID }, result);
            return BadRequest(new { message = "Failed to create identityverification. A identityverification with the same name may already exist." });
        }

        // PUT api/<IdentityVerificationController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateIdentityVerification")]
        public async Task<IActionResult> UpdateIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, Guid id, [FromBody] IdentityVerificationDto identityverificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await identityverificationService.UpdateIdentityVerification(id, identityverificationDto);
            if (result == null)
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<IdentityVerificationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteIdentityVerification")]
        public async Task<IActionResult> DeleteIdentityVerification([FromServices] IIdentityVerificationService identityverificationService, Guid id)
        {
            try
            {
                await identityverificationService.DeleteIdentityVerification(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "IdentityVerification with the specified ID was not found." });
            }
        }

        // POST: api/identityverification/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateIdentityVerificationStatus")]
        public async Task<ActionResult> UpdateIdentityVerificationStatus([FromServices] IIdentityVerificationService identityverificationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var identityverification = await identityverificationService.UpdateIdentityVerificationStatus(id, status);
            if (identityverification == null)
                return NotFound($"IdentityVerification with ID {id} not found.");
            return Ok(identityverification);
        }
    }
}