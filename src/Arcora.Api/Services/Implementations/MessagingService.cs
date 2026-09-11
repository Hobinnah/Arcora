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

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IMessagingService"/>. A direct thread is uniquely keyed by
    /// (TenantID, OrganizationID); any active organization member may reply. Edits/deletes are constrained
    /// to a 5-minute window and to the original sender. Every send pushes a real-time event and queues
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
        private readonly IEmailQueue? emailQueue;
        private readonly IEmailSender? emailSender;

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
            IEmailQueue? emailQueue = null,
            IEmailSender? emailSender = null)
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
            this.emailQueue = emailQueue;
            this.emailSender = emailSender;
        }

        /// <inheritdoc/>
        public async Task<ConversationDto> GetOrCreateDirectThreadAsync(StartThreadRequest request)
        {
            var existing = await conversationRepository.GetDirectThreadAsync(request.TenantID, request.OrganizationID);
            if (existing != null)
                return mapper.Map<ConversationDto>(existing);

            var conversation = new Conversation
            {
                ConversationID = Guid.NewGuid(),
                ConversationType = "DIRECT",
                TenantID = request.TenantID,
                OrganizationID = request.OrganizationID,
                Subject = request.Subject,
                Status = "OPEN",
                LastMessageAt = null,
                CapturedDate = DateTime.UtcNow
            };

            conversation = await conversationRepository.Create(conversation) ?? conversation;
            await conversationRepository.Save();

            await EnsureParticipantsAsync(conversation);

            return mapper.Map<ConversationDto>(conversation);
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto> SendMessageAsync(SendMessageRequest request)
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
                SentAt = now,
                CapturedDate = now
            };

            message = await messageRepository.Create(message) ?? message;

            conversation.LastMessageAt = now;
            conversation.UpdatedDate = now;
            await conversationRepository.Update(conversation);
            await conversationRepository.Save();
            await messageRepository.Save();

            var dto = mapper.Map<ConversationMessageDto>(message);

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

            // Fire-and-forget notification emails without blocking the caller.
            _ = NotifyRecipientsAsync(conversation, message);

            return dto;
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto?> EditMessageAsync(Guid messageId, string message, ConversationActor actor)
        {
            var existing = await messageRepository.GetByID(messageId);
            if (existing == null || existing.DeletedAt != null)
                return null;

            EnsureSenderAndWindow(existing, actor);

            existing.Message = message;
            existing.EditedAt = DateTime.UtcNow;
            await messageRepository.Update(existing);
            await messageRepository.Save();

            var dto = mapper.Map<ConversationMessageDto>(existing);

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
        public async Task<PagedResult<ConversationSummaryDto>> GetInboxAsync(InboxQuery query)
        {
            var (items, total) = await conversationRepository.GetInboxAsync(
                query.TenantID, query.OrganizationID, query.PageNumber, query.PageSize);

            var isTenantView = query.TenantID.HasValue && query.TenantID != Guid.Empty;

            var rows = new List<ConversationSummaryDto>();
            foreach (var c in items)
            {
                var latest = await messageRepository.GetLatestAsync(c.ConversationID);
                var lastReadAt = await GetLastReadAtAsync(c.ConversationID, query.TenantID, query.OrganizationID);
                var unread = await messageRepository.CountUnreadAsync(c.ConversationID, lastReadAt, null);

                // From the tenant's inbox the counterparty is the organization; from the host's inbox it is the tenant.
                var counterpartyName = isTenantView
                    ? (c.Organization?.DisplayName ?? c.Organization?.LegalName)
                    : (c.Tenant?.User != null ? $"{c.Tenant.User.FirstName} {c.Tenant.User.LastName}".Trim() : null);

                rows.Add(new ConversationSummaryDto
                {
                    ConversationID = c.ConversationID,
                    TenantID = c.TenantID,
                    OrganizationID = c.OrganizationID,
                    Subject = c.Subject,
                    Status = c.Status,
                    CounterpartyName = string.IsNullOrWhiteSpace(counterpartyName) ? "Conversation" : counterpartyName,
                    LastMessagePreview = Preview(latest?.Message),
                    LastMessageAt = c.LastMessageAt ?? latest?.SentAt,
                    UnreadCount = unread
                });
            }

            if (query.UnreadOnly)
                rows = rows.Where(r => r.UnreadCount > 0).ToList();

            return new PagedResult<ConversationSummaryDto> { Data = rows, TotalCount = total };
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationMessageDto>> GetThreadMessagesAsync(Guid conversationId, int pageNumber, int pageSize)
        {
            var (items, total) = await messageRepository.GetThreadMessagesAsync(conversationId, pageNumber, pageSize);
            return new PagedResult<ConversationMessageDto>
            {
                Data = mapper.Map<List<ConversationMessageDto>>(items),
                TotalCount = total
            };
        }

        /// <inheritdoc/>
        public async Task MarkReadAsync(Guid conversationId, ConversationActor actor)
        {
            var participants = await participantRepository.Find(p =>
                p.ConversationID == conversationId &&
                ((actor.TenantID != null && p.TenantID == actor.TenantID) ||
                 (actor.OrganizationMemberID != null && p.OrganizationMemberID == actor.OrganizationMemberID) ||
                 (actor.UserID != null && p.UserID == actor.UserID)));

            var participant = participants?.FirstOrDefault(p => p != null);
            if (participant == null)
                return;

            participant.LastReadAt = DateTime.UtcNow;
            await participantRepository.Update(participant);
            await participantRepository.Save();
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
                var actionUrl = string.IsNullOrWhiteSpace(frontendUrl)
                    ? "#"
                    : $"{frontendUrl}/messages/{conversation.ConversationID}";
                var preview = string.IsNullOrWhiteSpace(message.Message) ? "(no content)" : message.Message!;
                var subject = conversation.Subject;

                var senderName = await ResolveSenderNameAsync(message);

                // Recipients: the tenant/user + every active organization member, excluding the sender.
                var recipients = new List<(string Email, string Name)>();

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
                        recipients.Add((email!, name));
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
                        recipients.Add((email!, name));
                    }
                }

                foreach (var (email, name) in recipients.GroupBy(r => r.Email).Select(g => g.First()))
                {
                    var html = EmailTemplates.BuildNewMessageEmail(
                        recipientName: name,
                        senderName: senderName,
                        messagePreview: preview,
                        conversationSubject: subject,
                        actionUrl: actionUrl,
                        companyName: brand,
                        supportEmail: companyEmail);

                    await DispatchEmailAsync(email, $"New message from {senderName} — {brand}", html);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to send message notification emails for message {MessageId}. Timestamp: {Timestamp}", message.ConversationMessageID, DateTime.UtcNow);
            }
        }

        private async Task<string> ResolveSenderNameAsync(ConversationMessage message)
        {
            if (message.SenderTenantID.HasValue)
            {
                var tenant = await tenantRepository.GetByID(message.SenderTenantID.Value);
                if (tenant != null)
                    tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;
                if (tenant?.User != null)
                    return $"{tenant.User.FirstName} {tenant.User.LastName}".Trim();
            }

            if (message.SenderOrganizationMemberID.HasValue)
            {
                var members = await organizationMemberRepository.GetOrganizationMembersAsync();
                var member = members.FirstOrDefault(m => m.OrganizationMemberID == message.SenderOrganizationMemberID);
                if (member?.User != null)
                    return $"{member.User.FirstName} {member.User.LastName}".Trim();
            }

            return "A participant";
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
