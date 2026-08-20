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
    public class LeaseSignatoriesService : ILeaseSignatoriesService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseSignatoriesService> logger;
        private readonly ILeaseSignatoriesRepository leasesignatoriesRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseSignatoriesService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseSignatoriesService> logger, ILeaseSignatoriesRepository leasesignatoriesRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leasesignatoriesRepository = leasesignatoriesRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseSignatoriesDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseSignatories> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseSignatories>>(Cache.LEASESIGNATORIES.ToString()) ?? new List<LeaseSignatories>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leasesignatoriesRepository.GetLeaseSignatoriesAsync())?.Where(x => x != null) ?? new List<LeaseSignatories>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseSignatories>>(Cache.LEASESIGNATORIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseSignatories by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseSignatoriesDto>
                {
                    Data = new List<LeaseSignatoriesDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseSignatories> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseSignatoryID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseSignatoriesDto>>(pagedEntities);
            return new PagedResult<LeaseSignatoriesDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseSignatoriesDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseSignatories> entities = cache.Get<IEnumerable<LeaseSignatories>>(Cache.LEASESIGNATORIES.ToString()) ?? new List<LeaseSignatories>();
                LeaseSignatories? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseSignatoryID == ID);
                }
                else
                {
                    match = await this.leasesignatoriesRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseSignatoriesDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseSignatories by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseSignatoriesDto> CreateLeaseSignatories(LeaseSignatoriesDto leasesignatoriesDto)
        {
            LeaseSignatories leaseSignatories = new LeaseSignatories();
            IEnumerable<LeaseSignatories?> checkEntity;
            try
            {
                checkEntity = await this.leasesignatoriesRepository.Find(x => x.Name!.ToLower().Trim() == leasesignatoriesDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseSignatories = this.mapper.Map<LeaseSignatories>(leasesignatoriesDto);
                    leaseSignatories.LeaseSignatoryID = Guid.NewGuid();
                    leaseSignatories.UserID = leasesignatoriesDto.UserID == 0 ? null : leasesignatoriesDto.UserID;
                    leaseSignatories.TenantID = leasesignatoriesDto.TenantID == Guid.Empty ? null : leasesignatoriesDto.TenantID;
                    leaseSignatories.OrganizationMemberID = leasesignatoriesDto.OrganizationMemberID == Guid.Empty ? null : leasesignatoriesDto.OrganizationMemberID;
                    leaseSignatories.ProviderSignerID = string.IsNullOrEmpty(leasesignatoriesDto.ProviderSignerID) ? null : leasesignatoriesDto.ProviderSignerID;
                    leaseSignatories.CapturedDate = DateTime.UtcNow;
                    leaseSignatories = await leasesignatoriesRepository.Create(leaseSignatories) ?? new LeaseSignatories();
                    await leasesignatoriesRepository.Save();
                    cache.Remove(Cache.LEASESIGNATORIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseSignatories. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseSignatoriesDto>(leaseSignatories);
        }

        /// <inheritdoc/>
        public async Task<LeaseSignatoriesDto?> UpdateLeaseSignatories(Guid id, LeaseSignatoriesDto leasesignatoriesDto)
        {
            try
            {
                var existing = await this.leasesignatoriesRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseSignatories leaseSignatories = this.mapper.Map<LeaseSignatories>(leasesignatoriesDto);
                leaseSignatories = await leasesignatoriesRepository.Update(leaseSignatories) ?? new LeaseSignatories();
                await leasesignatoriesRepository.Save();
                cache.Remove(Cache.LEASESIGNATORIES.ToString());
                leasesignatoriesDto = this.mapper.Map<LeaseSignatoriesDto>(leaseSignatories);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseSignatories. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leasesignatoriesDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseSignatories(Guid ID)
        {
            try
            {
                var leaseSignatories = await this.leasesignatoriesRepository.GetByID(ID);
                if (leaseSignatories == null)
                    throw new KeyNotFoundException("LeaseSignatories with the specified ID was not found.");
                await leasesignatoriesRepository.Delete(leaseSignatories);
                await leasesignatoriesRepository.Save();
                cache.Remove(Cache.LEASESIGNATORIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseSignatories . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseSignatoriesDto?> UpdateLeaseSignatoriesStatus(Guid id, string status)
        {
            var leaseSignatories = await leasesignatoriesRepository.GetByID(id);
            if (leaseSignatories == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                leaseSignatories.Status = "Pending";
            }
            else
            {
                leaseSignatories.Status = status;
            }

            await leasesignatoriesRepository.Update(leaseSignatories);
            await leasesignatoriesRepository.Save();
            cache.Remove(Cache.LEASESIGNATORIES.ToString());
            return this.mapper.Map<LeaseSignatoriesDto>(leaseSignatories);
        }
    }
}