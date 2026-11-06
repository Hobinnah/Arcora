using AutoMapper;
using Arcora.Api;
using Arcora.Api.DTOs;
using Arcora.Api.Email;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Realtime;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Arcora.Api.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IMessagingService"/>. A listing enquiry thread is scoped by
    /// (TenantID, OrganizationID, ListingID); any active organization member may reply. Edits/deletes are
    /// constrained to a 5-minute window and to the original sender. Every send pushes a real-time event and queues
    /// notification emails to the tenant and all active organization members (except the sender).
    /// </summary>
    public class MessagingService : IMessagingService
    {
        private const int EditDeleteWindowMinutes = 5;

        private readonly IMapper mapper;
        private readonly ILogger<MessagingService> logger;
        private readonly IConfiguration configuration;
        private readonly IConversationRepository conversationRepository;
        private readonly IConversationMessageRepository messageRepository;
        private readonly IConversationParticipantRepository participantRepository;
        private readonly ITenantRepository tenantRepository;
        private readonly IOrganizationMemberRepository organizationMemberRepository;
        private readonly IPreferenceService preferenceService;
        private readonly IHubContext<MessagingHub> hubContext;
        private readonly IListingRepository listingRepository;
        private readonly IListingPhotoRepository listingPhotoRepository;
        private readonly IConversationMessageReactionRepository reactionRepository;
        private readonly IEmailQueue? emailQueue;
        private readonly IEmailSender? emailSender;
        private readonly IFileStorageService? fileStorageService;

        public const long MaxAttachmentBytes = 10 * 1024 * 1024;
        private const int SendLimitPerWindow = 20;
        private static readonly TimeSpan SendLimitWindow = TimeSpan.FromSeconds(60);
        private static readonly Dictionary<string, Queue<DateTime>> recentSends = new();
        private static readonly JsonSerializerOptions webJson = new(JsonSerializerDefaults.Web);

        public MessagingService(
            IMapper mapper,
            ILogger<MessagingService> logger,
            IConfiguration configuration,
            IConversationRepository conversationRepository,
            IConversationMessageRepository messageRepository,
            IConversationParticipantRepository participantRepository,
            ITenantRepository tenantRepository,
            IOrganizationMemberRepository organizationMemberRepository,
            IPreferenceService preferenceService,
            IHubContext<MessagingHub> hubContext,
            IListingRepository listingRepository,
            IListingPhotoRepository listingPhotoRepository,
            IConversationMessageReactionRepository reactionRepository,
            IEmailQueue? emailQueue = null,
            IEmailSender? emailSender = null,
            IFileStorageService? fileStorageService = null)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.configuration = configuration;
            this.conversationRepository = conversationRepository;
            this.messageRepository = messageRepository;
            this.participantRepository = participantRepository;
            this.tenantRepository = tenantRepository;
            this.organizationMemberRepository = organizationMemberRepository;
            this.preferenceService = preferenceService;
            this.hubContext = hubContext;
            this.listingRepository = listingRepository;
            this.listingPhotoRepository = listingPhotoRepository;
            this.reactionRepository = reactionRepository;
            this.emailQueue = emailQueue;
            this.emailSender = emailSender;
            this.fileStorageService = fileStorageService;
        }

        /// <inheritdoc/>
        public async Task<ConversationDto> GetOrCreateDirectThreadAsync(StartThreadRequest request, long? userID = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Listing? enquiryListing = null;
            StartThreadRequest? enquiry = null;
            if (request.ListingID.HasValue)
            {
                if (!userID.HasValue) throw new UnauthorizedAccessException();
                if (string.IsNullOrWhiteSpace(request.Message))
                    throw new ArgumentException("A message to the host is required.");
                if (request.ProposedStartDate == null || request.ProposedEndDate == null || request.ProposedEndDate <= request.ProposedStartDate)
                    throw new ArgumentException("A valid proposed start and end date are required.");
                enquiry = request;
                // A chat only needs the listing row, not its full leases/reviews/calendar graph.
                enquiryListing = await listingRepository.GetByID(request.ListingID.Value);
                if (enquiryListing == null || enquiryListing.Status != "PUBLISHED")
                    throw new KeyNotFoundException("This listing is no longer available for enquiries.");
                var proposedMonths = FullMonthsBetween(request.ProposedStartDate.Value, request.ProposedEndDate.Value);
                if (proposedMonths < enquiryListing.MinimumLeaseMonths)
                    throw new ArgumentException($"This home requires a lease of at least {enquiryListing.MinimumLeaseMonths} month{(enquiryListing.MinimumLeaseMonths == 1 ? "" : "s")}.");
                var tenant = await tenantRepository.GetTenantByUserIDAsync(userID.Value)
                    ?? throw new ApiProblemException(409, "Tenant setup required", "Complete your tenant profile before messaging a host.");
                request = new StartThreadRequest
                {
                    TenantID = tenant.TenantID,
                    OrganizationID = enquiryListing.OrganizationID,
                    Subject = enquiryListing.Title,
                    ListingID = enquiryListing.ListingID
                };
            }
            var existing = await conversationRepository.GetDirectThreadAsync(request.TenantID, request.OrganizationID, request.ListingID);
            if (enquiry != null && existing != null && ConversationStatuses.IsBlocked(existing.Status))
                throw new ApiProblemException(409, "Conversation blocked", "You can't message this host right now.");
            var conversation = existing ?? new Conversation
            {
                ConversationID = Guid.NewGuid(),
                ConversationType = "DIRECT",
                TenantID = request.TenantID,
                OrganizationID = request.OrganizationID,
                ListingID = request.ListingID,
                Subject = request.Subject,
                Status = "OPEN",
                LastMessageAt = null,
                CapturedDate = DateTime.UtcNow
            };

            if (existing == null)
            {
                conversation = await conversationRepository.CreateDirectThreadOrGetExistingAsync(conversation);
                if (enquiry != null && ConversationStatuses.IsBlocked(conversation.Status))
                    throw new ApiProblemException(409, "Conversation blocked", "You can't message this host right now.");
            }

            await EnsureParticipantsAsync(conversation);

            if (enquiryListing != null && enquiry != null)
            {
                var photos = await listingPhotoRepository.Find(p => p.ListingID == enquiryListing.ListingID);
                var photo = photos.Where(p => p != null).OrderByDescending(p => p!.IsCoverPhoto).ThenBy(p => p!.DisplayOrder).FirstOrDefault();
                cancellationToken.ThrowIfCancellationRequested();
                // Each dated enquiry is its own request: the renter's words first, then the listing snapshot.
                // Recipient lookup uses this request's DbContext, so finish it before returning.
                var (_, enquiryMessage, enquiryConversation) = await SendMessageCoreAsync(new SendMessageRequest
                {
                    ConversationID = conversation.ConversationID,
                    SenderTenantID = request.TenantID,
                    SenderUserID = userID,
                    MessageType = "TEXT",
                    Message = enquiry.Message!.Trim()
                }, notifyByEmail: false);
                await SendMessageCoreAsync(new SendMessageRequest
                {
                    ConversationID = conversation.ConversationID,
                    SenderTenantID = request.TenantID,
                    SenderUserID = userID,
                    MessageType = "ENQUIRY_CARD",
                    Message = JsonSerializer.Serialize(new ListingEnquiryCard(
                        enquiryListing.ListingID, enquiryListing.Title ?? "Rental home",
                        enquiryListing.BaseMonthlyRentAmount, enquiryListing.Currency ?? "CAD", photo?.ListingPhotoID,
                        enquiry.ProposedStartDate, enquiry.ProposedEndDate, enquiryListing.SecurityDepositAmount),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web))
                }, notifyByEmail: false);
                await NotifyRecipientsAsync(enquiryConversation, enquiryMessage);
            }
            return mapper.Map<ConversationDto>(conversation);
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto> SendMessageAsync(SendMessageRequest request) => (await SendMessageCoreAsync(request, notifyByEmail: true)).Dto;

        /// <inheritdoc/>
        public async Task<ConversationMessageDto> SendUserMessageAsync(SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("Message content is required.");
            await PrepareUserSendAsync(request);
            request.MessageType = "TEXT";
            request.Message = request.Message.Trim();
            return (await SendMessageCoreAsync(request, notifyByEmail: true)).Dto;
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto> SendAttachmentAsync(SendMessageRequest request, Stream content, string fileName, long length, CancellationToken cancellationToken = default)
        {
            if (!request.AttachmentSendKey.HasValue || request.AttachmentSendKey == Guid.Empty)
                throw new ArgumentException("AttachmentSendKey must be a non-empty UUID reused for retries.");
            if (fileStorageService == null)
                throw new ApiProblemException(503, "Attachments unavailable", "File sharing is not available right now.");
            if (length <= 0 || length > MaxAttachmentBytes)
                throw new ArgumentException("Attachments must be 10 MB or smaller.");

            using var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, cancellationToken);
            if (buffer.Length == 0 || buffer.Length > MaxAttachmentBytes)
                throw new ArgumentException("Attachments must be 10 MB or smaller.");
            // Trust the bytes, not the browser-supplied content type or extension.
            var kind = DetectAttachmentKind(buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, 16)))
                ?? throw new ArgumentException("Only JPG, PNG, WEBP or GIF photos and PDF documents can be shared.");

            await PrepareUserSendAsync(request, acquireSendSlot: false);
            cancellationToken.ThrowIfCancellationRequested();

            var name = SafeFileName(fileName, kind.Extension);
            var senderScope = request.SenderTenantID.HasValue
                ? $"tenant:{request.SenderTenantID:N}" : $"member:{request.SenderOrganizationMemberID:N}";
            var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
            {
                Name = name,
                kind.ContentType,
                request.ReplyToMessageID,
                ContentHash = Convert.ToHexString(SHA256.HashData(buffer.GetBuffer().AsSpan(0, (int)buffer.Length)))
            }, webJson))));
            await using var transaction = await messageRepository.BeginAttachmentSendAsync(
                request.ConversationID, cancellationToken);
            var existing = await messageRepository.GetAttachmentSendAsync(request.ConversationID, senderScope, request.AttachmentSendKey.Value);
            if (existing != null)
            {
                if (!string.Equals(existing.AttachmentPayloadHash, payloadHash, StringComparison.Ordinal))
                    throw new ApiProblemException(409, "Attachment send key reused", "This attachment send key was already used with a different file or reply.");
                await transaction.CommitAsync(cancellationToken);
                var replay = mapper.Map<ConversationMessageDto>(existing);
                replay.SenderName = await ResolveSenderNameAsync(existing, firstNameOnly: true);
                await EnrichAsync(new[] { replay });
                return replay;
            }

            AcquireSendSlot(request);
            buffer.Position = 0;
            var upload = await fileStorageService.UploadAsync(kind.Category, name, buffer, kind.ContentType, cancellationToken);
            request.MessageType = "ATTACHMENT";
            request.Message = JsonSerializer.Serialize(
                new MessageAttachment(upload.BlobName, kind.Category.ToString(), name, kind.ContentType, buffer.Length), webJson);
            var commitStarted = false;
            try
            {
                return (await SendMessageCoreAsync(request, notifyByEmail: true,
                    attachmentIdentity: new(senderScope, request.AttachmentSendKey.Value, payloadHash),
                    onSaved: async () =>
                    {
                        // Persist the replay identity before external notifications or response enrichment can fail.
                        commitStarted = true;
                        await transaction.CommitAsync(cancellationToken);
                    })).Dto;
            }
            catch
            {
                if (!commitStarted)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    try { await fileStorageService.DeleteAsync(kind.Category, upload.BlobName, CancellationToken.None); }
                    catch (Exception ex) { logger.LogWarning(ex, "Could not remove failed attachment upload {BlobName}.", upload.BlobName); }
                }
                // A failed commit acknowledgement is ambiguous: never delete a potentially committed attachment.
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string> SetBlockedAsync(Guid conversationId, ConversationActor actor, bool blocked)
        {
            var conversation = await conversationRepository.GetByID(conversationId)
                ?? throw new KeyNotFoundException("Conversation with the specified ID was not found.");
            var (tenantId, _) = await ResolveParticipantAsync(conversation, actor, actor.UserID, "Only conversation participants can change this conversation.");
            var side = tenantId != null ? ConversationStatuses.BlockedByTenant : ConversationStatuses.BlockedByHost;

            if (blocked)
            {
                // Already blocked by either side: the thread is closed either way, so keep the original blocker in charge.
                if (ConversationStatuses.IsBlocked(conversation.Status))
                    return conversation.Status!;
                conversation.Status = side;
            }
            else
            {
                if (!ConversationStatuses.IsBlocked(conversation.Status))
                    return conversation.Status ?? ConversationStatuses.Open;
                if (!string.Equals(conversation.Status, side, StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Only the participant who blocked this conversation can unblock it.");
                conversation.Status = ConversationStatuses.Open;
            }

            conversation.UpdatedDate = DateTime.UtcNow;
            await conversationRepository.Update(conversation);
            await conversationRepository.Save();
            logger.LogInformation("Conversation {ConversationId} status changed to {Status}. Timestamp: {Timestamp}", conversationId, conversation.Status, DateTime.UtcNow);

            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(conversationId))
                    .SendAsync("ConversationStatusChanged", new { ConversationID = conversationId, conversation.Status });
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push status change for conversation {ConversationId}. Timestamp: {Timestamp}", conversationId, DateTime.UtcNow);
            }
            return conversation.Status!;
        }

        /// <inheritdoc/>
        public async Task ReportConversationAsync(Guid conversationId, ConversationActor actor, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Please tell us what went wrong.");
            var conversation = await conversationRepository.GetByID(conversationId)
                ?? throw new KeyNotFoundException("Conversation with the specified ID was not found.");
            var (tenantId, memberId) = await ResolveParticipantAsync(conversation, actor, actor.UserID, "Only conversation participants can report this conversation.");
            var reporter = tenantId != null ? $"tenant {tenantId}" : $"organization member {memberId}";
            reason = reason.Trim();
            logger.LogWarning("Conversation {ConversationId} reported by {Reporter}: {Reason}. Timestamp: {Timestamp}", conversationId, reporter, reason, DateTime.UtcNow);

            try
            {
                var (companyName, companyEmail) = await GetCompanyInfoAsync();
                if (string.IsNullOrWhiteSpace(companyEmail))
                    return;
                var html = $"<p>A conversation was reported on {System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName)}.</p>"
                    + $"<p><strong>Conversation:</strong> {conversationId}<br/><strong>Reported by:</strong> {System.Net.WebUtility.HtmlEncode(reporter)}</p>"
                    + $"<p><strong>Reason:</strong></p><p style=\"white-space:pre-wrap\">{System.Net.WebUtility.HtmlEncode(reason)}</p>";
                await DispatchEmailAsync(companyEmail, $"Conversation reported ({conversationId})", html);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to email the report for conversation {ConversationId}. Timestamp: {Timestamp}", conversationId, DateTime.UtcNow);
            }
        }

        /// <summary>Shared rules for messages typed or uploaded in the chat UI. Normalizes the sender to one validated side.</summary>
        private async Task PrepareUserSendAsync(SendMessageRequest request, bool acquireSendSlot = true)
        {
            var conversation = await conversationRepository.GetByID(request.ConversationID)
                ?? throw new KeyNotFoundException("Conversation with the specified ID was not found.");
            var (tenantId, memberId) = await ResolveParticipantAsync(conversation,
                new ConversationActor { TenantID = request.SenderTenantID, OrganizationMemberID = request.SenderOrganizationMemberID },
                request.SenderUserID, "Only conversation participants can send messages.");
            request.SenderTenantID = tenantId;
            request.SenderOrganizationMemberID = memberId;

            if (ConversationStatuses.IsBlocked(conversation.Status))
                throw new ApiProblemException(409, "Conversation blocked", "This conversation is blocked, so new messages can't be sent.");

            if (request.ReplyToMessageID.HasValue)
            {
                var quoted = await messageRepository.GetByID(request.ReplyToMessageID.Value);
                if (quoted == null || quoted.ConversationID != conversation.ConversationID)
                    throw new ArgumentException("You can only reply to a message in this conversation.");
            }

            if (acquireSendSlot)
                AcquireSendSlot(request);
        }

        private static void AcquireSendSlot(SendMessageRequest request)
        {
            if (!TryAcquireSendSlot(request.SenderTenantID?.ToString() ?? request.SenderOrganizationMemberID!.Value.ToString()))
                throw new ApiProblemException(429, "Too many messages", "You're sending messages too quickly. Please wait a moment and try again.");
        }

        /// <summary>Sliding-window limit per sender; protects inboxes and email notifications from floods.</summary>
        private static bool TryAcquireSendSlot(string senderKey)
        {
            var now = DateTime.UtcNow;
            lock (recentSends)
            {
                if (!recentSends.TryGetValue(senderKey, out var sends))
                    recentSends[senderKey] = sends = new Queue<DateTime>();
                while (sends.Count > 0 && now - sends.Peek() >= SendLimitWindow)
                    sends.Dequeue();
                if (sends.Count >= SendLimitPerWindow)
                    return false;
                sends.Enqueue(now);
                // Keep the map from growing without bound across many senders.
                if (recentSends.Count > 10_000)
                    foreach (var key in recentSends.Where(p => p.Value.Count == 0 || now - p.Value.Last() >= SendLimitWindow).Select(p => p.Key).ToList())
                        recentSends.Remove(key);
                return true;
            }
        }

        private sealed record AttachmentKind(string ContentType, string Extension, Enums.StorageCategory Category);

        private static AttachmentKind? DetectAttachmentKind(ReadOnlySpan<byte> head)
        {
            static bool Starts(ReadOnlySpan<byte> bytes, params byte[] prefix) => bytes.Length >= prefix.Length && bytes[..prefix.Length].SequenceEqual(prefix);
            if (Starts(head, 0xFF, 0xD8, 0xFF)) return new("image/jpeg", ".jpg", Enums.StorageCategory.Image);
            if (Starts(head, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A)) return new("image/png", ".png", Enums.StorageCategory.Image);
            if (Starts(head, (byte)'G', (byte)'I', (byte)'F', (byte)'8')) return new("image/gif", ".gif", Enums.StorageCategory.Image);
            if (head.Length >= 12 && Starts(head, (byte)'R', (byte)'I', (byte)'F', (byte)'F') && head[8..12].SequenceEqual("WEBP"u8))
                return new("image/webp", ".webp", Enums.StorageCategory.Image);
            if (Starts(head, (byte)'%', (byte)'P', (byte)'D', (byte)'F', (byte)'-')) return new("application/pdf", ".pdf", Enums.StorageCategory.Document);
            return null;
        }

        private static string SafeFileName(string? fileName, string extension)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(fileName ?? string.Empty));
            name = new string(name.Where(c => !char.IsControl(c) && !Path.GetInvalidFileNameChars().Contains(c)).ToArray()).Trim();
            if (name.Length > 80) name = name[..80].Trim();
            return (string.IsNullOrWhiteSpace(name) ? "attachment" : name) + extension;
        }

        private static int FullMonthsBetween(DateOnly start, DateOnly end)
        {
            var months = (end.Year - start.Year) * 12 + end.Month - start.Month;
            if (end.Day < start.Day) months -= 1;
            return months;
        }

        private sealed record AttachmentIdentity(string SenderScope, Guid SendKey, string PayloadHash);

        private async Task<(ConversationMessageDto Dto, ConversationMessage Message, Conversation Conversation)> SendMessageCoreAsync(
            SendMessageRequest request, bool notifyByEmail, AttachmentIdentity? attachmentIdentity = null, Func<Task>? onSaved = null)
        {
            var conversation = await conversationRepository.GetByID(request.ConversationID)
                ?? throw new KeyNotFoundException("Conversation with the specified ID was not found.");

            var now = DateTime.UtcNow;
            var message = new ConversationMessage
            {
                ConversationMessageID = Guid.NewGuid(),
                ConversationID = conversation.ConversationID,
                SenderUserID = request.SenderUserID,
                SenderTenantID = request.SenderTenantID,
                SenderOrganizationMemberID = request.SenderOrganizationMemberID,
                Message = request.Message,
                MessageType = string.IsNullOrWhiteSpace(request.MessageType) ? "TEXT" : request.MessageType,
                ReplyToMessageID = request.ReplyToMessageID,
                AttachmentSendKey = attachmentIdentity?.SendKey,
                AttachmentSenderScope = attachmentIdentity?.SenderScope,
                AttachmentPayloadHash = attachmentIdentity?.PayloadHash,
                SentAt = now,
                CapturedDate = now
            };

            message = await messageRepository.Create(message) ?? message;
            // Repository.Update clears the shared change tracker, so persist the new message first.
            await messageRepository.Save();

            conversation.LastMessageAt = now;
            conversation.UpdatedDate = now;
            await conversationRepository.Update(conversation);
            await conversationRepository.Save();

            if (onSaved != null)
                await onSaved();

            var dto = mapper.Map<ConversationMessageDto>(message);
            dto.SenderName = await ResolveSenderNameAsync(message, firstNameOnly: true);
            await EnrichAsync(new[] { dto });

            // Real-time push to everyone currently viewing the thread.
            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(conversation.ConversationID))
                    .SendAsync("MessageReceived", dto);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push real-time message for conversation {ConversationId}. Timestamp: {Timestamp}", conversation.ConversationID, DateTime.UtcNow);
            }

            if (notifyByEmail)
                await NotifyRecipientsAsync(conversation, message);

            return (dto, message, conversation);
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto?> EditMessageAsync(Guid messageId, string message, ConversationActor actor)
        {
            var existing = await messageRepository.GetByID(messageId);
            if (existing == null || existing.DeletedAt != null)
                return null;

            EnsureSenderAndWindow(existing, actor);
            if (!string.Equals(existing.MessageType, "TEXT", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only text messages can be edited.");

            existing.Message = message;
            existing.EditedAt = DateTime.UtcNow;
            await messageRepository.Update(existing);
            await messageRepository.Save();

            var dto = mapper.Map<ConversationMessageDto>(existing);
            dto.SenderName = await ResolveSenderNameAsync(existing, firstNameOnly: true);
            dto.Reactions = mapper.Map<List<ConversationMessageReactionDto>>(
                await reactionRepository.GetForMessagesAsync(new[] { existing.ConversationMessageID }));
            await EnrichAsync(new[] { dto });

            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(existing.ConversationID))
                    .SendAsync("MessageEdited", dto);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push edited message {MessageId}. Timestamp: {Timestamp}", messageId, DateTime.UtcNow);
            }

            return dto;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMessageAsync(Guid messageId, ConversationActor actor)
        {
            var existing = await messageRepository.GetByID(messageId);
            if (existing == null || existing.DeletedAt != null)
                return false;

            EnsureSenderAndWindow(existing, actor);

            existing.DeletedAt = DateTime.UtcNow;
            await messageRepository.Update(existing);
            await messageRepository.Save();

            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(existing.ConversationID))
                    .SendAsync("MessageDeleted", new { existing.ConversationMessageID, existing.ConversationID });
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push deleted message {MessageId}. Timestamp: {Timestamp}", messageId, DateTime.UtcNow);
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<List<ConversationMessageReactionDto>?> ReactToMessageAsync(Guid messageId, string emoji, ConversationActor actor)
        {
            if (!MessageReactionEmojis.Allowed.Contains(emoji))
                throw new ArgumentException("That reaction is not supported.", nameof(emoji));

            var message = await messageRepository.GetByID(messageId);
            if (message == null || message.DeletedAt != null)
                return null;
            var conversation = await conversationRepository.GetByID(message.ConversationID);
            if (conversation == null)
                return null;

            var (tenantId, organizationMemberId) = await ResolveParticipantAsync(conversation, actor, null, "Only conversation participants can react to messages.");

            var reactions = mapper.Map<List<ConversationMessageReactionDto>>(
                await reactionRepository.ToggleAsync(messageId, emoji, tenantId, organizationMemberId));

            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(conversation.ConversationID))
                    .SendAsync("MessageReactionsChanged", new { ConversationMessageID = messageId, conversation.ConversationID, Reactions = reactions });
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push reactions for message {MessageId}. Timestamp: {Timestamp}", messageId, DateTime.UtcNow);
            }

            return reactions;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationSummaryDto>> GetInboxAsync(InboxQuery query)
        {
            var search = query.Search?.Trim();
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Max(1, query.PageSize);
            // Filters must apply before paging, otherwise a page can come back short or empty while matches exist later.
            var filterInMemory = query.UnreadOnly || !string.IsNullOrEmpty(search);
            var (items, total) = filterInMemory
                ? await conversationRepository.GetInboxAsync(query.TenantID, query.OrganizationID, 1, int.MaxValue)
                : await conversationRepository.GetInboxAsync(query.TenantID, query.OrganizationID, pageNumber, pageSize);

            var isTenantView = query.TenantID.HasValue && query.TenantID != Guid.Empty;
            if (!string.IsNullOrEmpty(search))
                items = items.Where(c => MatchesSearch(c, search)).ToList();

            var unreadByConversation = new Dictionary<Guid, int>();
            foreach (var c in items)
            {
                var lastReadAt = await GetLastReadAtAsync(c.ConversationID, query.TenantID, query.OrganizationID);
                unreadByConversation[c.ConversationID] = await messageRepository.CountUnreadAsync(c.ConversationID, lastReadAt, fromOrganization: isTenantView);
            }

            if (query.UnreadOnly)
                items = items.Where(c => unreadByConversation[c.ConversationID] > 0).ToList();
            if (filterInMemory)
            {
                total = items.Count;
                items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            var membersByOrganization = items.Any(c => c.OrganizationID.HasValue)
                ? (await organizationMemberRepository.GetOrganizationMembersAsync())
                    .Where(m => string.Equals(m.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                    .ToLookup(m => m.OrganizationID)
                : Enumerable.Empty<OrganizationMember>().ToLookup(m => m.OrganizationID);

            var rows = new List<ConversationSummaryDto>();
            foreach (var c in items)
            {
                var latest = await messageRepository.GetLatestAsync(c.ConversationID);
                var unread = unreadByConversation[c.ConversationID];
                var isListingReview = string.Equals(c.ConversationType, "LISTING_REVIEW", StringComparison.OrdinalIgnoreCase);

                // From the tenant's inbox the counterparty is the organization; from the host's inbox it is the tenant.
                var counterpartyName = isTenantView
                    ? (c.Organization?.DisplayName ?? c.Organization?.LegalName)
                    : isListingReview ? "Arcora review team"
                    : (c.Tenant?.User != null ? $"{c.Tenant.User.FirstName} {c.Tenant.User.LastName}".Trim() : null);

                var tenantNames = FirstNames(new[] { c.Tenant?.User?.FirstName });
                var memberNames = c.OrganizationID.HasValue
                    ? FirstNames(membersByOrganization[c.OrganizationID.Value].Select(m => m.User?.FirstName))
                    : new List<string>();

                rows.Add(new ConversationSummaryDto
                {
                    ConversationID = c.ConversationID,
                    TenantID = c.TenantID,
                    OrganizationID = c.OrganizationID,
                    ListingID = c.ListingID,
                    Subject = c.Subject,
                    Status = c.Status,
                    CounterpartyName = string.IsNullOrWhiteSpace(counterpartyName) ? "Conversation" : counterpartyName,
                    ParticipantNames = isTenantView ? memberNames.Concat(tenantNames).ToList() : isListingReview ? new List<string> { "Arcora review team" } : tenantNames.Concat(memberNames).ToList(),
                    TenantFirstName = tenantNames.FirstOrDefault(),
                    OrganizationName = NullIfBlank(c.Organization?.DisplayName) ?? NullIfBlank(c.Organization?.LegalName),
                    LastMessagePreview = PreviewOf(latest),
                    LastMessageAt = c.LastMessageAt ?? latest?.SentAt,
                    UnreadCount = unread
                });
            }

            return new PagedResult<ConversationSummaryDto> { Data = rows, TotalCount = total };
        }

        /// <inheritdoc/>
        public async Task<int> CountUnreadConversationsAsync(Guid? tenantId, Guid? organizationId)
        {
            if ((tenantId == null || tenantId == Guid.Empty) && (organizationId == null || organizationId == Guid.Empty))
                return 0;

            var isTenantView = tenantId.HasValue && tenantId != Guid.Empty;
            var (items, _) = await conversationRepository.GetInboxAsync(tenantId, organizationId, 1, int.MaxValue);
            var count = 0;
            foreach (var c in items)
            {
                var lastReadAt = await GetLastReadAtAsync(c.ConversationID, tenantId, organizationId);
                if (await messageRepository.CountUnreadAsync(c.ConversationID, lastReadAt, fromOrganization: isTenantView) > 0)
                    count++;
            }
            return count;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationMessageDto>> GetThreadMessagesAsync(Guid conversationId, int pageNumber, int pageSize, DateTime? before = null)
        {
            var (items, total) = await messageRepository.GetThreadMessagesAsync(conversationId, pageNumber, pageSize, before);
            var dtos = mapper.Map<List<ConversationMessageDto>>(items);
            var reactions = (await reactionRepository.GetForMessagesAsync(items.Select(m => m.ConversationMessageID).ToList()))
                .ToLookup(r => r.ConversationMessageID);
            foreach (var dto in dtos)
                dto.Reactions = mapper.Map<List<ConversationMessageReactionDto>>(reactions[dto.ConversationMessageID ?? Guid.Empty]);
            await EnrichAsync(dtos);
            return new PagedResult<ConversationMessageDto>
            {
                Data = dtos,
                TotalCount = total
            };
        }

        /// <inheritdoc/>
        public async Task MarkReadAsync(Guid conversationId, ConversationActor actor)
        {
            var participant = await FindReadingParticipantAsync(conversationId, actor);
            if (participant == null)
            {
                // Members who joined the organization after the thread started have no participant row yet.
                var conversation = await conversationRepository.GetByID(conversationId);
                if (conversation == null)
                    return;
                await EnsureParticipantsAsync(conversation);
                participant = await FindReadingParticipantAsync(conversationId, actor);
                if (participant == null)
                    return;
            }

            participant.LastReadAt = DateTime.UtcNow;
            await participantRepository.Update(participant);
            await participantRepository.Save();

            var receipt = (await BuildReadReceiptsAsync(new[] { participant })).FirstOrDefault();
            if (receipt == null)
                return;
            try
            {
                await hubContext.Clients.Group(MessagingHub.GroupName(conversationId))
                    .SendAsync("ConversationRead", receipt);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to push read receipt for conversation {ConversationId}. Timestamp: {Timestamp}", conversationId, DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<List<ConversationReadReceiptDto>> GetReadReceiptsAsync(Guid conversationId)
        {
            var participants = (await participantRepository.Find(p => p.ConversationID == conversationId && p.LastReadAt != null))?
                .Where(p => p != null).Select(p => p!).ToList() ?? new List<ConversationParticipant>();
            return await BuildReadReceiptsAsync(participants);
        }

        /// <summary>
        /// Picks the participant row for the side the actor is acting as. A person can be both the renter and a host
        /// member, so the role IDs take priority over the shared user ID.
        /// </summary>
        private async Task<ConversationParticipant?> FindReadingParticipantAsync(Guid conversationId, ConversationActor actor)
        {
            var participants = (await participantRepository.Find(p =>
                p.ConversationID == conversationId &&
                ((actor.TenantID != null && p.TenantID == actor.TenantID) ||
                 (actor.OrganizationMemberID != null && p.OrganizationMemberID == actor.OrganizationMemberID) ||
                 (actor.UserID != null && p.UserID == actor.UserID))))?
                .Where(p => p != null).Select(p => p!).ToList() ?? new List<ConversationParticipant>();

            return participants.FirstOrDefault(p => actor.TenantID != null && p.TenantID == actor.TenantID)
                ?? participants.FirstOrDefault(p => actor.OrganizationMemberID != null && p.OrganizationMemberID == actor.OrganizationMemberID)
                ?? (actor.TenantID == null && actor.OrganizationMemberID == null ? participants.FirstOrDefault() : null);
        }

        private async Task<List<ConversationReadReceiptDto>> BuildReadReceiptsAsync(IEnumerable<ConversationParticipant> participants)
        {
            var readers = participants.Where(p => p.LastReadAt.HasValue).ToList();
            if (readers.Count == 0)
                return new List<ConversationReadReceiptDto>();

            var membersById = readers.Any(p => p.OrganizationMemberID.HasValue)
                ? (await organizationMemberRepository.GetOrganizationMembersAsync()).ToDictionary(m => m.OrganizationMemberID)
                : new Dictionary<Guid, OrganizationMember>();

            var receipts = new List<ConversationReadReceiptDto>();
            foreach (var p in readers)
            {
                string? firstName = null;
                if (p.TenantID.HasValue)
                {
                    var tenant = await tenantRepository.GetByID(p.TenantID.Value);
                    if (tenant != null)
                        tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;
                    firstName = tenant?.User?.FirstName;
                }
                else if (p.OrganizationMemberID.HasValue && membersById.TryGetValue(p.OrganizationMemberID.Value, out var member))
                {
                    firstName = member.User?.FirstName;
                }

                receipts.Add(new ConversationReadReceiptDto
                {
                    ConversationID = p.ConversationID,
                    TenantID = p.TenantID,
                    OrganizationMemberID = p.OrganizationMemberID,
                    FirstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName.Trim(),
                    LastReadAt = p.LastReadAt!.Value
                });
            }
            return receipts;
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------

        /// <summary>Guards that the actor is the original sender and that the message is still within the edit/delete window.</summary>
        private static void EnsureSenderAndWindow(ConversationMessage message, ConversationActor actor)
        {
            var isSender =
                (actor.TenantID != null && message.SenderTenantID == actor.TenantID) ||
                (actor.OrganizationMemberID != null && message.SenderOrganizationMemberID == actor.OrganizationMemberID) ||
                (actor.UserID != null && message.SenderUserID == actor.UserID);

            if (!isSender)
                throw new UnauthorizedAccessException("Only the original sender can modify this message.");

            if (DateTime.UtcNow > message.SentAt.AddMinutes(EditDeleteWindowMinutes))
                throw new InvalidOperationException($"Messages can only be edited or deleted within {EditDeleteWindowMinutes} minutes of being sent.");
        }

        /// <summary>Ensures the tenant and all active organization members are recorded as conversation participants.</summary>
        private async Task EnsureParticipantsAsync(Conversation conversation)
        {
            try
            {
                var now = DateTime.UtcNow;
                var existing = (await participantRepository.Find(p => p.ConversationID == conversation.ConversationID))?
                    .Where(p => p != null).ToList() ?? new List<ConversationParticipant?>();

                bool needsSave = false;

                if (conversation.TenantID.HasValue &&
                    !existing.Any(p => p!.TenantID == conversation.TenantID))
                {
                    await participantRepository.Create(new ConversationParticipant
                    {
                        ConversationParticipantID = Guid.NewGuid(),
                        ConversationID = conversation.ConversationID,
                        TenantID = conversation.TenantID,
                        ParticipantRole = "TENANT",
                        JoinedAt = now,
                        IsMuted = false,
                        CapturedDate = now
                    });
                    needsSave = true;
                }

                if (conversation.OrganizationID.HasValue)
                {
                    var members = await GetActiveMembersAsync(conversation.OrganizationID.Value);
                    foreach (var member in members)
                    {
                        if (existing.Any(p => p!.OrganizationMemberID == member.OrganizationMemberID))
                            continue;

                        await participantRepository.Create(new ConversationParticipant
                        {
                            ConversationParticipantID = Guid.NewGuid(),
                            ConversationID = conversation.ConversationID,
                            OrganizationMemberID = member.OrganizationMemberID,
                            UserID = member.UserID,
                            ParticipantRole = member.IsPrimaryOwner ? "OWNER" : "MEMBER",
                            JoinedAt = now,
                            IsMuted = false,
                            CapturedDate = now
                        });
                        needsSave = true;
                    }
                }

                if (needsSave)
                    await participantRepository.Save();
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to ensure participants for conversation {ConversationId}. Timestamp: {Timestamp}", conversation.ConversationID, DateTime.UtcNow);
            }
        }

        /// <summary>Resolves the last read timestamp for the acting side of the conversation.</summary>
        private async Task<DateTime?> GetLastReadAtAsync(Guid conversationId, Guid? tenantId, Guid? organizationId)
        {
            var participants = (await participantRepository.Find(p =>
                p.ConversationID == conversationId &&
                ((tenantId != null && p.TenantID == tenantId) ||
                 (organizationId != null && p.OrganizationMemberID != null))))?
                .Where(p => p != null).ToList() ?? new List<ConversationParticipant?>();

            // For a host inbox any member's read state is used; pick the most recent read timestamp.
            return participants
                .Where(p => p!.LastReadAt.HasValue)
                .Max(p => p!.LastReadAt);
        }

        private async Task<List<OrganizationMember>> GetActiveMembersAsync(Guid organizationId)
        {
            var members = await organizationMemberRepository.GetOrganizationMembersAsync();
            return members
                .Where(m => m.OrganizationID == organizationId &&
                            string.Equals(m.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>Queues notification emails to the tenant/user and all active organization members except the sender.</summary>
        private async Task NotifyRecipientsAsync(Conversation conversation, ConversationMessage message)
        {
            try
            {
                var (companyName, companyEmail) = await GetCompanyInfoAsync();
                var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
                var frontendUrl = (configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');
                var preview = message.MessageType == "ENQUIRY_CARD"
                    ? $"Rental enquiry: {conversation.Subject}"
                    : message.MessageType == "ATTACHMENT"
                        ? PreviewOf(message)
                        : string.IsNullOrWhiteSpace(message.Message) ? "(no content)" : message.Message!;
                var subject = conversation.Subject;

                var senderName = await ResolveSenderNameAsync(message);

                // Recipients: the tenant/user + every active organization member, excluding the sender.
                var recipients = new List<(string Email, string Name, bool IsHost)>();

                if (conversation.TenantID.HasValue && message.SenderTenantID != conversation.TenantID)
                {
                    var tenant = await tenantRepository.GetByID(conversation.TenantID.Value);
                    if (tenant != null)
                        tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;

                    var email = tenant?.User?.Email;
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var name = tenant?.User != null
                            ? $"{tenant.User.FirstName} {tenant.User.LastName}".Trim()
                            : "there";
                        recipients.Add((email!, name, false));
                    }
                }

                if (conversation.OrganizationID.HasValue)
                {
                    var members = await GetActiveMembersAsync(conversation.OrganizationID.Value);
                    foreach (var member in members)
                    {
                        if (message.SenderOrganizationMemberID == member.OrganizationMemberID)
                            continue;

                        var email = member.User?.Email;
                        if (string.IsNullOrWhiteSpace(email))
                            continue;

                        var name = member.User != null
                            ? $"{member.User.FirstName} {member.User.LastName}".Trim()
                            : "there";
                        recipients.Add((email!, name, true));
                    }
                }

                foreach (var (email, name, isHost) in recipients.GroupBy(r => r.Email).Select(g => g.First()))
                {
                    var inboxPath = isHost ? "/hosting/messages" : "/messages";
                    var actionUrl = string.IsNullOrWhiteSpace(frontendUrl)
                        ? "#"
                        : $"{frontendUrl}{inboxPath}?conversationID={conversation.ConversationID}";
                    var html = EmailTemplates.BuildNewMessageEmail(
                        recipientName: name,
                        senderName: senderName,
                        messagePreview: preview,
                        conversationSubject: subject,
                        actionUrl: actionUrl,
                        companyName: brand,
                        supportEmail: companyEmail);

                    await DispatchEmailAsync(email, $"New message from {senderName} � {brand}", html);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to send message notification emails for message {MessageId}. Timestamp: {Timestamp}", message.ConversationMessageID, DateTime.UtcNow);
            }
        }

        private async Task<string> ResolveSenderNameAsync(ConversationMessage message, bool firstNameOnly = false)
        {
            string Name(User user) => firstNameOnly && !string.IsNullOrWhiteSpace(user.FirstName)
                ? user.FirstName.Trim()
                : $"{user.FirstName} {user.LastName}".Trim();

            if (message.SenderTenantID.HasValue)
            {
                var tenant = await tenantRepository.GetByID(message.SenderTenantID.Value);
                if (tenant != null)
                    tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;
                if (tenant?.User != null)
                    return Name(tenant.User);
            }

            if (message.SenderOrganizationMemberID.HasValue)
            {
                var members = await organizationMemberRepository.GetOrganizationMembersAsync();
                var member = members.FirstOrDefault(m => m.OrganizationMemberID == message.SenderOrganizationMemberID);
                if (member?.User != null)
                    return Name(member.User);
            }

            return "A participant";
        }

        private static List<string> FirstNames(IEnumerable<string?> names) => names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .ToList();

        private static string? NullIfBlank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        /// <summary>
        /// Resolves which side of the thread the actor is on: the thread's tenant, or an active member of its organization.
        /// When <paramref name="userId"/> is known (from the auth token) it must own that tenant or member record.
        /// </summary>
        private async Task<(Guid? TenantID, Guid? OrganizationMemberID)> ResolveParticipantAsync(Conversation conversation, ConversationActor actor, long? userId, string error)
        {
            if (actor.TenantID != null && actor.TenantID == conversation.TenantID)
            {
                if (userId == null || (await tenantRepository.GetByID(actor.TenantID.Value))?.UserID == userId)
                    return (actor.TenantID, null);
            }
            else if (actor.OrganizationMemberID != null && conversation.OrganizationID != null)
            {
                var member = await organizationMemberRepository.GetByID(actor.OrganizationMemberID.Value);
                if (member != null && member.OrganizationID == conversation.OrganizationID
                    && string.Equals(member.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase)
                    && (userId == null || member.UserID == userId))
                    return (null, member.OrganizationMemberID);
            }
            throw new UnauthorizedAccessException(error);
        }

        /// <summary>Adds display-only fields: read URLs for attachments and the quoted text of replies.</summary>
        private async Task EnrichAsync(IReadOnlyCollection<ConversationMessageDto> dtos)
        {
            foreach (var dto in dtos.Where(d => d.MessageType == "ATTACHMENT" && d.DeletedAt == null))
            {
                var attachment = ParseAttachment(dto.Message);
                if (attachment == null || fileStorageService == null || !Enum.TryParse<Enums.StorageCategory>(attachment.Category, out var category))
                    continue;
                try
                {
                    dto.AttachmentUrl = await fileStorageService.GetReadSasUrlAsync(category, attachment.BlobName);
                }
                catch (Exception er)
                {
                    logger.LogError(er, "Failed to create a read URL for attachment message {MessageId}. Timestamp: {Timestamp}", dto.ConversationMessageID, DateTime.UtcNow);
                }
            }

            var quoted = new Dictionary<Guid, (string Preview, string? SenderName)>();
            foreach (var id in dtos.Where(d => d.ReplyToMessageID.HasValue).Select(d => d.ReplyToMessageID!.Value).Distinct())
            {
                var original = await messageRepository.GetByID(id);
                if (original != null)
                    quoted[id] = original.DeletedAt != null
                        ? ("Message unsent", null)
                        : (PreviewOf(original), await ResolveSenderNameAsync(original, firstNameOnly: true));
            }
            foreach (var dto in dtos)
            {
                if (dto.ReplyToMessageID.HasValue && quoted.TryGetValue(dto.ReplyToMessageID.Value, out var reply))
                {
                    dto.ReplyToPreview = reply.Preview;
                    dto.ReplyToSenderName = reply.SenderName;
                }
            }
        }

        private static MessageAttachment? ParseAttachment(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            try
            {
                var attachment = JsonSerializer.Deserialize<MessageAttachment>(json, webJson);
                return string.IsNullOrWhiteSpace(attachment?.BlobName) ? null : attachment;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string PreviewOf(ConversationMessage? message) => message?.MessageType switch
        {
            null => string.Empty,
            "ENQUIRY_CARD" => "Rental enquiry",
            "ATTACHMENT" => "\U0001F4CE " + (NullIfBlank(ParseAttachment(message.Message)?.Name) ?? "Attachment"),
            _ => Preview(message.Message)
        };

        private static bool MatchesSearch(Conversation conversation, string search)
        {
            bool Has(string? value) => !string.IsNullOrWhiteSpace(value) && value.Contains(search, StringComparison.OrdinalIgnoreCase);
            var user = conversation.Tenant?.User;
            return Has(user?.FirstName) || Has(user?.LastName) || Has($"{user?.FirstName} {user?.LastName}")
                || Has(conversation.Organization?.DisplayName) || Has(conversation.Organization?.LegalName)
                || Has(conversation.Subject);
        }

        private static string Preview(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            text = text.Trim();
            return text.Length <= 120 ? text : text.Substring(0, 117) + "...";
        }

        private async Task<(string CompanyName, string CompanyEmail)> GetCompanyInfoAsync()
        {
            try
            {
                var preference = await preferenceService.GetPreference();
                if (preference != null)
                    return (preference.CompanyName ?? "", preference.CompanyEmail ?? "");
            }
            catch
            { /* non-critical */
            }

            return ("", "");
        }

        private async Task DispatchEmailAsync(string to, string subject, string htmlBody)
        {
            if (emailQueue is not null)
            {
                await emailQueue.EnqueueAsync(new EmailMessage(to, subject, htmlBody));
                return;
            }

            if (emailSender is not null)
            {
                await emailSender.SendEmailAsync(to, subject, htmlBody);
            }
        }
    }
}
