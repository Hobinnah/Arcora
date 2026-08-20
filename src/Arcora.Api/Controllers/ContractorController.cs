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
    public class ContractorController : ControllerBase
    {
        // GET: api/<ContractorController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllContractors")]
        public async Task<IActionResult> Get([FromServices] IContractorService contractorService, [FromQuery] Paging paging)
        {
            return Ok(await contractorService.GetAll(paging));
        }

        // GET api/<ContractorController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetContractorByID")]
        public async Task<IActionResult> GetContractorByID([FromServices] IContractorService contractorService, Guid id)
        {
            var result = await contractorService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Contractor with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ContractorController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateContractor")]
        public async Task<IActionResult> CreateContractor([FromServices] IContractorService contractorService, [FromBody] ContractorDto contractorDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await contractorService.CreateContractor(contractorDto);
            if (result.ContractorID != null && result.ContractorID != Guid.Empty)
                return CreatedAtRoute("GetContractorByID", new { id = result.ContractorID }, result);
            return BadRequest(new { message = "Failed to create contractor. A contractor with the same name may already exist." });
        }

        // PUT api/<ContractorController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateContractor")]
        public async Task<IActionResult> UpdateContractor([FromServices] IContractorService contractorService, Guid id, [FromBody] ContractorDto contractorDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await contractorService.UpdateContractor(id, contractorDto);
            if (result == null)
                return NotFound(new { message = "Contractor with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ContractorController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteContractor")]
        public async Task<IActionResult> DeleteContractor([FromServices] IContractorService contractorService, Guid id)
        {
            try
            {
                await contractorService.DeleteContractor(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contractor with the specified ID was not found." });
            }
        }

        // POST: api/contractor/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateContractorStatus")]
        public async Task<ActionResult> UpdateContractorStatus([FromServices] IContractorService contractorService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var contractor = await contractorService.UpdateContractorStatus(id, status);
            if (contractor == null)
                return NotFound($"Contractor with ID {id} not found.");
            return Ok(contractor);
        }
    }
}