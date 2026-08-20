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
    public class ConversationParticipantService : IConversationParticipantService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ConversationParticipantService> logger;
        private readonly IConversationParticipantRepository conversationparticipantRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ConversationParticipantService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ConversationParticipantService> logger, IConversationParticipantRepository conversationparticipantRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.conversationparticipantRepository = conversationparticipantRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ConversationParticipantDto>> GetAll(Paging paging)
        {
            IEnumerable<ConversationParticipant> entities;
            try
            {
                entities = cache.Get<IEnumerable<ConversationParticipant>>(Cache.CONVERSATIONPARTICIPANTS.ToString()) ?? new List<ConversationParticipant>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.conversationparticipantRepository.GetConversationParticipantAsync())?.Where(x => x != null) ?? new List<ConversationParticipant>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ConversationParticipant>>(Cache.CONVERSATIONPARTICIPANTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ConversationParticipant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ConversationParticipantDto>
                {
                    Data = new List<ConversationParticipantDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ConversationParticipant> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ParticipantRole) && x.ParticipantRole.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ConversationParticipantID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ConversationParticipantDto>>(pagedEntities);
            return new PagedResult<ConversationParticipantDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ConversationParticipantDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ConversationParticipant> entities = cache.Get<IEnumerable<ConversationParticipant>>(Cache.CONVERSATIONPARTICIPANTS.ToString()) ?? new List<ConversationParticipant>();
                ConversationParticipant? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ConversationParticipantID == ID);
                }
                else
                {
                    match = await this.conversationparticipantRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ConversationParticipantDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ConversationParticipant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ConversationParticipantDto> CreateConversationParticipant(ConversationParticipantDto conversationparticipantDto)
        {
            ConversationParticipant conversationParticipant = new ConversationParticipant();
            IEnumerable<ConversationParticipant?> checkEntity;
            try
            {
                checkEntity = await this.conversationparticipantRepository.Find(x => x.ParticipantRole!.ToLower().Trim() == conversationparticipantDto.ParticipantRole!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    conversationParticipant = this.mapper.Map<ConversationParticipant>(conversationparticipantDto);
                    conversationParticipant.ConversationParticipantID = Guid.NewGuid();
                    conversationParticipant.UserID = conversationparticipantDto.UserID == 0 ? null : conversationparticipantDto.UserID;
                    conversationParticipant.TenantID = conversationparticipantDto.TenantID == Guid.Empty ? null : conversationparticipantDto.TenantID;
                    conversationParticipant.OrganizationMemberID = conversationparticipantDto.OrganizationMemberID == Guid.Empty ? null : conversationparticipantDto.OrganizationMemberID;
                    conversationParticipant.CapturedDate = DateTime.UtcNow;
                    conversationParticipant = await conversationparticipantRepository.Create(conversationParticipant) ?? new ConversationParticipant();
                    await conversationparticipantRepository.Save();
                    cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ConversationParticipant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ConversationParticipantDto>(conversationParticipant);
        }

        /// <inheritdoc/>
        public async Task<ConversationParticipantDto?> UpdateConversationParticipant(Guid id, ConversationParticipantDto conversationparticipantDto)
        {
            try
            {
                var existing = await this.conversationparticipantRepository.GetByID(id);
                if (existing == null)
                    return null;
                ConversationParticipant conversationParticipant = this.mapper.Map<ConversationParticipant>(conversationparticipantDto);
                conversationParticipant = await conversationparticipantRepository.Update(conversationParticipant) ?? new ConversationParticipant();
                await conversationparticipantRepository.Save();
                cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
                conversationparticipantDto = this.mapper.Map<ConversationParticipantDto>(conversationParticipant);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ConversationParticipant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return conversationparticipantDto;
        }

        /// <inheritdoc/>
        public async Task DeleteConversationParticipant(Guid ID)
        {
            try
            {
                var conversationParticipant = await this.conversationparticipantRepository.GetByID(ID);
                if (conversationParticipant == null)
                    throw new KeyNotFoundException("ConversationParticipant with the specified ID was not found.");
                await conversationparticipantRepository.Delete(conversationParticipant);
                await conversationparticipantRepository.Save();
                cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ConversationParticipant . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}