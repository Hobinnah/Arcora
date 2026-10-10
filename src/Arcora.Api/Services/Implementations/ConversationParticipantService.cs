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
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Arcora.Api.Exceptions;

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
                logger.LogError(er, "An error occurred while fetching conversation participants. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
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
            try
            {
                var userId = conversationparticipantDto.UserID.GetValueOrDefault();
                var tenantId = conversationparticipantDto.TenantID.GetValueOrDefault();
                var organizationMemberId = conversationparticipantDto.OrganizationMemberID.GetValueOrDefault();
                ValidateIdentity(conversationparticipantDto);
                var existing = await FindExistingParticipant(conversationparticipantDto);
                if (existing != null)
                    return this.mapper.Map<ConversationParticipantDto>(existing);

                conversationParticipant = this.mapper.Map<ConversationParticipant>(conversationparticipantDto);
                conversationParticipant.ConversationParticipantID = Guid.NewGuid();
                conversationParticipant.UserID = userId == 0 ? null : userId;
                conversationParticipant.TenantID = tenantId == Guid.Empty ? null : tenantId;
                conversationParticipant.OrganizationMemberID = organizationMemberId == Guid.Empty ? null : organizationMemberId;
                conversationParticipant.JoinedAt = conversationparticipantDto.JoinedAt == default ? DateTime.UtcNow : conversationparticipantDto.JoinedAt;
                conversationParticipant.CapturedDate = DateTime.UtcNow;
                conversationParticipant = await conversationparticipantRepository.Create(conversationParticipant)
                    ?? throw new InvalidOperationException("The participant could not be created.");
                await conversationparticipantRepository.Save();
                cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
            }
            catch (DbUpdateException er) when (er.InnerException is SqlException { Number: 2601 or 2627 })
            {
                cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
                var existing = await FindExistingParticipant(conversationparticipantDto);
                if (existing != null)
                    return this.mapper.Map<ConversationParticipantDto>(existing);
                logger.LogError(er, "A participant uniqueness violation could not be resolved.");
                throw;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ConversationParticipant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ConversationParticipantDto>(conversationParticipant);
        }

        private static void ValidateIdentity(ConversationParticipantDto request)
        {
            if (request.ConversationID == Guid.Empty)
                throw new ApiProblemException(400, "Invalid participant", "A conversation ID is required.");
            if (request.UserID < 0)
                throw new ApiProblemException(400, "Invalid participant", "User ID must be positive when supplied.");
            if (request.UserID.GetValueOrDefault() <= 0
                && request.TenantID.GetValueOrDefault() == Guid.Empty
                && request.OrganizationMemberID.GetValueOrDefault() == Guid.Empty)
                throw new ApiProblemException(400, "Invalid participant", "A user, tenant, or organization member ID is required.");
        }

        private async Task<ConversationParticipant?> FindExistingParticipant(ConversationParticipantDto request, Guid? excludedID = null)
        {
            var userID = request.UserID.GetValueOrDefault();
            var tenantID = request.TenantID.GetValueOrDefault();
            var memberID = request.OrganizationMemberID.GetValueOrDefault();
            var candidates = (await conversationparticipantRepository.Find(x =>
                x.ConversationID == request.ConversationID
                && (!excludedID.HasValue || x.ConversationParticipantID != excludedID.Value)
                && ((userID > 0 && x.UserID == userID)
                    || (tenantID != Guid.Empty && x.TenantID == tenantID)
                    || (memberID != Guid.Empty && x.OrganizationMemberID == memberID))))
                .OfType<ConversationParticipant>().ToList();
            if (candidates.Count == 0) return null;
            var existing = candidates[0];
            if (candidates.Count > 1
                || (userID > 0 && existing.UserID != userID)
                || (tenantID != Guid.Empty && existing.TenantID != tenantID)
                || (memberID != Guid.Empty && existing.OrganizationMemberID != memberID))
                throw new ApiProblemException(409, "Participant identity conflict",
                    "The supplied identities conflict with an existing participant in this conversation.");
            return existing;
        }

        /// <inheritdoc/>
        public async Task<ConversationParticipantDto?> UpdateConversationParticipant(Guid id, ConversationParticipantDto conversationparticipantDto)
        {
            try
            {
                var existing = await this.conversationparticipantRepository.GetByID(id);
                if (existing == null)
                    return null;
                ValidateIdentity(conversationparticipantDto);
                if (await FindExistingParticipant(conversationparticipantDto, id) != null)
                    throw new ApiProblemException(409, "Participant already exists", "This identity already participates in the conversation.");
                ConversationParticipant conversationParticipant = this.mapper.Map<ConversationParticipant>(conversationparticipantDto);
                conversationParticipant.ConversationParticipantID = id;
                conversationParticipant.UserID = conversationparticipantDto.UserID == 0 ? null : conversationparticipantDto.UserID;
                conversationParticipant.TenantID = conversationparticipantDto.TenantID == Guid.Empty ? null : conversationparticipantDto.TenantID;
                conversationParticipant.OrganizationMemberID = conversationparticipantDto.OrganizationMemberID == Guid.Empty ? null : conversationparticipantDto.OrganizationMemberID;
                conversationParticipant = await conversationparticipantRepository.Update(conversationParticipant)
                    ?? throw new InvalidOperationException("The participant could not be updated.");
                await conversationparticipantRepository.Save();
                cache.Remove(Cache.CONVERSATIONPARTICIPANTS.ToString());
                conversationparticipantDto = this.mapper.Map<ConversationParticipantDto>(conversationParticipant);
            }
            catch (DbUpdateException er) when (er.InnerException is SqlException { Number: 2601 or 2627 })
            {
                logger.LogWarning(er, "A participant update conflicts with an existing identity.");
                throw new ApiProblemException(409, "Participant already exists",
                    "This identity already participates in the conversation.");
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