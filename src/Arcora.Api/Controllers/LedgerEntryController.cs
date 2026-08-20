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
    public class LedgerEntryController : ControllerBase
    {
        // GET: api/<LedgerEntryController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLedgerEntries")]
        public async Task<IActionResult> Get([FromServices] ILedgerEntryService ledgerentryService, [FromQuery] Paging paging)
        {
            return Ok(await ledgerentryService.GetAll(paging));
        }

        // GET api/<LedgerEntryController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLedgerEntryByID")]
        public async Task<IActionResult> GetLedgerEntryByID([FromServices] ILedgerEntryService ledgerentryService, Guid id)
        {
            var result = await ledgerentryService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LedgerEntry with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LedgerEntryController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLedgerEntry")]
        public async Task<IActionResult> CreateLedgerEntry([FromServices] ILedgerEntryService ledgerentryService, [FromBody] LedgerEntryDto ledgerentryDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgerentryService.CreateLedgerEntry(ledgerentryDto);
            if (result.LedgerEntryID != null && result.LedgerEntryID != Guid.Empty)
                return CreatedAtRoute("GetLedgerEntryByID", new { id = result.LedgerEntryID }, result);
            return BadRequest(new { message = "Failed to create ledgerentry. A ledgerentry with the same name may already exist." });
        }

        // PUT api/<LedgerEntryController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLedgerEntry")]
        public async Task<IActionResult> UpdateLedgerEntry([FromServices] ILedgerEntryService ledgerentryService, Guid id, [FromBody] LedgerEntryDto ledgerentryDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ledgerentryService.UpdateLedgerEntry(id, ledgerentryDto);
            if (result == null)
                return NotFound(new { message = "LedgerEntry with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LedgerEntryController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLedgerEntry")]
        public async Task<IActionResult> DeleteLedgerEntry([FromServices] ILedgerEntryService ledgerentryService, Guid id)
        {
            try
            {
                await ledgerentryService.DeleteLedgerEntry(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LedgerEntry with the specified ID was not found." });
            }
        }
    }
}