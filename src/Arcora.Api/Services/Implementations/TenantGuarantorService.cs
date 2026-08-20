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
    public class TenantGuarantorService : ITenantGuarantorService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantGuarantorService> logger;
        private readonly ITenantGuarantorRepository tenantguarantorRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantGuarantorService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantGuarantorService> logger, ITenantGuarantorRepository tenantguarantorRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantguarantorRepository = tenantguarantorRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantGuarantorDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantGuarantor> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString()) ?? new List<TenantGuarantor>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantguarantorRepository.GetTenantGuarantorAsync())?.Where(x => x != null) ?? new List<TenantGuarantor>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantGuarantor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantGuarantorDto>
                {
                    Data = new List<TenantGuarantorDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantGuarantor> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantGuarantorID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantGuarantorDto>>(pagedEntities);
            return new PagedResult<TenantGuarantorDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantGuarantor> entities = cache.Get<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString()) ?? new List<TenantGuarantor>();
                TenantGuarantor? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantGuarantorID == ID);
                }
                else
                {
                    match = await this.tenantguarantorRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantGuarantorDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantGuarantor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto> CreateTenantGuarantor(TenantGuarantorDto tenantguarantorDto)
        {
            TenantGuarantor tenantGuarantor = new TenantGuarantor();
            IEnumerable<TenantGuarantor?> checkEntity;
            try
            {
                checkEntity = await this.tenantguarantorRepository.Find(x => x.LastName!.ToLower().Trim() == tenantguarantorDto.LastName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantGuarantor = this.mapper.Map<TenantGuarantor>(tenantguarantorDto);
                    tenantGuarantor.TenantGuarantorID = Guid.NewGuid();
                    tenantGuarantor.UserID = tenantguarantorDto.UserID == 0 ? null : tenantguarantorDto.UserID;
                    tenantGuarantor.CapturedDate = DateTime.UtcNow;
                    tenantGuarantor = await tenantguarantorRepository.Create(tenantGuarantor) ?? new TenantGuarantor();
                    await tenantguarantorRepository.Save();
                    cache.Remove(Cache.TENANTGUARANTORS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantGuarantor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> UpdateTenantGuarantor(Guid id, TenantGuarantorDto tenantguarantorDto)
        {
            try
            {
                var existing = await this.tenantguarantorRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantGuarantor tenantGuarantor = this.mapper.Map<TenantGuarantor>(tenantguarantorDto);
                tenantGuarantor = await tenantguarantorRepository.Update(tenantGuarantor) ?? new TenantGuarantor();
                await tenantguarantorRepository.Save();
                cache.Remove(Cache.TENANTGUARANTORS.ToString());
                tenantguarantorDto = this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantGuarantor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantguarantorDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantGuarantor(Guid ID)
        {
            try
            {
                var tenantGuarantor = await this.tenantguarantorRepository.GetByID(ID);
                if (tenantGuarantor == null)
                    throw new KeyNotFoundException("TenantGuarantor with the specified ID was not found.");
                await tenantguarantorRepository.Delete(tenantGuarantor);
                await tenantguarantorRepository.Save();
                cache.Remove(Cache.TENANTGUARANTORS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantGuarantor . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> UpdateTenantGuarantorStatus(Guid id, string status)
        {
            var tenantGuarantor = await tenantguarantorRepository.GetByID(id);
            if (tenantGuarantor == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                tenantGuarantor.Status = "Pending";
            }
            else
            {
                tenantGuarantor.Status = status;
            }

            await tenantguarantorRepository.Update(tenantGuarantor);
            await tenantguarantorRepository.Save();
            cache.Remove(Cache.TENANTGUARANTORS.ToString());
            return this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
        }
    }
}