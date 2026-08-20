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
    public class LeaseService : ILeaseService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseService> logger;
        private readonly ILeaseRepository leaseRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseService> logger, ILeaseRepository leaseRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leaseRepository = leaseRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseDto>> GetAll(Paging paging)
        {
            IEnumerable<Lease> entities;
            try
            {
                entities = cache.Get<IEnumerable<Lease>>(Cache.LEASES.ToString()) ?? new List<Lease>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leaseRepository.GetLeaseAsync())?.Where(x => x != null) ?? new List<Lease>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Lease>>(Cache.LEASES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Lease by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseDto>
                {
                    Data = new List<LeaseDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Lease> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.LeaseNumber) && x.LeaseNumber.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseDto>>(pagedEntities);
            return new PagedResult<LeaseDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Lease> entities = cache.Get<IEnumerable<Lease>>(Cache.LEASES.ToString()) ?? new List<Lease>();
                Lease? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseID == ID);
                }
                else
                {
                    match = await this.leaseRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Lease by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDto> CreateLease(LeaseDto leaseDto)
        {
            Lease lease = new Lease();
            IEnumerable<Lease?> checkEntity;
            try
            {
                checkEntity = await this.leaseRepository.Find(x => x.LeaseNumber!.ToLower().Trim() == leaseDto.LeaseNumber!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    lease = this.mapper.Map<Lease>(leaseDto);
                    lease.LeaseID = Guid.NewGuid();
                    lease.RentalApplicationID = leaseDto.RentalApplicationID == Guid.Empty ? null : leaseDto.RentalApplicationID;
                    lease.CapturedDate = DateTime.UtcNow;
                    lease = await leaseRepository.Create(lease) ?? new Lease();
                    await leaseRepository.Save();
                    cache.Remove(Cache.LEASES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Lease. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseDto>(lease);
        }

        /// <inheritdoc/>
        public async Task<LeaseDto?> UpdateLease(Guid id, LeaseDto leaseDto)
        {
            try
            {
                var existing = await this.leaseRepository.GetByID(id);
                if (existing == null)
                    return null;
                Lease lease = this.mapper.Map<Lease>(leaseDto);
                lease = await leaseRepository.Update(lease) ?? new Lease();
                await leaseRepository.Save();
                cache.Remove(Cache.LEASES.ToString());
                leaseDto = this.mapper.Map<LeaseDto>(lease);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Lease. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leaseDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLease(Guid ID)
        {
            try
            {
                var lease = await this.leaseRepository.GetByID(ID);
                if (lease == null)
                    throw new KeyNotFoundException("Lease with the specified ID was not found.");
                await leaseRepository.Delete(lease);
                await leaseRepository.Save();
                cache.Remove(Cache.LEASES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Lease . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDto?> UpdateLeaseStatus(Guid id, string status)
        {
            var lease = await leaseRepository.GetByID(id);
            if (lease == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                lease.Status = "Pending";
            }
            else
            {
                lease.Status = status;
            }

            await leaseRepository.Update(lease);
            await leaseRepository.Save();
            cache.Remove(Cache.LEASES.ToString());
            return this.mapper.Map<LeaseDto>(lease);
        }
    }
}