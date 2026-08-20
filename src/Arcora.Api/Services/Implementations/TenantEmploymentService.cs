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
    public class TenantEmploymentService : ITenantEmploymentService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantEmploymentService> logger;
        private readonly ITenantEmploymentRepository tenantemploymentRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantEmploymentService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantEmploymentService> logger, ITenantEmploymentRepository tenantemploymentRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantemploymentRepository = tenantemploymentRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantEmploymentDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantEmployment> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantEmployment>>(Cache.TENANTEMPLOYMENTS.ToString()) ?? new List<TenantEmployment>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantemploymentRepository.GetTenantEmploymentAsync())?.Where(x => x != null) ?? new List<TenantEmployment>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantEmployment>>(Cache.TENANTEMPLOYMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantEmployment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantEmploymentDto>
                {
                    Data = new List<TenantEmploymentDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantEmployment> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.EmployerName) && x.EmployerName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantEmploymentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantEmploymentDto>>(pagedEntities);
            return new PagedResult<TenantEmploymentDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantEmploymentDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantEmployment> entities = cache.Get<IEnumerable<TenantEmployment>>(Cache.TENANTEMPLOYMENTS.ToString()) ?? new List<TenantEmployment>();
                TenantEmployment? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantEmploymentID == ID);
                }
                else
                {
                    match = await this.tenantemploymentRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantEmploymentDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantEmployment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantEmploymentDto> CreateTenantEmployment(TenantEmploymentDto tenantemploymentDto)
        {
            TenantEmployment tenantEmployment = new TenantEmployment();
            IEnumerable<TenantEmployment?> checkEntity;
            try
            {
                checkEntity = await this.tenantemploymentRepository.Find(x => x.EmployerName!.ToLower().Trim() == tenantemploymentDto.EmployerName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantEmployment = this.mapper.Map<TenantEmployment>(tenantemploymentDto);
                    tenantEmployment.TenantEmploymentID = Guid.NewGuid();
                    tenantEmployment.CapturedDate = DateTime.UtcNow;
                    tenantEmployment = await tenantemploymentRepository.Create(tenantEmployment) ?? new TenantEmployment();
                    await tenantemploymentRepository.Save();
                    cache.Remove(Cache.TENANTEMPLOYMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantEmployment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantEmploymentDto>(tenantEmployment);
        }

        /// <inheritdoc/>
        public async Task<TenantEmploymentDto?> UpdateTenantEmployment(Guid id, TenantEmploymentDto tenantemploymentDto)
        {
            try
            {
                var existing = await this.tenantemploymentRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantEmployment tenantEmployment = this.mapper.Map<TenantEmployment>(tenantemploymentDto);
                tenantEmployment = await tenantemploymentRepository.Update(tenantEmployment) ?? new TenantEmployment();
                await tenantemploymentRepository.Save();
                cache.Remove(Cache.TENANTEMPLOYMENTS.ToString());
                tenantemploymentDto = this.mapper.Map<TenantEmploymentDto>(tenantEmployment);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantEmployment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantemploymentDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantEmployment(Guid ID)
        {
            try
            {
                var tenantEmployment = await this.tenantemploymentRepository.GetByID(ID);
                if (tenantEmployment == null)
                    throw new KeyNotFoundException("TenantEmployment with the specified ID was not found.");
                await tenantemploymentRepository.Delete(tenantEmployment);
                await tenantemploymentRepository.Save();
                cache.Remove(Cache.TENANTEMPLOYMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantEmployment . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}