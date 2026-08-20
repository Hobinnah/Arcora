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
    public class LedgerEntryService : ILedgerEntryService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LedgerEntryService> logger;
        private readonly ILedgerEntryRepository ledgerentryRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LedgerEntryService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LedgerEntryService> logger, ILedgerEntryRepository ledgerentryRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.ledgerentryRepository = ledgerentryRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LedgerEntryDto>> GetAll(Paging paging)
        {
            IEnumerable<LedgerEntry> entities;
            try
            {
                entities = cache.Get<IEnumerable<LedgerEntry>>(Cache.LEDGERENTRIES.ToString()) ?? new List<LedgerEntry>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.ledgerentryRepository.GetLedgerEntryAsync())?.Where(x => x != null) ?? new List<LedgerEntry>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LedgerEntry>>(Cache.LEDGERENTRIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerEntry by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LedgerEntryDto>
                {
                    Data = new List<LedgerEntryDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LedgerEntry> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LedgerEntryID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LedgerEntryDto>>(pagedEntities);
            return new PagedResult<LedgerEntryDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LedgerEntryDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LedgerEntry> entities = cache.Get<IEnumerable<LedgerEntry>>(Cache.LEDGERENTRIES.ToString()) ?? new List<LedgerEntry>();
                LedgerEntry? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LedgerEntryID == ID);
                }
                else
                {
                    match = await this.ledgerentryRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LedgerEntryDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerEntry by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LedgerEntryDto> CreateLedgerEntry(LedgerEntryDto ledgerentryDto)
        {
            LedgerEntry ledgerEntry = new LedgerEntry();
            IEnumerable<LedgerEntry?> checkEntity;
            try
            {
                checkEntity = await this.ledgerentryRepository.Find(x => x.Description!.ToLower().Trim() == ledgerentryDto.Description!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    ledgerEntry = this.mapper.Map<LedgerEntry>(ledgerentryDto);
                    ledgerEntry.LedgerEntryID = Guid.NewGuid();
                    ledgerEntry.CapturedDate = DateTime.UtcNow;
                    ledgerEntry = await ledgerentryRepository.Create(ledgerEntry) ?? new LedgerEntry();
                    await ledgerentryRepository.Save();
                    cache.Remove(Cache.LEDGERENTRIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LedgerEntry. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LedgerEntryDto>(ledgerEntry);
        }

        /// <inheritdoc/>
        public async Task<LedgerEntryDto?> UpdateLedgerEntry(Guid id, LedgerEntryDto ledgerentryDto)
        {
            try
            {
                var existing = await this.ledgerentryRepository.GetByID(id);
                if (existing == null)
                    return null;
                LedgerEntry ledgerEntry = this.mapper.Map<LedgerEntry>(ledgerentryDto);
                ledgerEntry = await ledgerentryRepository.Update(ledgerEntry) ?? new LedgerEntry();
                await ledgerentryRepository.Save();
                cache.Remove(Cache.LEDGERENTRIES.ToString());
                ledgerentryDto = this.mapper.Map<LedgerEntryDto>(ledgerEntry);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LedgerEntry. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return ledgerentryDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLedgerEntry(Guid ID)
        {
            try
            {
                var ledgerEntry = await this.ledgerentryRepository.GetByID(ID);
                if (ledgerEntry == null)
                    throw new KeyNotFoundException("LedgerEntry with the specified ID was not found.");
                await ledgerentryRepository.Delete(ledgerEntry);
                await ledgerentryRepository.Save();
                cache.Remove(Cache.LEDGERENTRIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LedgerEntry . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}