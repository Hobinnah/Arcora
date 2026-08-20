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
    public class ConversationMessageController : ControllerBase
    {
        // GET: api/<ConversationMessageController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllConversationMessages")]
        public async Task<IActionResult> Get([FromServices] IConversationMessageService conversationmessageService, [FromQuery] Paging paging)
        {
            return Ok(await conversationmessageService.GetAll(paging));
        }

        // GET api/<ConversationMessageController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetConversationMessageByID")]
        public async Task<IActionResult> GetConversationMessageByID([FromServices] IConversationMessageService conversationmessageService, Guid id)
        {
            var result = await conversationmessageService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ConversationMessage with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ConversationMessageController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateConversationMessage")]
        public async Task<IActionResult> CreateConversationMessage([FromServices] IConversationMessageService conversationmessageService, [FromBody] ConversationMessageDto conversationmessageDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationmessageService.CreateConversationMessage(conversationmessageDto);
            if (result.ConversationMessageID != null && result.ConversationMessageID != Guid.Empty)
                return CreatedAtRoute("GetConversationMessageByID", new { id = result.ConversationMessageID }, result);
            return BadRequest(new { message = "Failed to create conversationmessage. A conversationmessage with the same name may already exist." });
        }

        // PUT api/<ConversationMessageController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateConversationMessage")]
        public async Task<IActionResult> UpdateConversationMessage([FromServices] IConversationMessageService conversationmessageService, Guid id, [FromBody] ConversationMessageDto conversationmessageDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationmessageService.UpdateConversationMessage(id, conversationmessageDto);
            if (result == null)
                return NotFound(new { message = "ConversationMessage with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ConversationMessageController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteConversationMessage")]
        public async Task<IActionResult> DeleteConversationMessage([FromServices] IConversationMessageService conversationmessageService, Guid id)
        {
            try
            {
                await conversationmessageService.DeleteConversationMessage(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ConversationMessage with the specified ID was not found." });
            }
        }
    }
}