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
    public class LeaseRenewalsService : ILeaseRenewalsService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseRenewalsService> logger;
        private readonly ILeaseRenewalsRepository leaserenewalsRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseRenewalsService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseRenewalsService> logger, ILeaseRenewalsRepository leaserenewalsRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leaserenewalsRepository = leaserenewalsRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseRenewalsDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseRenewals> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseRenewals>>(Cache.LEASERENEWALS.ToString()) ?? new List<LeaseRenewals>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leaserenewalsRepository.GetLeaseRenewalsAsync())?.Where(x => x != null) ?? new List<LeaseRenewals>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseRenewals>>(Cache.LEASERENEWALS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseRenewals by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseRenewalsDto>
                {
                    Data = new List<LeaseRenewalsDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseRenewals> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Notes) && x.Notes.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseRenewalID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseRenewalsDto>>(pagedEntities);
            return new PagedResult<LeaseRenewalsDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseRenewalsDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseRenewals> entities = cache.Get<IEnumerable<LeaseRenewals>>(Cache.LEASERENEWALS.ToString()) ?? new List<LeaseRenewals>();
                LeaseRenewals? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseRenewalID == ID);
                }
                else
                {
                    match = await this.leaserenewalsRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseRenewalsDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseRenewals by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseRenewalsDto> CreateLeaseRenewals(LeaseRenewalsDto leaserenewalsDto)
        {
            LeaseRenewals leaseRenewals = new LeaseRenewals();
            IEnumerable<LeaseRenewals?> checkEntity;
            try
            {
                checkEntity = await this.leaserenewalsRepository.Find(x => x.Notes!.ToLower().Trim() == leaserenewalsDto.Notes!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseRenewals = this.mapper.Map<LeaseRenewals>(leaserenewalsDto);
                    leaseRenewals.LeaseRenewalID = Guid.NewGuid();
                    leaseRenewals.CapturedDate = DateTime.UtcNow;
                    leaseRenewals = await leaserenewalsRepository.Create(leaseRenewals) ?? new LeaseRenewals();
                    await leaserenewalsRepository.Save();
                    cache.Remove(Cache.LEASERENEWALS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseRenewals. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseRenewalsDto>(leaseRenewals);
        }

        /// <inheritdoc/>
        public async Task<LeaseRenewalsDto?> UpdateLeaseRenewals(Guid id, LeaseRenewalsDto leaserenewalsDto)
        {
            try
            {
                var existing = await this.leaserenewalsRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseRenewals leaseRenewals = this.mapper.Map<LeaseRenewals>(leaserenewalsDto);
                leaseRenewals = await leaserenewalsRepository.Update(leaseRenewals) ?? new LeaseRenewals();
                await leaserenewalsRepository.Save();
                cache.Remove(Cache.LEASERENEWALS.ToString());
                leaserenewalsDto = this.mapper.Map<LeaseRenewalsDto>(leaseRenewals);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseRenewals. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leaserenewalsDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseRenewals(Guid ID)
        {
            try
            {
                var leaseRenewals = await this.leaserenewalsRepository.GetByID(ID);
                if (leaseRenewals == null)
                    throw new KeyNotFoundException("LeaseRenewals with the specified ID was not found.");
                await leaserenewalsRepository.Delete(leaseRenewals);
                await leaserenewalsRepository.Save();
                cache.Remove(Cache.LEASERENEWALS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseRenewals . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseRenewalsDto?> UpdateLeaseRenewalsStatus(Guid id, string status)
        {
            var leaseRenewals = await leaserenewalsRepository.GetByID(id);
            if (leaseRenewals == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                leaseRenewals.Status = "Pending";
            }
            else
            {
                leaseRenewals.Status = status;
            }

            await leaserenewalsRepository.Update(leaseRenewals);
            await leaserenewalsRepository.Save();
            cache.Remove(Cache.LEASERENEWALS.ToString());
            return this.mapper.Map<LeaseRenewalsDto>(leaseRenewals);
        }
    }
}