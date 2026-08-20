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
    public class FeeController : ControllerBase
    {
        // GET: api/<FeeController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllFees")]
        public async Task<IActionResult> Get([FromServices] IFeeService feeService, [FromQuery] Paging paging)
        {
            return Ok(await feeService.GetAll(paging));
        }

        // GET api/<FeeController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetFeeByID")]
        public async Task<IActionResult> GetFeeByID([FromServices] IFeeService feeService, Guid id)
        {
            var result = await feeService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Fee with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<FeeController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateFee")]
        public async Task<IActionResult> CreateFee([FromServices] IFeeService feeService, [FromBody] FeeDto feeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await feeService.CreateFee(feeDto);
            if (result.FeeID != null && result.FeeID != Guid.Empty)
                return CreatedAtRoute("GetFeeByID", new { id = result.FeeID }, result);
            return BadRequest(new { message = "Failed to create fee. A fee with the same name may already exist." });
        }

        // PUT api/<FeeController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateFee")]
        public async Task<IActionResult> UpdateFee([FromServices] IFeeService feeService, Guid id, [FromBody] FeeDto feeDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await feeService.UpdateFee(id, feeDto);
            if (result == null)
                return NotFound(new { message = "Fee with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<FeeController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteFee")]
        public async Task<IActionResult> DeleteFee([FromServices] IFeeService feeService, Guid id)
        {
            try
            {
                await feeService.DeleteFee(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Fee with the specified ID was not found." });
            }
        }
    }
}