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
    public class LedgerTransactionController : ControllerBase
    {
        // GET: api/<LedgerTransactionController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLedgerTransactions")]
        public async Task<IActionResult> Get([FromServices] ILedgerTransactionService ledgertransactionService, [FromQuery] Paging paging)
        {
            return Ok(await ledgertransactionService.GetAll(paging));
        }

        // GET api/<LedgerTransactionController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLedgerTransactionByID")]
        public async Task<IActionResult> GetLedgerTransactionByID([FromServices] ILedgerTransactionService ledgertransactionService, Guid id)
        {
            var result = await ledgertransactionService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LedgerTransaction with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LedgerTransactionController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLedgerTransaction")]
        public async Task<IActionResult> CreateLedgerTransaction([FromServices] ILedgerTransactionService ledgertransactionService, [FromBody] LedgerTransactionDto ledgertransactionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgertransactionService.CreateLedgerTransaction(ledgertransactionDto);
            if (result.LedgerTransactionID != null && result.LedgerTransactionID != Guid.Empty)
                return CreatedAtRoute("GetLedgerTransactionByID", new { id = result.LedgerTransactionID }, result);
            return BadRequest(new { message = "Failed to create ledgertransaction. A ledgertransaction with the same name may already exist." });
        }

        // PUT api/<LedgerTransactionController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLedgerTransaction")]
        public async Task<IActionResult> UpdateLedgerTransaction([FromServices] ILedgerTransactionService ledgertransactionService, Guid id, [FromBody] LedgerTransactionDto ledgertransactionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgertransactionService.UpdateLedgerTransaction(id, ledgertransactionDto);
            if (result == null)
                return NotFound(new { message = "LedgerTransaction with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LedgerTransactionController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLedgerTransaction")]
        public async Task<IActionResult> DeleteLedgerTransaction([FromServices] ILedgerTransactionService ledgertransactionService, Guid id)
        {
            try
            {
                await ledgertransactionService.DeleteLedgerTransaction(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LedgerTransaction with the specified ID was not found." });
            }
        }

        // POST: api/ledgertransaction/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateLedgerTransactionStatus")]
        public async Task<ActionResult> UpdateLedgerTransactionStatus([FromServices] ILedgerTransactionService ledgertransactionService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var ledgertransaction = await ledgertransactionService.UpdateLedgerTransactionStatus(id, status);
            if (ledgertransaction == null)
                return NotFound($"LedgerTransaction with ID {id} not found.");
            return Ok(ledgertransaction);
        }
    }
}