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
    public class LedgerTransactionService : ILedgerTransactionService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LedgerTransactionService> logger;
        private readonly ILedgerTransactionRepository ledgertransactionRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public LedgerTransactionService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LedgerTransactionService> logger, ILedgerTransactionRepository ledgertransactionRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.ledgertransactionRepository = ledgertransactionRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LedgerTransactionDto>> GetAll(Paging paging)
        {
            IEnumerable<LedgerTransaction> entities;
            try
            {
                entities = cache.Get<IEnumerable<LedgerTransaction>>(Cache.LEDGERTRANSACTIONS.ToString()) ?? new List<LedgerTransaction>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.ledgertransactionRepository.GetLedgerTransactionAsync())?.Where(x => x != null) ?? new List<LedgerTransaction>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LedgerTransaction>>(Cache.LEDGERTRANSACTIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerTransaction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LedgerTransactionDto>
                {
                    Data = new List<LedgerTransactionDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LedgerTransaction> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LedgerTransactionID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LedgerTransactionDto>>(pagedEntities);
            return new PagedResult<LedgerTransactionDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LedgerTransactionDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LedgerTransaction> entities = cache.Get<IEnumerable<LedgerTransaction>>(Cache.LEDGERTRANSACTIONS.ToString()) ?? new List<LedgerTransaction>();
                LedgerTransaction? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LedgerTransactionID == ID);
                }
                else
                {
                    match = await this.ledgertransactionRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<LedgerTransactionDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LedgerTransaction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LedgerTransactionDto> CreateLedgerTransaction(LedgerTransactionDto ledgertransactionDto)
        {
            LedgerTransaction ledgerTransaction = new LedgerTransaction();
            IEnumerable<LedgerTransaction?> checkEntity;
            try
            {
                checkEntity = await this.ledgertransactionRepository.Find(x => x.ReferenceNumber!.ToLower().Trim() == ledgertransactionDto.ReferenceNumber!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    ledgerTransaction = this.mapper.Map<LedgerTransaction>(ledgertransactionDto);
                    ledgerTransaction.LedgerTransactionID = Guid.NewGuid();
                    ledgerTransaction.PaymentID = ledgertransactionDto.PaymentID == Guid.Empty ? null : ledgertransactionDto.PaymentID;
                    ledgerTransaction.InvoiceMasterID = ledgertransactionDto.InvoiceMasterID == Guid.Empty ? null : ledgertransactionDto.InvoiceMasterID;
                    ledgerTransaction.RefundID = ledgertransactionDto.RefundID == Guid.Empty ? null : ledgertransactionDto.RefundID;
                    ledgerTransaction.PayoutID = ledgertransactionDto.PayoutID == 0 ? null : ledgertransactionDto.PayoutID;
                    ledgerTransaction.ReversedTransactionID = ledgertransactionDto.ReversedTransactionID == Guid.Empty ? null : ledgertransactionDto.ReversedTransactionID;
                    ledgerTransaction.CapturedDate = DateTime.UtcNow;
                    ledgerTransaction = await ledgertransactionRepository.Create(ledgerTransaction) ?? new LedgerTransaction();
                    await ledgertransactionRepository.Save();
                    cache.Remove(Cache.LEDGERTRANSACTIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LedgerTransaction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LedgerTransactionDto>(ledgerTransaction);
        }

        /// <inheritdoc/>
        public async Task<LedgerTransactionDto?> UpdateLedgerTransaction(Guid id, LedgerTransactionDto ledgertransactionDto)
        {
            try
            {
                var existing = await this.ledgertransactionRepository.GetByID(id);
                if (existing == null)
                    return null;
                LedgerTransaction ledgerTransaction = this.mapper.Map<LedgerTransaction>(ledgertransactionDto);
                ledgerTransaction = await ledgertransactionRepository.Update(ledgerTransaction) ?? new LedgerTransaction();
                await ledgertransactionRepository.Save();
                cache.Remove(Cache.LEDGERTRANSACTIONS.ToString());
                ledgertransactionDto = this.mapper.Map<LedgerTransactionDto>(ledgerTransaction);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LedgerTransaction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return ledgertransactionDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLedgerTransaction(Guid ID)
        {
            try
            {
                var ledgerTransaction = await this.ledgertransactionRepository.GetByID(ID);
                if (ledgerTransaction == null)
                    throw new KeyNotFoundException("LedgerTransaction with the specified ID was not found.");
                await ledgertransactionRepository.Delete(ledgerTransaction);
                await ledgertransactionRepository.Save();
                cache.Remove(Cache.LEDGERTRANSACTIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LedgerTransaction . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LedgerTransactionDto?> UpdateLedgerTransactionStatus(Guid id, string status)
        {
            var ledgerTransaction = await ledgertransactionRepository.GetByID(id);
            if (ledgerTransaction == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                ledgerTransaction.Status = "Pending";
            }
            else
            {
                ledgerTransaction.Status = status;
            }

            await ledgertransactionRepository.Update(ledgerTransaction);
            await ledgertransactionRepository.Save();
            cache.Remove(Cache.LEDGERTRANSACTIONS.ToString());
            return this.mapper.Map<LedgerTransactionDto>(ledgerTransaction);
        }
    }
}