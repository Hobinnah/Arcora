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
    public class SecurityDepositController : ControllerBase
    {
        // GET: api/<SecurityDepositController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllSecurityDeposits")]
        public async Task<IActionResult> Get([FromServices] ISecurityDepositService securitydepositService, [FromQuery] Paging paging)
        {
            return Ok(await securitydepositService.GetAll(paging));
        }

        // GET api/<SecurityDepositController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetSecurityDepositByID")]
        public async Task<IActionResult> GetSecurityDepositByID([FromServices] ISecurityDepositService securitydepositService, Guid id)
        {
            var result = await securitydepositService.GetID(id);
            if (result == null)
                return NotFound(new { message = "SecurityDeposit with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<SecurityDepositController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateSecurityDeposit")]
        public async Task<IActionResult> CreateSecurityDeposit([FromServices] ISecurityDepositService securitydepositService, [FromBody] SecurityDepositDto securitydepositDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await securitydepositService.CreateSecurityDeposit(securitydepositDto);
            if (result.SecurityDepositID != null && result.SecurityDepositID != Guid.Empty)
                return CreatedAtRoute("GetSecurityDepositByID", new { id = result.SecurityDepositID }, result);
            return BadRequest(new { message = "Failed to create securitydeposit. A securitydeposit with the same name may already exist." });
        }

        // PUT api/<SecurityDepositController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateSecurityDeposit")]
        public async Task<IActionResult> UpdateSecurityDeposit([FromServices] ISecurityDepositService securitydepositService, Guid id, [FromBody] SecurityDepositDto securitydepositDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await securitydepositService.UpdateSecurityDeposit(id, securitydepositDto);
            if (result == null)
                return NotFound(new { message = "SecurityDeposit with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<SecurityDepositController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteSecurityDeposit")]
        public async Task<IActionResult> DeleteSecurityDeposit([FromServices] ISecurityDepositService securitydepositService, Guid id)
        {
            try
            {
                await securitydepositService.DeleteSecurityDeposit(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "SecurityDeposit with the specified ID was not found." });
            }
        }

        // POST: api/securitydeposit/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateSecurityDepositStatus")]
        public async Task<ActionResult> UpdateSecurityDepositStatus([FromServices] ISecurityDepositService securitydepositService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var securitydeposit = await securitydepositService.UpdateSecurityDepositStatus(id, status);
            if (securitydeposit == null)
                return NotFound($"SecurityDeposit with ID {id} not found.");
            return Ok(securitydeposit);
        }
    }
}