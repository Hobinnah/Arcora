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
    public class LeaseOccupantsService : ILeaseOccupantsService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseOccupantsService> logger;
        private readonly ILeaseOccupantsRepository leaseoccupantsRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseOccupantsService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseOccupantsService> logger, ILeaseOccupantsRepository leaseoccupantsRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leaseoccupantsRepository = leaseoccupantsRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseOccupantsDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseOccupants> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseOccupants>>(Cache.LEASEOCCUPANTS.ToString()) ?? new List<LeaseOccupants>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leaseoccupantsRepository.GetLeaseOccupantsAsync())?.Where(x => x != null) ?? new List<LeaseOccupants>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseOccupants>>(Cache.LEASEOCCUPANTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseOccupants by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseOccupantsDto>
                {
                    Data = new List<LeaseOccupantsDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseOccupants> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseOccupantID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseOccupantsDto>>(pagedEntities);
            return new PagedResult<LeaseOccupantsDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseOccupantsDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseOccupants> entities = cache.Get<IEnumerable<LeaseOccupants>>(Cache.LEASEOCCUPANTS.ToString()) ?? new List<LeaseOccupants>();
                LeaseOccupants? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseOccupantID == ID);
                }
                else
                {
                    match = await this.leaseoccupantsRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseOccupantsDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseOccupants by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseOccupantsDto> CreateLeaseOccupants(LeaseOccupantsDto leaseoccupantsDto)
        {
            LeaseOccupants leaseOccupants = new LeaseOccupants();
            IEnumerable<LeaseOccupants?> checkEntity;
            try
            {
                checkEntity = await this.leaseoccupantsRepository.Find(x => x.LastName!.ToLower().Trim() == leaseoccupantsDto.LastName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseOccupants = this.mapper.Map<LeaseOccupants>(leaseoccupantsDto);
                    leaseOccupants.LeaseOccupantID = Guid.NewGuid();
                    leaseOccupants.LeaseRenewalID = leaseoccupantsDto.LeaseRenewalID == Guid.Empty ? null : leaseoccupantsDto.LeaseRenewalID;
                    leaseOccupants.TenantID = leaseoccupantsDto.TenantID == Guid.Empty ? null : leaseoccupantsDto.TenantID;
                    leaseOccupants.UserID = leaseoccupantsDto.UserID == 0 ? null : leaseoccupantsDto.UserID;
                    leaseOccupants.CapturedDate = DateTime.UtcNow;
                    leaseOccupants = await leaseoccupantsRepository.Create(leaseOccupants) ?? new LeaseOccupants();
                    await leaseoccupantsRepository.Save();
                    cache.Remove(Cache.LEASEOCCUPANTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseOccupants. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseOccupantsDto>(leaseOccupants);
        }

        /// <inheritdoc/>
        public async Task<LeaseOccupantsDto?> UpdateLeaseOccupants(Guid id, LeaseOccupantsDto leaseoccupantsDto)
        {
            try
            {
                var existing = await this.leaseoccupantsRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseOccupants leaseOccupants = this.mapper.Map<LeaseOccupants>(leaseoccupantsDto);
                leaseOccupants = await leaseoccupantsRepository.Update(leaseOccupants) ?? new LeaseOccupants();
                await leaseoccupantsRepository.Save();
                cache.Remove(Cache.LEASEOCCUPANTS.ToString());
                leaseoccupantsDto = this.mapper.Map<LeaseOccupantsDto>(leaseOccupants);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseOccupants. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leaseoccupantsDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseOccupants(Guid ID)
        {
            try
            {
                var leaseOccupants = await this.leaseoccupantsRepository.GetByID(ID);
                if (leaseOccupants == null)
                    throw new KeyNotFoundException("LeaseOccupants with the specified ID was not found.");
                await leaseoccupantsRepository.Delete(leaseOccupants);
                await leaseoccupantsRepository.Save();
                cache.Remove(Cache.LEASEOCCUPANTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseOccupants . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseOccupantsDto?> UpdateLeaseOccupantsStatus(Guid id, string status)
        {
            var leaseOccupants = await leaseoccupantsRepository.GetByID(id);
            if (leaseOccupants == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                leaseOccupants.Status = "Pending";
            }
            else
            {
                leaseOccupants.Status = status;
            }

            await leaseoccupantsRepository.Update(leaseOccupants);
            await leaseoccupantsRepository.Save();
            cache.Remove(Cache.LEASEOCCUPANTS.ToString());
            return this.mapper.Map<LeaseOccupantsDto>(leaseOccupants);
        }
    }
}