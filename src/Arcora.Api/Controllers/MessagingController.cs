using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    /// <summary>
    /// Direct host-to-tenant messaging API. Threads are keyed by (tenant, organization); any active
    /// organization member can reply. Messages may be edited or deleted only within 5 minutes of creation.
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class MessagingController : ControllerBase
    {
        // POST api/Messaging/StartThread
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "StartThread")]
        public async Task<IActionResult> StartThread([FromServices] IMessagingService messagingService, [FromBody] StartThreadRequest request)
        {
            if (request.TenantID == Guid.Empty || request.OrganizationID == Guid.Empty)
                return BadRequest(new { message = "TenantID and OrganizationID are required." });

            var result = await messagingService.GetOrCreateDirectThreadAsync(request);
            return Ok(result);
        }

        // POST api/Messaging/SendMessage
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "SendMessage")]
        public async Task<IActionResult> SendMessage([FromServices] IMessagingService messagingService, [FromBody] SendMessageRequest request)
        {
            if (request.ConversationID == Guid.Empty)
                return BadRequest(new { message = "ConversationID is required." });
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { message = "Message content is required." });

            try
            {
                var result = await messagingService.SendMessageAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT api/Messaging/EditMessage/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "EditMessage")]
        public async Task<IActionResult> EditMessage([FromServices] IMessagingService messagingService, Guid id, [FromBody] EditMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest(new { message = "Message content is required." });

            try
            {
                var result = await messagingService.EditMessageAsync(id, request!.Message!, request.Actor);
                if (result == null)
                    return NotFound(new { message = "Message with the specified ID was not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/Messaging/DeleteMessage/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteMessage")]
        public async Task<IActionResult> DeleteMessage([FromServices] IMessagingService messagingService, Guid id, [FromBody] ConversationActor actor)
        {
            try
            {
                var deleted = await messagingService.DeleteMessageAsync(id, actor);
                if (!deleted)
                    return NotFound(new { message = "Message with the specified ID was not found." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/Messaging/Inbox
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetInbox")]
        public async Task<IActionResult> Inbox([FromServices] IMessagingService messagingService, [FromQuery] InboxQuery query)
        {
            return Ok(await messagingService.GetInboxAsync(query));
        }

        // GET api/Messaging/Thread/{conversationId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{conversationId}", Name = "GetThread")]
        public async Task<IActionResult> Thread([FromServices] IMessagingService messagingService, Guid conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30)
        {
            return Ok(await messagingService.GetThreadMessagesAsync(conversationId, pageNumber, pageSize));
        }

        // POST api/Messaging/MarkRead/{conversationId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("{conversationId}", Name = "MarkRead")]
        public async Task<IActionResult> MarkRead([FromServices] IMessagingService messagingService, Guid conversationId, [FromBody] ConversationActor actor)
        {
            await messagingService.MarkReadAsync(conversationId, actor);
            return NoContent();
        }
    }
}
