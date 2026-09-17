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
    public class ConversationParticipantController : ControllerBase
    {
        // GET: api/<ConversationParticipantController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllConversationParticipants")]
        public async Task<IActionResult> Get([FromServices] IConversationParticipantService conversationparticipantService, [FromQuery] Paging paging)
        {
            return Ok(await conversationparticipantService.GetAll(paging));
        }

        // GET api/<ConversationParticipantController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetConversationParticipantByID")]
        public async Task<IActionResult> GetConversationParticipantByID([FromServices] IConversationParticipantService conversationparticipantService, Guid id)
        {
            var result = await conversationparticipantService.GetID(id);
            if (result == null)
                return NotFound(new { message = "ConversationParticipant with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<ConversationParticipantController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateConversationParticipant")]
        public async Task<IActionResult> CreateConversationParticipant([FromServices] IConversationParticipantService conversationparticipantService, [FromBody] ConversationParticipantDto conversationparticipantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationparticipantService.CreateConversationParticipant(conversationparticipantDto);
            if (result.ConversationParticipantID != null && result.ConversationParticipantID != Guid.Empty)
                return CreatedAtRoute("GetConversationParticipantByID", new { id = result.ConversationParticipantID }, result);
            return BadRequest(new { message = "Failed to create conversationparticipant. A conversationparticipant with the same name may already exist." });
        }

        // PUT api/<ConversationParticipantController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateConversationParticipant")]
        public async Task<IActionResult> UpdateConversationParticipant([FromServices] IConversationParticipantService conversationparticipantService, Guid id, [FromBody] ConversationParticipantDto conversationparticipantDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await conversationparticipantService.UpdateConversationParticipant(id, conversationparticipantDto);
            if (result == null)
                return NotFound(new { message = "ConversationParticipant with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ConversationParticipantController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteConversationParticipant")]
        public async Task<IActionResult> DeleteConversationParticipant([FromServices] IConversationParticipantService conversationparticipantService, Guid id)
        {
            try
            {
                await conversationparticipantService.DeleteConversationParticipant(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "ConversationParticipant with the specified ID was not found." });
            }
        }
    }
}