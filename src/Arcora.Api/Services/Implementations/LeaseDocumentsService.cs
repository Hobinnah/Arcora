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
    public class LeaseDocumentsService : ILeaseDocumentsService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseDocumentsService> logger;
        private readonly ILeaseDocumentsRepository leasedocumentsRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseDocumentsService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseDocumentsService> logger, ILeaseDocumentsRepository leasedocumentsRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leasedocumentsRepository = leasedocumentsRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseDocumentsDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseDocuments> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString()) ?? new List<LeaseDocuments>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leasedocumentsRepository.GetLeaseDocumentsAsync())?.Where(x => x != null) ?? new List<LeaseDocuments>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocuments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseDocumentsDto>
                {
                    Data = new List<LeaseDocumentsDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseDocuments> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.OriginalFilename) && x.OriginalFilename.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseDocumentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseDocumentsDto>>(pagedEntities);
            return new PagedResult<LeaseDocumentsDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseDocuments> entities = cache.Get<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString()) ?? new List<LeaseDocuments>();
                LeaseDocuments? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseDocumentID == ID);
                }
                else
                {
                    match = await this.leasedocumentsRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LeaseDocumentsDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocuments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto> CreateLeaseDocuments(LeaseDocumentsDto leasedocumentsDto)
        {
            LeaseDocuments leaseDocuments = new LeaseDocuments();
            IEnumerable<LeaseDocuments?> checkEntity;
            try
            {
                checkEntity = await this.leasedocumentsRepository.Find(x => x.OriginalFilename!.ToLower().Trim() == leasedocumentsDto.OriginalFilename!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseDocuments = this.mapper.Map<LeaseDocuments>(leasedocumentsDto);
                    leaseDocuments.LeaseDocumentID = Guid.NewGuid();
                    leaseDocuments.LeaseRenewalID = leasedocumentsDto.LeaseRenewalID == Guid.Empty ? null : leasedocumentsDto.LeaseRenewalID;
                    leaseDocuments.CapturedDate = DateTime.UtcNow;
                    leaseDocuments = await leasedocumentsRepository.Create(leaseDocuments) ?? new LeaseDocuments();
                    await leasedocumentsRepository.Save();
                    cache.Remove(Cache.LEASEDOCUMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseDocuments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseDocumentsDto>(leaseDocuments);
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto?> UpdateLeaseDocuments(Guid id, LeaseDocumentsDto leasedocumentsDto)
        {
            try
            {
                var existing = await this.leasedocumentsRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseDocuments leaseDocuments = this.mapper.Map<LeaseDocuments>(leasedocumentsDto);
                leaseDocuments = await leasedocumentsRepository.Update(leaseDocuments) ?? new LeaseDocuments();
                await leasedocumentsRepository.Save();
                cache.Remove(Cache.LEASEDOCUMENTS.ToString());
                leasedocumentsDto = this.mapper.Map<LeaseDocumentsDto>(leaseDocuments);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseDocuments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leasedocumentsDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseDocuments(Guid ID)
        {
            try
            {
                var leaseDocuments = await this.leasedocumentsRepository.GetByID(ID);
                if (leaseDocuments == null)
                    throw new KeyNotFoundException("LeaseDocuments with the specified ID was not found.");
                await leasedocumentsRepository.Delete(leaseDocuments);
                await leasedocumentsRepository.Save();
                cache.Remove(Cache.LEASEDOCUMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseDocuments . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}