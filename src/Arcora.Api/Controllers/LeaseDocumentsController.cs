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
    public class LeaseDocumentsController : ControllerBase
    {
        // GET: api/<LeaseDocumentsController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllLeaseDocuments")]
        public async Task<IActionResult> Get([FromServices] ILeaseDocumentsService leasedocumentsService, [FromQuery] Paging paging)
        {
            return Ok(await leasedocumentsService.GetAll(paging));
        }

        // GET api/<LeaseDocumentsController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetLeaseDocumentsByID")]
        public async Task<IActionResult> GetLeaseDocumentsByID([FromServices] ILeaseDocumentsService leasedocumentsService, Guid id)
        {
            var result = await leasedocumentsService.GetID(id);
            if (result == null)
                return NotFound(new { message = "LeaseDocuments with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<LeaseDocumentsController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateLeaseDocuments")]
        public async Task<IActionResult> CreateLeaseDocuments([FromServices] ILeaseDocumentsService leasedocumentsService, [FromBody] LeaseDocumentsDto leasedocumentsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasedocumentsService.CreateLeaseDocuments(leasedocumentsDto);
            if (result.LeaseDocumentID != null && result.LeaseDocumentID != Guid.Empty)
                return CreatedAtRoute("GetLeaseDocumentsByID", new { id = result.LeaseDocumentID }, result);
            return BadRequest(new { message = "Failed to create leasedocuments. A leasedocuments with the same name may already exist." });
        }

        // PUT api/<LeaseDocumentsController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateLeaseDocuments")]
        public async Task<IActionResult> UpdateLeaseDocuments([FromServices] ILeaseDocumentsService leasedocumentsService, Guid id, [FromBody] LeaseDocumentsDto leasedocumentsDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await leasedocumentsService.UpdateLeaseDocuments(id, leasedocumentsDto);
            if (result == null)
                return NotFound(new { message = "LeaseDocuments with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<LeaseDocumentsController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteLeaseDocuments")]
        public async Task<IActionResult> DeleteLeaseDocuments([FromServices] ILeaseDocumentsService leasedocumentsService, Guid id)
        {
            try
            {
                await leasedocumentsService.DeleteLeaseDocuments(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "LeaseDocuments with the specified ID was not found." });
            }
        }
    }
}