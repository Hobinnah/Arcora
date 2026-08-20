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
    public class LedgerAccountService : ILedgerAccountService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LedgerAccountService> logger;
        private readonly ILedgerAccountRepository ledgeraccountRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LedgerAccountService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LedgerAccountService> logger, ILedgerAccountRepository ledgeraccountRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.ledgeraccountRepository = ledgeraccountRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LedgerAccountDto>> GetAll(Paging paging)
        {
            IEnumerable<LedgerAccount> entities;
            try
            {
                entities = cache.Get<IEnumerable<LedgerAccount>>(Cache.LEDGERACCOUNTS.ToString()) ?? new List<LedgerAccount>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.ledgeraccountRepository.GetLedgerAccountAsync())?.Where(x => x != null) ?? new List<LedgerAccount>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LedgerAccount>>(Cache.LEDGERACCOUNTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LedgerAccountDto>
                {
                    Data = new List<LedgerAccountDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LedgerAccount> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LedgerAccountID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LedgerAccountDto>>(pagedEntities);
            return new PagedResult<LedgerAccountDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LedgerAccountDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LedgerAccount> entities = cache.Get<IEnumerable<LedgerAccount>>(Cache.LEDGERACCOUNTS.ToString()) ?? new List<LedgerAccount>();
                LedgerAccount? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LedgerAccountID == ID);
                }
                else
                {
                    match = await this.ledgeraccountRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LedgerAccountDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LedgerAccountDto> CreateLedgerAccount(LedgerAccountDto ledgeraccountDto)
        {
            LedgerAccount ledgerAccount = new LedgerAccount();
            IEnumerable<LedgerAccount?> checkEntity;
            try
            {
                checkEntity = await this.ledgeraccountRepository.Find(x => x.Name!.ToLower().Trim() == ledgeraccountDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    ledgerAccount = this.mapper.Map<LedgerAccount>(ledgeraccountDto);
                    ledgerAccount.LedgerAccountID = Guid.NewGuid();
                    ledgerAccount.CapturedDate = DateTime.UtcNow;
                    ledgerAccount = await ledgeraccountRepository.Create(ledgerAccount) ?? new LedgerAccount();
                    await ledgeraccountRepository.Save();
                    cache.Remove(Cache.LEDGERACCOUNTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LedgerAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LedgerAccountDto>(ledgerAccount);
        }

        /// <inheritdoc/>
        public async Task<LedgerAccountDto?> UpdateLedgerAccount(Guid id, LedgerAccountDto ledgeraccountDto)
        {
            try
            {
                var existing = await this.ledgeraccountRepository.GetByID(id);
                if (existing == null)
                    return null;
                LedgerAccount ledgerAccount = this.mapper.Map<LedgerAccount>(ledgeraccountDto);
                ledgerAccount = await ledgeraccountRepository.Update(ledgerAccount) ?? new LedgerAccount();
                await ledgeraccountRepository.Save();
                cache.Remove(Cache.LEDGERACCOUNTS.ToString());
                ledgeraccountDto = this.mapper.Map<LedgerAccountDto>(ledgerAccount);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LedgerAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return ledgeraccountDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLedgerAccount(Guid ID)
        {
            try
            {
                var ledgerAccount = await this.ledgeraccountRepository.GetByID(ID);
                if (ledgerAccount == null)
                    throw new KeyNotFoundException("LedgerAccount with the specified ID was not found.");
                await ledgeraccountRepository.Delete(ledgerAccount);
                await ledgeraccountRepository.Save();
                cache.Remove(Cache.LEDGERACCOUNTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LedgerAccount . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}