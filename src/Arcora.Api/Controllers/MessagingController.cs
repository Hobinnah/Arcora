using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private const int MaxEnquiryMessageLength = 2000;

        // POST api/Messaging/StartThread
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost(Name = "StartThread")]
        public async Task<IActionResult> StartThread([FromServices] IMessagingService messagingService, [FromBody] StartThreadRequest request, CancellationToken cancellationToken)
        {
            if (request.ListingID == Guid.Empty)
                return BadRequest(new { message = "ListingID must not be empty." });
            if (request.ListingID == null && (request.TenantID == Guid.Empty || request.OrganizationID == Guid.Empty))
                return BadRequest(new { message = "TenantID and OrganizationID are required." });
            if (request.ListingID.HasValue)
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest(new { message = "A message to the host is required." });
                if (request.Message.Trim().Length > MaxEnquiryMessageLength)
                    return BadRequest(new { message = $"Message must be {MaxEnquiryMessageLength} characters or fewer." });
                if (request.ProposedStartDate == null || request.ProposedEndDate == null)
                    return BadRequest(new { message = "Proposed start and end dates are required." });
                if (request.ProposedEndDate <= request.ProposedStartDate)
                    return BadRequest(new { message = "Proposed end date must be after the start date." });
                // One day of slack lets renters in timezones ahead of UTC pick their local "today".
                if (request.ProposedStartDate < DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))
                    return BadRequest(new { message = "Proposed start date cannot be in the past." });
            }

            long? userID = null;
            if (request.ListingID.HasValue)
            {
                var claim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                if (!long.TryParse(claim, out var authenticatedUserID) || authenticatedUserID <= 0)
                    return Unauthorized();
                userID = authenticatedUserID;
            }
            var result = await messagingService.GetOrCreateDirectThreadAsync(request, userID, cancellationToken);
            return Ok(result);
        }

        // POST api/Messaging/SendMessage
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPost(Name = "SendMessage")]
        public async Task<IActionResult> SendMessage([FromServices] IMessagingService messagingService, [FromBody] SendMessageRequest request)
        {
            if (request.ConversationID == Guid.Empty)
                return BadRequest(new { message = "ConversationID is required." });
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { message = "Message content is required." });
            if (request.Message.Trim().Length > MaxEnquiryMessageLength)
                return BadRequest(new { message = $"Message must be {MaxEnquiryMessageLength} characters or fewer." });
            if (CurrentUserID() is not long userID)
                return Unauthorized();
            request.SenderUserID = userID;

            try
            {
                return Ok(await messagingService.SendUserMessageAsync(request));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // POST api/Messaging/SendAttachment (multipart/form-data)
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [HttpPost(Name = "SendAttachment")]
        [RequestSizeLimit(Services.Implementations.MessagingService.MaxAttachmentBytes + 1024 * 1024)]
        public async Task<IActionResult> SendAttachment([FromServices] IMessagingService messagingService, [FromForm] SendAttachmentRequest request, CancellationToken cancellationToken)
        {
            if (!request.AttachmentSendKey.HasValue || request.AttachmentSendKey == Guid.Empty)
                return BadRequest(new { message = "AttachmentSendKey must be a non-empty UUID reused for retries." });
            if (request.ConversationID == Guid.Empty)
                return BadRequest(new { message = "ConversationID is required." });
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "Choose a photo or PDF to send." });
            if (request.File.Length > Services.Implementations.MessagingService.MaxAttachmentBytes)
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new { message = "Attachments must be 10 MB or smaller." });
            if (CurrentUserID() is not long userID)
                return Unauthorized();

            var sendRequest = new SendMessageRequest
            {
                AttachmentSendKey = request.AttachmentSendKey,
                ConversationID = request.ConversationID,
                ReplyToMessageID = request.ReplyToMessageID,
                SenderUserID = userID,
                SenderTenantID = request.SenderTenantID,
                SenderOrganizationMemberID = request.SenderOrganizationMemberID,
            };
            try
            {
                await using var stream = request.File.OpenReadStream();
                return Ok(await messagingService.SendAttachmentAsync(sendRequest, stream, request.File.FileName, request.File.Length, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // POST api/Messaging/Block/{conversationId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{conversationId}", Name = "BlockConversation")]
        public Task<IActionResult> Block([FromServices] IMessagingService messagingService, Guid conversationId, [FromBody] ConversationModerationRequest request)
            => SetBlocked(messagingService, conversationId, request, blocked: true);

        // POST api/Messaging/Unblock/{conversationId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{conversationId}", Name = "UnblockConversation")]
        public Task<IActionResult> Unblock([FromServices] IMessagingService messagingService, Guid conversationId, [FromBody] ConversationModerationRequest request)
            => SetBlocked(messagingService, conversationId, request, blocked: false);

        // POST api/Messaging/Report/{conversationId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{conversationId}", Name = "ReportConversation")]
        public async Task<IActionResult> Report([FromServices] IMessagingService messagingService, Guid conversationId, [FromBody] ConversationModerationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Reason))
                return BadRequest(new { message = "Please tell us what went wrong." });
            if (request.Reason.Trim().Length > 1000)
                return BadRequest(new { message = "Reason must be 1000 characters or fewer." });
            if (CurrentUserID() is not long userID)
                return Unauthorized();
            request.Actor ??= new ConversationActor();
            request.Actor.UserID = userID;

            try
            {
                await messagingService.ReportConversationAsync(conversationId, request.Actor, request.Reason);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        private async Task<IActionResult> SetBlocked(IMessagingService messagingService, Guid conversationId, ConversationModerationRequest? request, bool blocked)
        {
            if (CurrentUserID() is not long userID)
                return Unauthorized();
            var actor = request?.Actor ?? new ConversationActor();
            actor.UserID = userID;
            try
            {
                return Ok(new { Status = await messagingService.SetBlockedAsync(conversationId, actor, blocked) });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // GET api/Messaging/ConversationDetails/{conversationId}?tenantID=...|organizationMemberID=...
        [ProducesResponseType(typeof(ConversationDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{conversationId}", Name = "GetConversationDetails")]
        public async Task<IActionResult> ConversationDetails([FromServices] IConversationDetailsService detailsService, Guid conversationId, [FromQuery] Guid? tenantID, [FromQuery] Guid? organizationMemberID, CancellationToken cancellationToken)
        {
            var userID = CurrentUserID();
            if (userID is null)
                return Unauthorized(new { message = "Sign in to view this conversation." });

            try
            {
                var actor = new ConversationActor { UserID = userID, TenantID = tenantID, OrganizationMemberID = organizationMemberID };
                return Ok(await detailsService.GetDetailsAsync(conversationId, actor, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // PUT api/Messaging/ConversationNote/{conversationId}
        [ProducesResponseType(typeof(ConversationNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{conversationId}", Name = "SaveConversationNote")]
        public async Task<IActionResult> ConversationNote([FromServices] IConversationDetailsService detailsService, Guid conversationId, [FromBody] SaveConversationNoteRequest request, CancellationToken cancellationToken)
        {
            var userID = CurrentUserID();
            if (userID is null)
                return Unauthorized(new { message = "Sign in to update notes." });
            if (request?.Actor is null)
                return BadRequest(new { message = "The acting host is required." });
            if ((request.Note?.Trim().Length ?? 0) > Entities.ConversationNote.MaxLength)
                return BadRequest(new { message = $"Notes can be at most {Entities.ConversationNote.MaxLength} characters." });

            request.Actor.UserID = userID;
            try
            {
                var saved = await detailsService.SaveNoteAsync(conversationId, request.Actor, request.Note, cancellationToken);
                return saved is null ? NoContent() : Ok(saved);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private long? CurrentUserID()
        {
            var claim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return long.TryParse(claim, out var id) && id > 0 ? id : null;
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

        // POST api/Messaging/ReactToMessage/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{id}", Name = "ReactToMessage")]
        public async Task<IActionResult> ReactToMessage([FromServices] IMessagingService messagingService, Guid id, [FromBody] ReactToMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Emoji) || !MessageReactionEmojis.Allowed.Contains(request.Emoji))
                return BadRequest(new { message = "That reaction is not supported." });

            try
            {
                var reactions = await messagingService.ReactToMessageAsync(id, request.Emoji, request.Actor ?? new ConversationActor());
                if (reactions == null)
                    return NotFound(new { message = "Message with the specified ID was not found." });
                return Ok(reactions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // GET api/Messaging/Inbox
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetInbox")]
        public async Task<IActionResult> Inbox([FromServices] IMessagingService messagingService, [FromQuery] InboxQuery query)
        {
            return Ok(await messagingService.GetInboxAsync(query));
        }

        // GET api/Messaging/UnreadCount?tenantID=...|organizationID=...
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetUnreadCount")]
        public async Task<IActionResult> UnreadCount([FromServices] IMessagingService messagingService, [FromQuery] Guid? tenantID, [FromQuery] Guid? organizationID)
        {
            return Ok(new { UnreadConversations = await messagingService.CountUnreadConversationsAsync(tenantID, organizationID) });
        }

        // GET api/Messaging/Thread/{conversationId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{conversationId}", Name = "GetThread")]
        public async Task<IActionResult> Thread([FromServices] IMessagingService messagingService, Guid conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30, [FromQuery] DateTime? before = null)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            if (before.HasValue)
                before = before.Value.Kind == DateTimeKind.Local ? before.Value.ToUniversalTime() : DateTime.SpecifyKind(before.Value, DateTimeKind.Utc);
            return Ok(await messagingService.GetThreadMessagesAsync(conversationId, pageNumber, pageSize, before));
        }

        // POST api/Messaging/MarkRead/{conversationId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("{conversationId}", Name = "MarkRead")]
        public async Task<IActionResult> MarkRead([FromServices] IMessagingService messagingService, Guid conversationId, [FromBody] ConversationActor actor)
        {
            await messagingService.MarkReadAsync(conversationId, actor);
            return NoContent();
        }

        // GET api/Messaging/ReadReceipts/{conversationId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{conversationId}", Name = "GetReadReceipts")]
        public async Task<IActionResult> ReadReceipts([FromServices] IMessagingService messagingService, Guid conversationId)
        {
            return Ok(await messagingService.GetReadReceiptsAsync(conversationId));
        }
    }
}
