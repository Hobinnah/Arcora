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
    public class AttachmentController : ControllerBase
    {
        // GET: api/<AttachmentController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAttachments")]
        public async Task<IActionResult> Get([FromServices] IAttachmentService attachmentService, [FromQuery] Paging paging)
        {
            return Ok(await attachmentService.GetAll(paging));
        }

        // GET api/<AttachmentController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAttachmentByID")]
        public async Task<IActionResult> GetAttachmentByID([FromServices] IAttachmentService attachmentService, Guid id)
        {
            var result = await attachmentService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Attachment with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<AttachmentController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAttachment")]
        public async Task<IActionResult> CreateAttachment([FromServices] IAttachmentService attachmentService, [FromBody] AttachmentDto attachmentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await attachmentService.CreateAttachment(attachmentDto);
            if (result.AttachmentID != null && result.AttachmentID != Guid.Empty)
                return CreatedAtRoute("GetAttachmentByID", new { id = result.AttachmentID }, result);
            return BadRequest(new { message = "Failed to create attachment. A attachment with the same name may already exist." });
        }

        // PUT api/<AttachmentController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAttachment")]
        public async Task<IActionResult> UpdateAttachment([FromServices] IAttachmentService attachmentService, Guid id, [FromBody] AttachmentDto attachmentDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await attachmentService.UpdateAttachment(id, attachmentDto);
            if (result == null)
                return NotFound(new { message = "Attachment with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AttachmentController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAttachment")]
        public async Task<IActionResult> DeleteAttachment([FromServices] IAttachmentService attachmentService, Guid id)
        {
            try
            {
                await attachmentService.DeleteAttachment(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Attachment with the specified ID was not found." });
            }
        }
    }
}