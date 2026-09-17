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
    public class RentalApplicationController : ControllerBase
    {
        // GET: api/<RentalApplicationController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllRentalApplications")]
        public async Task<IActionResult> Get([FromServices] IRentalApplicationService rentalapplicationService, [FromQuery] Paging paging)
        {
            return Ok(await rentalapplicationService.GetAll(paging));
        }

        // GET api/<RentalApplicationController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRentalApplicationByID")]
        public async Task<IActionResult> GetRentalApplicationByID([FromServices] IRentalApplicationService rentalapplicationService, Guid id)
        {
            var result = await rentalapplicationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "RentalApplication with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<RentalApplicationController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateRentalApplication")]
        public async Task<IActionResult> CreateRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, [FromBody] RentalApplicationDto rentalapplicationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await rentalapplicationService.CreateRentalApplication(rentalapplicationDto);
            if (result.RentalApplicationID != null && result.RentalApplicationID != Guid.Empty)
                return CreatedAtRoute("GetRentalApplicationByID", new { id = result.RentalApplicationID }, result);
            return BadRequest(new { message = "Failed to create rentalapplication. A rentalapplication with the same name may already exist." });
        }

        // PUT api/<RentalApplicationController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateRentalApplication")]
        public async Task<IActionResult> UpdateRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, Guid id, [FromBody] RentalApplicationDto rentalapplicationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await rentalapplicationService.UpdateRentalApplication(id, rentalapplicationDto);
            if (result == null)
                return NotFound(new { message = "RentalApplication with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<RentalApplicationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteRentalApplication")]
        public async Task<IActionResult> DeleteRentalApplication([FromServices] IRentalApplicationService rentalapplicationService, Guid id)
        {
            try
            {
                await rentalapplicationService.DeleteRentalApplication(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "RentalApplication with the specified ID was not found." });
            }
        }

        // POST: api/rentalapplication/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateRentalApplicationStatus")]
        public async Task<ActionResult> UpdateRentalApplicationStatus([FromServices] IRentalApplicationService rentalapplicationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var rentalapplication = await rentalapplicationService.UpdateRentalApplicationStatus(id, status);
            if (rentalapplication == null)
                return NotFound($"RentalApplication with ID {id} not found.");
            return Ok(rentalapplication);
        }

        // GET: api/RentalApplication/GetApplicationStages
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetApplicationStages")]
        public ActionResult<IReadOnlyList<RentalApplicationStageDto>> GetApplicationStages([FromServices] IRentalApplicationService rentalapplicationService)
        {
            return Ok(rentalapplicationService.GetApplicationStages());
        }
    }
}