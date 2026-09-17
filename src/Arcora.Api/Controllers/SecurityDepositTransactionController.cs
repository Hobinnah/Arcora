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
    public class SecurityDepositTransactionController : ControllerBase
    {
        // GET: api/<SecurityDepositTransactionController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllSecurityDepositTransactions")]
        public async Task<IActionResult> Get([FromServices] ISecurityDepositTransactionService securitydeposittransactionService, [FromQuery] Paging paging)
        {
            return Ok(await securitydeposittransactionService.GetAll(paging));
        }

        // GET api/<SecurityDepositTransactionController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetSecurityDepositTransactionByID")]
        public async Task<IActionResult> GetSecurityDepositTransactionByID([FromServices] ISecurityDepositTransactionService securitydeposittransactionService, Guid id)
        {
            var result = await securitydeposittransactionService.GetID(id);
            if (result == null)
                return NotFound(new { message = "SecurityDepositTransaction with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<SecurityDepositTransactionController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateSecurityDepositTransaction")]
        public async Task<IActionResult> CreateSecurityDepositTransaction([FromServices] ISecurityDepositTransactionService securitydeposittransactionService, [FromBody] SecurityDepositTransactionDto securitydeposittransactionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await securitydeposittransactionService.CreateSecurityDepositTransaction(securitydeposittransactionDto);
            if (result.SecurityDepositTransactionID != null && result.SecurityDepositTransactionID != Guid.Empty)
                return CreatedAtRoute("GetSecurityDepositTransactionByID", new { id = result.SecurityDepositTransactionID }, result);
            return BadRequest(new { message = "Failed to create securitydeposittransaction. A securitydeposittransaction with the same name may already exist." });
        }

        // PUT api/<SecurityDepositTransactionController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateSecurityDepositTransaction")]
        public async Task<IActionResult> UpdateSecurityDepositTransaction([FromServices] ISecurityDepositTransactionService securitydeposittransactionService, Guid id, [FromBody] SecurityDepositTransactionDto securitydeposittransactionDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await securitydeposittransactionService.UpdateSecurityDepositTransaction(id, securitydeposittransactionDto);
            if (result == null)
                return NotFound(new { message = "SecurityDepositTransaction with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<SecurityDepositTransactionController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteSecurityDepositTransaction")]
        public async Task<IActionResult> DeleteSecurityDepositTransaction([FromServices] ISecurityDepositTransactionService securitydeposittransactionService, Guid id)
        {
            try
            {
                await securitydeposittransactionService.DeleteSecurityDepositTransaction(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "SecurityDepositTransaction with the specified ID was not found." });
            }
        }
    }
}