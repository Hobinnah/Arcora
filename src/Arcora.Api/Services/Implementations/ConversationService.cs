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
    public class ConversationService : IConversationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ConversationService> logger;
        private readonly IConversationRepository conversationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ConversationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ConversationService> logger, IConversationRepository conversationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.conversationRepository = conversationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationDto>> GetAll(Paging paging)
        {
            IEnumerable<Conversation> entities;
            try
            {
                entities = cache.Get<IEnumerable<Conversation>>(Cache.CONVERSATIONS.ToString()) ?? new List<Conversation>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.conversationRepository.GetConversationAsync())?.Where(x => x != null) ?? new List<Conversation>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Conversation>>(Cache.CONVERSATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Conversation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ConversationDto>
                {
                    Data = new List<ConversationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Conversation> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Subject) && x.Subject.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ConversationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ConversationDto>>(pagedEntities);
            return new PagedResult<ConversationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ConversationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Conversation> entities = cache.Get<IEnumerable<Conversation>>(Cache.CONVERSATIONS.ToString()) ?? new List<Conversation>();
                Conversation? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ConversationID == ID);
                }
                else
                {
                    match = await this.conversationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ConversationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Conversation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ConversationDto> CreateConversation(ConversationDto conversationDto)
        {
            Conversation conversation = new Conversation();
            IEnumerable<Conversation?> checkEntity;
            try
            {
                checkEntity = await this.conversationRepository.Find(x => x.Subject!.ToLower().Trim() == conversationDto.Subject!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    conversation = this.mapper.Map<Conversation>(conversationDto);
                    conversation.ConversationID = Guid.NewGuid();
                    conversation.LeaseID = conversationDto.LeaseID == Guid.Empty ? null : conversationDto.LeaseID;
                    conversation.LeaseRenewalID = conversationDto.LeaseRenewalID == Guid.Empty ? null : conversationDto.LeaseRenewalID;
                    conversation.MaintenanceRequestID = conversationDto.MaintenanceRequestID == Guid.Empty ? null : conversationDto.MaintenanceRequestID;
                    conversation.DisputeID = conversationDto.DisputeID == Guid.Empty ? null : conversationDto.DisputeID;
                    conversation.CapturedDate = DateTime.UtcNow;
                    conversation = await conversationRepository.Create(conversation) ?? new Conversation();
                    await conversationRepository.Save();
                    cache.Remove(Cache.CONVERSATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Conversation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ConversationDto>(conversation);
        }

        /// <inheritdoc/>
        public async Task<ConversationDto?> UpdateConversation(Guid id, ConversationDto conversationDto)
        {
            try
            {
                var existing = await this.conversationRepository.GetByID(id);
                if (existing == null)
                    return null;
                Conversation conversation = this.mapper.Map<Conversation>(conversationDto);
                conversation = await conversationRepository.Update(conversation) ?? new Conversation();
                await conversationRepository.Save();
                cache.Remove(Cache.CONVERSATIONS.ToString());
                conversationDto = this.mapper.Map<ConversationDto>(conversation);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Conversation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return conversationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteConversation(Guid ID)
        {
            try
            {
                var conversation = await this.conversationRepository.GetByID(ID);
                if (conversation == null)
                    throw new KeyNotFoundException("Conversation with the specified ID was not found.");
                await conversationRepository.Delete(conversation);
                await conversationRepository.Save();
                cache.Remove(Cache.CONVERSATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Conversation . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ConversationDto?> UpdateConversationStatus(Guid id, string status)
        {
            var conversation = await conversationRepository.GetByID(id);
            if (conversation == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                conversation.Status = "Pending";
            }
            else
            {
                conversation.Status = status;
            }

            await conversationRepository.Update(conversation);
            await conversationRepository.Save();
            cache.Remove(Cache.CONVERSATIONS.ToString());
            return this.mapper.Map<ConversationDto>(conversation);
        }
    }
}