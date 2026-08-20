// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class ConversationMessageService : IConversationMessageService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ConversationMessageService> logger;
        private readonly IConversationMessageRepository conversationmessageRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ConversationMessageService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ConversationMessageService> logger, IConversationMessageRepository conversationmessageRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.conversationmessageRepository = conversationmessageRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationMessageDto>> GetAll(Paging paging)
        {
            IEnumerable<ConversationMessage> entities;
            try
            {
                entities = cache.Get<IEnumerable<ConversationMessage>>(Cache.CONVERSATIONMESSAGES.ToString()) ?? new List<ConversationMessage>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.conversationmessageRepository.GetConversationMessageAsync())?.Where(x => x != null) ?? new List<ConversationMessage>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ConversationMessage>>(Cache.CONVERSATIONMESSAGES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ConversationMessage by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ConversationMessageDto>
                {
                    Data = new List<ConversationMessageDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ConversationMessage> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Message) && x.Message.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ConversationMessageID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ConversationMessageDto>>(pagedEntities);
            return new PagedResult<ConversationMessageDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ConversationMessage> entities = cache.Get<IEnumerable<ConversationMessage>>(Cache.CONVERSATIONMESSAGES.ToString()) ?? new List<ConversationMessage>();
                ConversationMessage? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ConversationMessageID == ID);
                }
                else
                {
                    match = await this.conversationmessageRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ConversationMessageDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ConversationMessage by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto> CreateConversationMessage(ConversationMessageDto conversationmessageDto)
        {
            ConversationMessage conversationMessage = new ConversationMessage();
            IEnumerable<ConversationMessage?> checkEntity;
            try
            {
                checkEntity = await this.conversationmessageRepository.Find(x => x.Message!.ToLower().Trim() == conversationmessageDto.Message!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    conversationMessage = this.mapper.Map<ConversationMessage>(conversationmessageDto);
                    conversationMessage.ConversationMessageID = Guid.NewGuid();
                    conversationMessage.SenderUserID = conversationmessageDto.SenderUserID == 0 ? null : conversationmessageDto.SenderUserID;
                    conversationMessage.SenderTenantID = conversationmessageDto.SenderTenantID == Guid.Empty ? null : conversationmessageDto.SenderTenantID;
                    conversationMessage.SenderOrganizationMemberID = conversationmessageDto.SenderOrganizationMemberID == Guid.Empty ? null : conversationmessageDto.SenderOrganizationMemberID;
                    conversationMessage.ReplyToMessageID = conversationmessageDto.ReplyToMessageID == Guid.Empty ? null : conversationmessageDto.ReplyToMessageID;
                    conversationMessage.CapturedDate = DateTime.UtcNow;
                    conversationMessage = await conversationmessageRepository.Create(conversationMessage) ?? new ConversationMessage();
                    await conversationmessageRepository.Save();
                    cache.Remove(Cache.CONVERSATIONMESSAGES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ConversationMessage. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ConversationMessageDto>(conversationMessage);
        }

        /// <inheritdoc/>
        public async Task<ConversationMessageDto?> UpdateConversationMessage(Guid id, ConversationMessageDto conversationmessageDto)
        {
            try
            {
                var existing = await this.conversationmessageRepository.GetByID(id);
                if (existing == null)
                    return null;
                ConversationMessage conversationMessage = this.mapper.Map<ConversationMessage>(conversationmessageDto);
                conversationMessage = await conversationmessageRepository.Update(conversationMessage) ?? new ConversationMessage();
                await conversationmessageRepository.Save();
                cache.Remove(Cache.CONVERSATIONMESSAGES.ToString());
                conversationmessageDto = this.mapper.Map<ConversationMessageDto>(conversationMessage);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ConversationMessage. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return conversationmessageDto;
        }

        /// <inheritdoc/>
        public async Task DeleteConversationMessage(Guid ID)
        {
            try
            {
                var conversationMessage = await this.conversationmessageRepository.GetByID(ID);
                if (conversationMessage == null)
                    throw new KeyNotFoundException("ConversationMessage with the specified ID was not found.");
                await conversationmessageRepository.Delete(conversationMessage);
                await conversationmessageRepository.Save();
                cache.Remove(Cache.CONVERSATIONMESSAGES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ConversationMessage . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}