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
    public class LedgerAccountController : ControllerBase
    {
        // GET: api/<LedgerAccountController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLedgerAccounts")]
        public async Task<IActionResult> Get([FromServices] ILedgerAccountService ledgeraccountService, [FromQuery] Paging paging)
        {
            return Ok(await ledgeraccountService.GetAll(paging));
        }

        // GET api/<LedgerAccountController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLedgerAccountByID")]
        public async Task<IActionResult> GetLedgerAccountByID([FromServices] ILedgerAccountService ledgeraccountService, Guid id)
        {
            var result = await ledgeraccountService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LedgerAccount with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LedgerAccountController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLedgerAccount")]
        public async Task<IActionResult> CreateLedgerAccount([FromServices] ILedgerAccountService ledgeraccountService, [FromBody] LedgerAccountDto ledgeraccountDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgeraccountService.CreateLedgerAccount(ledgeraccountDto);
            if (result.LedgerAccountID != null && result.LedgerAccountID != Guid.Empty)
                return CreatedAtRoute("GetLedgerAccountByID", new { id = result.LedgerAccountID }, result);
            return BadRequest(new { message = "Failed to create ledgeraccount. A ledgeraccount with the same name may already exist." });
        }

        // PUT api/<LedgerAccountController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLedgerAccount")]
        public async Task<IActionResult> UpdateLedgerAccount([FromServices] ILedgerAccountService ledgeraccountService, Guid id, [FromBody] LedgerAccountDto ledgeraccountDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgeraccountService.UpdateLedgerAccount(id, ledgeraccountDto);
            if (result == null)
                return NotFound(new { message = "LedgerAccount with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LedgerAccountController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLedgerAccount")]
        public async Task<IActionResult> DeleteLedgerAccount([FromServices] ILedgerAccountService ledgeraccountService, Guid id)
        {
            try
            {
                await ledgeraccountService.DeleteLedgerAccount(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LedgerAccount with the specified ID was not found." });
            }
        }
    }
}