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
    public class ReceiptMasterController : ControllerBase
    {
        // GET: api/<ReceiptMasterController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllReceiptMasters")]
        public async Task<IActionResult> Get([FromServices] IReceiptMasterService receiptmasterService, [FromQuery] Paging paging)
        {
            return Ok(await receiptmasterService.GetAll(paging));
        }

        // GET api/<ReceiptMasterController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetReceiptMasterByID")]
        public async Task<IActionResult> GetReceiptMasterByID([FromServices] IReceiptMasterService receiptmasterService, long id)
        {
            var result = await receiptmasterService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ReceiptMaster with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ReceiptMasterController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateReceiptMaster")]
        public async Task<IActionResult> CreateReceiptMaster([FromServices] IReceiptMasterService receiptmasterService, [FromBody] ReceiptMasterDto receiptmasterDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await receiptmasterService.CreateReceiptMaster(receiptmasterDto);
            if (result != null && result.ReceiptMasterID != 0)
                return CreatedAtRoute("GetReceiptMasterByID", new { id = result.ReceiptMasterID }, result);
            return BadRequest(new { message = "Failed to create receiptmaster. A receiptmaster with the same name may already exist." });
        }

        // PUT api/<ReceiptMasterController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateReceiptMaster")]
        public async Task<IActionResult> UpdateReceiptMaster([FromServices] IReceiptMasterService receiptmasterService, long id, [FromBody] ReceiptMasterDto receiptmasterDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await receiptmasterService.UpdateReceiptMaster(id, receiptmasterDto);
            if (result == null)
                return NotFound(new { message = "ReceiptMaster with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ReceiptMasterController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteReceiptMaster")]
        public async Task<IActionResult> DeleteReceiptMaster([FromServices] IReceiptMasterService receiptmasterService, long id)
        {
            try
            {
                await receiptmasterService.DeleteReceiptMaster(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ReceiptMaster with the specified ID was not found." });
            }
        }
    }
}