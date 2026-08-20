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
    public class TenantService : ITenantService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantService> logger;
        private readonly ITenantRepository tenantRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantService> logger, ITenantRepository tenantRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantRepository = tenantRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantDto>> GetAll(Paging paging)
        {
            IEnumerable<Tenant> entities;
            try
            {
                entities = cache.Get<IEnumerable<Tenant>>(Cache.TENANTS.ToString()) ?? new List<Tenant>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantRepository.GetTenantAsync())?.Where(x => x != null) ?? new List<Tenant>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Tenant>>(Cache.TENANTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Tenant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantDto>
                {
                    Data = new List<TenantDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Tenant> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Code) && x.Code.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantDto>>(pagedEntities);
            return new PagedResult<TenantDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Tenant> entities = cache.Get<IEnumerable<Tenant>>(Cache.TENANTS.ToString()) ?? new List<Tenant>();
                Tenant? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantID == ID);
                }
                else
                {
                    match = await this.tenantRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Tenant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantDto> CreateTenant(TenantDto tenantDto)
        {
            Tenant tenant = new Tenant();
            IEnumerable<Tenant?> checkEntity;
            try
            {
                checkEntity = await this.tenantRepository.Find(x => x.Code!.ToLower().Trim() == tenantDto.Code!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenant = this.mapper.Map<Tenant>(tenantDto);
                    tenant.TenantID = Guid.NewGuid();
                    tenant.CapturedDate = DateTime.UtcNow;
                    tenant = await tenantRepository.Create(tenant) ?? new Tenant();
                    await tenantRepository.Save();
                    cache.Remove(Cache.TENANTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Tenant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantDto>(tenant);
        }

        /// <inheritdoc/>
        public async Task<TenantDto?> UpdateTenant(Guid id, TenantDto tenantDto)
        {
            try
            {
                var existing = await this.tenantRepository.GetByID(id);
                if (existing == null)
                    return null;
                Tenant tenant = this.mapper.Map<Tenant>(tenantDto);
                tenant = await tenantRepository.Update(tenant) ?? new Tenant();
                await tenantRepository.Save();
                cache.Remove(Cache.TENANTS.ToString());
                tenantDto = this.mapper.Map<TenantDto>(tenant);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Tenant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenant(Guid ID)
        {
            try
            {
                var tenant = await this.tenantRepository.GetByID(ID);
                if (tenant == null)
                    throw new KeyNotFoundException("Tenant with the specified ID was not found.");
                await tenantRepository.Delete(tenant);
                await tenantRepository.Save();
                cache.Remove(Cache.TENANTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Tenant . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}