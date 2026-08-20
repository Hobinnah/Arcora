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
    public class TenantScreeningCheckService : ITenantScreeningCheckService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantScreeningCheckService> logger;
        private readonly ITenantScreeningCheckRepository tenantscreeningcheckRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantScreeningCheckService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantScreeningCheckService> logger, ITenantScreeningCheckRepository tenantscreeningcheckRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantscreeningcheckRepository = tenantscreeningcheckRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantScreeningCheckDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantScreeningCheck> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantScreeningCheck>>(Cache.TENANTSCREENINGCHECKS.ToString()) ?? new List<TenantScreeningCheck>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantscreeningcheckRepository.GetTenantScreeningCheckAsync())?.Where(x => x != null) ?? new List<TenantScreeningCheck>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantScreeningCheck>>(Cache.TENANTSCREENINGCHECKS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantScreeningCheck by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantScreeningCheckDto>
                {
                    Data = new List<TenantScreeningCheckDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantScreeningCheck> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderReferenceID) && x.ProviderReferenceID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantScreeningCheckID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantScreeningCheckDto>>(pagedEntities);
            return new PagedResult<TenantScreeningCheckDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantScreeningCheckDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantScreeningCheck> entities = cache.Get<IEnumerable<TenantScreeningCheck>>(Cache.TENANTSCREENINGCHECKS.ToString()) ?? new List<TenantScreeningCheck>();
                TenantScreeningCheck? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantScreeningCheckID == ID);
                }
                else
                {
                    match = await this.tenantscreeningcheckRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantScreeningCheckDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantScreeningCheck by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantScreeningCheckDto> CreateTenantScreeningCheck(TenantScreeningCheckDto tenantscreeningcheckDto)
        {
            TenantScreeningCheck tenantScreeningCheck = new TenantScreeningCheck();
            IEnumerable<TenantScreeningCheck?> checkEntity;
            try
            {
                checkEntity = await this.tenantscreeningcheckRepository.Find(x => x.ProviderReferenceID!.ToLower().Trim() == tenantscreeningcheckDto.ProviderReferenceID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantScreeningCheck = this.mapper.Map<TenantScreeningCheck>(tenantscreeningcheckDto);
                    tenantScreeningCheck.TenantScreeningCheckID = Guid.NewGuid();
                    tenantScreeningCheck.RentalApplicationID = tenantscreeningcheckDto.RentalApplicationID == Guid.Empty ? null : tenantscreeningcheckDto.RentalApplicationID;
                    tenantScreeningCheck.ProviderReferenceID = string.IsNullOrEmpty(tenantscreeningcheckDto.ProviderReferenceID) ? null : tenantscreeningcheckDto.ProviderReferenceID;
                    tenantScreeningCheck.ConsentCapturedAt = DateTime.UtcNow;
                    tenantScreeningCheck.CapturedDate = DateTime.UtcNow;
                    tenantScreeningCheck = await tenantscreeningcheckRepository.Create(tenantScreeningCheck) ?? new TenantScreeningCheck();
                    await tenantscreeningcheckRepository.Save();
                    cache.Remove(Cache.TENANTSCREENINGCHECKS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantScreeningCheck. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantScreeningCheckDto>(tenantScreeningCheck);
        }

        /// <inheritdoc/>
        public async Task<TenantScreeningCheckDto?> UpdateTenantScreeningCheck(Guid id, TenantScreeningCheckDto tenantscreeningcheckDto)
        {
            try
            {
                var existing = await this.tenantscreeningcheckRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantScreeningCheck tenantScreeningCheck = this.mapper.Map<TenantScreeningCheck>(tenantscreeningcheckDto);
                tenantScreeningCheck = await tenantscreeningcheckRepository.Update(tenantScreeningCheck) ?? new TenantScreeningCheck();
                await tenantscreeningcheckRepository.Save();
                cache.Remove(Cache.TENANTSCREENINGCHECKS.ToString());
                tenantscreeningcheckDto = this.mapper.Map<TenantScreeningCheckDto>(tenantScreeningCheck);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantScreeningCheck. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantscreeningcheckDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantScreeningCheck(Guid ID)
        {
            try
            {
                var tenantScreeningCheck = await this.tenantscreeningcheckRepository.GetByID(ID);
                if (tenantScreeningCheck == null)
                    throw new KeyNotFoundException("TenantScreeningCheck with the specified ID was not found.");
                await tenantscreeningcheckRepository.Delete(tenantScreeningCheck);
                await tenantscreeningcheckRepository.Save();
                cache.Remove(Cache.TENANTSCREENINGCHECKS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantScreeningCheck . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantScreeningCheckDto?> UpdateTenantScreeningCheckStatus(Guid id, string status)
        {
            var tenantScreeningCheck = await tenantscreeningcheckRepository.GetByID(id);
            if (tenantScreeningCheck == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                tenantScreeningCheck.Status = "Pending";
            }
            else
            {
                tenantScreeningCheck.Status = status;
            }

            await tenantscreeningcheckRepository.Update(tenantScreeningCheck);
            await tenantscreeningcheckRepository.Save();
            cache.Remove(Cache.TENANTSCREENINGCHECKS.ToString());
            return this.mapper.Map<TenantScreeningCheckDto>(tenantScreeningCheck);
        }
    }
}