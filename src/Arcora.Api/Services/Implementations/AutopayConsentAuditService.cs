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
    public class AutopayConsentAuditService : IAutopayConsentAuditService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<AutopayConsentAuditService> logger;
        private readonly IAutopayConsentAuditRepository autopayconsentauditRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AutopayConsentAuditService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<AutopayConsentAuditService> logger, IAutopayConsentAuditRepository autopayconsentauditRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.autopayconsentauditRepository = autopayconsentauditRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AutopayConsentAuditDto>> GetAll(Paging paging)
        {
            IEnumerable<AutopayConsentAudit> entities;
            try
            {
                entities = await cache.GetOrCreateAsync(
                    Cache.AUTOPAYCONSENTAUDITS.ToString(),
                    async cancel =>
                        (await this.autopayconsentauditRepository.GetAutopayConsentAuditAsync())?.Where(x => x != null).ToList() ?? new List<AutopayConsentAudit>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AutopayConsentAudit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AutopayConsentAuditDto>
                {
                    Data = new List<AutopayConsentAuditDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<AutopayConsentAudit> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderReferenceID) && x.ProviderReferenceID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AutopayConsentAuditID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AutopayConsentAuditDto>>(pagedEntities);
            return new PagedResult<AutopayConsentAuditDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AutopayConsentAuditDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.AUTOPAYCONSENTAUDITS.ToString(),
                    async cancel =>
                        (await this.autopayconsentauditRepository.GetAutopayConsentAuditAsync())?.Where(x => x != null).ToList() ?? new List<AutopayConsentAudit>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });

                var match = entities.FirstOrDefault(x => x.AutopayConsentAuditID == ID);
                if (match == null)
                    match = await this.autopayconsentauditRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<AutopayConsentAuditDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AutopayConsentAudit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AutopayConsentAuditDto> CreateAutopayConsentAudit(AutopayConsentAuditDto autopayconsentauditDto)
        {
            AutopayConsentAudit autopayConsentAudit = new AutopayConsentAudit();
            IEnumerable<AutopayConsentAudit?> checkEntity;
            try
            {
                checkEntity = await this.autopayconsentauditRepository.Find(x => x.ProviderReferenceID!.ToLower().Trim() == autopayconsentauditDto.ProviderReferenceID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    autopayConsentAudit = this.mapper.Map<AutopayConsentAudit>(autopayconsentauditDto);
                    autopayConsentAudit.AutopayConsentAuditID = Guid.NewGuid();
                    autopayConsentAudit.ProviderReferenceID = string.IsNullOrEmpty(autopayconsentauditDto.ProviderReferenceID) ? null : autopayconsentauditDto.ProviderReferenceID;
                    autopayConsentAudit.CapturedDate = DateTime.UtcNow;
                    autopayConsentAudit = await autopayconsentauditRepository.Create(autopayConsentAudit) ?? new AutopayConsentAudit();
                    await autopayconsentauditRepository.Save();
                    await cache.RemoveAsync(Cache.AUTOPAYCONSENTAUDITS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating AutopayConsentAudit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AutopayConsentAuditDto>(autopayConsentAudit);
        }

        /// <inheritdoc/>
        public async Task<AutopayConsentAuditDto?> UpdateAutopayConsentAudit(Guid id, AutopayConsentAuditDto autopayconsentauditDto)
        {
            try
            {
                var existing = await this.autopayconsentauditRepository.GetByID(id);
                if (existing == null)
                    return null;
                AutopayConsentAudit autopayConsentAudit = this.mapper.Map<AutopayConsentAudit>(autopayconsentauditDto);
                autopayConsentAudit = await autopayconsentauditRepository.Update(autopayConsentAudit) ?? new AutopayConsentAudit();
                await autopayconsentauditRepository.Save();
                await cache.RemoveAsync(Cache.AUTOPAYCONSENTAUDITS.ToString());
                autopayconsentauditDto = this.mapper.Map<AutopayConsentAuditDto>(autopayConsentAudit);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating AutopayConsentAudit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return autopayconsentauditDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAutopayConsentAudit(Guid ID)
        {
            try
            {
                var autopayConsentAudit = await this.autopayconsentauditRepository.GetByID(ID);
                if (autopayConsentAudit == null)
                    throw new KeyNotFoundException("AutopayConsentAudit with the specified ID was not found.");
                await autopayconsentauditRepository.Delete(autopayConsentAudit);
                await autopayconsentauditRepository.Save();
                await cache.RemoveAsync(Cache.AUTOPAYCONSENTAUDITS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting AutopayConsentAudit . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}