// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class TenantEmergencyContactService : ITenantEmergencyContactService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<TenantEmergencyContactService> logger;
        private readonly ITenantEmergencyContactRepository tenantemergencycontactRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantEmergencyContactService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<TenantEmergencyContactService> logger, ITenantEmergencyContactRepository tenantemergencycontactRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantemergencycontactRepository = tenantemergencycontactRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantEmergencyContactDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantEmergencyContact> entities;
            try
            {
                entities = await cache.GetOrCreateAsync(
                    Cache.TENANTEMERGENCYCONTACTS.ToString(),
                    async cancel =>
                        (await this.tenantemergencycontactRepository.GetTenantEmergencyContactAsync())?.Where(x => x != null).ToList() ?? new List<TenantEmergencyContact>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantEmergencyContact by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantEmergencyContactDto>
                {
                    Data = new List<TenantEmergencyContactDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantEmergencyContact> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantEmergencyContactID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantEmergencyContactDto>>(pagedEntities);
            return new PagedResult<TenantEmergencyContactDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantEmergencyContactDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.TENANTEMERGENCYCONTACTS.ToString(),
                    async cancel =>
                        (await this.tenantemergencycontactRepository.GetTenantEmergencyContactAsync())?.Where(x => x != null).ToList() ?? new List<TenantEmergencyContact>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });

                var match = entities.FirstOrDefault(x => x.TenantEmergencyContactID == ID);
                if (match == null)
                    match = await this.tenantemergencycontactRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<TenantEmergencyContactDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantEmergencyContact by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantEmergencyContactDto> CreateTenantEmergencyContact(TenantEmergencyContactDto tenantemergencycontactDto)
        {
            TenantEmergencyContact tenantEmergencyContact = new TenantEmergencyContact();
            IEnumerable<TenantEmergencyContact?> checkEntity;
            try
            {
                checkEntity = await this.tenantemergencycontactRepository.Find(x => x.Name!.ToLower().Trim() == tenantemergencycontactDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantEmergencyContact = this.mapper.Map<TenantEmergencyContact>(tenantemergencycontactDto);
                    tenantEmergencyContact.TenantEmergencyContactID = Guid.NewGuid();
                    tenantEmergencyContact.CapturedDate = DateTime.UtcNow;
                    tenantEmergencyContact = await tenantemergencycontactRepository.Create(tenantEmergencyContact) ?? new TenantEmergencyContact();
                    await tenantemergencycontactRepository.Save();
                    await cache.RemoveAsync(Cache.TENANTEMERGENCYCONTACTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantEmergencyContact. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantEmergencyContactDto>(tenantEmergencyContact);
        }

        /// <inheritdoc/>
        public async Task<TenantEmergencyContactDto?> UpdateTenantEmergencyContact(Guid id, TenantEmergencyContactDto tenantemergencycontactDto)
        {
            try
            {
                var existing = await this.tenantemergencycontactRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantEmergencyContact tenantEmergencyContact = this.mapper.Map<TenantEmergencyContact>(tenantemergencycontactDto);
                tenantEmergencyContact = await tenantemergencycontactRepository.Update(tenantEmergencyContact) ?? new TenantEmergencyContact();
                await tenantemergencycontactRepository.Save();
                await cache.RemoveAsync(Cache.TENANTEMERGENCYCONTACTS.ToString());
                tenantemergencycontactDto = this.mapper.Map<TenantEmergencyContactDto>(tenantEmergencyContact);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantEmergencyContact. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantemergencycontactDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantEmergencyContact(Guid ID)
        {
            try
            {
                var tenantEmergencyContact = await this.tenantemergencycontactRepository.GetByID(ID);
                if (tenantEmergencyContact == null)
                    throw new KeyNotFoundException("TenantEmergencyContact with the specified ID was not found.");
                await tenantemergencycontactRepository.Delete(tenantEmergencyContact);
                await tenantemergencycontactRepository.Save();
                await cache.RemoveAsync(Cache.TENANTEMERGENCYCONTACTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantEmergencyContact . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}