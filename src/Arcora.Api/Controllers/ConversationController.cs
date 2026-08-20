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
    public class ConversationController : ControllerBase
    {
        // GET: api/<ConversationController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllConversations")]
        public async Task<IActionResult> Get([FromServices] IConversationService conversationService, [FromQuery] Paging paging)
        {
            return Ok(await conversationService.GetAll(paging));
        }

        // GET api/<ConversationController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetConversationByID")]
        public async Task<IActionResult> GetConversationByID([FromServices] IConversationService conversationService, Guid id)
        {
            var result = await conversationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Conversation with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ConversationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateConversation")]
        public async Task<IActionResult> CreateConversation([FromServices] IConversationService conversationService, [FromBody] ConversationDto conversationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationService.CreateConversation(conversationDto);
            if (result.ConversationID != null && result.ConversationID != Guid.Empty)
                return CreatedAtRoute("GetConversationByID", new { id = result.ConversationID }, result);
            return BadRequest(new { message = "Failed to create conversation. A conversation with the same name may already exist." });
        }

        // PUT api/<ConversationController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateConversation")]
        public async Task<IActionResult> UpdateConversation([FromServices] IConversationService conversationService, Guid id, [FromBody] ConversationDto conversationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationService.UpdateConversation(id, conversationDto);
            if (result == null)
                return NotFound(new { message = "Conversation with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ConversationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteConversation")]
        public async Task<IActionResult> DeleteConversation([FromServices] IConversationService conversationService, Guid id)
        {
            try
            {
                await conversationService.DeleteConversation(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Conversation with the specified ID was not found." });
            }
        }

        // POST: api/conversation/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateConversationStatus")]
        public async Task<ActionResult> UpdateConversationStatus([FromServices] IConversationService conversationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var conversation = await conversationService.UpdateConversationStatus(id, status);
            if (conversation == null)
                return NotFound($"Conversation with ID {id} not found.");
            return Ok(conversation);
        }
    }
}