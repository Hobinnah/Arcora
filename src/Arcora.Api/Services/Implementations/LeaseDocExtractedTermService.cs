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
    public class LeaseDocExtractedTermService : ILeaseDocExtractedTermService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseDocExtractedTermService> logger;
        private readonly ILeaseDocExtractedTermRepository leasedocextractedtermRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseDocExtractedTermService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseDocExtractedTermService> logger, ILeaseDocExtractedTermRepository leasedocextractedtermRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leasedocextractedtermRepository = leasedocextractedtermRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseDocExtractedTermDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseDocExtractedTerm> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseDocExtractedTerm>>(Cache.LEASEDOCEXTRACTEDTERMS.ToString()) ?? new List<LeaseDocExtractedTerm>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leasedocextractedtermRepository.GetAll())?.Where(x => x != null).Cast<LeaseDocExtractedTerm>().ToList() ?? new List<LeaseDocExtractedTerm>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseDocExtractedTerm>>(Cache.LEASEDOCEXTRACTEDTERMS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocExtractedTerm by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseDocExtractedTermDto>
                {
                    Data = new List<LeaseDocExtractedTermDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseDocExtractedTerm> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ExtractionStatus) && x.ExtractionStatus.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseDocExtractedTermID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseDocExtractedTermDto>>(pagedEntities);
            return new PagedResult<LeaseDocExtractedTermDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseDocExtractedTermDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseDocExtractedTerm> entities = cache.Get<IEnumerable<LeaseDocExtractedTerm>>(Cache.LEASEDOCEXTRACTEDTERMS.ToString()) ?? new List<LeaseDocExtractedTerm>();
                LeaseDocExtractedTerm? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseDocExtractedTermID == ID);
                }
                else
                {
                    match = await this.leasedocextractedtermRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseDocExtractedTermDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocExtractedTerm by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDocExtractedTermDto> CreateLeaseDocExtractedTerm(LeaseDocExtractedTermDto leasedocextractedtermDto)
        {
            LeaseDocExtractedTerm leaseDocExtractedTerm = new LeaseDocExtractedTerm();
            IEnumerable<LeaseDocExtractedTerm?> checkEntity;
            try
            {
                checkEntity = await this.leasedocextractedtermRepository.Find(x => x.ExtractionStatus!.ToLower().Trim() == leasedocextractedtermDto.ExtractionStatus!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseDocExtractedTerm = this.mapper.Map<LeaseDocExtractedTerm>(leasedocextractedtermDto);
                    leaseDocExtractedTerm.LeaseDocExtractedTermID = Guid.NewGuid();
                    leaseDocExtractedTerm = await leasedocextractedtermRepository.Create(leaseDocExtractedTerm) ?? new LeaseDocExtractedTerm();
                    await leasedocextractedtermRepository.Save();
                    cache.Remove(Cache.LEASEDOCEXTRACTEDTERMS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseDocExtractedTerm. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseDocExtractedTermDto>(leaseDocExtractedTerm);
        }

        /// <inheritdoc/>
        public async Task<LeaseDocExtractedTermDto?> UpdateLeaseDocExtractedTerm(Guid id, LeaseDocExtractedTermDto leasedocextractedtermDto)
        {
            try
            {
                var existing = await this.leasedocextractedtermRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseDocExtractedTerm leaseDocExtractedTerm = this.mapper.Map<LeaseDocExtractedTerm>(leasedocextractedtermDto);
                leaseDocExtractedTerm = await leasedocextractedtermRepository.Update(leaseDocExtractedTerm) ?? new LeaseDocExtractedTerm();
                await leasedocextractedtermRepository.Save();
                cache.Remove(Cache.LEASEDOCEXTRACTEDTERMS.ToString());
                leasedocextractedtermDto = this.mapper.Map<LeaseDocExtractedTermDto>(leaseDocExtractedTerm);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseDocExtractedTerm. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leasedocextractedtermDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseDocExtractedTerm(Guid ID)
        {
            try
            {
                var leaseDocExtractedTerm = await this.leasedocextractedtermRepository.GetByID(ID);
                if (leaseDocExtractedTerm == null)
                    throw new KeyNotFoundException("LeaseDocExtractedTerm with the specified ID was not found.");
                await leasedocextractedtermRepository.Delete(leaseDocExtractedTerm);
                await leasedocextractedtermRepository.Save();
                cache.Remove(Cache.LEASEDOCEXTRACTEDTERMS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseDocExtractedTerm . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}