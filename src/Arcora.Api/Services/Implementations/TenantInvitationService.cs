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
    public class TenantInvitationService : ITenantInvitationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantInvitationService> logger;
        private readonly ITenantInvitationRepository tenantinvitationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantInvitationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantInvitationService> logger, ITenantInvitationRepository tenantinvitationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantinvitationRepository = tenantinvitationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantInvitationDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantInvitation> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString()) ?? new List<TenantInvitation>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantinvitationRepository.GetTenantInvitationAsync())?.Where(x => x != null) ?? new List<TenantInvitation>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantInvitation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantInvitationDto>
                {
                    Data = new List<TenantInvitationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantInvitation> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantInvitationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantInvitationDto>>(pagedEntities);
            return new PagedResult<TenantInvitationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantInvitation> entities = cache.Get<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString()) ?? new List<TenantInvitation>();
                TenantInvitation? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantInvitationID == ID);
                }
                else
                {
                    match = await this.tenantinvitationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantInvitationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantInvitation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto> CreateTenantInvitation(TenantInvitationDto tenantinvitationDto)
        {
            TenantInvitation tenantInvitation = new TenantInvitation();
            IEnumerable<TenantInvitation?> checkEntity;
            try
            {
                checkEntity = await this.tenantinvitationRepository.Find(x => x.Email!.ToLower().Trim() == tenantinvitationDto.Email!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantInvitation = this.mapper.Map<TenantInvitation>(tenantinvitationDto);
                    tenantInvitation.TenantInvitationID = Guid.NewGuid();
                    tenantInvitation.LeaseID = tenantinvitationDto.LeaseID == Guid.Empty ? null : tenantinvitationDto.LeaseID;
                    tenantInvitation.RentalApplicationID = tenantinvitationDto.RentalApplicationID == Guid.Empty ? null : tenantinvitationDto.RentalApplicationID;
                    tenantInvitation.CapturedDate = DateTime.UtcNow;
                    tenantInvitation = await tenantinvitationRepository.Create(tenantInvitation) ?? new TenantInvitation();
                    await tenantinvitationRepository.Save();
                    cache.Remove(Cache.TENANTINVITATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantInvitation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantInvitationDto>(tenantInvitation);
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> UpdateTenantInvitation(Guid id, TenantInvitationDto tenantinvitationDto)
        {
            try
            {
                var existing = await this.tenantinvitationRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantInvitation tenantInvitation = this.mapper.Map<TenantInvitation>(tenantinvitationDto);
                tenantInvitation = await tenantinvitationRepository.Update(tenantInvitation) ?? new TenantInvitation();
                await tenantinvitationRepository.Save();
                cache.Remove(Cache.TENANTINVITATIONS.ToString());
                tenantinvitationDto = this.mapper.Map<TenantInvitationDto>(tenantInvitation);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantInvitation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantinvitationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantInvitation(Guid ID)
        {
            try
            {
                var tenantInvitation = await this.tenantinvitationRepository.GetByID(ID);
                if (tenantInvitation == null)
                    throw new KeyNotFoundException("TenantInvitation with the specified ID was not found.");
                await tenantinvitationRepository.Delete(tenantInvitation);
                await tenantinvitationRepository.Save();
                cache.Remove(Cache.TENANTINVITATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantInvitation . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> UpdateTenantInvitationStatus(Guid id, string status)
        {
            var tenantInvitation = await tenantinvitationRepository.GetByID(id);
            if (tenantInvitation == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                tenantInvitation.Status = "Pending";
            }
            else
            {
                tenantInvitation.Status = status;
            }

            await tenantinvitationRepository.Update(tenantInvitation);
            await tenantinvitationRepository.Save();
            cache.Remove(Cache.TENANTINVITATIONS.ToString());
            return this.mapper.Map<TenantInvitationDto>(tenantInvitation);
        }
    }
}