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
    public class LeaseRecurringChargesService : ILeaseRecurringChargesService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<LeaseRecurringChargesService> logger;
        private readonly ILeaseRecurringChargesRepository leaserecurringchargesRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseRecurringChargesService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseRecurringChargesService> logger, ILeaseRecurringChargesRepository leaserecurringchargesRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leaserecurringchargesRepository = leaserecurringchargesRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseRecurringChargesDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseRecurringCharges> entities;
            try
            {
                entities = await cache.GetOrCreateAsync(
                    Cache.LEASERECURRINGCHARGES.ToString(),
                    async cancel =>
                        (await this.leaserecurringchargesRepository.GetLeaseRecurringChargesAsync())?.Where(x => x != null).ToList() ?? new List<LeaseRecurringCharges>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseRecurringCharges by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseRecurringChargesDto>
                {
                    Data = new List<LeaseRecurringChargesDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseRecurringCharges> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseRecurringChargeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseRecurringChargesDto>>(pagedEntities);
            return new PagedResult<LeaseRecurringChargesDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseRecurringChargesDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.LEASERECURRINGCHARGES.ToString(),
                    async cancel =>
                        (await this.leaserecurringchargesRepository.GetLeaseRecurringChargesAsync())?.Where(x => x != null).ToList() ?? new List<LeaseRecurringCharges>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });

                var match = entities.FirstOrDefault(x => x.LeaseRecurringChargeID == ID);
                if (match == null)
                    match = await this.leaserecurringchargesRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<LeaseRecurringChargesDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseRecurringCharges by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseRecurringChargesDto> CreateLeaseRecurringCharges(LeaseRecurringChargesDto leaserecurringchargesDto)
        {
            LeaseRecurringCharges leaseRecurringCharges = new LeaseRecurringCharges();
            IEnumerable<LeaseRecurringCharges?> checkEntity;
            try
            {
                checkEntity = await this.leaserecurringchargesRepository.Find(x => x.Description!.ToLower().Trim() == leaserecurringchargesDto.Description!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseRecurringCharges = this.mapper.Map<LeaseRecurringCharges>(leaserecurringchargesDto);
                    leaseRecurringCharges.LeaseRecurringChargeID = Guid.NewGuid();
                    leaseRecurringCharges.LeaseRenewalID = leaserecurringchargesDto.LeaseRenewalID == Guid.Empty ? null : leaserecurringchargesDto.LeaseRenewalID;
                    leaseRecurringCharges.FeeID = leaserecurringchargesDto.FeeID == Guid.Empty ? null : leaserecurringchargesDto.FeeID;
                    leaseRecurringCharges.CapturedDate = DateTime.UtcNow;
                    leaseRecurringCharges = await leaserecurringchargesRepository.Create(leaseRecurringCharges) ?? new LeaseRecurringCharges();
                    await leaserecurringchargesRepository.Save();
                    await cache.RemoveAsync(Cache.LEASERECURRINGCHARGES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseRecurringCharges. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseRecurringChargesDto>(leaseRecurringCharges);
        }

        /// <inheritdoc/>
        public async Task<LeaseRecurringChargesDto?> UpdateLeaseRecurringCharges(Guid id, LeaseRecurringChargesDto leaserecurringchargesDto)
        {
            try
            {
                var existing = await this.leaserecurringchargesRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseRecurringCharges leaseRecurringCharges = this.mapper.Map<LeaseRecurringCharges>(leaserecurringchargesDto);
                leaseRecurringCharges = await leaserecurringchargesRepository.Update(leaseRecurringCharges) ?? new LeaseRecurringCharges();
                await leaserecurringchargesRepository.Save();
                await cache.RemoveAsync(Cache.LEASERECURRINGCHARGES.ToString());
                leaserecurringchargesDto = this.mapper.Map<LeaseRecurringChargesDto>(leaseRecurringCharges);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseRecurringCharges. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leaserecurringchargesDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseRecurringCharges(Guid ID)
        {
            try
            {
                var leaseRecurringCharges = await this.leaserecurringchargesRepository.GetByID(ID);
                if (leaseRecurringCharges == null)
                    throw new KeyNotFoundException("LeaseRecurringCharges with the specified ID was not found.");
                await leaserecurringchargesRepository.Delete(leaseRecurringCharges);
                await leaserecurringchargesRepository.Save();
                await cache.RemoveAsync(Cache.LEASERECURRINGCHARGES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseRecurringCharges . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}